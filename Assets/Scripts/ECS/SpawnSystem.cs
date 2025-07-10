using BusinessGame.ECS.Components;
using Leopotam.EcsLite;
using UnityEngine;
using Timer = BusinessGame.ECS.Components.Timer;

public class SpawnSystem : IEcsInitSystem // Можно убрать IEcsRunSystem, если не нужен
{
	public void Init(IEcsSystems systems)
	{
		var world = systems.GetWorld();

		var filter = world.Filter<ConfigHolderComponent>()
			.Inc<ObjectsParent>()
			.End();

		foreach (var entity in filter)
		{
			CreateInitialSlots(world, entity);

			//world.DelEntity(entity);
			break; // Допустимо, если уверены, что entity ровно один
		}
	}

	private void CreateInitialSlots(EcsWorld world, int configEntity)
	{
		var configHolderPool = world.GetPool<ConfigHolderComponent>();
		var configPool = world.GetPool<ConfigComponent>();
		var businessViewPool = world.GetPool<BusinessViewComponent>();
		var upgradesPool = world.GetPool<Upgrades>();

		var parentPool = world.GetPool<ObjectsParent>();
		var timerPool = world.GetPool<Timer>();
		var incomePool = world.GetPool<Income>();
		var levelPool = world.GetPool<Level>();

		var mainConfig = configHolderPool.Get(configEntity);
		var objectsParentComponent = parentPool.Get(configEntity);

		foreach (var config in mainConfig.Value.Configs)
		{
			var businessEntity = world.NewEntity();

			var businessView = Object.Instantiate(
				mainConfig.Value.Prefab,
				objectsParentComponent.Parent);

			ref var configComponent = ref configPool.Add(businessEntity);
			configComponent.Value = config;

			ref var timer = ref timerPool.Add(businessEntity);
			timer.Value = config.Delay;

			ref var income = ref incomePool.Add(businessEntity);
			income.Value = config.BaseIncome;

			ref var level = ref levelPool.Add(businessEntity);

			ref var businessViewComponent = ref businessViewPool.Add(businessEntity);

			businessViewComponent.Value = businessView;

			ref var upgrades = ref upgradesPool.Add(businessEntity);
			upgrades.Value = config.GetUpgradesCopy();

			for (var i = 0; i < upgrades.Value.Length; i++)
			{
				upgrades.Value[i].Hash = businessView.UpgradeViews[i] != null
					? businessView.UpgradeViews[i].Hash
					: -1;
				businessViewComponent.Value.UpgradeViews[i].Initialize(upgrades.Value[i], mainConfig.Value.GetUpgradeKeys(config)[i]);
			}

			businessView.Initialize(businessEntity, world, config, mainConfig.Value.GetTitle(config));
		}
	}
}