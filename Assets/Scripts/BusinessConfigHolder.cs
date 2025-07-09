using BusinessGame.UI;
using UnityEngine;

namespace BusinessGame.Configs
{
	[CreateAssetMenu(
		fileName = "BusinessConfigHolder",
		menuName = "Configs/Business Config Holder", order = 1
		)]
	public class ConfigHolder : ScriptableObject
    {
		[SerializeField] private BusinessLocalization _localization;
		[SerializeField] private Config[] _configs;
        public Config[] Configs => _configs;

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