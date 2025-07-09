using BusinessGame.ECS.Components;
using BusinessGame.UI;
using System;
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
		public Config[] Configs => _configs;
		public BusinessView Prefab => _prefab;
		public FirstTimeData FirstPlayData => _firstTimeData;

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