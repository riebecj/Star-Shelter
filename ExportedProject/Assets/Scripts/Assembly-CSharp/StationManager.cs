using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StationManager : MonoBehaviour
{
	public static StationManager instance;

	public int stationCount = 5;

	public string[] SpaceShips;

	public Transform[] stationPositions;

	private List<int> values = new List<int>();

	internal List<Transform> spawnPositions = new List<Transform>();

	public List<Transform> spawnedStations = new List<Transform>();

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		OnSpawnStations();
	}

	public void ResetPostions()
	{
		spawnPositions.Clear();
		Transform[] array = stationPositions;
		foreach (Transform item in array)
		{
			spawnPositions.Add(item);
		}
	}

	public void OnSpawnStations()
	{
		ResetPostions();
		values.Clear();
		for (int i = 0; i < SpaceShips.Length; i++)
		{
			values.Add(i);
		}
		for (int j = 0; j < stationCount; j++)
		{
			int num = values[Random.Range(0, values.Count)];
			values.Remove(num);
			SceneManager.LoadSceneAsync(SpaceShips[num], LoadSceneMode.Additive);
		}
	}

	public void FindPosition(Transform station)
	{
		int index = Random.Range(0, spawnPositions.Count);
		Transform transform = spawnPositions[index];
		DistanceCheck(spawnPositions[index].position);
		spawnPositions.Remove(transform);
		station.position = transform.position;
		station.rotation = transform.rotation;
		station.eulerAngles = new Vector3(station.eulerAngles.x, Random.Range(0, 360), station.eulerAngles.z);
		spawnedStations.Add(station);
	}

	private void DistanceCheck(Vector3 goal)
	{
		foreach (Transform spawnedStation in spawnedStations)
		{
			if (Vector3.Distance(spawnedStation.position, goal) < 50f)
			{
				Debug.Log("Ships Too Close!!! Check");
			}
		}
	}
}
