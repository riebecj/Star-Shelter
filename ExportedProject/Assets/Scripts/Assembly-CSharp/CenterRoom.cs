using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;

public class CenterRoom : MonoBehaviour
{
	public static CenterRoom instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		GetComponent<Room>().Oxygen = PreviewLabs.PlayerPrefs.GetFloat("CenterOxygen");
		GetComponent<Room>().LoadProps();
		GetComponent<Room>().LoadHoles();
		List<RoomNode> nodes = GetComponent<Room>().nodes;
		foreach (RoomNode item in nodes)
		{
			item.CheckConnections();
		}
	}

	public void Save()
	{
		PreviewLabs.PlayerPrefs.SetFloat("CenterOxygen", GetComponent<Room>().Oxygen);
		GetComponent<Room>().SaveProps();
		GetComponent<Room>().SaveHoles();
	}
}
