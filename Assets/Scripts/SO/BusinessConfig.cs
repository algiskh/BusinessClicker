using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BusinessGame.Configs
{
	[CreateAssetMenu(
		   fileName = "BusinessConfig",
		   menuName = "Configs/BusinessConfig", order = 1
		   )]
	public class Config : ScriptableObject
	{
		[Serializable]
		public struct UpgradeConfig 
		{
			[NonSerialized] public bool IsObtained;
			[NonSerialized] public int Hash;
			public long Price;
			public float Multiplier;
		}

		[SerializeField] private string _id;
		[SerializeField] private float _baseDelay;
		[SerializeField] private long _baseCost;
		[SerializeField] private long _baseIncome;
		[SerializeField] private UpgradeConfig[] _upgrades = new UpgradeConfig[2];

		public string Id => _id;
		public float Delay => _baseDelay;
		public long BaseIncome => _baseIncome;
		public UpgradeConfig[] Upgrades => _upgrades;

		public long GetIncome(int level, IEnumerable<UpgradeConfig> upgrades)
		{
			var upgradesMultiplier = 1 + upgrades.Where(u => u.IsObtained).Sum(b => b.Multiplier);
			return (long)(level * _baseIncome * upgradesMultiplier);
		}

		public long LevelUpPrice(int level)
		{
			return (long)(_baseCost * (level + 1));
		}

		public UpgradeConfig[] GetUpgradesCopy()
		{
			return _upgrades.ToArray();
		}
	}
}