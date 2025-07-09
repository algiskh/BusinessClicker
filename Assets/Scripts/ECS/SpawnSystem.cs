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

		var parentPool = world.GetPool<ObjectsParent>();
		var timerPool = world.GetPool<Timer>();
		var incomePool = world.GetPool<Income>();
		var levelPool = world.GetPool<Level>();


		var configHolderComponent = configHolderPool.Get(configEntity);
		var objectsParentComponent = parentPool.Get(configEntity);

		foreach (var config in configHolderComponent.ConfigHolder.Configs)
		{
			var businessEntity = world.NewEntity();

			var businessView = Object.Instantiate(
				configHolderComponent.ConfigHolder.Prefab,
				objectsParentComponent.Parent);

			ref var configComponent = ref configPool.Add(businessEntity);
			configComponent.config = config;

			ref var timer = ref timerPool.Add(businessEntity);
			timer.Value = config.Delay;

			ref var income = ref incomePool.Add(businessEntity);
			
			ref var level = ref levelPool.Add(businessEntity);

			businessView.Initialize(businessEntity, world, config);
		}
	}
}