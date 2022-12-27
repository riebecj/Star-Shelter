using System.Collections;
using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using VRTK;

public class SeedSlot : MonoBehaviour
{
	public bool full;

	public Transform plantPos;

	internal GameObject plant;

	public PlantUI UI;

	internal string seedStored = string.Empty;

	public static List<SeedSlot> seedSlots = new List<SeedSlot>();

	private Transform capsule;

	private void OnEnable()
	{
		seedSlots.Add(this);
	}

	private void OnDisable()
	{
		seedSlots.Remove(this);
	}

	private void OnBreak()
	{
		base.gameObject.SetActive(false);
	}

	private void Start()
	{
		OnLoad();
	}

	public void OnSave()
	{
		if (seedStored != null)
		{
			PreviewLabs.PlayerPrefs.SetString(string.Concat(base.transform.position, "SeedType"), seedStored);
			if ((bool)GetComponentInChildren<FruitPlant>())
			{
				GetComponentInChildren<FruitPlant>().OnSave();
			}
			else if ((bool)GetComponentInChildren<OxygenPlant>())
			{
				GetComponentInChildren<OxygenPlant>().OnSave();
			}
		}
	}

	public void OnLoad()
	{
		if (PreviewLabs.PlayerPrefs.GetString(string.Concat(base.transform.position, "SeedType")) != null)
		{
			OnSpawnPlant(PreviewLabs.PlayerPrefs.GetString(string.Concat(base.transform.position, "SeedType")));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!full && (bool)other.GetComponent<Seed>() && other.GetComponent<VRTK_InteractableObject>().IsGrabbed())
		{
			capsule = other.transform;
			capsule.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
			capsule.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
			capsule.GetComponent<Collider>().isTrigger = true;
			OnSpawnPlant(other.GetComponent<Seed>().seedType.ToString());
			StartCoroutine("LerpCapsule");
		}
	}

	private IEnumerator LerpCapsule()
	{
		while (Vector3.Distance(capsule.position, plantPos.position + Vector3.down * 0.05f) > 0.05f)
		{
			capsule.position = Vector3.Lerp(capsule.position, plantPos.position + Vector3.down * 0.05f, 8f * Time.deltaTime);
			yield return new WaitForSeconds(0.02f);
		}
		capsule.gameObject.SetActive(false);
	}

	public void OnSpawnPlant(string seedType)
	{
		if (seedType != string.Empty)
		{
			full = true;
		}
		if ((bool)UI && seedType != string.Empty)
		{
			UI.gameObject.SetActive(true);
			PlantUI.plantUIs.Add(UI);
		}
		if (seedType == Seed.SeedType.Oxygen.ToString())
		{
			plant = Object.Instantiate(Inventory.instance.oxygenPlants[Random.Range(0, Inventory.instance.oxygenPlants.Length)], plantPos.position, plantPos.rotation);
			GetComponentInParent<Room>().oxygenPlants.Add(plant.GetComponent<OxygenPlant>());
			GetComponentInParent<Room>().oxygenGenerationRate += plant.GetComponent<OxygenPlant>().oxygenPerSecond;
			plant.GetComponent<OxygenPlant>().slot = this;
			if ((bool)UI)
			{
				UI.Setup("Oxygen creation", Seed.seedInfo[4]);
			}
			PreviewLabs.PlayerPrefs.SetBool("lowOxygen_GrowPlant", true);
			HintManager.instance.lowOxygen_GrowPlant = true;
		}
		else if (seedType == Seed.SeedType.Grape.ToString())
		{
			plant = Object.Instantiate(Inventory.instance.fruitplants[Random.Range(0, Inventory.instance.fruitplants.Length)], plantPos.position, plantPos.rotation);
			GetComponentInParent<Room>().fruitPlants.Add(plant.GetComponent<FruitPlant>());
			plant.GetComponent<FruitPlant>().fruitType = FruitPlant.FruitType.Grape;
			plant.GetComponent<FruitPlant>().slot = this;
			if ((bool)UI)
			{
				UI.Setup("Fruitplant", Seed.seedInfo[0]);
			}
		}
		else if (seedType == Seed.SeedType.Apple.ToString())
		{
			plant = Object.Instantiate(Inventory.instance.fruitplants[Random.Range(0, Inventory.instance.fruitplants.Length)], plantPos.position, plantPos.rotation);
			GetComponentInParent<Room>().fruitPlants.Add(plant.GetComponent<FruitPlant>());
			plant.GetComponent<FruitPlant>().fruitType = FruitPlant.FruitType.Apple;
			plant.GetComponent<FruitPlant>().slot = this;
			if ((bool)UI)
			{
				UI.Setup("Fruitplant", Seed.seedInfo[1]);
			}
		}
		else if (seedType == Seed.SeedType.Orange.ToString())
		{
			plant = Object.Instantiate(Inventory.instance.fruitplants[Random.Range(0, Inventory.instance.fruitplants.Length)], plantPos.position, plantPos.rotation);
			GetComponentInParent<Room>().fruitPlants.Add(plant.GetComponent<FruitPlant>());
			plant.GetComponent<FruitPlant>().fruitType = FruitPlant.FruitType.Orange;
			plant.GetComponent<FruitPlant>().slot = this;
			if ((bool)UI)
			{
				UI.Setup("Fruitplant", Seed.seedInfo[2]);
			}
		}
		else if (seedType == Seed.SeedType.Pear.ToString())
		{
			plant = Object.Instantiate(Inventory.instance.fruitplants[Random.Range(0, Inventory.instance.fruitplants.Length)], plantPos.position, plantPos.rotation);
			GetComponentInParent<Room>().fruitPlants.Add(plant.GetComponent<FruitPlant>());
			plant.GetComponent<FruitPlant>().fruitType = FruitPlant.FruitType.Pear;
			plant.GetComponent<FruitPlant>().slot = this;
			if ((bool)UI)
			{
				UI.Setup("Fruitplant", Seed.seedInfo[3]);
			}
		}
		if ((bool)ObjectivePlantPot.instance)
		{
			ObjectivePlantPot.instance.OnObjectiveComplete();
		}
		seedStored = seedType;
		if (plant != null)
		{
			plant.transform.SetParent(base.transform);
		}
	}

	public void CraftingComplete()
	{
	}
}
