using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;

public class BaseLoader : MonoBehaviour
{
	public List<Room> AllRooms = new List<Room>();

	public List<GameObject> RoomsList = new List<GameObject>();

	public List<GameObject> AllGates = new List<GameObject>();

	public List<GameObject> GateList = new List<GameObject>();

	public List<GameObject> HoleTypes = new List<GameObject>();

	public List<GameObject> leakHoles = new List<GameObject>();

	internal List<GameObject> glassLeakHoles = new List<GameObject>();

	public GameObject gateProxy;

	public GameObject RoomWreckage;

	public GameObject leakUI;

	public GameObject leakParticle;

	public static BaseLoader instance;

	public bool deleteSaves;

	internal bool isLoading;

	private void Awake()
	{
		instance = this;
		isLoading = true;
		Invoke("EndLoad", 0.5f);
	}

	private void EndLoad()
	{
		isLoading = false;
		WallNode.Toggle(false);
	}

	private void Start()
	{
		if (!(Application.loadedLevelName != "MainScene"))
		{
			OnLoad();
			Invoke("SpawnGateProxies", 0.5f);
			FindHolePositions();
		}
	}

	public void OnSave()
	{
		for (int i = 0; i < AllRooms.Count; i++)
		{
			if (AllRooms[i] != null && !AllRooms[i].name.Contains("Center"))
			{
				PreviewLabs.PlayerPrefs.SetString("RoomObject" + i, AllRooms[i].name);
				PreviewLabs.PlayerPrefs.SetFloat("RoomXPosition" + i, AllRooms[i].transform.localPosition.x);
				PreviewLabs.PlayerPrefs.SetFloat("RoomYPosition" + i, AllRooms[i].transform.localPosition.y);
				PreviewLabs.PlayerPrefs.SetFloat("RoomZPosition" + i, AllRooms[i].transform.localPosition.z);
				PreviewLabs.PlayerPrefs.SetFloat("RoomXRotation" + i, AllRooms[i].transform.localEulerAngles.x);
				PreviewLabs.PlayerPrefs.SetFloat("RoomYRotation" + i, AllRooms[i].transform.localEulerAngles.y);
				PreviewLabs.PlayerPrefs.SetFloat("RoomZRotation" + i, AllRooms[i].transform.localEulerAngles.z);
				PreviewLabs.PlayerPrefs.SetFloat("RoomOxygen" + i, AllRooms[i].GetComponent<Room>().Oxygen);
				AllRooms[i].GetComponent<Room>().OnSave();
			}
			else
			{
				PreviewLabs.PlayerPrefs.SetString("RoomObject" + i, string.Empty);
			}
		}
		for (int j = 0; j < AllGates.Count; j++)
		{
			PreviewLabs.PlayerPrefs.SetString("GateObject" + j, AllGates[j].name);
			PreviewLabs.PlayerPrefs.SetFloat("GateXPosition" + j, AllGates[j].transform.position.x);
			PreviewLabs.PlayerPrefs.SetFloat("GateYPosition" + j, AllGates[j].transform.position.y);
			PreviewLabs.PlayerPrefs.SetFloat("GateZPosition" + j, AllGates[j].transform.position.z);
			PreviewLabs.PlayerPrefs.SetFloat("GateXRotation" + j, AllGates[j].transform.eulerAngles.x);
			PreviewLabs.PlayerPrefs.SetFloat("GateYRotation" + j, AllGates[j].transform.eulerAngles.y);
			PreviewLabs.PlayerPrefs.SetFloat("GateZRotation" + j, AllGates[j].transform.eulerAngles.z);
			PreviewLabs.PlayerPrefs.SetBool("GateOpen" + j, AllGates[j].GetComponent<Gate>().open);
			PreviewLabs.PlayerPrefs.SetBool("GateAutomatic" + j, AllGates[j].GetComponent<Gate>().automatic);
		}
		PreviewLabs.PlayerPrefs.SetInt("AllGateCount", AllGates.Count);
		PreviewLabs.PlayerPrefs.SetInt("RoomCount", AllRooms.Count);
		if ((bool)CenterRoom.instance)
		{
			CenterRoom.instance.Save();
		}
		for (int k = 0; k < RoomSave.startRooms.Count; k++)
		{
			RoomSave.startRooms[k].Save();
		}
	}

