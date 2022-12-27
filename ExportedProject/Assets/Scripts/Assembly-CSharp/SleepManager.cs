using PreviewLabs;
using UnityEngine;
using VRTK;

public class SleepManager : MonoBehaviour
{
	public void OnSleep()
	{
		VRTK_ScreenFade.Start(Color.black, 3f);
		Invoke("RespawnWorld", 4f);
		HintManager.instance.sleep1 = true;
		HintManager.instance.sleep2 = true;
		PreviewLabs.PlayerPrefs.SetBool("sleep1", true);
		PreviewLabs.PlayerPrefs.SetBool("sleep2", true);
	}

	private void RespawnWorld()
	{
		Camera.main.farClipPlane = 1f;
		ClearOldStations();
		ClearOldLists();
		SpawnNewStations();
		WreckManager.instance.ClearWrecks();
		WreckManager.instance.OnWreckSpawn();
		Invoke("Resume", 1f);
		VRTK_ScreenFade.Start(Color.clear, 2f);
	}

	private void Resume()
	{
		Camera.main.farClipPlane = 1000f;
	}

	private void ClearOldStations()
	{
		for (int i = 0; i < StationManager.instance.spawnedStations.Count; i++)
		{
			Object.Destroy(StationManager.instance.spawnedStations[i].gameObject);
		}
		StationManager.instance.spawnedStations.Clear();
	}

	private void SpawnNewStations()
	{
		StationManager.instance.OnSpawnStations();
	}

	private void ClearOldLists()
	{
		SpawnNode.spawnNodes.Clear();
	}
}
