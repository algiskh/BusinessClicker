using System;
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
		private class Upgrade
		{
			public float Price;
			public float Multiplier;
		}

		[SerializeField] private string _id;
		[SerializeField] private float _baseDelay;
		[SerializeField] private long _baseCost;
		[SerializeField] private long _baseIncome;
		[SerializeField] private Upgrade[] _upgrades = new Upgrade[2];

		public string Id => _id;
        public float Delay => _baseDelay;
		public long BaseCost => _baseCost;
		public long BaseIncome => _baseIncome;
		public long Income (int level) => (long)(level * _baseIncome * (1 + _upgrades.Sum(b => b.Multiplier)));
		public long LevelUpPrice(int level)
		{
			if (level < 1 || level > _upgrades.Length)
			{
				return _baseCost;
			}
			return (long)(_baseCost * _upgrades[level - 1].Price);
		}
	}
}