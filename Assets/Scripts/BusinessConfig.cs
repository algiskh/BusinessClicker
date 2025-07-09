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
		[SerializeField] private float _baseCost;
		[SerializeField] private float _baseIncome;
		[SerializeField] private Upgrade[] _upgrades = new Upgrade[2];

		public string Id => _id;
        public float BaseDelay => _baseDelay;
		public float BaseCost => _baseCost;
		public float BaseIncome => _baseIncome;
		public float Income (int level) => level * _baseIncome * (1 + _upgrades.Sum(b => b.Multiplier));
	}
}