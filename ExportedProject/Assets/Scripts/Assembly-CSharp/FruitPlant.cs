using System.Collections;
using PreviewLabs;
using UnityEngine;
using VRTK;

public class FruitPlant : MonoBehaviour
{
	public enum FruitType
	{
		Grape = 0,
		Apple = 1,
		Orange = 2,
		Pear = 3
	}

	internal GameObject fruit;

	public GameObject ripeIcon;

	public Transform fruitSpawnPos;

	internal GameObject currentFruit;

	public GameObject[] fruitObjects;

	public FruitType fruitType;

	internal SeedSlot slot;

	public float health = 100f;

	private float growRate = 0.01f;

	internal float growScale;

	private string ID;

	private bool growing;

	private void Start()
	{
		ID = GetComponentInParent<SeedSlot>().transform.position.ToString();
		ripeIcon.SetActive(false);
		Setup();
		if (!PreviewLabs.PlayerPrefs.GetBool(ID + "SeedDead"))
		{
		}
		base.transform.localScale = Vector3.zero;
		StartCoroutine("Grow");
		OnLoad();
	}

	private void OnEnable()
	{
		if (growing)
		{
			StartCoroutine("Grow");
		}
	}

	public void OnSave()
	{
		PreviewLabs.PlayerPrefs.SetFloat(ID + "growScale", growScale);
		PreviewLabs.PlayerPrefs.SetInt(ID + "PlantHealth", (int)health);
		if (currentFruit != null)
		{
			PreviewLabs.PlayerPrefs.SetFloat(ID + "fruitGrowth", currentFruit.GetComponent<Food>().growthScale);
		}
		else
		{
			PreviewLabs.PlayerPrefs.SetFloat(ID + "fruitGrowth", 0f);
		}
	}

	public void OnLoad()
	{
		growScale = PreviewLabs.PlayerPrefs.GetFloat(ID + "growScale");
		health = PreviewLabs.PlayerPrefs.GetInt(ID + "PlantHealth");
		if (currentFruit != null)
		{
			currentFruit.GetComponent<Food>().growthScale = PreviewLabs.PlayerPrefs.GetFloat(ID + "fruitGrowth");
			currentFruit.transform.SetParent(base.transform);
		}
	}

	private void Setup()
	{
		if (fruitType == FruitType.Grape)
		{
			fruit = fruitObjects[0];
		}
		else if (fruitType == FruitType.Apple)
		{
			fruit = fruitObjects[1];
		}
		else if (fruitType == FruitType.Orange)
		{
			fruit = fruitObjects[2];
		}
		else if (fruitType == FruitType.Pear)
		{
			fruit = fruitObjects[3];
		}
	}

	private IEnumerator Grow()
	{
		growScale = 0.1f;
		float refreshRate = 0.1f;
		if ((bool)GetComponent<CraftComponent>())
		{
			Object.Destroy(GetComponent<CraftComponent>());
		}
		growing = true;
		while (growScale < 1f)
		{
			growScale += refreshRate * 0.01f;
			health = growScale * 100f;
			base.transform.localScale = new Vector3(growScale, growScale, growScale);
			UpdateUI();
			yield return new WaitForSeconds(refreshRate);
		}
		growing = false;
		health = 100f;
		UpdateUI();
		base.transform.localScale = Vector3.one;
		AddCraftComponent();
		StartCoroutine("CheckSpawn");
	}

	private void AddCraftComponent()
	{
		CraftComponent craftComponent = base.gameObject.AddComponent<CraftComponent>();
		craftComponent.craftMaterials.Add(NanoInventory.instance.craftMaterials[2]);
		craftComponent.materialCounts[0] = 1;
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;
	}

	private IEnumerator CheckSpawn()
	{
		Setup();
		while (true)
		{
			if (currentFruit == null)
			{
				SpawnFruit();
			}
			yield return new WaitForSeconds(30f);
		}
	}

	private void SpawnFruit()
	{
		currentFruit = Object.Instantiate(fruit, fruitSpawnPos.position, fruitSpawnPos.rotation);
		currentFruit.GetComponent<Food>().spawner = this;
		currentFruit.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
		currentFruit.transform.localScale = Vector3.zero;
		currentFruit.GetComponent<Food>().StartCoroutine("Grow");
		currentFruit.name = fruit.name;
		currentFruit.transform.SetParent(base.transform);
		currentFruit.GetComponent<Food>().growthScale = PreviewLabs.PlayerPrefs.GetFloat(ID + "fruitGrowth");
		PreviewLabs.PlayerPrefs.SetFloat(ID + "fruitGrowth", 0f);
	}

	public void OnFruitGrown()
	{
		ripeIcon.SetActive(true);
	}

	public void OnLoot()
	{
		currentFruit = null;
		ripeIcon.SetActive(false);
		PreviewLabs.PlayerPrefs.SetFloat(ID + "fruitGrowth", 0f);
		UpdateFruitGrowth(0f);
	}

	public void UpdateUI()
	{
		slot.UI.UpdateGrowth(growScale);
		slot.UI.UpdateHealth(health);
	}

	public void UpdateFruitGrowth(float _growth)
	{
		slot.UI.UpdateGrowth(_growth);
	}

	public void OnTakeDamage(float damage)
	{
		health -= damage;
		if (health < 0f)
		{
			OnDeath();
		}
		UpdateUI();
	}

	private void OnDeath()
	{
		if (GetComponentInParent<Room>().fruitPlants.Contains(this))
		{
			OnLoot();
			GetComponentInParent<SeedSlot>().seedStored = null;
			GetComponent<MeshRenderer>().material = GameManager.instance.plantDeadMaterial;
			GetComponent<CraftComponent>().startMats = GetComponent<MeshRenderer>().materials;
			StopCoroutine("Grow");
			if (currentFruit != null)
			{
				currentFruit.SetActive(false);
			}
			PreviewLabs.PlayerPrefs.SetString(ID + "SeedType", string.Empty);
			slot.UI.Setup("Empty", string.Empty);
			PreviewLabs.PlayerPrefs.SetInt(ID + "PlantHealth", 100);
			PreviewLabs.PlayerPrefs.SetFloat(ID + "growScale", 0f);
			slot.UI.gameObject.SetActive(false);
			PlantUI.plantUIs.Remove(slot.UI);
			GetComponentInParent<Room>().fruitPlants.Remove(this);
			GameAudioManager.instance.AddToSuitQueue(SuitManager.instance.plantDeath);
		}
	}

	private void OnSalvage()
	{
		OnDeath();
		slot.UI.Reset();
	}

	private void OnDisable()
	{
		slot.full = false;
		if ((bool)GetComponentInParent<SeedSlot>())
		{
			PreviewLabs.PlayerPrefs.SetFloat(ID + "growScale", 0f);
			PreviewLabs.PlayerPrefs.SetInt(ID + "PlantHealth", 100);
		}
		slot.UI.gameObject.SetActive(false);
		if ((bool)GetComponentInParent<Room>() && GetComponentInParent<Room>().fruitPlants.Contains(this))
		{
			GetComponentInParent<Room>().fruitPlants.Remove(this);
		}
	}
}
