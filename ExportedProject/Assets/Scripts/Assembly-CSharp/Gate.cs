using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
	public List<RoomNode> Nodes = new List<RoomNode>();

	public static List<Gate> gates = new List<Gate>();

	public bool open;

	public bool automatic = true;

	public bool holoGate;

	public AutomaticDoorToggle[] buttons;

	internal GateNode myNode;

	internal bool wasLeaking;

	internal float leakRate = 5f;

	private void Awake()
	{
		gates.Add(this);
		BaseLoader.instance.AllGates.Add(base.gameObject);
	}

	private void Start()
	{
		FindNodes();
		if (!open && Nodes.Count > 0)
		{
			Nodes[0].CancelInvoke("StartLeak");
			Nodes[0].StopLeak();
		}
		foreach (RoomNode node in Nodes)
		{
			node.open = false;
			node.CheckForLeak();
		}
		OxygenManager.instance.UpdateState();
		if (holoGate)
		{
			StartCoroutine("CheckRoomForLeak");
		}
	}

	private IEnumerator CheckRoomForLeak()
	{
		while (true)
		{
			open = true;
			foreach (RoomNode node in Nodes)
			{
				node.open = true;
			}
			foreach (RoomNode node2 in Nodes)
			{
				if (!(node2.room.leakRate > 0f) && Nodes.Count != 1)
				{
					continue;
				}
				open = false;
				foreach (RoomNode node3 in Nodes)
				{
					node3.open = false;
				}
			}
			OxygenManager.instance.UpdateState();
			yield return new WaitForSeconds(1f);
		}
	}

	public void OnOpen()
	{
		open = true;
		foreach (RoomNode node in Nodes)
		{
			node.open = true;
			node.CheckForLeak();
		}
		if (Nodes.Count == 1)
		{
			if (!wasLeaking)
			{
				Nodes[0].room.leakRate += leakRate;
			}
			Nodes[0].room.leakSource.Add(base.transform);
			wasLeaking = true;
		}
		OxygenManager.instance.UpdateState();
	}

	public void OnClose()
	{
		open = false;
		foreach (RoomNode node in Nodes)
		{
			node.open = false;
			node.CheckForLeak();
		}
		if (Nodes.Count == 1)
		{
			Nodes[0].room.leakSource.Remove(base.transform);
			if (wasLeaking)
			{
				Nodes[0].room.leakRate -= leakRate;
			}
			wasLeaking = false;
			Nodes[0].StopLeak();
		}
		OxygenManager.instance.UpdateState();
	}

	public void OnLoad(bool open, bool _automatic)
	{
		automatic = _automatic;
		if ((bool)GetComponent<GateSensor>())
		{
			GetComponent<GateSensor>().active = automatic;
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].SetState(automatic);
		}
	}

	public void FindNodes()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.5f);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if ((bool)collider.GetComponent<RoomNode>() && !collider.GetComponentInParent<RoomProxy>())
			{
				RoomNode component = collider.GetComponent<RoomNode>();
				if (!Nodes.Contains(component))
				{
					Nodes.Add(component);
					component.gate = this;
					component.CheckForLeak();
				}
			}
		}
	}

	public void UpdateState()
	{
		if (Nodes.Count == 0 && (bool)BaseLoader.instance)
		{
			if (BaseLoader.instance.AllGates.Contains(base.gameObject))
			{
				BaseLoader.instance.AllGates.Remove(base.gameObject);
			}
			if (gates.Contains(this))
			{
				gates.Remove(this);
			}
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (gates.Contains(this))
		{
			gates.Remove(this);
		}
		OnRemove();
	}

	public void OnRemove()
	{
		if (BaseLoader.instance.AllGates.Contains(base.gameObject))
		{
			BaseLoader.instance.AllGates.Remove(base.gameObject);
		}
		if ((bool)myNode && !GateNode.gateNodes.Contains(myNode))
		{
			myNode.gameObject.SetActive(true);
			GateNode.gateNodes.Add(myNode);
			myNode.crafted = false;
		}
		foreach (RoomNode node in Nodes)
		{
			node.open = true;
			node.gate = null;
			node.CheckForLeak();
		}
		OxygenManager.instance.UpdateState();
	}

	public void ToggleAuto(ToggleTrigger button)
	{
		if (!button.On)
		{
			automatic = false;
		}
		else
		{
			automatic = true;
		}
		if ((bool)GetComponent<GateSensor>())
		{
			GetComponent<GateSensor>().active = automatic;
		}
	}

	public void ToggleAuto(AutomaticDoorToggle button)
	{
		if (!button.On)
		{
			automatic = false;
		}
		else
		{
			automatic = true;
		}
		if ((bool)GetComponent<GateSensor>())
		{
			GetComponent<GateSensor>().active = automatic;
		}
	}

	public void SyncToAnimationState()
	{
		if ((bool)GetComponent<GateSensor>() && GetComponent<GateSensor>().animator.GetBool("Open"))
		{
			OnOpen();
		}
		else
		{
			OnClose();
		}
	}
}
