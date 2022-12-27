using UnityEngine;

public class Station : MonoBehaviour
{
	private void Awake()
	{
		StationManager.instance.FindPosition(base.transform);
		Invoke("AddToMap", 1f);
	}

	private void AddToMap()
	{
		for (int i = 0; i < HoloMap.holoMaps.Count; i++)
		{
			HoloMap.holoMaps[i].AddShip(base.transform);
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < HoloMap.holoMaps.Count; i++)
		{
			HoloMap.holoMaps[i].RemoveShip(base.transform);
		}
	}
}
