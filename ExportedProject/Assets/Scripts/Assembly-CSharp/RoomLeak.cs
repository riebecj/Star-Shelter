using System.Collections;
using UnityEngine;
using VRTK;

public class RoomLeak : MonoBehaviour
{
	public int leakRate;

	private Room _room;

	internal bool isRepairing;

	private Collider triggerCol;

	public Material startMat;

	private float alpha = 0.45f;

	internal GameObject controller;

	internal float Vibration = 0.1f;

	private MeshRenderer renderer;

	public GameObject leakPoint;

	private ParticleSystem particle;

	private void Start()
	{
		renderer = GetComponentInChildren<MeshRenderer>();
		startMat = GameManager.instance.atlasMaterial;
		if (leakPoint == null)
		{
			leakPoint = GetClosestLeakNode(base.transform).gameObject;
		}
		if (BaseLoader.instance.leakHoles.Contains(leakPoint))
		{
			BaseLoader.instance.leakHoles.Remove(leakPoint);
		}
		particle = GetComponentInChildren<ParticleSystem>();
		StartCoroutine("CheckForOxygen");
	}

	public void Setup(Room room)
	{
		base.transform.SetParent(room.transform);
		_room = room;
		_room.leakRate += leakRate;
		_room.holes.Add(this);
	}

	private void OnDisable()
	{
		_room.holes.Remove(this);
		_room.leakRate -= leakRate;
		BaseLoader.instance.leakHoles.Remove(base.gameObject);
	}

