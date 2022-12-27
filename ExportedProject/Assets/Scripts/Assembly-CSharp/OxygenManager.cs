using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenManager : MonoBehaviour
{
	public static OxygenManager instance;

	public List<OxygenGroup> oxygenGroups = new List<OxygenGroup>();

	internal List<Room> visitedRooms = new List<Room>();

	public Stack<Room> roomStack = new Stack<Room>();

	public List<Room> roomPool = new List<Room>();

	private void Awake()
	{
		instance = this;
		StartCoroutine("UpdateRoomOxygen");
	}

	public void UpdateState()
	{
		oxygenGroups.Clear();
		roomPool.Clear();
		visitedRooms.Clear();
		roomStack.Clear();
		for (int i = 0; i < BaseLoader.instance.AllRooms.Count; i++)
		{
			roomPool.Add(BaseLoader.instance.AllRooms[i]);
		}
		while (roomPool.Count > 0)
		{
			DepthFirstSearch();
			CreateNewGroup();
		}
		foreach (Gate gate in Gate.gates)
		{
			gate.FindNodes();
		}
		BaseLoader.instance.SpawnGateProxies();
	}

	private void DepthFirstSearch()
	{
		roomStack.Push(roomPool[0]);
		visitedRooms.Add(roomPool[0]);
		while (roomStack.Count > 0)
		{
			Room room = roomStack.Pop();
			for (int i = 0; i < room.nodes.Count; i++)
			{
				if (room.nodes[i].open && (bool)room.nodes[i].connectedNode && !visitedRooms.Contains(room.nodes[i].connectedNode.room) && room.nodes[i].connectedNode.open)
				{
					roomStack.Push(room.nodes[i].connectedNode.room);
					visitedRooms.Add(room.nodes[i].connectedNode.room);
				}
			}
		}
	}

	private void CreateNewGroup()
	{
		if (visitedRooms.Count > 0)
		{
			OxygenGroup oxygenGroup = ScriptableObject.CreateInstance("OxygenGroup") as OxygenGroup;
			foreach (Room visitedRoom in visitedRooms)
			{
				oxygenGroup.Rooms.Add(visitedRoom);
				roomPool.Remove(visitedRoom);
			}
			oxygenGroup.Setup();
			oxygenGroups.Add(oxygenGroup);
		}
		visitedRooms.Clear();
	}

	private IEnumerator UpdateRoomOxygen()
	{
		float refreshRate = 0.25f;
		while (true)
		{
			float oxygenLoss = 0f;
			float oxygenGain = 0f;
			float oxygenCap = 0f;
			float collectiveOxygen = 0f;
			for (int i = 0; i < oxygenGroups.Count; i++)
			{
				oxygenGroups[i].UpdateRoomStates(refreshRate);
				for (int j = 0; j < oxygenGroups[i].Rooms.Count; j++)
				{
					if (oxygenGroups[i].TotalOxygen > 1f)
					{
						oxygenLoss += oxygenGroups[i].Rooms[j].leakRate;
					}
					oxygenGain += oxygenGroups[i].Rooms[j].oxygenGenerationRate;
					collectiveOxygen += oxygenGroups[i].Rooms[j].Oxygen;
					oxygenCap += oxygenGroups[i].Rooms[j].OxgenCapacity;
				}
				oxygenLoss += oxygenGroups[i].drainRate;
				BaseManager.instance.collectiveOxygen = collectiveOxygen;
				BaseManager.instance.collectiveOxygenCap = oxygenCap;
			}
			if ((bool)BaseManager.instance)
			{
				BaseManager.instance.UpdateOxygenUI(oxygenLoss, oxygenGain);
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}
}
