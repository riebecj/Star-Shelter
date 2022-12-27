using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;

public class BaseStorage : MonoBehaviour
{
	public GameObject Bubble;

	internal List<GameObject> inventory = new List<GameObject>();

	private Collider collider;

	private MeshRenderer visual;

	public GameObject[] startObjects;

	internal bool opened;

	private void Awake()
	{
		collider = GetComponent<SphereCollider>();
		visual = GetComponent<MeshRenderer>();
	}

	public void SpawnStartObjects()
	{
		GameObject[] array = startObjects;
		foreach (GameObject original in array)
		{
			Vector3 position = Bubble.transform.position + Random.onUnitSphere * 0.15f + Vector3.down * 0.3f;
			GameObject gameObject = Object.Instantiate(original, position, Random.rotation);
			gameObject.transform.SetParent(Bubble.transform);
		}
	}

	public void CheckDropInInventory(GameObject droppedObject)
	{
		if ((bool)Bubble && Bubble.GetComponent<SphereCollider>().bounds.Contains(droppedObject.transform.position))
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
			newObject.transform.SetParent(Bubble.transform);
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
			PreviewLabs.PlayerPrefs.SetString("StorageObject" + i, inventory[i].name);
			PreviewLabs.PlayerPrefs.SetFloat("StorageXPosition" + i, inventory[i].transform.localPosition.x);
			PreviewLabs.PlayerPrefs.SetFloat("StorageYPosition" + i, inventory[i].transform.localPosition.y);
			PreviewLabs.PlayerPrefs.SetFloat("StorageZPosition" + i, inventory[i].transform.localPosition.z);
			PreviewLabs.PlayerPrefs.SetFloat("StorageXRotation" + i, inventory[i].transform.localEulerAngles.x);
			PreviewLabs.PlayerPrefs.SetFloat("StorageYRotation" + i, inventory[i].transform.localEulerAngles.y);
			PreviewLabs.PlayerPrefs.SetFloat("StorageZRotation" + i, inventory[i].transform.localEulerAngles.z);
		}
		PreviewLabs.PlayerPrefs.SetInt("StorageCount", inventory.Count);
	}

	public void OnLoad()
	{
		bool flag = false;
		for (int i = 0; i < PreviewLabs.PlayerPrefs.GetInt("StorageCount"); i++)
		{
			GameObject gameObject = null;
			foreach (GameObject @object in LootSpawnManager.instance.objectList)
			{
				if (!(@object.name == PreviewLabs.PlayerPrefs.GetString("StorageObject" + i)))
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
					gameObject2.transform.SetParent(Bubble.transform);
					gameObject2.transform.localPosition = new Vector3(PreviewLabs.PlayerPrefs.GetFloat("StorageXPosition" + i), PreviewLabs.PlayerPrefs.GetFloat("StorageYPosition" + i), PreviewLabs.PlayerPrefs.GetFloat("StorageZPosition" + i));
					gameObject2.transform.localEulerAngles = new Vector3(PreviewLabs.PlayerPrefs.GetFloat("StorageXRotation" + i), PreviewLabs.PlayerPrefs.GetFloat("StorageYRotation" + i), PreviewLabs.PlayerPrefs.GetFloat("StorageZRotation" + i));
					AddToInventory(gameObject2);
				}
			}
		}
		TurnOff();
	}

	public void TurnOn()
	{
		Bubble.SetActive(true);
		opened = true;
	}

	public void TurnOff()
	{
		Bubble.SetActive(false);
	}
}
