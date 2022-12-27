using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomProxy : MonoBehaviour
{
	internal bool blocked;

	internal bool touchingNode;

	public GameObject[] nodes;

	private float gridSize = 1f;

	public static RoomProxy instance;

	internal RoomNode lastNode;

	internal bool isRoom;

	private void Awake()
	{
		instance = this;
	}

	private void OnEnable()
	{
		StartCoroutine("OnUpdate");
		base.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
	}

	public void Reset()
	{
		touchingNode = false;
	}

	private IEnumerator OnUpdate()
	{
		while (true)
		{
			OnMove();
			yield return new WaitForSeconds(0.1f);
		}
	}

	public void OnMove()
	{
		if (OnCheckCollision())
		{
			GetComponent<MeshRenderer>().material = BaseCraftManager.instance.cantCraft;
			blocked = true;
		}
		else if (touchingNode)
		{
			GetComponent<MeshRenderer>().material = BaseCraftManager.instance.canCraft;
			blocked = false;
		}
		else
		{
			GetComponent<MeshRenderer>().material = BaseCraftManager.instance.holo;
		}
		Vector3 position = BaseCraftManager.instance.roomPointer.transform.position;
		position /= gridSize;
		position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
		position /= gridSize;
		position.y = Mathf.RoundToInt(BaseCraftManager.instance.roomPointer.transform.position.y / 6f) * 6;
		base.transform.position = position;
		if (isRoom)
		{
			RoomNode closestNode = GetClosestNode();
			base.transform.position = closestNode.transform.position;
			base.transform.LookAt(closestNode.transform.position - (closestNode.transform.position - new Vector3(closestNode.transform.parent.position.x, closestNode.transform.position.y, closestNode.transform.parent.position.z)).normalized * 100f);
		}
		else
		{
			RoomNode closestNode2 = GetClosestNode();
			base.transform.position = closestNode2.transform.position + closestNode2.transform.forward * 3f;
		}
	}

	public bool OnCheckCollision()
	{
		foreach (Room allRoom in BaseLoader.instance.AllRooms)
		{
			if (allRoom != this && allRoom.OxgenCapacity != 0f && allRoom.GetComponent<Collider>().bounds.Intersects(GetComponent<Collider>().bounds))
			{
				return true;
			}
		}
		return false;
	}

	public void ToggleLockIn(bool value)
	{
		if (value)
		{
			GetComponent<MeshRenderer>().material = BaseCraftManager.instance.canCraft;
		}
		else
		{
			GetComponent<MeshRenderer>().material = BaseCraftManager.instance.cantCraft;
		}
		touchingNode = value;
	}

	public void SalvageCaps()
	{
		Room room = null;
		foreach (Room allRoom in BaseLoader.instance.AllRooms)
		{
			if (allRoom != this && allRoom.OxgenCapacity == 0f && allRoom.GetComponent<Collider>().bounds.Intersects(GetComponent<Collider>().bounds))
			{
				room = allRoom;
				allRoom.gameObject.SetActive(false);
			}
		}
		if ((bool)room)
		{
			room.OnRemove();
		}
	}

	private RoomNode GetClosestNode()
	{
		RoomNode result = null;
		float num = float.PositiveInfinity;
		List<RoomNode> list = new List<RoomNode>();
		foreach (RoomNode roomNode in RoomNode.roomNodes)
		{
			if (!roomNode.transform.parent.name.Contains("Cap"))
			{
				list.Add(roomNode);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			float num2 = Vector3.Distance(BaseCraftManager.instance.roomPointer.transform.position, list[i].transform.position);
			if (num2 < num)
			{
				num = num2;
				result = list[i];
			}
		}
		return result;
	}

	private Transform GetMyNode(Transform otherNode)
	{
		Transform result = null;
		float num = float.PositiveInfinity;
		for (int i = 0; i < nodes.Length; i++)
		{
			float num2 = Vector3.Distance(otherNode.position, nodes[i].transform.position);
			if (num2 < num)
			{
				num = num2;
				result = nodes[i].transform;
			}
		}
		return result;
	}
}
