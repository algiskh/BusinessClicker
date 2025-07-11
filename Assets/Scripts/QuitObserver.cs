using BusinessGame.ECS.Components;
using Leopotam.EcsLite;
using UnityEngine;

public class QuitObserver : MonoBehaviour
{
	private EcsWorld _world;
	public void Initialize(EcsWorld world)
	{
		_world = world;
	}

	void OnApplicationQuit()
	{
		RequestSave();
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
			RequestSave();
	}

	void OnApplicationFocus(bool focus)
	{
		if (!focus)
			RequestSave();
	}

	private void RequestSave()
	{
		if (_world != null)
		{
			var entity = _world.NewEntity();
			ref var requestSave = ref _world.GetPool<RequestSave>().Add(entity);
			requestSave.IgnoreCooldown = true;
		}
	}

}
