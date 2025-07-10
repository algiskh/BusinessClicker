using Leopotam.EcsLite;
using BusinessGame.ECS.Components;
using System.Collections.Generic;
using Unity.Collections;

namespace BusinessGame.ECS
{
	public class SoftCurrencySystem : IEcsInitSystem, IEcsRunSystem
	{
		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			var currencyEntity = world.NewEntity();
			var currencyPool = world.GetPool<SoftCurrency>();
			ref var currency = ref currencyPool.Add(currencyEntity);

		}

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			var currencyPool = world.GetPool<SoftCurrency>();
			var currencyFilter = world.Filter<SoftCurrency>().End();

			foreach (var entity in currencyFilter)
			{
				IterateCurrency(world, entity, currencyPool);
			}
		}

		private void IterateCurrency(EcsWorld world, int entity, EcsPool<SoftCurrency> currencyPool)
		{
			ref var currency = ref currencyPool.Get(entity);
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

			var entitiesToRemove = new List<int>();
			foreach (var entity in addSoftFilter)
			{
				ref var addRequest = ref addSoftPool.Get(entity);

				addSoft += addRequest.Amount;
				entitiesToRemove.Add(entity);
			}
			entitiesToRemove.ForEach(e => world.DelEntity(e));

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
					request.IsApproved = true;
				}
				else
				{
					world.DelEntity(entity);
				}
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