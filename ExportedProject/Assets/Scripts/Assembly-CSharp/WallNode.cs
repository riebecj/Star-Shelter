using System.Collections.Generic;
using UnityEngine;

public class WallNode : MonoBehaviour
{
	public bool isCorner;

	internal Room room;

	internal Mesh storedMesh;

	internal bool crafted;

	internal WallStructure wallStructure;

	public static List<WallNode> wallNodes = new List<WallNode>();

	public GameObject leakParticle;

	internal bool wasLeaking;

	private void Awake()
	{
		wallNodes.Add(this);
	}

	private void Start()
	{
		room = GetComponentInParent<Room>();
		storedMesh = GetComponent<MeshFilter>().sharedMesh;
		SetupLeakParticle();
		Invoke("CheckForLeak", 1f);
	}

	private void SetupLeakParticle()
	{
		leakParticle = Object.Instantiate(BaseLoader.instance.leakParticle, base.transform);
		leakParticle.transform.localPosition = Vector3.zero;
		leakParticle.transform.rotation = Quaternion.Inverse(base.transform.rotation);
		leakParticle.GetComponent<LeakParticle>().room = room;
		leakParticle.transform.SetParent(base.transform.parent);
		leakParticle.SetActive(false);
	}

	public void OnEnter()
	{
		if (!crafted && !(BaseCraftManager.instance.currentWall == null))
		{
			if (isCorner)
			{
				GetComponent<MeshFilter>().sharedMesh = BaseCraftManager.instance.currentWall.corner.GetComponent<MeshFilter>().sharedMesh;
			}
			else
			{
				GetComponent<MeshFilter>().sharedMesh = BaseCraftManager.instance.currentWall.prefab.GetComponent<MeshFilter>().sharedMesh;
			}
			GetComponent<MeshRenderer>().material = BaseCraftManager.instance.canCraft;
			BaseCraftManager.instance.targetWall = this;
		}
	}

	public void OnExit()
	{
		if (!crafted)
		{
			GetComponent<MeshFilter>().sharedMesh = storedMesh;
			GetComponent<MeshRenderer>().material = BaseCraftManager.instance.holo;
			BaseCraftManager.instance.targetWall = null;
		}
	}

	public void OnCraft(WallStructure wall)
	{
		CraftComponent craftcomponent = null;
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if (wall == NanoInventory.instance.craftedObjects[i])
			{
				craftcomponent = NanoInventory.instance.craftedObjects[i].prefab.GetComponent<CraftComponent>();
				if (!GameManager.instance.loading && !GameManager.instance.creativeMode)
				{
					NanoInventory.instance.craftedObjectCounts[i]--;
				}
				break;
			}
		}
		wallStructure = wall;
		if (isCorner)
		{
			GetComponent<MeshFilter>().sharedMesh = wall.corner.GetComponent<MeshFilter>().sharedMesh;
		}
		else
		{
			GetComponent<MeshFilter>().sharedMesh = wall.prefab.GetComponent<MeshFilter>().sharedMesh;
		}
		GetComponent<MeshCollider>().isTrigger = false;
		GetComponent<MeshCollider>().convex = false;
		GetComponent<MeshRenderer>().material = wall.prefab.GetComponent<MeshRenderer>().sharedMaterial;
		AddCraftComponent(craftcomponent);
		crafted = true;
		base.gameObject.layer = 0;
		base.tag = "Wall";
		wallNodes.Remove(this);
		CheckForLeak();
	}

	public void OnSalvage()
	{
		if ((bool)GetComponent<CraftComponent>())
		{
			GetComponent<CraftComponent>().StopAllCoroutines();
			Object.Destroy(GetComponent<CraftComponent>());
		}
		GetComponent<MeshRenderer>().material = BaseCraftManager.instance.holo;
		GetComponent<MeshFilter>().sharedMesh = storedMesh;
		GetComponent<MeshCollider>().convex = true;
		GetComponent<MeshCollider>().isTrigger = true;
		crafted = false;
		base.gameObject.layer = 0;
		wallNodes.Add(this);
		base.tag = "Untagged";
		base.gameObject.SetActive(false);
		CheckForLeak();
	}

	public static void Toggle(bool value)
	{
		if (value)
		{
			for (int i = 0; i < wallNodes.Count; i++)
			{
				wallNodes[i].gameObject.SetActive(true);
			}
		}
		else
		{
			for (int j = 0; j < wallNodes.Count; j++)
			{
				wallNodes[j].gameObject.SetActive(false);
			}
		}
	}

	private void AddCraftComponent(CraftComponent _craftcomponent)
	{
		CraftComponent craftComponent = base.gameObject.AddComponent<CraftComponent>();
		craftComponent.materialCounts = new int[_craftcomponent.materialCounts.Length];
		for (int i = 0; i < _craftcomponent.materialCounts.Length; i++)
		{
			craftComponent.materialCounts[i] = _craftcomponent.materialCounts[i];
			craftComponent.craftMaterials.Add(_craftcomponent.craftMaterials[i]);
		}
	}

	private void OnDestroy()
	{
		if (wallNodes.Contains(this))
		{
			wallNodes.Remove(this);
		}
	}

	public void CheckForLeak()
	{
		CancelInvoke();
		if (!crafted)
		{
			Invoke("StartLeak", 1f);
		}
		else
		{
			Invoke("StopLeak", 1f);
		}
	}

	public void StartLeak()
	{
		CancelInvoke("StopLeak");
		if ((bool)room && room.group.TotalOxygen > 5f)
		{
			leakParticle.SetActive(true);
		}
		room.leakRate += 5f;
		wasLeaking = true;
	}

	public void StopLeak()
	{
		CancelInvoke("StartLeak");
		leakParticle.SetActive(false);
		if (wasLeaking)
		{
			room.leakRate -= 5f;
		}
		wasLeaking = false;
	}
}
