using BusinessGame.ECS.Components;
using Leopotam.EcsLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using SerializationData = BusinessGame.Serialization.SerializationData;
using Timer = BusinessGame.ECS.Components.Timer;

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

			if (world.TryGetAsSingleton<OnRequestSave>(out _))
			{
				world.DeleteAllWith<OnRequestSave>();
				TryToSave(world);
			}
		}

		private void TryToLoad(EcsWorld world)
		{
			var configHolder = world.GetAsSingleton<ConfigHolderComponent>();
			var savePath = configHolder.Value.SavePath;

			if (!File.Exists(savePath))
			{
				Debug.Log($"Save file not found: {savePath}");
				return;
			}

			try
			{
				var json = File.ReadAllText(savePath);
				var saveData = JsonUtility.FromJson<SerializationData>(json);

				if (saveData == null)
				{
					Debug.LogError("Failed to deserialize save file.");
					return;
				}

				ref var currency = ref world.GetAsSingleton<SoftCurrency>();
				currency.Value = saveData.SoftCurrency;

				ref var upgradeRequest = ref world.CreateEventEntity<UpdateSoftUI>();
				upgradeRequest.Value = currency.Value;

				var loadedDataEntity = world.NewEntity();
				ref var loadedData = ref world.GetPool<LoadedDataComponent>().Add(loadedDataEntity);
				loadedData.Value = saveData;

			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error loading save: {ex}");
			}
		}

		public void TryToSave(EcsWorld world)
		{
			Debug.Log($"Try to save from ECS!");
			var configHolder = world.GetAsSingleton<ConfigHolderComponent>();
			var savePath = configHolder.Value.SavePath;

			var currency = world.GetAsSingleton<SoftCurrency>();

			var configPool = world.GetPool<ConfigComponent>();
			var timerPool = world.GetPool<Timer>();
			var levelPool = world.GetPool<Level>();
			var upgradesPool = world.GetPool<Upgrades>();
			var businessViewPool = world.GetPool<BusinessViewComponent>();

			var filter = world.Filter<BusinessViewComponent>()
				.Inc<ConfigComponent>()
				.Inc<Level>()
				.Inc<Timer>()
				.Inc<Upgrades>()
				.End();

			var businessesToSave = new List<SerializationData.SerializedBusiness>();

			foreach (var entity in filter)
			{
				var upgrades = upgradesPool.Get(entity);

				var businessToSave = new SerializationData.SerializedBusiness
				{
					Id = configPool.Get(entity).Value.Id,
					Level = levelPool.Get(entity).Value,
					Timer = timerPool.Get(entity).Value,
					Upgrades = upgrades.Value.Select(b => b.IsObtained).ToArray()
				};

				businessesToSave.Add(businessToSave);
			}

			var newSave = new SerializationData
			{
				SoftCurrency = currency.Value,
				Businesses = businessesToSave.ToArray()
			};

			try
			{
				var json = JsonUtility.ToJson(newSave, true);
				File.WriteAllText(savePath, json);
				Debug.Log($"Save complete: {savePath}");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error saving file: {ex}");
			}
		}
	}
}
