using BusinessGame.ECS.Components;
using Leopotam.EcsLite;
using Unity.Collections.LowLevel.Unsafe;

namespace BusinessGame.ECS
{
	/// <summary>
	/// ѕоследн€€ из систем, котора€ будет выполн€тьс€ в кадре
	/// </summary>
	public class UISystem : IEcsInitSystem, IEcsRunSystem
	{
		public void Init(IEcsSystems systems)
		{

		}

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			var updateUIFilter = world.Filter<UpdateSoftUI>().End();

			if (updateUIFilter.GetEntitiesCount() > 0)
			{
				var updateEntity = updateUIFilter.GetRawEntities()[0];
				UpdateTopPanel(world, updateEntity);
				foreach (var entity in updateUIFilter)
				{
					world.DelEntity(entity);
				}
			}
		}

		private void UpdateTopPanel(EcsWorld world, int entity)
		{
			var topPanelPool = world.GetPool<TopPanelComponent>();
			var updateUIPool = world.GetPool<UpdateSoftUI>();

			var filterTopPanel = world.Filter<TopPanelComponent>()
				.End();
			if (filterTopPanel.GetEntitiesCount() > 0)
			{
				foreach (var panelEntity in filterTopPanel)
				{
					ref var topPanel = ref topPanelPool.Get(panelEntity);
					var updateUI = updateUIPool.Get(entity); // mark as updated
					topPanel.Value.UpdateView(updateUI.Value);
				}

			}
		}
	}
} 