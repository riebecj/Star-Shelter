using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
	public GameObject Drone;

	public int maxDrones = 3;

	internal List<GameObject> drones = new List<GameObject>();

	public static DroneManager instance;

	internal float coolDown = 600f;

	private int index;

	public bool spawnAttackDrone;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (!IntroManager.instance && !GameManager.instance.creativeMode)
		{
			StartCoroutine("CheckSpawn");
		}
	}

	private IEnumerator CheckSpawn()
	{
		while (true)
		{
			if (DifficultyManager.instance.GetScore() > 15 && !IsInvoking("CoolDown") && drones.Count < maxDrones)
			{
				SpawnDrone();
			}
			if (spawnAttackDrone)
			{
				SpawnAttackDrones(1);
				spawnAttackDrone = false;
			}
			yield return new WaitForSeconds(5f);
		}
	}

	private void SpawnDrone()
	{
		GameObject item = Object.Instantiate(position: new Vector3(Random.Range(-100, 100), 50f, Random.Range(-100, 100)), original: Drone, rotation: base.transform.rotation);
		drones.Add(item);
		Invoke("CoolDown", coolDown - (float)(DifficultyManager.instance.GetScore() * 4));
	}

	public void SpawnAttackDrones(int count)
	{
		for (int i = 0; i < count; i++)
		{
			Vector3 position = BaseManager.instance.transform.position + Random.insideUnitSphere * 100f;
			GameObject gameObject = Object.Instantiate(Drone, position, base.transform.rotation);
			drones.Add(gameObject);
			gameObject.GetComponent<DroneAI>().targetBase = true;
		}
	}

	private void CoolDown()
	{
	}
}