	public void OnLoad()
	{
		for (int i = 0; i < PreviewLabs.PlayerPrefs.GetInt("RoomCount"); i++)
		{
			GameObject gameObject = null;
			foreach (GameObject rooms in RoomsList)
			{
				if (rooms.name == PreviewLabs.PlayerPrefs.GetString("RoomObject" + i))
				{
					gameObject = rooms;
					GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation);
					gameObject2.name = gameObject.name;
					gameObject2.transform.localPosition = new Vector3(PreviewLabs.PlayerPrefs.GetFloat("RoomXPosition" + i), PreviewLabs.PlayerPrefs.GetFloat("RoomYPosition" + i), PreviewLabs.PlayerPrefs.GetFloat("RoomZPosition" + i));
					gameObject2.transform.localEulerAngles = new Vector3(PreviewLabs.PlayerPrefs.GetFloat("RoomXRotation" + i), PreviewLabs.PlayerPrefs.GetFloat("RoomYRotation" + i), PreviewLabs.PlayerPrefs.GetFloat("RoomZRotation" + i));
					gameObject2.GetComponent<Room>().OnLoad(i, PreviewLabs.PlayerPrefs.GetFloat("RoomOxygen" + i));
				}
			}
		}
		for (int j = 0; j < AllGates.Count; j++)
		{
			if (PreviewLabs.PlayerPrefs.HasKey("GateOpen" + j))
			{
				AllGates[j].GetComponent<Gate>().OnLoad(PreviewLabs.PlayerPrefs.GetBool("GateOpen" + j), PreviewLabs.PlayerPrefs.GetBool("GateAutomatic" + j));
			}
		}
		for (int k = 0; k < PreviewLabs.PlayerPrefs.GetInt("AllGateCount"); k++)
		{
			GameObject gameObject3 = null;
			foreach (GameObject gate in GateList)
			{
				if (gate.name == PreviewLabs.PlayerPrefs.GetString("GateObject" + k))
				{
					gameObject3 = gate;
					GameObject gameObject4 = Object.Instantiate(gameObject3, base.transform.position, base.transform.rotation);
					gameObject4.name = gameObject3.name;
					gameObject4.transform.position = new Vector3(PreviewLabs.PlayerPrefs.GetFloat("GateXPosition" + k), PreviewLabs.PlayerPrefs.GetFloat("GateYPosition" + k), PreviewLabs.PlayerPrefs.GetFloat("GateZPosition" + k));
					gameObject4.transform.eulerAngles = new Vector3(PreviewLabs.PlayerPrefs.GetFloat("GateXRotation" + k), PreviewLabs.PlayerPrefs.GetFloat("GateYRotation" + k), PreviewLabs.PlayerPrefs.GetFloat("GateZRotation" + k));
					if (PreviewLabs.PlayerPrefs.HasKey("GateOpen" + k))
					{
						gameObject4.GetComponent<Gate>().OnLoad(PreviewLabs.PlayerPrefs.GetBool("GateOpen" + k), PreviewLabs.PlayerPrefs.GetBool("GateAutomatic" + k));
					}
				}
			}
		}
		OxygenManager.instance.Invoke("UpdateState", 0.25f);
	}

	public void SpawnGateProxies()
	{
		for (int i = 0; i < RoomNode.roomNodes.Count; i++)
		{
			if (!RoomNode.roomNodes[i].gate)
			{
				RoomNode.roomNodes[i].CheckGateForProxy();
			}
		}
	}

	private void FindHolePositions()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("LeakPoint");
		for (int i = 0; i < array.Length; i++)
		{
			leakHoles.Add(array[i]);
		}
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("LeakPointGlass");
		for (int j = 0; j < array2.Length; j++)
		{
			glassLeakHoles.Add(array2[j]);
			leakHoles.Add(array2[j]);
		}
	}
}
