using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Recipe : MonoBehaviour
{
	public int powerCost = 1;

	[Tooltip("Dont create in world")]
	public bool addToNanoInventory = true;

	public List<CraftMaterial> craftMaterials = new List<CraftMaterial>();

	public int[] materialCosts = new int[1];

	public float craftTime;

	internal float startTime;

	internal Image craftProgressUI;

	internal Text craftProgressNumber;

	internal List<MeshRenderer> visuals = new List<MeshRenderer>();

	private GameObject craftBar;

	public GameObject[] delayedObjects;

	private void Start()
	{
		craftTime /= CraftingManager.instance.craftSpeedMultiplier;
		if (GameManager.instance.debugMode || GameManager.instance.loading)
		{
			craftTime = 0.1f;
		}
		if (delayedObjects.Length > 0)
		{
			Invoke("EnableDelayedObjects", craftTime + 0.5f);
		}
		if ((bool)base.gameObject.transform.Find("PlacementCollider"))
		{
			base.gameObject.transform.Find("PlacementCollider").gameObject.SetActive(false);
		}
	}

	private void EnableDelayedObjects()
	{
		for (int i = 0; i < delayedObjects.Length; i++)
		{
			delayedObjects[i].SetActive(true);
		}
	}

	public void Construct()
	{
		Debug.Log("Recipe Construct");
		if (CraftingManager.instance.craftBars.Count > 0)
		{
			craftBar = CraftingManager.instance.craftBars[0];
			CraftingManager.instance.craftBars.Remove(craftBar);
			craftBar.SetActive(true);
			craftBar.transform.SetParent(base.transform);
			craftProgressUI = craftBar.GetComponent<CraftBar>().ring;
			craftProgressNumber = craftBar.GetComponent<CraftBar>().number;
			craftProgressUI.gameObject.SetActive(true);
			startTime = craftTime;
			StartCoroutine("Constructing");
		}
		else
		{
			Invoke("Construct", 0.15f);
		}
	}

	private IEnumerator Constructing()
	{
		Debug.Log("B");
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = true;
			if (meshRenderer.material.name.StartsWith(GameManager.instance.atlasMaterial.name))
			{
				meshRenderer.material = GameManager.instance.craftMaterial;
				visuals.Add(meshRenderer);
			}
		}
		foreach (MeshRenderer visual in visuals)
		{
			visual.material.SetFloat("_Cutoff", 1f);
		}
		float waitTime = 0.05f;
		while (craftTime > 0f)
		{
			craftTime -= waitTime;
			float value = (startTime - craftTime) / startTime;
			craftProgressUI.fillAmount = value;
			craftProgressNumber.text = (craftTime + 1f).ToString("F0");
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
		CraftingManager.instance.craftBars.Add(craftBar);
		craftBar.SetActive(false);
		SendMessage("CraftingComplete", SendMessageOptions.DontRequireReceiver);
		if ((bool)ArmUIManager.instance)
		{
			ArmUIManager.instance.UpdateUI();
		}
	}

	public bool CanPlace(Transform node)
	{
		if (GameManager.instance.debugMode)
		{
			return true;
		}
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if (NanoInventory.instance.craftedObjects[i].prefab == base.gameObject && NanoInventory.instance.craftedObjectCounts[i] > 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasPart()
	{
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if (NanoInventory.instance.craftedObjects[i].prefab == base.gameObject && NanoInventory.instance.craftedObjectCounts[i] > 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool InventoryCheck()
	{
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if (NanoInventory.instance.craftedObjects[i].prefab == base.gameObject && NanoInventory.instance.craftedObjectCounts[i] > 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanCraft()
	{
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			for (int j = 0; j < NanoInventory.instance.craftMaterials.Count; j++)
			{
				if (NanoInventory.instance.craftMaterials[j] == craftMaterials[i])
				{
					if (NanoInventory.instance.GetGlobalMaterialCount(j) < materialCosts[i])
					{
						return false;
					}
					if (BaseManager.instance.power < (float)powerCost)
					{
						return false;
					}
				}
			}
		}
		if (NanoInventory.instance.GetGlobalMass() >= NanoInventory.instance.GetGlobalCapacity() && base.tag == "CraftMaterial")
		{
			return false;
		}
		return true;
	}

	public bool CanUpgrade()
	{
		Upgrade component = GetComponent<Upgrade>();
		if ((bool)component)
		{
			if (component.suitUpgrade == Upgrade.SuitUpgrade.oxygen && SuitManager.instance.OxygenCapacity > 3)
			{
				return false;
			}
			if (component.suitUpgrade == Upgrade.SuitUpgrade.power && SuitManager.instance.PowerCapacity > 3)
			{
				return false;
			}
			if (component.suitUpgrade == Upgrade.SuitUpgrade.thrusters && SuitManager.instance.ThrusterSpeed > 3)
			{
				return false;
			}
		}
		return true;
	}

	private void AddCraftComponent()
	{
		CraftComponent craftComponent = base.gameObject.AddComponent<CraftComponent>();
		craftComponent.materialCounts = new int[materialCosts.Length];
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			craftComponent.craftMaterials.Add(craftMaterials[i]);
			craftComponent.materialCounts[i] = (int)((float)materialCosts[i] * 0.8f);
		}
	}
}
