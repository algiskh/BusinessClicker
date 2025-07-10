using UnityEngine;
using BusinessGame.Configs;
using Leopotam.EcsLite;
using BusinessGame.ECS.Components;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms;

namespace BusinessGame.UI
{

	public class BusinessView : MonoBehaviour
	{
		[SerializeField] private ConfigHolder _configHolder;
		[SerializeField] private TMP_Text _titleText;
		[SerializeField] private TMP_Text _levelField;
		[SerializeField] private TMP_Text _incomeField;
		[SerializeField] private TMP_Text _lvlUpField;
		[SerializeField] private Slider _slider;

		[SerializeField] private ButtonWrapper _levelUpButton;
		[SerializeField] private UpgradeView[] _upgradeButtons;

		private Config _config;

		public int EntityId { get; private set; }
		public EcsWorld World { get; private set; }
		public ButtonWrapper LevelUpButton => _levelUpButton;
		public UpgradeView[] UpgradeViews => _upgradeButtons;

		public void Initialize(int entityId, EcsWorld world, Config config)
		{
			EntityId = entityId;
			World = world;
			_config = config;
			_slider.maxValue = _config.Delay;
			UpdateStats();
		}

		public void UpdateStats()
		{
			var currentLevel = GetCurrentLevel();
			_levelField.text = currentLevel.ToString();
			_incomeField.text = $"{GetIncome()}$";
			_lvlUpField.text = $"Цена: {GetNextLevelUpPrice(currentLevel)}$";
		}

		public void UpdateSlider()
		{
			_slider.value = _config.Delay - GetTimerValue();
		}

		public void UpdateUpgrades()
		{
			var pool = World.GetPool<Upgrades>();
			var upgrades = pool.Get(EntityId);

			foreach (var upgrade in upgrades.Value)
			{
				for (int i = 0; i < _upgradeButtons.Length; i++)
				{
					if (_upgradeButtons[i].Hash == upgrade.Hash 
						&& upgrade.Hash != -1
						&& upgrade.IsObtained)
					{
						_upgradeButtons[i].UpdateView();

					}
				}
			}
		}

		private int GetCurrentLevel()
		{
			var pool = World.GetPool<Level>();
			var level = pool.Get(EntityId);
			return level.Value;
		}

		private long GetIncome()
		{
			var pool = World.GetPool<Income>();
			var income = pool.Get(EntityId);

			Debug.Log($"Доход сейчас {income.Value}");
			return income.Value;
		}

		private float GetTimerValue()
		{
			var pool = World.GetPool<Timer>();
			var timer = pool.Get(EntityId);
			return timer.Value;
		}

		private long GetNextLevelUpPrice(int level)
		{
			return _config.LevelUpPrice(level);
		}
	}
}