using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;

public class RoomSave : MonoBehaviour
{
	public static List<RoomSave> startRooms = new List<RoomSave>();

	private void Awake()
	{
		startRooms.Add(this);
		WallNode[] walls = GetComponent<Room>().walls;
		for (int i = 0; i < walls.Length; i++)
		{
			if (PreviewLabs.PlayerPrefs.HasKey(i + "WallType" + base.transform.position))
			{
				continue;
			}
			PreviewLabs.PlayerPrefs.SetString(i + "WallType" + base.transform.position, "Wall (Standard)");
			for (int j = 0; j < NanoInventory.instance.craftedObjects.Count; j++)
			{
				if (PreviewLabs.PlayerPrefs.GetString(i + "WallType" + base.transform.position) == NanoInventory.instance.craftedObjects[j].name)
				{
					walls[i].gameObject.SetActive(true);
					walls[i].OnCraft(NanoInventory.instance.craftedObjects[j] as WallStructure);
				}
			}
		}
		if (PreviewLabs.PlayerPrefs.GetBool("RoomRemoved" + base.transform.position))
		{
			GetComponent<Room>().OnRemove();
			base.gameObject.SetActive(false);
		}
	}

	private void Start()
	{
		GetComponent<Room>().Oxygen = PreviewLabs.PlayerPrefs.GetFloat("Oxygen" + base.transform.position);
		GetComponent<Room>().LoadProps();
		GetComponent<Room>().LoadHoles();
		WallNode[] walls = GetComponent<Room>().walls;
		for (int i = 0; i < walls.Length; i++)
		{
			if (!PreviewLabs.PlayerPrefs.HasKey(i + "WallType" + base.transform.position))
			{
				continue;
			}
			PreviewLabs.PlayerPrefs.SetString(i + "WallType" + base.transform.position, "Wall (Standard)");
			for (int j = 0; j < NanoInventory.instance.craftedObjects.Count; j++)
			{
				if (PreviewLabs.PlayerPrefs.GetString(i + "WallType" + base.transform.position) == NanoInventory.instance.craftedObjects[j].name)
				{
					walls[i].gameObject.SetActive(true);
					walls[i].OnCraft(NanoInventory.instance.craftedObjects[j] as WallStructure);
				}
			}
		}
	}

	public void Save()
	{
		PreviewLabs.PlayerPrefs.SetFloat("Oxygen" + base.transform.position, GetComponent<Room>().Oxygen);
		GetComponent<Room>().SaveProps();
		GetComponent<Room>().SaveHoles();
		WallNode[] walls = GetComponent<Room>().walls;
		for (int i = 0; i < walls.Length; i++)
		{
			if (walls[i].crafted)
			{
				PreviewLabs.PlayerPrefs.SetString(i + "WallType" + base.transform.position, walls[i].wallStructure.name);
			}
			else if (PreviewLabs.PlayerPrefs.HasKey(i + "WallType" + base.transform.position))
			{
				PreviewLabs.PlayerPrefs.SetString(i + "WallType" + base.transform.position, string.Empty);
			}
		}
	}

	public void OnSalvage()
	{
		OnRemove();
	}

	public void OnRemove()
	{
		PreviewLabs.PlayerPrefs.SetBool("RoomRemoved" + base.transform.position, true);
	}

	private void OnDisable()
	{
		startRooms.Remove(this);
	}
}
