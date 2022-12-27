using System.Collections.Generic;
using PreviewLabs;
using Sirenix.OdinInspector;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public static Inventory instance;

	public MeshRenderer[] buttons;

	[ListDrawerSettings(DraggableItems = true, Expanded = false, ShowIndexLabels = true, ShowPaging = false, ShowItemCount = true)]
	public GameObject[] fruitplants;

	[ListDrawerSettings(DraggableItems = true, Expanded = false, ShowIndexLabels = true, ShowPaging = false, ShowItemCount = true)]
	public GameObject[] oxygenPlants;

	public Material buttonMaterial;

	public Material buttonHighlightedMaterial;

	public GameObject inventoryBubble;

	internal static List<GameObject> inventory = new List<GameObject>();

	internal bool hasNanoMass;

	private void Awake()
	{
		instance = this;
	}

	public void SelectCraftObjects(int index)
	{
		MeshRenderer[] array = buttons;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.material = buttonMaterial;
		}
	}

	public void CheckDropInInventory(GameObject droppedObject)
	{
		if (inventoryBubble.GetComponent<SphereCollider>().bounds.Contains(droppedObject.GetComponentInChildren<Collider>().bounds.center))
		{
			AddToInventory(droppedObject);
		}
	}

	private void AddToInventory(GameObject newObject)
	{
		if (!inventory.Contains(newObject))
		{
			TutorialManager.instance.OnPhysicalInventoryComplete();
			newObject.GetComponent<AddToInventory>().OnRelease();
			newObject.GetComponent<AddToInventory>().CancelInvoke("Restore");
			newObject.layer = 21;
			inventory.Add(newObject);
			newObject.GetComponent<Rigidbody>().isKinematic = true;
			newObject.transform.SetParent(inventoryBubble.transform);
			newObject.SendMessage("OnAddedToInventory", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ToggleMenu(Transform parent)
	{
		if (parent.name != "RightController")
		{
			inventoryBubble.transform.parent.SetParent(parent);
			inventoryBubble.transform.parent.localPosition = new Vector3(-0.0319f, 0f, -0.1423f);
			inventoryBubble.transform.parent.localEulerAngles = new Vector3(0f, 270f, 0f);
		}
		else
		{
			inventoryBubble.transform.parent.SetParent(parent);
			inventoryBubble.transform.parent.localPosition = new Vector3(0.0319f, 0f, -0.1423f);
			inventoryBubble.transform.parent.localEulerAngles = new Vector3(0f, 90f, 0f);
		}
		inventoryBubble.transform.parent.gameObject.SetActive(!inventoryBubble.transform.parent.gameObject.activeSelf);
	}

	public void OnSave()
	{
		for (int i = 0; i < inventory.Count; i++)
		{
			if (inventory[i] != null)
			{
				PreviewLabs.PlayerPrefs.SetString("InventoryObject" + i, inventory[i].name);
				PreviewLabs.PlayerPrefs.SetFloat("InventoryXPosition" + i, inventory[i].transform.localPosition.x);
				PreviewLabs.PlayerPrefs.SetFloat("InventoryYPosition" + i, inventory[i].transform.localPosition.y);
				PreviewLabs.PlayerPrefs.SetFloat("InventoryZPosition" + i, inventory[i].transform.localPosition.z);
				PreviewLabs.PlayerPrefs.SetFloat("InventoryXRotation" + i, inventory[i].transform.localEulerAngles.x);
				PreviewLabs.PlayerPrefs.SetFloat("InventoryYRotation" + i, inventory[i].transform.localEulerAngles.y);
				PreviewLabs.PlayerPrefs.SetFloat("InventoryZRotation" + i, inventory[i].transform.localEulerAngles.z);
			}
		}
		PreviewLabs.PlayerPrefs.SetInt("InventoryCount", inventory.Count);
	}

	public void OnLoad()
	{
		bool flag = false;
		for (int i = 0; i < PreviewLabs.PlayerPrefs.GetInt("InventoryCount"); i++)
		{
			GameObject gameObject = null;
			foreach (GameObject @object in LootSpawnManager.instance.objectList)
			{
				if (!(@object.name == PreviewLabs.PlayerPrefs.GetString("InventoryObject" + i)))
				{
					continue;
				}
				gameObject = @object;
				if (!(gameObject.name == "HandShield") || !flag)
				{
					if (gameObject.name == "HandShield")
					{
						flag = true;
					}
					GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation);
					gameObject2.name = gameObject.name;
					gameObject2.layer = 21;
					gameObject2.transform.SetParent(inventoryBubble.transform);
					gameObject2.transform.localPosition = new Vector3(PreviewLabs.PlayerPrefs.GetFloat("InventoryXPosition" + i), PreviewLabs.PlayerPrefs.GetFloat("InventoryYPosition" + i), PreviewLabs.PlayerPrefs.GetFloat("InventoryZPosition" + i));
					gameObject2.transform.localEulerAngles = new Vector3(PreviewLabs.PlayerPrefs.GetFloat("InventoryXRotation" + i), PreviewLabs.PlayerPrefs.GetFloat("InventoryYRotation" + i), PreviewLabs.PlayerPrefs.GetFloat("InventoryZRotation" + i));
					AddToInventory(gameObject2);
				}
			}
		}
	}
}
