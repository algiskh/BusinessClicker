using BusinessGame.ECS.Components;
using Leopotam.EcsLite;
using System.Diagnostics;

namespace BusinessGame.ECS
{
	public class SaveLoadSystem : IEcsInitSystem, IEcsRunSystem
	{
		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			TryToLoad(world);
		}

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			var saveFilter = world.Filter<OnRequestSave>().End();
			if (saveFilter.GetEntitiesCount() > 0)
			{
				foreach (var entity in saveFilter)
				{
					world.DelEntity(entity);
				}
				TryToSave(world);
			}
		}

		private void TryToLoad(EcsWorld world)
		{
			UnityEngine.Debug.Log($"Try To Load");

			return;
			var hasBeenLoaded = world.NewEntity(); // push serialized data further in the pipeline
			var loadPool = world.GetPool<LoadedData>();
			ref var loaded = ref loadPool.Add(hasBeenLoaded);

			// loaded = serialized data;
		}


		public void TryToSave(EcsWorld world)
		{
			// send save
		}
	}
}