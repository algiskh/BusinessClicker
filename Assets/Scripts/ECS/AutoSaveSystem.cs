using Leopotam.EcsLite;
using UnityEngine;
using BusinessGame.ECS.Components;
using BusinessGame.ECS;

public class AutoSaveSystem : IEcsInitSystem, IEcsRunSystem
{
	public void Init(IEcsSystems systems)
	{
		var world = systems.GetWorld();
		var configHolder = world.GetAsSingleton<ConfigHolderComponent>();
		var entity = world.NewEntity();
		ref var timer = ref world.GetPool<AutoSaveTimer>().Add(entity);
		timer.NextSaveTime = Time.time + configHolder.Value.AutoSaveCoolDown;
	}

	public void Run(IEcsSystems systems)
	{
		var world = systems.GetWorld();

		var configHolder = world.GetAsSingleton<ConfigHolderComponent>();
		var interval = configHolder.Value.AutoSaveCoolDown;

		ref var autosaveTimer = ref world.GetAsSingleton<AutoSaveTimer>();

		if (Time.time >= autosaveTimer.NextSaveTime)
		{
			var reqEntity = world.NewEntity();
			ref var request = ref world.GetPool<RequestSave>().Add(reqEntity);
			request.IgnoreCooldown = false;

			autosaveTimer.NextSaveTime = Time.time + interval;
		}
	}
}
