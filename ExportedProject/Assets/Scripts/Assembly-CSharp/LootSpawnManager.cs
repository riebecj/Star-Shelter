using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawnManager : MonoBehaviour
{
	public GameObject[] Tier1;

	public GameObject[] Tier2;

	public GameObject[] Tier3;

	public GameObject[] otherInventoryObjects;

	internal List<GameObject> objectList = new List<GameObject>();

	public int[] Tier1DropChance;

	public int[] Tier2DropChance;

	public int[] Tier3DropChance;

	public static LootSpawnManager instance;

	private void Awake()
	{
		instance = this;
		GameObject[] tier = Tier1;
		foreach (GameObject item in tier)
		{
			if (!objectList.Contains(item))
			{
				objectList.Add(item);
			}
		}
		GameObject[] tier2 = Tier2;
		foreach (GameObject item2 in tier2)
		{
			if (!objectList.Contains(item2))
			{
				objectList.Add(item2);
			}
		}
		GameObject[] tier3 = Tier3;
		foreach (GameObject item3 in tier3)
		{
			if (!objectList.Contains(item3))
			{
				objectList.Add(item3);
			}
		}
		GameObject[] array = otherInventoryObjects;
		foreach (GameObject item4 in array)
		{
			if (!objectList.Contains(item4))
			{
				objectList.Add(item4);
			}
		}
	}

	private void Start()
	{
		if (!IntroManager.instance)
		{
			StartCoroutine("CheckSpawnRate");
		}
	}

	private IEnumerator CheckSpawnRate()
	{
		while (true)
		{
			UpdateSpawnRate();
			yield return new WaitForSeconds(5f);
		}
	}

	private void UpdateSpawnRate()
	{
		int minutesPlayed = GameManager.instance.GetMinutesPlayed();
		Tier1DropChance[2] = Mathf.Clamp(40 - minutesPlayed / 2, 20, 40);
		Tier1DropChance[7] = Mathf.Clamp(30 - minutesPlayed / 2, 15, 30);
		Tier1DropChance[8] = Mathf.Clamp(30 - minutesPlayed / 2, 15, 30);
	}
}
