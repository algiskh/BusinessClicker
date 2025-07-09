using BusinessGame.Configs;
using BusinessGame.ECS;
using BusinessGame.ECS.Components;
using BusinessGame.UI;
using Leopotam.EcsLite;
using UnityEngine;


public class EntryPoint : MonoBehaviour
{
    [SerializeField] private ConfigHolder _configHolder;
    [SerializeField] private Transform _slotsParent;
	[SerializeField] private TopPanel _topPanel;

	private EcsWorld _world;
	private IEcsSystems _systems;

	void Awake()
	{
		_world = new EcsWorld();
		_systems = new EcsSystems(_world);

		SetupSpawnData();

		_systems
			.Add(new SaveLoadSystem())
			.Add(new SpawnSystem())
			.Add(new BusinessSystem())
			.Add(new TimerSystem())
			.Add(new SoftCurrencySystem())
			.Add(new UISystem())

			.Init();
	}

	private void SetupSpawnData()
	{
		var appEntity = _world.NewEntity();
		var configPool = _world.GetPool<ConfigHolderComponent>();
		var parentPool = _world.GetPool<ObjectsParent>();
		var topPanelPool = _world.GetPool<TopPanelComponent>();

		ref var config = ref configPool.Add(appEntity);
		ref var parent = ref parentPool.Add(appEntity);
		ref var topPanel = ref topPanelPool.Add(appEntity);


		config.ConfigHolder = _configHolder;
		parent.Parent = _slotsParent;
	}

	private void OnDestroy()
	{
		if (_systems != null)
		{
			_systems.Destroy();
			_systems = null;
		}
		if (_world != null)
		{
			_world.Destroy();
			_world = null;
		}
	}
}
