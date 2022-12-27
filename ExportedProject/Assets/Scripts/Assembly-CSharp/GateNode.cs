using System.Collections.Generic;
using UnityEngine;

public class GateNode : MonoBehaviour
{
	public List<RoomNode> Nodes = new List<RoomNode>();

	internal Room room;

	internal bool crafted;

	public static List<GateNode> gateNodes = new List<GateNode>();

	private void Awake()
	{
		gateNodes.Add(this);
	}

	private void Start()
	{
		room = GetComponentInParent<Room>();
		base.gameObject.SetActive(false);
	}

	public void OnEnter()
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.material = BaseCraftManager.instance.canCraft;
		}
		BaseCraftManager.instance.targetGate = this;
	}

	public void OnExit()
	{
		if (!crafted)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer meshRenderer in array)
			{
				meshRenderer.material = BaseCraftManager.instance.holo;
			}
			BaseCraftManager.instance.targetGate = null;
		}
	}

	public void OnCraft(GameObject gate)
	{
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if (!(gate == NanoInventory.instance.craftedObjects[i].prefab))
			{
				continue;
			}
			if (NanoInventory.instance.craftedObjectCounts[i] == 0 && !GameManager.instance.creativeMode)
			{
				return;
			}
			if (!GameManager.instance.creativeMode)
			{
				NanoInventory.instance.craftedObjectCounts[i]--;
			}
			break;
		}
		crafted = true;
		GameObject gameObject = Object.Instantiate(gate, base.transform.position, base.transform.rotation);
		gameObject.name = gate.name;
		gameObject.GetComponent<Gate>().myNode = this;
		gateNodes.Remove(this);
		base.gameObject.SetActive(false);
	}

	public static void Toggle(bool value)
	{
		if (value)
		{
			for (int i = 0; i < gateNodes.Count; i++)
			{
				gateNodes[i].gameObject.SetActive(true);
				if (!GameManager.instance.loading)
				{
					gateNodes[i].FindNodes();
				}
			}
		}
		else
		{
			for (int j = 0; j < gateNodes.Count; j++)
			{
				gateNodes[j].gameObject.SetActive(false);
			}
		}
	}

	public void FindNodes()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.5f);
		bool flag = true;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if ((bool)collider.GetComponent<RoomNode>())
			{
				flag = false;
			}
		}
		if (flag)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		gateNodes.Remove(this);
	}
}
