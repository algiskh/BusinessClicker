using BusinessGame.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BusinessGame.UI
{
	[CreateAssetMenu(
		fileName = "BusinessLocalization",
		menuName = "BusinessGame/Localization/BusinessLocalization"
	)]
	public class BusinessLocalization : ScriptableObject
	{
		[Serializable]
		public struct Entry
		{
			public string Key;
			[TextArea]
			public string Text; // Default EN text
		}

		[SerializeField] private Entry[] _entries;


		public string[] GetUpgradeKeys(Config config)
		{
			string prefix = $"{config.Id}_upgrade";
			return _entries
			.Where(e => e.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
			.OrderBy(e => e.Key)
			.Select(e => e.Key)
			.ToArray();
		}

		public string GetText(string key)
		{
			return _entries.FirstOrDefault(e => e.Key == key).Text;
		}

		public Entry[] GetBusinessEntries(Config config)
		{
			string prefix = $"{config.Id}_";
			return _entries
				.Where(e => e.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				.ToArray();
		}
	}
}