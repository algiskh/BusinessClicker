using BusinessGame.UI;
using System;
using System.IO;
using UnityEngine;

namespace BusinessGame.Configs
{
	[CreateAssetMenu(
		fileName = "BusinessConfigHolder",
		menuName = "Configs/Business Config Holder", order = 1
		)]
	public class ConfigHolder : ScriptableObject
    {
		[Serializable]
		public struct FirstTimeData
		{
			public string[] BusinessIds;
			public int StartLevel;
		}

		[SerializeField] private BusinessLocalization _localization;
		[SerializeField] private Config[] _configs;
		[SerializeField] private FirstTimeData _firstTimeData;
		[SerializeField] private BusinessView _prefab;
		[SerializeField] private string _saveName = "save.json";

		public Config[] Configs => _configs;
		public BusinessView Prefab => _prefab;
		public FirstTimeData FirstPlayData => _firstTimeData;
		public string SavePath => Path.Combine(Application.persistentDataPath, _saveName);

		public string GetTitle(Config config)
		{
			return _localization.GetTitle(config);
		}
		public string[] GetUpgradeKeys(Config config)
		{
			return _localization.GetUpgradeKeys(config);
		}

		public Config GetConfig(string id)
		{
			foreach (var config in _configs)
			{
				if (config.Id == id)
				{
					return config;
				}
			}
			Debug.LogError($"Config with ID {id} not found.");
			return null;
		}
	}
}