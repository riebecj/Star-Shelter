using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;

public class StorageBox : MonoBehaviour
{
	public GameObject boundBox;

	public List<GameObject> inventory = new List<GameObject>();

	public Collider collider;

	private MeshRenderer visual;

	internal bool opened;

	private string boxID;

	public static List<StorageBox> storageBoxes = new List<StorageBox>();

	private Collider newObject;

	private void Awake()
	{
		storageBoxes.Add(this);
		visual = GetComponent<MeshRenderer>();
	}

	private void Start()
	{
		boxID = base.transform.position.ToString();
		OnLoad();
	}

	public void CheckDropInInventory(GameObject droppedObject)
	{
		if (collider.bounds.Contains(droppedObject.GetComponent<Collider>().bounds.center))
		{
			AddToInventory(droppedObject);
		}
	}

	private void AddToInventory(GameObject newObject)
	{
		if (!inventory.Contains(newObject))
		{
			newObject.GetComponent<AddToInventory>().OnRelease();
			inventory.Add(newObject);
			newObject.GetComponent<Rigidbody>().isKinematic = true;
			newObject.transform.SetParent(boundBox.transform);
			newObject.layer = 21;
			newObject.SendMessage("OnAddedToInventory", SendMessageOptions.DontRequireReceiver);
			HintManager.instance.inventorySaving = true;
			PreviewLabs.PlayerPrefs.SetBool("inventorySaving", true);
		}
	}

	public void OnSave()
	{
		for (int i = 0; i < inventory.Count; i++)
		{
			PreviewLabs.PlayerPrefs.SetString(boxID + "StorageObject" + i, inventory[i].name);
			if (inventory[i].name.Contains("FruitSeed"))
			{
				PreviewLabs.PlayerPrefs.SetInt(boxID + "SeedType" + i, inventory[i].GetComponent<Seed>().value);
			}
			PreviewLabs.PlayerPrefs.SetFloat(boxID + "StorageXPosition" + i, inventory[i].transform.localPosition.x);
			PreviewLabs.PlayerPrefs.SetFloat(boxID + "StorageYPosition" + i, inventory[i].transform.localPosition.y);
			PreviewLabs.PlayerPrefs.SetFloat(boxID + "StorageZPosition" + i, inventory[i].transform.localPosition.z);
			PreviewLabs.PlayerPrefs.SetFloat(boxID + "StorageXRotation" + i, inventory[i].transform.localEulerAngles.x);
			PreviewLabs.PlayerPrefs.SetFloat(boxID + "StorageYRotation" + i, inventory[i].transform.localEulerAngles.y);
			PreviewLabs.PlayerPrefs.SetFloat(boxID + "StorageZRotation" + i, inventory[i].transform.localEulerAngles.z);
		}
		PreviewLabs.PlayerPrefs.SetInt(boxID + "StorageCount", inventory.Count);
	}

	public void OnLoad()
	{
		bool flag = false;
		for (int i = 0; i < PreviewLabs.PlayerPrefs.GetInt(boxID + "StorageCount"); i++)
		{
			GameObject gameObject = null;
			foreach (GameObject @object in LootSpawnManager.instance.objectList)
			{
				if (!(@object.name == PreviewLabs.PlayerPrefs.GetString(boxID + "StorageObject" + i)))
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
					gameObject2.transform.SetParent(boundBox.transform);
					gameObject2.transform.localPosition = new Vector3(PreviewLabs.PlayerPrefs.GetFloat(boxID + "StorageXPosition" + i), PreviewLabs.PlayerPrefs.GetFloat(boxID + "StorageYPosition" + i), PreviewLabs.PlayerPrefs.GetFloat(boxID + "StorageZPosition" + i));
					gameObject2.transform.localEulerAngles = new Vector3(PreviewLabs.PlayerPrefs.GetFloat(boxID + "StorageXRotation" + i), PreviewLabs.PlayerPrefs.GetFloat(boxID + "StorageYRotation" + i), PreviewLabs.PlayerPrefs.GetFloat(boxID + "StorageZRotation" + i));
					AddToInventory(gameObject2);
					if (gameObject.name.Contains("FruitSeed"))
					{
						gameObject.GetComponent<Seed>().SetType(PreviewLabs.PlayerPrefs.GetInt(boxID + "SeedType" + i));
					}
				}
			}
		}
	}

	private void OnDisable()
	{
		storageBoxes.Remove(this);
	}
}
