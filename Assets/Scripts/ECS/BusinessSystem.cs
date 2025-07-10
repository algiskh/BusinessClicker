using BusinessGame.ECS.Components;
using Leopotam.EcsLite;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

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

			// Pools
			var configPool = world.GetPool<ConfigComponent>();
			var timerPool = world.GetPool<Timer>();
			var incomePool = world.GetPool<Income>();
			var addSoftPool = world.GetPool<AddSoftRequest>();
			var levelPool = world.GetPool<Level>();
			var businessViewPool = world.GetPool<BusinessViewComponent>();
			var upgradesPool = world.GetPool<Upgrades>();

			var businessFilter = world.Filter<BusinessViewComponent>()
				.Inc<ConfigComponent>()
				.Inc<Level>()
				.Inc<Timer>()
				.Inc<Upgrades>()
				.End();

			// 1. Iterate timers
			foreach (var entity in businessFilter)
			{
				ref var timer = ref timerPool.Get(entity);
				ref var level = ref levelPool.Get(entity);
				ref var businessView = ref businessViewPool.Get(entity);
				ref var upgrades = ref upgradesPool.Get(entity);
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

			// 2. Check
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

				if (approve.Purpose == SpendPurpose.LevelUp)
				{
					if (!levelPool.Has(approve.TargetEntity))
						continue;
					targetLevel.Value++;
				}
				else if (approve.Purpose == SpendPurpose.Upgrade)
				{
					if (!upgradesPool.Has(targetEntity))
						continue;
					var upgradeIndex = approve.AdditionalTarget; // index in array of upgrades
					if (upgradeIndex < 0 || upgradeIndex >= upgrades.Value.Length)
						continue;
					upgrades.Value[upgradeIndex].IsObtained = true;

					ref var businessView = ref businessViewPool.Get(approve.TargetEntity);
				}

				income.Value = configPool.Get(targetEntity).Value.GetIncome(targetLevel.Value, upgrades.Value);

				var updateRequestEntity = world.NewEntity();
				ref var updateRequest = ref updateViewPool.Add(updateRequestEntity);
				updateRequest.Target = approve.TargetEntity;
			}

			foreach (var approveEntity in approvesToRemove)
			{
				if (approvePool.Get(approveEntity).IsApproved)
					world.DelEntity(approveEntity);
			}
		}

		private void TryToDeserialize(EcsWorld world)
		{
			var loadFilter = world.Filter<LoadedData>().End();

			if (loadFilter.GetEntitiesCount() > 0)
			{
				var entity = loadFilter.GetRawEntities()[0];
				var loadPool = world.GetPool<LoadedData>();
				var save = loadPool.Get(entity);

				Deserialize(world, save);

				foreach (var e in loadFilter)
					world.DelEntity(e);
			}
			else
			{
				SetStartParameters(world);
			}
		}

		private void Deserialize(EcsWorld world, LoadedData data)
		{

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
			var firstData = configHolderComponent.ConfigHolder.FirstPlayData;

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
				{
					upgrades.Value[i].IsObtained = false;
				}
			}
		}
	}
}
