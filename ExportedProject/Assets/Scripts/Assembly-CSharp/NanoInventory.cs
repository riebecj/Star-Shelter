using System.Collections.Generic;
using PreviewLabs;
using Sirenix.OdinInspector;
using UnityEngine;

public class NanoInventory : MonoBehaviour
{
	public static NanoInventory instance;

	public int nanoCap = 25;

	public int nanoMass;

	[ListDrawerSettings(NumberOfItemsPerPage = 10, DraggableItems = false, Expanded = false, ShowIndexLabels = true, ShowItemCount = false)]
	public List<CraftMaterial> craftMaterials = new List<CraftMaterial>();

	[ListDrawerSettings(NumberOfItemsPerPage = 10, DraggableItems = false, Expanded = false, ShowIndexLabels = true, ShowItemCount = false)]
	public List<int> materialCounts = new List<int>();

	[ListDrawerSettings(NumberOfItemsPerPage = 10, DraggableItems = false, Expanded = false, ShowIndexLabels = true, ShowItemCount = false)]
	public List<CraftedObject> craftedObjects = new List<CraftedObject>();

	[ListDrawerSettings(NumberOfItemsPerPage = 10, DraggableItems = false, Expanded = false, ShowIndexLabels = true, ShowItemCount = false)]
	public List<int> craftedObjectCounts = new List<int>();

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		OnLoad();
		nanoMass = GetNanoMass();
	}

	public void OnSave()
	{
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			PreviewLabs.PlayerPrefs.SetInt(craftMaterials[i].name + "Count", materialCounts[i]);
		}
		for (int j = 0; j < craftedObjects.Count; j++)
		{
			PreviewLabs.PlayerPrefs.SetInt(craftedObjects[j].name + "Count", craftedObjectCounts[j]);
		}
	}

	public void OnLoad()
	{
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			materialCounts[i] = PreviewLabs.PlayerPrefs.GetInt(craftMaterials[i].name + "Count");
		}
		for (int j = 0; j < craftedObjects.Count; j++)
		{
			craftedObjectCounts[j] = PreviewLabs.PlayerPrefs.GetInt(craftedObjects[j].name + "Count");
		}
	}

	public void ClearComponents()
	{
		for (int i = 0; i < materialCounts.Count; i++)
		{
			materialCounts[i] = 0;
		}
		nanoMass = 0;
		ArmUIManager.instance.UpdateInventoryUI();
	}

	public int GetNanoMass()
	{
		int num = 0;
		for (int i = 0; i < materialCounts.Count; i++)
		{
			num += materialCounts[i];
		}
		nanoMass = num;
		if ((bool)ArmUIManager.instance)
		{
			ArmUIManager.instance.UpdateInventoryUI();
		}
		return num;
	}

	internal void UpgradeSuitNanoCapacity()
	{
		nanoCap += 50;
	}

	public int GetGlobalMaterialCount(int index)
	{
		int num = 0;
		num += materialCounts[index];
		for (int i = 0; i < NanoStorage.nanoStorages.Count; i++)
		{
			num += NanoStorage.nanoStorages[i].materialCounts[index];
		}
		return num;
	}

	public int GetGlobalMass()
	{
		int num = 0;
		num += nanoMass;
		for (int i = 0; i < NanoStorage.nanoStorages.Count; i++)
		{
			num += NanoStorage.nanoStorages[i].GetNanoMass();
		}
		return num;
	}

	public int GetGlobalCapacity()
	{
		int num = 0;
		num += nanoCap;
		for (int i = 0; i < NanoStorage.nanoStorages.Count; i++)
		{
			num += NanoStorage.nanoStorages[i].nanoCap;
		}
		return num;
	}
}
