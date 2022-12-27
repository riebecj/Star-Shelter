using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CraftComponent : MonoBehaviour
{
	public List<CraftMaterial> craftMaterials = new List<CraftMaterial>();

	public int[] materialCounts = new int[1];

	public static List<CraftComponent> craftComponents = new List<CraftComponent>();

	private VRTK_InteractableObject interact;

	internal Transform hand;

	internal GameObject controller;

	internal float Vibration = 0.1f;

	private GameObject uiObject;

	internal Material[] materials;

	internal Material[] startMats;

	internal Collider triggerCol;

	private float alpha = 0.45f;

	public bool randomize;

	[Tooltip("Incase you dont want the first collider found in the hierarchy")]
	public Collider salvageCollider;

	private float craftTime = 1f;

	private float startTime;

	public float salvageTime = 1f;

	internal bool outlineOn;

	internal List<MeshRenderer> visuals = new List<MeshRenderer>();

	internal bool constructing;

	public void Setup()
	{
		startMats = GetComponent<MeshRenderer>().materials;
		materials = startMats;
	}

	private void Awake()
	{
		if (startMats == null)
		{
			startMats = GetComponent<MeshRenderer>().materials;
			materials = startMats;
		}
	}

	private void Start()
	{
		craftComponents.Add(this);
		if ((bool)GetComponent<VRTK_InteractableObject>())
		{
			interact = GetComponent<VRTK_InteractableObject>();
			interact.InteractableObjectGrabbed += DoObjectGrab;
		}
		if (randomize)
		{
			for (int i = 0; i < craftMaterials.Count; i++)
			{
				craftMaterials[i] = NanoInventory.instance.craftMaterials[UnityEngine.Random.Range(0, NanoInventory.instance.craftMaterials.Count)];
			}
		}
		if ((bool)salvageCollider)
		{
			salvageTime = Mathf.Clamp(salvageCollider.bounds.size.magnitude, 1f, 8f);
		}
		else if ((bool)GetComponentInChildren<Collider>())
		{
			salvageTime = Mathf.Clamp(GetComponentInChildren<Collider>().bounds.size.magnitude, 1f, 8f);
		}
	}

	private void SetupUI()
	{
		uiObject = UnityEngine.Object.Instantiate(GameManager.instance.componentUI, base.transform.position, base.transform.rotation);
		uiObject.transform.position = new Vector3(base.transform.position.x, GetComponentInChildren<Collider>().bounds.max.y, base.transform.position.z) + Vector3.up * 0.01f;
		uiObject.transform.SetParent(base.transform);
		uiObject.GetComponent<ComponentUI>().Setup(this);
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
	}

	public void OnAdd(bool fromSalvage)
	{
		int num = 0;
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			num += materialCounts[i];
		}
		int num2 = 15000;
		while (NanoInventory.instance.GetGlobalMass() < NanoInventory.instance.GetGlobalCapacity() && num > 0 && num2 > 0)
		{
			for (int j = 0; j < NanoInventory.instance.craftMaterials.Count; j++)
			{
				for (int k = 0; k < craftMaterials.Count; k++)
				{
					if (materialCounts[k] > 0 && NanoInventory.instance.craftMaterials[j] == craftMaterials[k])
					{
						if (NanoInventory.instance.GetNanoMass() < NanoInventory.instance.nanoCap)
						{
							NanoInventory.instance.materialCounts[j]++;
						}
						else
						{
							for (int l = 0; l < NanoStorage.nanoStorages.Count; l++)
							{
								if (NanoStorage.nanoStorages[l].GetNanoMass() < NanoStorage.nanoStorages[l].nanoCap)
								{
									NanoStorage.nanoStorages[l].materialCounts[j]++;
								}
							}
						}
						materialCounts[k]--;
						num--;
					}
					num2--;
				}
			}
		}
		StatManager.instance.UpdateText();
		ArmUIManager.instance.UpdateUI();
	}

	private void StartMaterialDerioration()
	{
		materials = GetComponent<MeshRenderer>().materials;
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i] = GameManager.instance.lootDeteriorate;
		}
		GetComponent<MeshRenderer>().materials = materials;
	}

	public IEnumerator OnSalvage()
	{
		CraftingManager.instance.isSalvaging = true;
		StartMaterialDerioration();
		while (alpha < salvageTime + 0.45f && SuitManager.instance.power > 1f)
		{
			alpha += 0.02f * CraftingManager.instance.salvageSpeedMultiplier;
			Material[] array = materials;
			foreach (Material material in array)
			{
				material.SetFloat("_Cutoff", Mathf.Clamp(alpha / (salvageTime * 4f) + 0.45f, 0.45f, 0.7f));
			}
			CancelInvoke("ForceExit");
			Invoke("ForceExit", 1f);
			GetComponent<MeshRenderer>().materials = materials;
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(controller), Vibration);
			yield return new WaitForSeconds(0.02f);
		}
		if (!(alpha < salvageTime))
		{
			if (!base.transform.root.GetComponent<BaseManager>() || (bool)GetComponent<DisableOnSalvage>())
			{
				OnCompleteSalvage();
			}
			else
			{
				TriggerSalvageWarning();
			}
		}
		CraftingManager.instance.scrapObject = null;
		CraftingManager.instance.isSalvaging = false;
		if (triggerCol != null)
		{
			triggerCol.GetComponentInParent<HandController>().componentUI.SetActive(false);
		}
		if (!IntroManager.instance)
		{
			for (int j = 0; j < CraftStation.craftStations.Count; j++)
			{
				CraftStation.craftStations[j].UpdateUI();
			}
		}
		GameManager.instance.leftController.parent.GetComponent<HandController>().CompleteSalvage();
	}

	public IEnumerator OnDroneSalvage()
	{
		StartMaterialDerioration();
		while (alpha < salvageTime + 0.45f && SuitManager.instance.power > 1f)
		{
			alpha += 0.02f * CraftingManager.instance.salvageSpeedMultiplier;
			Material[] array = materials;
			foreach (Material material in array)
			{
				material.SetFloat("_Cutoff", Mathf.Clamp(alpha / (salvageTime * 4f) + 0.45f, 0.45f, 0.7f));
			}
			CancelInvoke("ForceExitDrone");
			Invoke("ForceExitDrone", 1f);
			GetComponent<MeshRenderer>().materials = materials;
			yield return new WaitForSeconds(0.02f);
		}
		if (!(alpha < salvageTime))
		{
			OnCompleteSalvage();
		}
		for (int j = 0; j < CraftStation.craftStations.Count; j++)
		{
			CraftStation.craftStations[j].UpdateUI();
		}
	}

	private void TriggerSalvageWarning()
	{
		ArmUIManager.instance.warningComponent = this;
		ArmUIManager.instance.ShowSalvageWarning();
	}

	public void OnCompleteSalvage()
	{
		if (!DroneHelper.instance.VRControlled)
		{
			SuitManager.instance.power -= 3.5f;
		}
		else
		{
			DroneHelper.instance.OnDrawPower(2f);
		}
		if (IntroManager.instance == null)
		{
			DroneHelper.instance.ToggleActiveCursor(false);
		}
		if (SuitManager.instance.power < 0f)
		{
			SuitManager.instance.power = 0f;
		}
		OnAdd(true);
		ArmUIManager.instance.OnAddComponent(uiObject);
		if ((bool)interact)
		{
			interact.ForceStopInteracting();
		}
		if (!GetComponent<WallNode>() && !IsInvoking("Destruct"))
		{
			Invoke("Destruct", 0.05f);
		}
		else if ((bool)GetComponent<WallNode>())
		{
			GetComponent<WallNode>().OnSalvage();
		}
		NanoInventory.instance.GetNanoMass();
	}

	public void CancelSalvage()
	{
		for (int i = 0; i < startMats.Length; i++)
		{
			materials[i] = startMats[i];
		}
		GetComponent<MeshRenderer>().materials = startMats;
		alpha = 0.45f;
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<SalvageCone>() && !CraftingManager.instance.repairObject && !CraftingManager.instance.isRepairing)
		{
			SetTarget(other);
		}
	}

	public void SetTarget(Collider other)
	{
		if (!CraftingManager.instance.repairObject)
		{
			if (CraftingManager.instance.isSalvaging)
			{
				Debug.Log("Warning");
			}
			CraftingManager.instance.scrapObject = base.gameObject;
			triggerCol = other;
			if ((bool)other.GetComponentInParent<HandController>().componentUI)
			{
				other.GetComponentInParent<HandController>().componentUI.SetActive(true);
				other.GetComponentInParent<HandController>().componentUI.GetComponent<ComponentUI>().Setup(this);
			}
			CraftingManager.instance.repairObject = null;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<SalvageCone>())
		{
			OnExitTarget(other, false);
		}
		if ((bool)other.GetComponent<DroneCone>())
		{
			OnExitTarget(other, true);
		}
	}

	public void OnExitTarget(Collider other, bool isDrone)
	{
		if ((CraftingManager.instance.scrapObject == base.gameObject || DroneHelper.instance.scrapObject == base.gameObject) && other == triggerCol)
		{
			StopCoroutine("OnSalvage");
			StopCoroutine("OnDroneSalvage");
			for (int i = 0; i < startMats.Length; i++)
			{
				materials[i] = startMats[i];
			}
			GetComponent<MeshRenderer>().materials = startMats;
			alpha = 0.45f;
			if (!isDrone)
			{
				CraftingManager.instance.scrapObject = null;
				CraftingManager.instance.isSalvaging = false;
				other.GetComponentInParent<HandController>().componentUI.SetActive(false);
			}
			else
			{
				DroneHelper.instance.scrapObject = null;
				DroneHelper.instance.isSalvaging = false;
			}
			ToggleOutline(true);
			triggerCol = null;
		}
	}

	private void ForceExit()
	{
		if (triggerCol != null)
		{
			OnExitTarget(triggerCol, false);
		}
	}

	private void ForceExitDrone()
	{
		if (triggerCol != null)
		{
			OnExitTarget(triggerCol, true);
		}
	}

	private void OnDisable()
	{
		if (CraftingManager.instance.scrapObject == base.gameObject && (bool)triggerCol)
		{
			triggerCol.GetComponentInParent<HandController>().componentUI.SetActive(false);
		}
	}

	private void Destruct()
	{
		base.gameObject.SendMessage("OnSalvage", SendMessageOptions.DontRequireReceiver);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void ToggleOutline(bool On)
	{
		if (outlineOn != On)
		{
			outlineOn = On;
			Material[] array = new Material[GetComponent<MeshRenderer>().materials.Length + 1];
			GetComponent<MeshRenderer>().materials.CopyTo(array, 0);
			if (On)
			{
				array[array.Length - 1] = GameManager.instance.outlineMaterial;
				GetComponent<MeshRenderer>().materials = array;
			}
			else
			{
				GetComponent<MeshRenderer>().materials = startMats;
			}
		}
	}

	private static Component GetRandomEnumComponent()
	{
		Array values = Enum.GetValues(typeof(Component));
		return (Component)values.GetValue(UnityEngine.Random.Range(1, values.Length));
	}

	public IEnumerator Constructing()
	{
		constructing = true;
		if ((bool)GetComponent<Recipe>())
		{
			craftTime = GetComponent<Recipe>().craftTime;
		}
		startTime = craftTime;
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = true;
			if (meshRenderer.material.name.StartsWith(GameManager.instance.atlasMaterial.name) && (!meshRenderer.GetComponent<Recipe>() || meshRenderer.transform == base.transform))
			{
				meshRenderer.material = GameManager.instance.craftMaterial;
				visuals.Add(meshRenderer);
			}
		}
		foreach (MeshRenderer visual in visuals)
		{
			visual.material.EnableKeyword("_Cutoff");
			visual.material.SetFloat("_Cutoff", 0.75f);
		}
		float waitTime = 0.05f;
		while (craftTime > 0f)
		{
			craftTime -= waitTime;
			float value = 0.25f + (startTime - craftTime) / startTime / 2f;
			foreach (MeshRenderer visual2 in visuals)
			{
				visual2.material.SetFloat("_Cutoff", 1f - value);
			}
			yield return new WaitForSeconds(waitTime);
		}
		foreach (MeshRenderer visual3 in visuals)
		{
			visual3.material = GameManager.instance.atlasMaterial;
		}
		constructing = false;
		base.gameObject.SendMessage("CraftingComplete", SendMessageOptions.DontRequireReceiver);
	}

	private void OnDestroy()
	{
		craftComponents.Remove(this);
	}
}
