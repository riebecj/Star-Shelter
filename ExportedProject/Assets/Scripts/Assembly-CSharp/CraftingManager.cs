using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CraftingManager : MonoBehaviour
{
	public static CraftingManager instance;

	public List<GameObject> craftBars = new List<GameObject>();

	public List<GameObject> repairTiles = new List<GameObject>();

	public GameObject craftObject;

	public GameObject scrapObject;

	public GameObject repairObject;

	public Transform targetedNode;

	public Transform ArmPos;

	public Transform repairTilesParent;

	internal float craftSpeedMultiplier = 1f;

	internal float salvageSpeedMultiplier = 1f;

	public Transform proxy;

	internal bool isSalvaging;

	internal bool isRepairing;

	private HandController lastSource;

	private Collider lastRoomCollider;

	public Material canCraftMat;

	public Material cantCraftMat;

	internal bool canCraft;

	internal bool colliding;

	internal bool floorObject;

	private void Awake()
	{
		if (!IntroManager.instance)
		{
			IEnumerator enumerator = repairTilesParent.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					repairTiles.Add(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
		}
		instance = this;
	}

	public void OnTriggerPressed(HandController source)
	{
		if (craftObject != null && canCraft && craftObject.tag != "Door" && craftObject.tag != "Wall")
		{
			OnCraftProp();
		}
	}

	private void OnCraftProp()
	{
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if (craftObject == NanoInventory.instance.craftedObjects[i].prefab)
			{
				if (NanoInventory.instance.craftedObjectCounts[i] <= 0 && !GameManager.instance.creativeMode)
				{
					return;
				}
				NanoInventory.instance.craftedObjectCounts[i]--;
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(craftObject, proxy.position, proxy.rotation);
		gameObject.name = craftObject.name;
		lastRoomCollider.GetComponentInParent<Room>().OnAddProp(gameObject);
		if ((bool)gameObject.GetComponent<CraftComponent>())
		{
			gameObject.GetComponent<CraftComponent>().Setup();
			gameObject.GetComponent<CraftComponent>().StartCoroutine("Constructing");
		}
		proxy.gameObject.SetActive(false);
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().Toggle(false);
		craftObject = null;
		canCraft = false;
		ArmUIManager.instance.UpdateUI();
	}

	public void AttemtSalvage(GameObject controller)
	{
		if (!(scrapObject != null) || IsInvoking("RepairCooldown") || isRepairing || !(repairObject == null))
		{
			return;
		}
		if (LeackCheck())
		{
			Debug.Log("Too close to leak");
		}
		else if (SuitManager.instance.power > 5f || DroneHelper.instance.VRControlled)
		{
			if (NanoInventory.instance.GetNanoMass() < NanoInventory.instance.nanoCap || IntroManager.instance != null)
			{
				if (!isSalvaging)
				{
					controller.GetComponent<HandController>().ToggleSalvageCone();
					scrapObject.GetComponent<CraftComponent>().controller = controller;
					scrapObject.GetComponent<CraftComponent>().StartCoroutine("OnSalvage");
				}
			}
			else
			{
				SuitManager.instance.InventoryFull();
			}
		}
		else
		{
			SuitManager.instance.LowPowerPrompt();
		}
	}

	private bool LeackCheck()
	{
		Collider[] array = Physics.OverlapSphere(HandController.currentHand.transform.position, 0.5f);
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i].GetComponent<RoomLeak>())
			{
				return true;
			}
		}
		return false;
	}

	public void StopSalvage()
	{
		if (scrapObject != null)
		{
			scrapObject.GetComponent<CraftComponent>().StopCoroutine("OnSalvage");
			isSalvaging = false;
		}
	}

	public void StopRepair()
	{
		if (repairObject != null)
		{
			GameObject gameObject = repairObject;
			repairObject.GetComponent<RoomLeak>().StopCoroutine("Repair");
			repairObject.GetComponent<RoomLeak>().OnCancelRepair();
			isRepairing = false;
			repairObject = gameObject;
			Invoke("RepairCooldown", 0.5f);
		}
		StopSalvage();
	}

	public void AttemtRepair(HandController source)
	{
		if (repairObject == null || isRepairing || IsInvoking("RepairCooldown"))
		{
			return;
		}
		Transform transform = repairObject.transform;
		if (transform.GetComponent<RoomLeak>().isRepairing)
		{
			return;
		}
		if ((NanoInventory.instance.GetGlobalMaterialCount(0) >= transform.GetComponent<RoomLeak>().leakRate && (SuitManager.instance.power >= (float)transform.GetComponent<RoomLeak>().leakRate || (DroneHelper.instance.VRControlled && DroneHelper.instance.power >= (float)transform.GetComponent<RoomLeak>().leakRate))) || GameManager.instance.debugMode)
		{
			isRepairing = true;
			source.ToggleRepairCone();
			transform.GetComponent<RoomLeak>().controller = source.gameObject;
			transform.GetComponent<RoomLeak>().StartCoroutine("Repair");
			UpdateUI();
			source.OnRepairAudio();
		}
		else
		{
			if (NanoInventory.instance.GetGlobalMaterialCount(0) > 0 && SuitManager.instance.power < 3f)
			{
				SuitManager.instance.LowPowerPrompt();
			}
			else
			{
				SuitManager.instance.LowResourcePrompt();
			}
			repairObject = null;
		}
	}

	public void UpdateUI()
	{
		ArmUIManager.instance.UpdateUI();
	}

	private void RepairCooldown()
	{
	}

	public void UpdateProxyState(Vector3 point, Vector3 normal, Collider roomCollider)
	{
		if (BaseManager.instance.inBase)
		{
			if (normal.y > 0.5f)
			{
				if (floorObject)
				{
					canCraft = true;
				}
				else
				{
					canCraft = false;
				}
			}
			else if (normal.y < -0.5f)
			{
				canCraft = false;
			}
			else if (floorObject)
			{
				canCraft = false;
			}
			else
			{
				canCraft = true;
			}
			if (colliding)
			{
				canCraft = false;
			}
		}
		else if (colliding || roomCollider.bounds.Contains(point + normal))
		{
			canCraft = false;
		}
		else
		{
			canCraft = true;
		}
		if (canCraft)
		{
			proxy.GetComponent<MeshRenderer>().material = canCraftMat;
		}
		else
		{
			proxy.GetComponent<MeshRenderer>().material = cantCraftMat;
		}
		proxy.position = point;
		if (!floorObject)
		{
			proxy.LookAt(point + normal * 5f, Vector3.up);
			if (BaseManager.instance.inBase)
			{
				proxy.localEulerAngles = new Vector3(0f, proxy.localEulerAngles.y, proxy.localEulerAngles.z);
			}
		}
		lastRoomCollider = roomCollider;
	}

	public void RotateProxy(bool right)
	{
		if (floorObject)
		{
			if (right)
			{
				proxy.Rotate(Vector3.up, 45f, Space.World);
			}
			else
			{
				proxy.Rotate(Vector3.up, -45f, Space.World);
			}
		}
	}

	public void OnCancelCraft()
	{
		craftObject = null;
		proxy.gameObject.SetActive(false);
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().currentActivationState = 0;
		GameManager.instance.rightController.GetComponent<VRTK_Pointer>().Toggle(false);
	}
}
