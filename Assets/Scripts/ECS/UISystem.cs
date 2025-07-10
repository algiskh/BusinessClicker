using BusinessGame.ECS.Components;
using BusinessGame.UI;
using Leopotam.EcsLite;
using System.Collections.Generic;

namespace BusinessGame.ECS
{
	/// <summary>
	/// Последняя из систем, которая будет выполняться в кадре
	/// </summary>
	public class UISystem : IEcsInitSystem, IEcsRunSystem
	{
		public void Init(IEcsSystems systems) { }

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			// +++++ Pools +++++
			var businessViewPool = world.GetPool<BusinessViewComponent>();
			var configPool = world.GetPool<ConfigComponent>();
			var requestSpendPool = world.GetPool<RequestSpendSoft>();
			var levelPool = world.GetPool<Level>();
			var updateViewPool = world.GetPool<UpdateViewRequest>();
			var upgradesPool = world.GetPool<Upgrades>();

			// +++++ Refresh Top Panel +++++
			var updateUIFilter = world.Filter<UpdateSoftUI>().End();
			if (world.TryGetAsSingleton<UpdateSoftUI>(out var update))
			{
				ref var topPanel = ref world.GetAsSingleton<TopPanelComponent>();
				topPanel.Value.UpdateView(update.Value);

				world.DeleteAllWith<UpdateSoftUI>();
			}

			// +++++ Get all entities to update
			var entitiesToUpdate = new HashSet<int>();
			foreach (var reqEntity in world.Filter<UpdateViewRequest>().End())
			{
				var target = updateViewPool.Get(reqEntity).Target;
				entitiesToUpdate.Add(target);
			}

			// +++++ Update business views
			foreach (var entity in world.Filter<BusinessViewComponent>()
										.Inc<ConfigComponent>()
										.Inc<Level>()
										.Inc<Upgrades>()
										.End())
			{
				ref var businessView = ref businessViewPool.Get(entity);
				var button = businessView.Value.LevelUpButton;
				var upgradeButtons = businessView.Value.UpgradeViews;

				ref var config = ref configPool.Get(entity);
				ref var level = ref levelPool.Get(entity);

				for (var i = 0; i < upgradeButtons.Length; i++)
				{
					var UpgradeView = upgradeButtons[i];
					if (UpgradeView.UpgradeButton.IsClicked)
					{
						UnityEngine.Debug.Log($"Клик по кнопке Upgrade для {config.Value.Id} (уровень {level.Value}) для {entity}");
						int request = world.NewEntity();
						ref var requestSpend = ref requestSpendPool.Add(request);
						ref var upgrades = ref upgradesPool.Get(entity);
						requestSpend.Amount = UpgradeView.Config.Price;
						requestSpend.TargetEntity = entity;
						requestSpend.Purpose = SpendPurpose.Upgrade;
						requestSpend.AdditionalTarget = i; // Upgrade index
						UpgradeView.UpgradeButton.Unclick();
					}

				}

				// Click handling
				if (button.IsClicked)
				{
					UnityEngine.Debug.Log($"Клик по кнопке LevelUp для {config.Value.Id} (уровень {level.Value}) для {entity}");
					int request = world.NewEntity();
					ref var requestSpend = ref requestSpendPool.Add(request);
					requestSpend.Amount = config.Value.LevelUpPrice(level.Value);
					requestSpend.TargetEntity = entity;
					requestSpend.Purpose = SpendPurpose.LevelUp;
					button.Unclick();
				}

				if (entitiesToUpdate.Count > 0)
					UnityEngine.Debug.Log($"Есть {entitiesToUpdate.Count} для обновления.");

				// Обновление view
				if (entitiesToUpdate.Contains(entity))
				{
					UnityEngine.Debug.Log($"Обновляем UI");
					businessView.Value.UpdateStats();
					businessView.Value.UpdateUpgrades();
				}
			}

			// --- 4. Удаляем все запросы на обновление view одним проходом ---
			world.DeleteAllWith<UpdateViewRequest>();
		}
	}
}
