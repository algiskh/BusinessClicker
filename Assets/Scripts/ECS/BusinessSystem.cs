using BusinessGame.ECS.Components;
using Leopotam.EcsLite;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using Timer = BusinessGame.ECS.Components.Timer;

namespace BusinessGame.ECS
{
	public class BusinessSystem : IEcsInitSystem, IEcsRunSystem
	{
		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			var loadFilter = world.Filter<LoadedData>().End();

			if (loadFilter.GetEntitiesCount() > 0)
			{
				var entity = loadFilter.GetRawEntities()[0];
				var loadPool = world.GetPool<LoadedData>();
				var save = loadPool.Get(entity);

				Deserialize(world, save);

				foreach (var e in loadFilter)
				{
					world.DelEntity(e);
				}
			}
			else
			{
				SetStartParameters(world);
			}
		}

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			var configPool = world.GetPool<ConfigComponent>();
			var timerPool = world.GetPool<Timer>();
			var incomePool = world.GetPool<Income>();
			var addSoftPool = world.GetPool<AddSoftRequest>();
			var levelPool = world.GetPool<Level>();

			var businessFilter = world.Filter<BusinessViewComponent>()
				.Inc<ConfigComponent>()
				.Inc<Level>()
				.Inc<Timer>()
				.End();

			foreach (var entity in businessFilter)
			{
				ref var timer = ref timerPool.Get(entity);
				if (timer.Value <= 0)
				{
					timer.Value = configPool.Get(entity).config.Delay;

					var addSoftEntity = world.NewEntity();
					addSoftPool.Add(addSoftEntity);
					ref var addSoftRequest = ref addSoftPool.Get(addSoftEntity);
					var level = levelPool.Get(entity).Value;

					addSoftRequest.Amount = configPool.Get(entity).config.Income(level);
				}
				else
				{
					timer.Value -= UnityEngine.Time.deltaTime;
				}
			}
		}

		private void Deserialize(EcsWorld world, LoadedData data)
		{

		}

		private void SetStartParameters(EcsWorld world)
		{
			var configHolderPool = world.GetPool<ConfigHolderComponent>();
			var configPool = world.GetPool<ConfigComponent>();
			var levelPool = world.GetPool<Level>();
			var incomePool = world.GetPool<Income>();
			var timerPool = world.GetPool<Components.Timer>();

			var configHolderFilter = world.Filter<ConfigHolderComponent>().End();
			var configHolderComponent = configHolderPool.Get(configHolderFilter.GetRawEntities()[0]);

			var firstData = configHolderComponent.ConfigHolder.FirstPlayData;

			var businessEntities = world.Filter<ConfigComponent>()
				.Inc<Level>()
				.Inc<Timer>()
				.Inc<Income>()
				.End();

			foreach (var b in businessEntities)
			{
				var configComponent = configPool.Get(b);


				if (firstData.BusinessIds.Contains(configComponent.config.Id))
				{
					ref var level = ref levelPool.Get(b);
					level.Value = firstData.StartLevel;
				}

				ref var income = ref incomePool.Get(b);
				income.Value = configComponent.config.BaseIncome;
			}
		}
	}
}