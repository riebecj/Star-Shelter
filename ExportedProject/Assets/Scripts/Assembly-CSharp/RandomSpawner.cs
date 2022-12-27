using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
	public bool disableOnStart;

	[Range(0f, 100f)]
	public float disableOnStartChance;

	public bool dontAlwaysSpawn;

	[Range(0f, 100f)]
	public float spawnChance;

	public GameObject[] spawnObjects;

	private void Start()
	{
		if (disableOnStart)
		{
			float num = Random.Range(0, 100);
			if (num < disableOnStartChance)
			{
				base.gameObject.SetActive(false);
			}
		}
		if (spawnObjects.Length == 0)
		{
			return;
		}
		if (dontAlwaysSpawn)
		{
			int num2 = Random.Range(0, 100);
			if ((float)num2 < spawnChance)
			{
				int index = Random.Range(0, spawnObjects.Length);
				OnSpawn(index);
			}
		}
		else
		{
			int index2 = Random.Range(0, spawnObjects.Length);
			OnSpawn(index2);
		}
	}

	public void OnSpawn(int index)
	{
		GameObject gameObject = Object.Instantiate(spawnObjects[index], base.transform.position, base.transform.rotation);
		gameObject.name = spawnObjects[index].name;
		gameObject.transform.SetParent(base.transform);
		if ((bool)base.transform.root.GetComponent<Wreckage>() && (bool)gameObject.GetComponent<Rigidbody>() && (bool)base.transform.root.GetComponent<Rigidbody>())
		{
			gameObject.GetComponent<Rigidbody>().velocity = base.transform.root.GetComponent<Rigidbody>().velocity;
		}
	}
}
