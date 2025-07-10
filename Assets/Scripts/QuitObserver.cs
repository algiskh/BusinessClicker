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
			Debug.Log($"Try to save!");
			var entity = _world.NewEntity();
			var requestSave = _world.GetPool<OnRequestSave>().Add(entity);
		}
	}

}
