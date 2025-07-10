using BusinessGame.ECS.Components;
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

			UpdateTopPanelIfNeeded(world);

			var updateViewPool = world.GetPool<UpdateViewRequest>();
			bool updateAll = UpdateAllViews(world, updateViewPool, out var entitiesToUpdate);

			UpdateBusinessViews(world, updateAll, entitiesToUpdate);

			world.DeleteAllWith<UpdateViewRequest>();
		}

		private void UpdateTopPanelIfNeeded(EcsWorld world)
		{
			if (world.TryGetAsSingleton<UpdateSoftUI>(out var update))
			{
				ref var topPanel = ref world.GetAsSingleton<TopPanelComponent>();
				topPanel.Value.UpdateView(update.Value);
				world.DeleteAllWith<UpdateSoftUI>();
			}
		}

		private bool UpdateAllViews(EcsWorld world, EcsPool<UpdateViewRequest> updateViewPool, out HashSet<int> entitiesToUpdate)
		{
			entitiesToUpdate = new HashSet<int>();
			bool isGlobal = false;

			foreach (var reqEntity in world.Filter<UpdateViewRequest>().End())
			{
				ref var req = ref updateViewPool.Get(reqEntity);
				if (req.IsGlobal)
				{
					isGlobal = true;
					break;
				}
				entitiesToUpdate.Add(req.Target);
			}

			return isGlobal;
		}

		private void UpdateBusinessViews(EcsWorld world, bool updateAll, HashSet<int> entitiesToUpdate)
		{
			var businessViewPool = world.GetPool<BusinessViewComponent>();
			var configPool = world.GetPool<ConfigComponent>();
			var requestSpendPool = world.GetPool<RequestSpendSoft>();
			var levelPool = world.GetPool<Level>();
			var upgradesPool = world.GetPool<Upgrades>();

			foreach (var entity in world.Filter<BusinessViewComponent>()
										.Inc<ConfigComponent>()
										.Inc<Level>()
										.Inc<Upgrades>()
										.End())
			{
				ref var businessView = ref businessViewPool.Get(entity);

				HandleUpgradeClicks(world, entity, ref businessView, requestSpendPool, upgradesPool, configPool, levelPool);
				HandleLevelUpClick(world, entity, ref businessView, requestSpendPool, configPool, levelPool);

				if (updateAll || entitiesToUpdate.Contains(entity))
				{
					businessView.Value.UpdateStats();
					businessView.Value.UpdateUpgrades();
				}
			}
		}

		private void HandleUpgradeClicks(
			EcsWorld world,
			int entity,
			ref BusinessViewComponent businessView,
			EcsPool<RequestSpendSoft> requestSpendPool,
			EcsPool<Upgrades> upgradesPool,
			EcsPool<ConfigComponent> configPool,
			EcsPool<Level> levelPool)
		{
			var upgradeViews = businessView.Value.UpgradeViews;
			ref var config = ref configPool.Get(entity);
			ref var level = ref levelPool.Get(entity);

			for (var i = 0; i < upgradeViews.Length; i++)
			{
				var upgradeView = upgradeViews[i];
				if (upgradeView.UpgradeButton.IsClicked)
				{
					int request = world.NewEntity();
					ref var requestSpend = ref requestSpendPool.Add(request);
					requestSpend.Amount = upgradeView.Config.Price;
					requestSpend.TargetEntity = entity;
					requestSpend.Purpose = SpendPurpose.Upgrade;
					requestSpend.AdditionalTarget = i;
					upgradeView.UpgradeButton.Unclick();
				}
			}
		}

		private void HandleLevelUpClick(
			EcsWorld world,
			int entity,
			ref BusinessViewComponent businessView,
			EcsPool<RequestSpendSoft> requestSpendPool,
			EcsPool<ConfigComponent> configPool,
			EcsPool<Level> levelPool)
		{
			var levelUpButton = businessView.Value.LevelUpButton;
			ref var config = ref configPool.Get(entity);
			ref var level = ref levelPool.Get(entity);

			if (levelUpButton.IsClicked)
			{
				int request = world.NewEntity();
				ref var requestSpend = ref requestSpendPool.Add(request);
				requestSpend.Amount = config.Value.LevelUpPrice(level.Value);
				requestSpend.TargetEntity = entity;
				requestSpend.Purpose = SpendPurpose.LevelUp;
				levelUpButton.Unclick();
			}
		}
	}
}
