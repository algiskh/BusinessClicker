using BusinessGame.ECS.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace BusinessGame.ECS
{
	public class TimerSystem : IEcsRunSystem
	{
		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			var timerPool = world.GetPool<Timer>();
			var levelPool = world.GetPool<Level>();
			var timerFilter = world.Filter<Timer>().Inc<Level>().End();

			foreach (var entity in timerFilter)
			{
				var level = levelPool.Get(entity);

				if (level.Value < 1)
				{
					continue;
				}

				IterateTimer(entity, timerPool);
			}
		}

		private void IterateTimer(int entity, EcsPool<Timer> pool)
		{
			ref var comp = ref pool.Get(entity);
			var delta = Time.deltaTime;
			comp.Value -= delta;
		}
	}
}
