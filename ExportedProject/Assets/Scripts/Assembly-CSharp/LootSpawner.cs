using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
	public enum LootTier
	{
		Tier1 = 0,
		Tier2 = 1,
		Tier3 = 2
	}

	public LootTier lootTier;

	public bool dontAlwaysSpawn;

	public int noSpawnWeight;

	private List<Vector2> minMax = new List<Vector2>();

	private int itemWeight;

	private void OnEnable()
	{
		Invoke("OnSpawn", 0.1f);
	}

	public void OnSpawn()
	{
		GameObject[] array = LootSpawnManager.instance.Tier1;
		int[] array2 = LootSpawnManager.instance.Tier1DropChance;
		if (lootTier == LootTier.Tier2)
		{
			array = LootSpawnManager.instance.Tier2;
			array2 = LootSpawnManager.instance.Tier2DropChance;
		}
		else if (lootTier == LootTier.Tier3)
		{
			array = LootSpawnManager.instance.Tier3;
			array2 = LootSpawnManager.instance.Tier3DropChance;
		}
		for (int i = 0; i < array.Length; i++)
		{
			minMax.Add(new Vector2(itemWeight, itemWeight + array2[i]));
			itemWeight += array2[i];
		}
		if (dontAlwaysSpawn)
		{
			itemWeight += noSpawnWeight;
		}
		int num = Random.Range(0, itemWeight);
		for (int j = 0; j < array.Length; j++)
		{
			if ((float)num >= minMax[j].x && (float)num <= minMax[j].y)
			{
				GameObject gameObject = Object.Instantiate(array[j], base.transform.position, base.transform.rotation);
				gameObject.transform.SetParent(base.transform);
				gameObject.name = array[j].name;
				if ((bool)base.transform.root.GetComponent<Wreckage>() && (bool)base.transform.root.GetComponent<Rigidbody>() && (bool)gameObject.GetComponent<Rigidbody>())
				{
					gameObject.GetComponent<Rigidbody>().velocity = base.transform.root.GetComponent<Rigidbody>().velocity;
				}
				break;
			}
		}
	}
}
