using Leopotam.EcsLite;
using BusinessGame.ECS.Components;

namespace BusinessGame.ECS
{
	/// <summary>
	/// Принимаем выплачиваемую игровую валюту
	/// Принимаем запросы на трату игровой валюты
	/// Если хватает, то списываем валюту и отправляем одобрения
	/// </summary>
	public class SoftCurrencySystem : IEcsRunSystem
	{
		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			int currencyEntity = 0;
			var currencyPool = world.GetPool<SoftCurrency>();

			ref var currency = ref currencyPool.Get(currencyEntity);

			var change = TryAddSoft(world) - TrySpendSoft(world, currency.Value);

			currency.Value += change;
			if (change != 0)
			{
				SendUIUpdate(world, currency.Value);
			}
		}

		private long TryAddSoft(EcsWorld world)
		{
			var addSoftPool = world.GetPool<AddSoftRequest>();
			var addSoftFilter = world.Filter<AddSoftRequest>().End();
			long addSoft = 0;

			foreach (var entity in addSoftFilter)
			{
				ref var addRequest = ref addSoftPool.Get(entity);

				addSoft += addRequest.Amount;
			}
			return addSoft > 0 ? addSoft : 0;
		}

		private long TrySpendSoft(EcsWorld world, long soft)
		{
			var requestSpendPool = world.GetPool<RequestSpendSoft>();

			var filter = world.Filter<RequestSpendSoft>().End();
			long spendSoft = 0;
			foreach (var entity in filter)
			{
				ref var request = ref requestSpendPool.Get(entity);

				if (soft >= request.Amount)
				{
					spendSoft += request.Amount;

					SendApprove(world, request.Target);
				}
				else
				{
					// FAIL EVENT
				}
				world.DelEntity(entity);
			}
			return spendSoft;
		}

		private void SendUIUpdate(EcsWorld world, long soft)
		{
			var updateSoftPool = world.GetPool<UpdateSoftUI>();

			var updateSoftUIEntity = world.NewEntity();
			ref var upgradeRequest = ref updateSoftPool.Add(updateSoftUIEntity);
			upgradeRequest.Value = soft;
		}

		private void SendApprove(EcsWorld world, int targetEntity)
		{
			var approveSpendPool = world.GetPool<ApproveSpendSoft>();

			var upgradeEntity = world.NewEntity();
			ref var upgradeRequest = ref approveSpendPool.Add(upgradeEntity);
			upgradeRequest.Target = targetEntity;
		}
	}
}