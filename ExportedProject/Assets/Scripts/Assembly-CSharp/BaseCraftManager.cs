using UnityEngine;
using VRTK;

public class BaseCraftManager : MonoBehaviour
{
	public GameObject roomProxy;

	public GameObject roomPointer;

	internal CraftedObject currentRoom;

	internal WallStructure currentWall;

	internal GameObject currentGate;

	internal WallNode targetWall;

	internal GateNode targetGate;

	public static BaseCraftManager instance;

	public Material cantCraft;

	public Material canCraft;

	public Material holo;

	internal VRTK_ControllerEvents events;

	internal VRTK_ControllerEvents rightHandEvents;

	internal Vector2 touchAxis;

	internal bool canPlace;

	internal float pointerDistance = 5f;

	internal bool active;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		events = GetComponentInParent<VRTK_ControllerEvents>();
		rightHandEvents = GameManager.instance.rightController.GetComponent<VRTK_ControllerEvents>();
	}

	private void Update()
	{
		if (!roomPointer.activeSelf)
		{
			return;
		}
		touchAxis = rightHandEvents.GetTouchpadAxis();
		if (touchAxis.y > 0.1f && pointerDistance < 15f)
		{
			pointerDistance += 5f * Time.deltaTime;
		}
		else if (touchAxis.y < -0.1f && pointerDistance > 1f)
		{
			pointerDistance -= 5f * Time.deltaTime;
		}
		roomPointer.transform.localPosition = new Vector3(0f, 0f - pointerDistance, pointerDistance);
		if ((bool)GetComponentInParent<VRTK_InteractGrab>().GetGrabbedObject())
		{
			return;
		}
		VRTK_BodyPhysics.instance.motionTimer = 0f;
		if (Mathf.Abs(touchAxis.x) < 0.25f)
		{
			active = false;
		}
		if (Mathf.Abs(touchAxis.x) > 0.7f && !active)
		{
			active = true;
			if (touchAxis.x > 0f)
			{
				roomProxy.transform.eulerAngles = new Vector3(0f, roomProxy.transform.eulerAngles.y + 90f, 0f);
			}
			else
			{
				roomProxy.transform.eulerAngles = new Vector3(0f, roomProxy.transform.eulerAngles.y - 90f, 0f);
			}
		}
	}

	public void SpawnProxy(CraftedObject newRoom)
	{
		currentRoom = newRoom;
		GameObject prefab = newRoom.prefab;
		roomPointer.SetActive(true);
		roomProxy.SetActive(true);
		roomProxy.GetComponent<MeshFilter>().sharedMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
		roomProxy.GetComponent<MeshCollider>().sharedMesh = prefab.GetComponent<MeshCollider>().sharedMesh;
		Structure structure = newRoom as Structure;
		if (structure.isRoom)
		{
			RoomProxy.instance.isRoom = true;
		}
		else
		{
			RoomProxy.instance.isRoom = false;
		}
		Room component = prefab.GetComponent<Room>();
		for (int i = 0; i < 4; i++)
		{
			if (component.nodes != null && component.nodes.Count > i)
			{
				roomProxy.GetComponent<RoomProxy>().nodes[i].SetActive(true);
				roomProxy.GetComponent<RoomProxy>().nodes[i].transform.localPosition = component.nodes[i].transform.localPosition;
			}
			else
			{
				roomProxy.GetComponent<RoomProxy>().nodes[i].SetActive(false);
			}
		}
	}

	public void OnTriggerPressed(HandController hand)
	{
		if (hand.transform == GameManager.instance.rightController.parent)
		{
			if ((bool)targetGate && (bool)currentGate)
			{
				CraftGate();
				targetGate = null;
			}
			else if ((bool)targetWall && (bool)currentWall)
			{
				CraftWall();
				targetWall = null;
			}
			else if (canPlace && (bool)currentRoom && !RoomProxy.instance.blocked)
			{
				OnCraftRoom();
				canPlace = false;
			}
		}
	}

	public void ToggleCanCraft(bool canCraft)
	{
		canPlace = canCraft;
	}

	public void OnCraftRoom()
	{
		RoomProxy.instance.SalvageCaps();
		GameObject gameObject = Object.Instantiate(currentRoom.prefab, roomProxy.transform.position, roomProxy.transform.rotation);
		gameObject.name = currentRoom.prefab.name;
		Invoke("UpdateOxygen", 0.1f);
		instance.ToggleCanCraft(false);
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if (currentRoom == NanoInventory.instance.craftedObjects[i] && !GameManager.instance.creativeMode)
			{
				NanoInventory.instance.craftedObjectCounts[i]--;
			}
		}
		gameObject.transform.SetParent(CenterRoom.instance.transform);
		OxygenCheck.instance.ColliderCheck();
		currentRoom = null;
		ArmUIManager.instance.UpdateUI();
		OnCancelCraft();
	}

	public void CraftWall()
	{
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if (NanoInventory.instance.craftedObjects[i] is WallStructure && NanoInventory.instance.craftedObjects[i] == currentWall && NanoInventory.instance.craftedObjectCounts[i] <= 0 && !GameManager.instance.creativeMode)
			{
				GameManager.instance.rightController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
				GameManager.instance.rightController.GetComponent<VRTK_Pointer>().Toggle(false);
				WallNode.Toggle(false);
				return;
			}
		}
		targetWall.OnCraft(currentWall);
		ArmUIManager.instance.UpdateUI();
	}

	public void CraftGate()
	{
		targetGate.OnCraft(currentGate);
		currentGate = null;
		ArmUIManager.instance.UpdateUI();
	}

	private void UpdateOxygen()
	{
		OxygenManager.instance.UpdateState();
	}

	public void OnCancelCraft()
	{
		currentGate = null;
		currentRoom = null;
		currentWall = null;
		if ((bool)roomProxy)
		{
			roomProxy.SetActive(false);
		}
		roomPointer.SetActive(false);
		if ((bool)RoomProxy.instance)
		{
			RoomProxy.instance.Reset();
		}
	}
}
