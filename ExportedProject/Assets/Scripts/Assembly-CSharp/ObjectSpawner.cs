using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	public Transform ObjectPrefab;

	public float MaxSpawnDistance = 3f;

	public float SpawnDelay = 0.1f;

	public VRLever EnergyLever;

	public VRLever PistonLever;

	public int SpawnNumber = 10;

	protected bool isSpawning;

	public bool test;

	public void Activate()
	{
		Debug.Log("Activingting" + EnergyLever.Value);
		if (EnergyLever.Value * (float)SpawnNumber > 1f)
		{
			PistonLever.Value = 0f;
			PistonLever.Interactable = false;
			EnergyLever.Interactable = false;
			StartCoroutine(SpawnObjects());
			GetComponent<AudioSource>().Play();
		}
		else
		{
			Debug.Log("ObjectSpawner: not enough spawns");
		}
	}

	public Vector3 GetRandomPointAroundPosition(Vector3 _pos, float _maxDistance)
	{
		_pos.x += Random.Range(0f - _maxDistance, _maxDistance);
		_pos.y += Random.Range(0f - _maxDistance, _maxDistance);
		_pos.z += Random.Range(0f - _maxDistance, _maxDistance);
		return _pos;
	}

	private IEnumerator SpawnObjects()
	{
		if (!isSpawning)
		{
			yield return new WaitForSeconds(1.5f);
			int spawnCount = (int)(EnergyLever.Value * (float)SpawnNumber);
			float energyCost = EnergyLever.Value / (float)SpawnNumber;
			while (spawnCount > 0)
			{
				Object.Instantiate(ObjectPrefab, GetRandomPointAroundPosition(base.transform.position, MaxSpawnDistance), Quaternion.identity);
				EnergyLever.Value -= energyCost;
				spawnCount--;
				yield return new WaitForSeconds(SpawnDelay);
			}
			EnergyLever.Interactable = true;
			PistonLever.Interactable = true;
		}
	}

	private void Update()
	{
		if (test)
		{
			test = false;
			Activate();
		}
	}
}
