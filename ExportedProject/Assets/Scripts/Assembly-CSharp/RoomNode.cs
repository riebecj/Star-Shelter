using System.Collections.Generic;
using UnityEngine;

public class RoomNode : MonoBehaviour
{
	public Room room;

	public bool open;

	public RoomNode connectedNode;

	public static List<RoomNode> roomNodes = new List<RoomNode>();

	public Gate gate;

	public static List<Vector3> gateProxyPositions = new List<Vector3>();

	internal GameObject leakParticle;

	public bool wasLeaking;

	private void Awake()
	{
		if (!GetComponentInParent<RoomProxy>())
		{
			roomNodes.Add(this);
			Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			Invoke("RemoveTempBody", 1f);
		}
		SetupLeakParticle();
	}

	private void RemoveTempBody()
	{
		Object.Destroy(GetComponent<Rigidbody>());
	}

	private void Start()
	{
		if (!GetComponentInParent<RoomProxy>())
		{
			Invoke("CheckForLeak", 0.1f);
		}
	}

	private void SetupLeakParticle()
	{
		leakParticle = Object.Instantiate(BaseLoader.instance.leakParticle, base.transform);
		leakParticle.transform.localPosition = Vector3.zero;
		leakParticle.transform.rotation = base.transform.rotation;
		leakParticle.GetComponent<LeakParticle>().room = room;
		leakParticle.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)room && (bool)other.GetComponent<RoomNode>() && !BaseLoader.instance.isLoading)
		{
			OnSnapOn(other.GetComponent<RoomNode>());
		}
	}

	private void OnSnapOn(RoomNode other)
	{
		if (connectedNode != null)
		{
			return;
		}
		if (gate == null)
		{
			open = true;
		}
		RoomNode component = other.GetComponent<RoomNode>();
		if (!other.GetComponentInParent<RoomProxy>())
		{
			component.OnAlignment(this);
		}
		if (!other.GetComponentInParent<RoomProxy>())
		{
			connectedNode = component;
		}
		if (!room.blocked || BaseLoader.instance.isLoading)
		{
			if ((bool)RoomProxy.instance)
			{
				RoomProxy.instance.ToggleLockIn(true);
			}
			room.lastConnectedNode = this;
			if (!BaseLoader.instance.isLoading)
			{
				BaseCraftManager.instance.ToggleCanCraft(true);
			}
			if (!other.GetComponentInParent<RoomProxy>() && !GetComponentInParent<RoomProxy>())
			{
				CheckForLeak();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<RoomNode>() && connectedNode == other.GetComponent<RoomNode>())
		{
			RoomProxy.instance.ToggleLockIn(false);
			RoomNode component = other.GetComponent<RoomNode>();
			component.OnDetach(this);
			connectedNode = null;
			BaseCraftManager.instance.ToggleCanCraft(false);
			if (!other.GetComponentInParent<RoomProxy>() && !GetComponentInParent<RoomProxy>())
			{
				CheckForLeak();
			}
		}
	}

	public void OnRemove()
	{
		if ((bool)connectedNode && connectedNode.connectedNode == this)
		{
			connectedNode.open = true;
			connectedNode.connectedNode = null;
			connectedNode.CheckForLeak();
		}
	}

	private void OnAlignment(RoomNode newNode)
	{
		if (gate == null)
		{
			open = true;
		}
		connectedNode = newNode;
		if ((bool)base.transform.parent.GetComponent<RoomProxy>())
		{
			RoomProxy.instance.lastNode = newNode;
		}
		CheckForLeak();
	}

	private void OnDetach(RoomNode newNode)
	{
		connectedNode = null;
		if ((bool)base.transform.parent.GetComponent<RoomProxy>() && RoomProxy.instance.lastNode == newNode)
		{
			RoomProxy.instance.lastNode = null;
		}
		CheckForLeak();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, 0.5f);
	}

	public void CheckConnections()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.1f);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if ((bool)collider.GetComponent<RoomNode>() && collider.GetComponent<RoomNode>() != this)
			{
				OnSnapOn(collider.GetComponent<RoomNode>());
				return;
			}
		}
		CheckForLeak();
	}

	private void OnDisable()
	{
		for (int i = 0; i < Gate.gates.Count; i++)
		{
			if (Gate.gates[i].Nodes.Contains(this))
			{
				Gate.gates[i].Nodes.Remove(this);
			}
			if ((bool)Gate.gates[i])
			{
				Gate.gates[i].UpdateState();
			}
		}
		roomNodes.Remove(this);
		CheckForLeak();
	}

	public void CheckGateForProxy()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.1f);
		bool flag = true;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if ((bool)collider.GetComponent<RoomNode>() && (bool)collider.GetComponent<RoomNode>().gate)
			{
				flag = false;
			}
		}
		if (flag && !gateProxyPositions.Contains(new Vector3(Mathf.Round(base.transform.position.x), Mathf.Round(base.transform.position.y), Mathf.Round(base.transform.position.z))))
		{
			Object.Instantiate(BaseLoader.instance.gateProxy, base.transform.position, base.transform.rotation);
			gateProxyPositions.Add(new Vector3(Mathf.Round(base.transform.position.x), Mathf.Round(base.transform.position.y), Mathf.Round(base.transform.position.z)));
		}
	}

	public void CheckForLeak()
	{
		if ((bool)GetComponentInParent<RoomProxy>() || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (connectedNode == null && gate == null)
		{
			open = true;
			if (!room.leakSource.Contains(base.transform))
			{
				room.leakSource.Add(base.transform);
			}
		}
		else if (room.leakSource.Contains(base.transform))
		{
			room.leakSource.Remove(base.transform);
		}
		if ((bool)gate && (!gate.open || gate.holoGate))
		{
			open = false;
		}
		CancelInvoke();
		if (open && connectedNode == null)
		{
			Invoke("StartLeak", 1f);
		}
		else
		{
			Invoke("StopLeak", 1f);
		}
	}

	public void StartLeak()
	{
		if (!wasLeaking)
		{
			CancelInvoke("StopLeak");
			if ((bool)room && room.group.TotalOxygen > 5f)
			{
				leakParticle.SetActive(true);
			}
			room.leakRate += 5f;
			wasLeaking = true;
		}
	}

	public void StopLeak()
	{
		if (wasLeaking)
		{
			if ((bool)leakParticle && leakParticle.activeSelf)
			{
				leakParticle.SetActive(false);
			}
			CancelInvoke("StartLeak");
			room.leakRate -= 5f;
			wasLeaking = false;
		}
	}

	public void OnLoadDoor()
	{
	}
}
