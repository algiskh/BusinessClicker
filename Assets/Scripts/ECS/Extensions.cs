using Leopotam.EcsLite;

namespace BusinessGame.ECS
{
	public static class Extensions
	{
		public static ref T GetAsSingleton<T>(this EcsWorld world) where T : struct
		{
			var pool = world.GetPool<T>();
			var filter = world.Filter<T>().End();

			var entities = filter.GetRawEntities();
			var count = filter.GetEntitiesCount();
			if (count == 0)
				throw new System.InvalidOperationException($"No singleton component of type {typeof(T).Name} found!");

			return ref pool.Get(entities[0]);
		}

		public static bool TryGetAsSingleton<T>(this EcsWorld world, out T value) where T : struct
		{
			var pool = world.GetPool<T>();
			var filter = world.Filter<T>().End();

			var entities = filter.GetRawEntities();
			var count = filter.GetEntitiesCount();
			if (count > 0)
			{
				value = pool.Get(entities[0]);
				return true;
			}
			value = default;
			return false;
		}

		public static void DeleteAllWith<T>(this EcsWorld world) where T : struct
		{
			var filter = world.Filter<T>().End();
			foreach (var entity in filter)
			{
				world.DelEntity(entity);
			}
		}

		public static ref T CreateEventEntity<T>(this EcsWorld world) where T : struct
		{
			var pool = world.GetPool<T>();
			var entity = world.NewEntity();
			return ref pool.Add(entity);
		}
	}
}