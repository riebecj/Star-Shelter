using System.Collections;
using UnityEngine;
using VRTK;

public class Food : MonoBehaviour
{
	public float nutritionValue = 8f;

	public float healingValue = 3f;

	public Mesh half;

	public Mesh eaten;

	public int bioMass = 1;

	public bool gradual;

	internal int value = 2;

	private MeshFilter meshFilter;

	internal VRTK_InteractableObject interact;

	internal FruitPlant spawner;

	internal float growthScale;

	internal bool growing;

	private void Start()
	{
		meshFilter = GetComponent<MeshFilter>();
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		if ((bool)spawner)
		{
			spawner.OnLoot();
		}
		base.transform.SetParent(null);
		TutorialManager.instance.ToggleEat();
		SpaceMask.instance.ToggleFoodInfo(true);
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		SpaceMask.instance.ToggleFoodInfo(false);
	}

	public IEnumerator Grow()
	{
		growing = true;
		growthScale = 0f;
		float refreshRate = 0.1f;
		GetComponent<Rigidbody>().isKinematic = true;
		if ((bool)GetComponent<CraftComponent>())
		{
			Object.Destroy(GetComponent<CraftComponent>());
		}
		while (growthScale < 1f)
		{
			growthScale += refreshRate / 240f;
			base.transform.localScale = new Vector3(growthScale, growthScale, growthScale);
			spawner.UpdateFruitGrowth(growthScale);
			yield return new WaitForSeconds(refreshRate);
		}
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		if ((bool)spawner)
		{
			spawner.OnFruitGrown();
		}
		GetComponent<Rigidbody>().isKinematic = false;
		interact.isGrabbable = true;
		AddCraftComponent();
	}

	private void AddCraftComponent()
	{
		CraftComponent craftComponent = base.gameObject.AddComponent<CraftComponent>();
		craftComponent.craftMaterials.Add(NanoInventory.instance.craftMaterials[2]);
		craftComponent.materialCounts[0] = 1;
	}

	public void OnEat()
	{
		value--;
		if (gradual)
		{
			if (value == 1)
			{
				float x = base.transform.localScale.x;
				base.transform.localScale = new Vector3(x, x, x);
				base.transform.LookAt(GameManager.instance.Head.position);
				if (half != null)
				{
					meshFilter.mesh = half;
				}
			}
			else if (value == 0)
			{
				meshFilter.mesh = eaten;
			}
		}
		else
		{
			value = 0;
			meshFilter.mesh = eaten;
			GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
			base.gameObject.SetActive(false);
		}
	}

	private void OnDisable()
	{
		if ((bool)spawner)
		{
			spawner.OnLoot();
		}
	}
}
