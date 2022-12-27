using UnityEngine;

public class DebugManager : MonoBehaviour
{
	public static DebugManager instance;

	public GameObject debugButton;

	public GameObject oxygen;

	public GameObject power;

	public GameObject health;

	public GameObject food;

	public GameObject craftResource;

	public GameObject oxygenSeed;

	public GameObject fruitSeed;

	public GameObject Titan;

	public Transform spawnPos;

	private void Awake()
	{
		instance = this;
	}

	public void EnableDebugMenu()
	{
		debugButton.SetActive(true);
	}

	public void OnDebugMenu()
	{
		ArmUIManager.instance.ShowDebugMenu();
	}

	public void SpawnResourceObject()
	{
		NanoInventory.instance.nanoCap = 100000;
		GameObject gameObject = Object.Instantiate(craftResource, spawnPos.position, Quaternion.identity);
	}

	public void SpawnOxygenCan()
	{
		GameObject gameObject = Object.Instantiate(oxygen, spawnPos.position, Quaternion.identity);
		gameObject.name = oxygen.name;
	}

	public void SpawnPowerCan()
	{
		GameObject gameObject = Object.Instantiate(power, spawnPos.position, Quaternion.identity);
		gameObject.name = power.name;
	}

	public void SpawnHealthCan()
	{
		GameObject gameObject = Object.Instantiate(health, spawnPos.position, Quaternion.identity);
		gameObject.name = health.name;
	}

	public void SpawnFood()
	{
		GameObject gameObject = Object.Instantiate(food, spawnPos.position, Quaternion.identity);
		gameObject.name = food.name;
	}

	public void SpawnOxygenSeed()
	{
		GameObject gameObject = Object.Instantiate(oxygenSeed, spawnPos.position, Quaternion.identity);
		gameObject.name = oxygenSeed.name;
	}

	public void SpawnFruitSeed()
	{
		GameObject gameObject = Object.Instantiate(fruitSeed, spawnPos.position, Quaternion.identity);
		gameObject.name = fruitSeed.name;
	}

	public void Update()
	{
		if (Application.isEditor)
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				SuitManager.instance.radiation = 10f;
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				float num = 30f;
				Vector3 position = base.transform.position + new Vector3(num, 0f, num);
				Object.Instantiate(DroneManager.instance.Drone, position, Quaternion.identity);
			}
		}
	}
}
