using UnityEngine;
using BusinessGame.Configs;
using Leopotam.EcsLite;
using BusinessGame.ECS.Components;
using UnityEngine.UI;

namespace BusinessGame.UI
{

	public class BusinessView : MonoBehaviour
	{
		[SerializeField] private ConfigHolder _configHolder;
		[SerializeField] private TMPro.TMP_Text _titleText;
		[SerializeField] private TMPro.TMP_Text _levelField;
		[SerializeField] private TMPro.TMP_Text _incomeField;
		[SerializeField] private TMPro.TMP_Text _lvlUpField;
		[SerializeField] private Slider _slider;

		[SerializeField] private ButtonWrapper[] _upgradeButtons;

		private Config _config;

		public int EntityId { get; private set; }
		public EcsWorld World { get; private set; }


		public void Initialize(int entityId, EcsWorld world, Config config)
		{
			EntityId = entityId;
			World = world;
			_config = config;
			_slider.maxValue = _config.Delay;
			UpdateView();
		}

		public void UpdateView()
		{
			var currentLevel = GetCurrentLevel();
			_lvlUpField.text = currentLevel.ToString();
			_incomeField.text = GetIncome().ToString();
			_lvlUpField.text = $"Цена: {GetNextLevelUpPrice(currentLevel)}$";
			_slider.value = _config.Delay - GetTimerValue();
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