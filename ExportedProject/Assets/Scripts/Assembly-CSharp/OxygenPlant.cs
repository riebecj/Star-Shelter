using System.Collections;
using PreviewLabs;
using UnityEngine;

public class OxygenPlant : MonoBehaviour
{
	public float oxygenPerSecond = 0.25f;

	public float health = 100f;

	internal SeedSlot slot;

	private float growRate = 0.01f;

	internal float growScale;

	private string ID;

	private void Start()
	{
		ID = GetComponentInParent<SeedSlot>().transform.position.ToString();
		base.transform.localScale = Vector3.zero;
		StartCoroutine("Grow");
		OnLoad();
	}

	public void OnSave()
	{
		PreviewLabs.PlayerPrefs.SetFloat(ID + "growScale", growScale);
		PreviewLabs.PlayerPrefs.SetInt(ID + "PlantHealth", (int)health);
	}

	public void OnLoad()
	{
		growScale = PreviewLabs.PlayerPrefs.GetFloat(ID + "growScale");
		health = PreviewLabs.PlayerPrefs.GetInt(ID + "PlantHealth");
	}

	public void OnTakeDamage(float damage)
	{
		health -= damage;
		if (health < 0f)
		{
			OnDeath();
		}
		if ((bool)base.transform.parent.GetComponentInChildren<PlantUI>())
		{
			base.transform.parent.GetComponentInChildren<PlantUI>().UpdateHealth(health);
		}
	}

	private void OnDeath()
	{
		if (GetComponentInParent<Room>().oxygenPlants.Contains(this))
		{
			GetComponentInParent<SeedSlot>().seedStored = null;
			GetComponent<MeshRenderer>().material = GameManager.instance.plantDeadMaterial;
			GetComponent<CraftComponent>().startMats = GetComponent<MeshRenderer>().materials;
			if ((bool)GetComponentInParent<Room>())
			{
				GetComponentInParent<Room>().oxygenGenerationRate -= oxygenPerSecond;
			}
			PreviewLabs.PlayerPrefs.SetString(ID + "SeedType", string.Empty);
			if ((bool)base.transform.parent.GetComponentInChildren<PlantUI>())
			{
				base.transform.parent.GetComponentInChildren<PlantUI>().Setup("Empty", string.Empty);
			}
			PreviewLabs.PlayerPrefs.SetFloat(ID + "growScale", 0.1f);
			PreviewLabs.PlayerPrefs.SetInt(ID + "PlantHealth", 100);
			base.transform.GetComponentInParent<SeedSlot>().UI.gameObject.SetActive(false);
			GetComponentInParent<Room>().oxygenPlants.Remove(this);
			StopCoroutine("Grow");
			GameAudioManager.instance.AddToSuitQueue(SuitManager.instance.plantDeath);
		}
	}

	private void OnSalvage()
	{
		OnDeath();
		if ((bool)base.transform.parent.GetComponentInChildren<PlantUI>())
		{
			base.transform.parent.GetComponentInChildren<PlantUI>().Reset();
		}
	}

	private IEnumerator Grow()
	{
		growScale = 0.1f;
		float refreshRate = 0.1f;
		while (growScale < 1f)
		{
			growScale += refreshRate * 0.01f;
			health = growScale * 100f;
			base.transform.localScale = new Vector3(growScale, growScale, growScale);
			UpdateUI();
			yield return new WaitForSeconds(refreshRate);
		}
		health = 100f;
		UpdateUI();
		base.transform.localScale = Vector3.one;
		AddCraftComponent();
	}

	private void AddCraftComponent()
	{
		CraftComponent craftComponent = base.gameObject.AddComponent<CraftComponent>();
		craftComponent.craftMaterials.Add(NanoInventory.instance.craftMaterials[2]);
		craftComponent.materialCounts[0] = 1;
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;
	}

	public void UpdateUI()
	{
		if ((bool)base.transform.parent.GetComponentInChildren<PlantUI>())
		{
			base.transform.parent.GetComponentInChildren<PlantUI>().UpdateGrowth(growScale);
			base.transform.parent.GetComponentInChildren<PlantUI>().UpdateHealth(health);
		}
	}

	private void OnDisable()
	{
		if ((bool)GetComponentInParent<Room>())
		{
			GetComponentInParent<Room>().oxygenGenerationRate -= oxygenPerSecond;
			if (GetComponentInParent<Room>().oxygenPlants.Contains(this))
			{
				GetComponentInParent<Room>().oxygenPlants.Remove(this);
			}
		}
		slot.full = false;
		if ((bool)base.transform.parent.GetComponentInChildren<PlantUI>())
		{
			base.transform.parent.GetComponentInChildren<PlantUI>().gameObject.SetActive(false);
		}
	}
}