	public IEnumerator Repair()
	{
		isRepairing = true;
		renderer.material = GameManager.instance.lootDeteriorate;
		while (alpha < 1f && SuitManager.instance.power > 1f)
		{
			alpha += 0.01f * CraftingManager.instance.salvageSpeedMultiplier;
			renderer.material.SetFloat("_Cutoff", Mathf.Clamp(alpha / 4f + 0.45f, 0.45f, 0.7f));
			if ((bool)controller)
			{
				VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(controller), Vibration);
			}
			yield return new WaitForSeconds(0.05f);
		}
		if (alpha < 1f)
		{
			yield break;
		}
		CraftingManager.instance.repairObject = null;
		if (NanoInventory.instance.materialCounts[0] >= leakRate)
		{
			NanoInventory.instance.materialCounts[0] -= leakRate;
		}
		else
		{
			int num = 100;
			int num2 = leakRate;
			while (num2 > 0 && num > 0)
			{
				if (NanoInventory.instance.materialCounts[0] > 0)
				{
					NanoInventory.instance.materialCounts[0]--;
					num2--;
				}
				for (int i = 0; i < NanoStorage.nanoStorages.Count; i++)
				{
					if (NanoStorage.nanoStorages[i].materialCounts[0] > 0 && num2 > 0)
					{
						NanoStorage.nanoStorages[i].materialCounts[0]--;
						num2--;
					}
				}
				num--;
			}
		}
		if (!DroneHelper.instance.VRControlled)
		{
			SuitManager.instance.power -= 3.5f;
		}
		else
		{
			DroneHelper.instance.OnDrawPower(2f);
		}
		if (SuitManager.instance.power < 0f)
		{
			SuitManager.instance.power = 0f;
		}
		_room.holes.Remove(this);
		BaseLoader.instance.leakHoles.Add(leakPoint);
		base.gameObject.SetActive(false);
		CraftingManager.instance.isRepairing = false;
		CraftingManager.instance.scrapObject = null;
		CraftingManager.instance.Invoke("RepairCooldown", 1f);
		if ((bool)triggerCol)
		{
			if ((bool)triggerCol.GetComponentInParent<HandController>())
			{
				triggerCol.GetComponentInParent<HandController>().componentUI.SetActive(false);
			}
			else if ((bool)triggerCol.GetComponent<DroneCone>())
			{
				triggerCol.GetComponent<DroneCone>().drone.OnExitRepair();
			}
		}
		if ((bool)PieIntroEvent.instance)
		{
			PieIntroEvent.instance.OnRepaired();
		}
	}

	public IEnumerator DroneRepair()
	{
		isRepairing = true;
		renderer.material = GameManager.instance.lootDeteriorate;
		while (alpha < 1f && SuitManager.instance.power > 1f)
		{
			alpha += 0.01f;
			renderer.material.SetFloat("_Cutoff", Mathf.Clamp(alpha / 4f + 0.45f, 0.45f, 0.7f));
			yield return new WaitForSeconds(0.05f);
		}
		if (alpha < 1f)
		{
			yield break;
		}
		if (NanoInventory.instance.materialCounts[0] >= leakRate)
		{
			NanoInventory.instance.materialCounts[0] -= leakRate;
		}
		else
		{
			int num = 100;
			int num2 = leakRate;
			while (num2 > 0 && num > 0)
			{
				if (NanoInventory.instance.materialCounts[0] > 0)
				{
					NanoInventory.instance.materialCounts[0]--;
					num2--;
				}
				for (int i = 0; i < NanoStorage.nanoStorages.Count; i++)
				{
					if (NanoStorage.nanoStorages[i].materialCounts[0] > 0 && num2 > 0)
					{
						NanoStorage.nanoStorages[i].materialCounts[0]--;
						num2--;
					}
				}
				num--;
			}
		}
		DroneHelper.instance.OnDrawPower(3.5f);
		_room.holes.Remove(this);
		BaseLoader.instance.leakHoles.Add(leakPoint);
		base.gameObject.SetActive(false);
		if ((bool)triggerCol && (bool)triggerCol.GetComponent<DroneCone>())
		{
			triggerCol.GetComponent<DroneCone>().drone.OnExitRepair();
		}
		if ((bool)PieIntroEvent.instance)
		{
			PieIntroEvent.instance.OnRepaired();
		}
	}

	public void OnCancelRepair()
	{
		CraftingManager.instance.isRepairing = false;
		isRepairing = false;
		renderer.material = startMat;
		alpha = 0.45f;
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<SalvageCone>())
		{
			CraftingManager.instance.repairObject = base.gameObject;
			triggerCol = other;
			other.GetComponentInParent<HandController>().componentUI.SetActive(true);
			other.GetComponentInParent<HandController>().componentUI.GetComponent<ComponentUI>().Setup(this);
			CraftingManager.instance.scrapObject = null;
		}
		if ((bool)other.GetComponent<DroneCone>())
		{
			triggerCol = other;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<SalvageCone>() && CraftingManager.instance.repairObject == base.gameObject && other == triggerCol)
		{
			StopAllCoroutines();
			isRepairing = false;
			OnCancelRepair();
			alpha = 0.45f;
			if (CraftingManager.instance.repairObject == base.gameObject)
			{
				CraftingManager.instance.repairObject = null;
			}
			CraftingManager.instance.scrapObject = null;
			other.GetComponentInParent<HandController>().componentUI.SetActive(false);
		}
		if ((bool)other.GetComponent<DroneCone>() && DroneHelper.instance.repairObject == base.gameObject && other == triggerCol)
		{
			DroneHelper.instance.OnExitRepair();
			isRepairing = false;
			renderer.material = startMat;
			alpha = 0.45f;
		}
	}

	private Transform GetClosestLeakNode(Transform leakPoint)
	{
		float num = float.PositiveInfinity;
		float num2 = 0f;
		Transform result = null;
		for (int i = 0; i < BaseLoader.instance.leakHoles.Count; i++)
		{
			num2 = Vector3.Distance(BaseLoader.instance.leakHoles[i].transform.position, leakPoint.position);
			if (num2 < num)
			{
				num = num2;
				result = BaseLoader.instance.leakHoles[i].transform;
			}
		}
		return result;
	}

	private IEnumerator CheckForOxygen()
	{
		while (true)
		{
			if (_room.Oxygen < 2f)
			{
				particle.gameObject.SetActive(false);
			}
			else
			{
				particle.gameObject.SetActive(true);
			}
			yield return new WaitForSeconds(2f);
		}
	}
}
