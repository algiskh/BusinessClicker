using BusinessGame.Configs;
using BusinessGame.ECS;
using BusinessGame.ECS.Components;
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

		// Add observer for application quit and unfocus
		gameObject.AddComponent<QuitObserver>().Initialize(_world);

		SetupSpawnData();

		_systems
			.Add(new SpawnSystem())
			.Add(new SoftCurrencySystem())
			.Add(new SaveLoadSystem())
			.Add(new AutoSaveSystem())
			.Add(new BusinessSystem())
			.Add(new UISystem())

			.Init();
	}

	private void Update()
	{
		_systems?.Run();
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


		config.Value = _configHolder;
		parent.Parent = _slotsParent;
		topPanel.Value = _topPanel;
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
