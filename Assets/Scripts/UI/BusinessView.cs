using UnityEngine;
using BusinessGame.Configs;
using Unity.Collections.LowLevel.Unsafe;
using Leopotam.EcsLite;
using BusinessGame.Components;

namespace BusinessGame.UI
{

	public class BusinessView : MonoBehaviour
	{
		[SerializeField] private ConfigHolder _configHolder;
		[SerializeField] private GameObject _businessPanel;
		[SerializeField] private TMPro.TMP_Text _titleText;
		[SerializeField] private TMPro.TMP_Text _levelField;
		[SerializeField] private TMPro.TMP_Text _incomeField;
		[SerializeField] private TMPro.TMP_Text _lvlUpField;

		[SerializeField] private ButtonWrapper[] _upgradeButtons;

		private Config _config;
		public int EntityId { get; private set; }
		public EcsWorld World { get; private set; }


		public void Init(int entityId, EcsWorld world, Config config)
		{
			EntityId = entityId;
			World = world;
			_config = config;
			UpdateView();
		}

		public void UpdateView()
		{

		}
	}
}