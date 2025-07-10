using BusinessGame.ECS.Components;
using BusinessGame.UI;
using Leopotam.EcsLite;
using System.Collections.Generic;

namespace BusinessGame.ECS
{
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
			if (world.TryGetAsSingleton<UpdateSoftUI>(out var update))
			{
				ref var topPanel = ref world.GetAsSingleton<TopPanelComponent>();
				topPanel.Value.UpdateView(update.Value);

				world.DeleteAllWith<UpdateSoftUI>();
			}

			// +++++ Get all entities to update +++++
			var entitiesToUpdate = new HashSet<int>();
			bool isGlobalUpdate = false;

			foreach (var reqEntity in world.Filter<UpdateViewRequest>().End())
			{
				ref var req = ref updateViewPool.Get(reqEntity);
				if (req.IsGlobal)
				{
					isGlobalUpdate = true;
				}
				else
				{
					entitiesToUpdate.Add(req.Target);
				}
			}

			// +++++ Update business views +++++
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
					var upgradeView = upgradeButtons[i];
					if (upgradeView.UpgradeButton.IsClicked)
					{
						UnityEngine.Debug.Log($"Клик по кнопке Upgrade для {config.Value.Id} (уровень {level.Value}) для {entity}");
						int request = world.NewEntity();
						ref var requestSpend = ref requestSpendPool.Add(request);
						ref var upgrades = ref upgradesPool.Get(entity);
						requestSpend.Amount = upgradeView.Config.Price;
						requestSpend.TargetEntity = entity;
						requestSpend.Purpose = SpendPurpose.Upgrade;
						requestSpend.AdditionalTarget = i; // Upgrade index
						upgradeView.UpgradeButton.Unclick();
					}
				}

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

				if (isGlobalUpdate || entitiesToUpdate.Contains(entity))
				{
					businessView.Value.UpdateStats();
					businessView.Value.UpdateUpgrades();
				}
			}

			world.DeleteAllWith<UpdateViewRequest>();
		}
	}
}