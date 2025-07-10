using BusinessGame.Configs;
using BusinessGame.ECS.Components;
using Leopotam.EcsLite;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BusinessGame.ECS
{
	public class BusinessSystem : IEcsInitSystem, IEcsRunSystem
	{
		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			TryToDeserialize(world);
		}

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			ProcessTimers(world);
			ProcessApproveRequests(world);
		}

		private void ProcessTimers(EcsWorld world)
		{
			var configPool = world.GetPool<ConfigComponent>();
			var timerPool = world.GetPool<Timer>();
			var incomePool = world.GetPool<Income>();
			var addSoftPool = world.GetPool<AddSoftRequest>();
			var levelPool = world.GetPool<Level>();
			var businessViewPool = world.GetPool<BusinessViewComponent>();

			var filter = world.Filter<BusinessViewComponent>()
				.Inc<ConfigComponent>()
				.Inc<Level>()
				.Inc<Timer>()
				.Inc<Upgrades>()
				.End();

			foreach (var entity in filter)
			{
				ref var timer = ref timerPool.Get(entity);
				ref var level = ref levelPool.Get(entity);
				ref var businessView = ref businessViewPool.Get(entity);
				ref var income = ref incomePool.Get(entity);

				if (level.Value < 1)
					continue;

				if (timer.Value <= 0)
				{
					timer.Value = configPool.Get(entity).Value.Delay;

					var addSoftEntity = world.NewEntity();
					ref var addSoftRequest = ref addSoftPool.Add(addSoftEntity);
					addSoftRequest.Amount = income.Value;
				}
				else
				{
					timer.Value -= Time.deltaTime;
				}
				businessView.Value.UpdateSlider();
			}
		}

		private void ProcessApproveRequests(EcsWorld world)
		{
			var configPool = world.GetPool<ConfigComponent>();
			var incomePool = world.GetPool<Income>();
			var levelPool = world.GetPool<Level>();
			var upgradesPool = world.GetPool<Upgrades>();
			var approvePool = world.GetPool<RequestSpendSoft>();
			var updateViewPool = world.GetPool<UpdateViewRequest>();

			var approveFilter = world.Filter<RequestSpendSoft>().End();
			var approvesToRemove = new List<int>();

			foreach (var approveEntity in approveFilter)
			{
				var approve = approvePool.Get(approveEntity);
				approvesToRemove.Add(approveEntity);

				if (!approve.IsApproved)
					continue;

				var targetEntity = approve.TargetEntity;
				ref var income = ref incomePool.Get(targetEntity);
				ref var targetLevel = ref levelPool.Get(targetEntity);
				ref var upgrades = ref upgradesPool.Get(targetEntity);

				switch (approve.Purpose)
				{
					case SpendPurpose.LevelUp:
						if (!levelPool.Has(targetEntity))
							continue;
						targetLevel.Value++;
						break;
					case SpendPurpose.Upgrade:
						if (!upgradesPool.Has(targetEntity))
							continue;
						var upgradeIndex = approve.AdditionalTarget;
						if (upgradeIndex < 0 || upgradeIndex >= upgrades.Value.Length)
							continue;
						upgrades.Value[upgradeIndex].IsObtained = true;
						break;
				}

				income.Value = configPool.Get(targetEntity).Value.GetIncome(targetLevel.Value, upgrades.Value);

				var updateRequestEntity = world.NewEntity();
				ref var updateRequest = ref updateViewPool.Add(updateRequestEntity);
				updateRequest.Target = targetEntity;
			}

			foreach (var approveEntity in approvesToRemove)
			{
				if (approvePool.Get(approveEntity).IsApproved)
					world.DelEntity(approveEntity);
			}
		}

		private void TryToDeserialize(EcsWorld world)
		{
			if (world.TryGetAsSingleton<LoadedDataComponent>(out var save))
			{
				Deserialize(world, save);

				world.DeleteAllWith<LoadedDataComponent>();
			}
			else
			{
				SetStartParameters(world);
			}

			var updateViewPool = world.GetPool<UpdateViewRequest>();
			var updateRequestEntity = world.NewEntity();
			ref var updateRequest = ref updateViewPool.Add(updateRequestEntity);
			updateRequest.IsGlobal = true;
		}

		private void Deserialize(EcsWorld world, LoadedDataComponent data)
		{
			var saveData = data.Value;

			var configHolder = world.GetAsSingleton<ConfigHolderComponent>();
			var configPool = world.GetPool<ConfigComponent>();
			var levelPool = world.GetPool<Level>();
			var timerPool = world.GetPool<Timer>();
			var upgradesPool = world.GetPool<Upgrades>();

			var businessEntitiesById = new Dictionary<string, int>();

			var filter = world.Filter<BusinessViewComponent>()
				.Inc<ConfigComponent>()
				.Inc<Level>()
				.Inc<Timer>()
				.Inc<Upgrades>()
				.End();

			foreach (var entity in filter)
			{
				var id = configPool.Get(entity).Value.Id;
				businessEntitiesById[id] = entity;
			}

			foreach (var business in saveData.Businesses)
			{
				if (!businessEntitiesById.TryGetValue(business.Id, out var entity))
				{
					Debug.LogWarning($"No active entity for business id: {business.Id}");
					continue;
				}

				ref var level = ref levelPool.Get(entity);
				level.Value = business.Level;

				ref var timer = ref timerPool.Get(entity);
				timer.Value = business.Timer;

				ref var upgrades = ref upgradesPool.Get(entity);
				var config = configHolder.Value.GetConfig(business.Id);

				if (config != null)
				{
					if (upgrades.Value == null || upgrades.Value.Length != config.Upgrades.Length)
						upgrades.Value = config.Upgrades.ToArray();

					for (var i = 0; i < config.Upgrades.Length; i++)
					{
						upgrades.Value[i].IsObtained = (i < business.Upgrades.Length) && business.Upgrades[i];
					}
				}
			}

			Debug.Log($"Save loaded (entities updated)");
		}

		private void SetStartParameters(EcsWorld world)
		{
			var businessViewPool = world.GetPool<BusinessViewComponent>();
			var configHolderPool = world.GetPool<ConfigHolderComponent>();
			var configPool = world.GetPool<ConfigComponent>();
			var levelPool = world.GetPool<Level>();
			var incomePool = world.GetPool<Income>();
			var upgradesPool = world.GetPool<Upgrades>();

			var configHolderFilter = world.Filter<ConfigHolderComponent>().End();
			var configHolderComponent = configHolderPool.Get(configHolderFilter.GetRawEntities()[0]);
			var firstData = configHolderComponent.Value.FirstPlayData;

			var businessEntities = world.Filter<ConfigComponent>()
				.Inc<BusinessViewComponent>()
				.Inc<Level>()
				.Inc<Timer>()
				.Inc<Income>()
				.Inc<Upgrades>()
				.End();

			foreach (var b in businessEntities)
			{
				var configComponent = configPool.Get(b);

				if (firstData.BusinessIds.Contains(configComponent.Value.Id))
				{
					ref var level = ref levelPool.Get(b);
					level.Value = firstData.StartLevel;
				}

				ref var income = ref incomePool.Get(b);
				income.Value = configComponent.Value.BaseIncome;

				ref var businessView = ref businessViewPool.Get(b);
				businessView.Value.UpdateStats();

				ref var upgrades = ref upgradesPool.Get(b);
				for (int i = 0; i < upgrades.Value.Length; i++)
					upgrades.Value[i].IsObtained = false;
			}
		}
	}
}
