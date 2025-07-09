using Leopotam.EcsLite;
using BusinessGame.Components;

public class SoftCurrencySystem : IEcsRunSystem
{
	public void Run(IEcsSystems systems)
	{
		var world = systems.GetWorld();

		// singleton (ID = 0)
		int currencyEntity = 0;
		var currencyPool = world.GetPool<SoftCurrencyComponent>();

		ref var currency = ref currencyPool.Get(currencyEntity);

		var addSoftPool = world.GetPool<AddSoftCurrencyComponent>();
		var addSoftFilter = world.Filter<AddSoftCurrencyComponent>().End();

		foreach (var entity in addSoftFilter)
		{
			ref var addRequest = ref addSoftPool.Get(entity);

			currency.Value += addRequest.Amount;
		}


		var spendRequestPool = world.GetPool<SpendSoftCurrencyRequest>();
		var upgradeRequestPool = world.GetPool<UpgradeRequest>();

		var filter = world.Filter<SpendSoftCurrencyRequest>().End();

		foreach (var entity in filter)
		{
			ref var request = ref spendRequestPool.Get(entity);

			if (currency.Value >= request.Amount)
			{
				currency.Value -= request.Amount;

				// Создаём UpgradeRequest-ивент для target (request.Target)
				var upgradeEntity = world.NewEntity();
				ref var upgradeRequest = ref upgradeRequestPool.Add(upgradeEntity);
				upgradeRequest.Target = request.Target; // <- смотри ниже комментарий

				// Можешь добавить дополнительные поля (например, уровень апгрейда)
			}
			else
			{
				// Здесь можешь создать Fail-ивент
			}
			world.DelEntity(entity);
		}
	}
}