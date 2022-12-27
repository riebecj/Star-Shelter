using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CraftStation : MonoBehaviour
{
	[ListDrawerSettings(DraggableItems = true, Expanded = false, ShowIndexLabels = true, ShowPaging = false, ShowItemCount = true)]
	private AudioSource audioSource;

	public AudioClip beep;

	public AudioClip notEnoughMaterials;

	public AudioClip notEnoughPower;

	internal int craftIndex;

	public Sprite[] craftComponentIcons;

	public Image[] currentComponentIcons;

	public Text[] resourceCost;

	public Text[] resourceName;

	[ListDrawerSettings(DraggableItems = true, Expanded = false, ShowIndexLabels = true, ShowPaging = false, ShowItemCount = true)]
	public Image[] buttonLabels;

	public GameObject craftIndicator;

	public GameObject craftProxy;

	public Transform craftPosition;

	public Text typeCount;

	public Text description;

	public Text powerCost;

	public Text currentObjName;

	internal GameObject craftObject;

	internal GameObject newCraftObject;

	internal bool isCrafting;

	public Color trueColor;

	public Color falseColor;

	public static List<CraftStation> craftStations = new List<CraftStation>();

	public List<CraftedObject> craftTab1 = new List<CraftedObject>();

	public List<CraftedObject> craftTab2 = new List<CraftedObject>();

	public List<CraftedObject> craftTab3 = new List<CraftedObject>();

	public List<CraftMaterial> craftMaterials = new List<CraftMaterial>();

	internal CraftBound craftBound;

	public Transform[] tabLists;

	public GameObject craftButtonProxy;

	public MeshFilter visual;

	public List<ScriptableObject> createdButtons = new List<ScriptableObject>();

	public List<CraftProxy> craftQueue = new List<CraftProxy>();

	public List<GameObject> activeProxies = new List<GameObject>();

	public Animator anim;

	internal CraftProxy oldProxy;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		craftStations.Add(this);
	}

	private void OnEnable()
	{
		StartCoroutine("UpdateState");
		if ((bool)NanoInventory.instance)
		{
			UpdateUI();
		}
	}

	private void Start()
	{
		Invoke("Initialize", 0.5f);
	}

	private void Initialize()
	{
		OnToggleTab(0);
		OnSelectCraft(craftTab1[0], null);
	}

	public void AddToQueue(CraftProxy proxy)
	{
		craftQueue.Add(proxy);
		proxy.transform.SetParent(base.transform);
		if (craftQueue.Count < 2)
		{
			if (proxy.craftedObject != null)
			{
				craftObject = proxy.craftedObject.prefab;
			}
			else
			{
				craftObject = proxy.craftMaterial.prefab;
			}
			OnCraft();
		}
	}

	public void SpawnProxy(ScriptableObject proxy)
	{
		if (proxy is CraftedObject)
		{
			CraftedObject craftedObject = (CraftedObject)proxy;
			GameObject gameObject = Object.Instantiate(craftProxy, visual.transform);
			visual.transform.localPosition = visual.transform.parent.position + new Vector3(0f, craftedObject.iconOffsetY, 0f);
			gameObject.GetComponent<CraftProxy>().OnSetup(craftedObject, null, this);
			visual.transform.rotation = visual.transform.parent.rotation * Quaternion.Euler(craftedObject.iconRot);
			oldProxy = gameObject.GetComponent<CraftProxy>();
		}
		else if (proxy is CraftMaterial)
		{
			CraftMaterial craftMaterial = (CraftMaterial)proxy;
			GameObject gameObject2 = Object.Instantiate(craftProxy, visual.transform);
			gameObject2.GetComponent<CraftProxy>().OnSetup(null, craftMaterial, this);
			oldProxy = gameObject2.GetComponent<CraftProxy>();
		}
	}

	public void OnCraft()
	{
		if (IsInvoking("CancelCooldown") || isCrafting)
		{
			return;
		}
		newCraftObject = craftObject;
		if (!newCraftObject.GetComponent<Recipe>().CanCraft())
		{
			if (!IsInvoking("WarningCooldown"))
			{
				if ((float)newCraftObject.GetComponent<Recipe>().powerCost > BaseManager.instance.power)
				{
					audioSource.PlayOneShot(notEnoughPower);
				}
				else if (NanoInventory.instance.GetGlobalMass() >= NanoInventory.instance.GetGlobalCapacity() && newCraftObject.tag == "CraftMaterial")
				{
					audioSource.PlayOneShot(SuitManager.instance.nanoInventoryFull);
					ArmUIManager.instance.ShowInventory();
				}
				else if (!newCraftObject.GetComponent<Recipe>().CanCraft())
				{
					audioSource.PlayOneShot(notEnoughMaterials);
				}
				Invoke("WarningCooldown", 1.5f);
			}
			return;
		}
		powerCost.text = "<color=lime>" + BaseManager.instance.power.ToString("F0") + "</color>/<color=cyan>" + craftObject.GetComponent<Recipe>().powerCost + "</color>";
		PlayClickAudio();
		isCrafting = true;
		StartCoroutine("Crafting");
		for (int i = 0; i < NanoInventory.instance.craftMaterials.Count; i++)
		{
			for (int j = 0; j < newCraftObject.GetComponent<Recipe>().craftMaterials.Count; j++)
			{
				if (NanoInventory.instance.craftMaterials[i] == newCraftObject.GetComponent<Recipe>().craftMaterials[j])
				{
					NanoInventory.instance.materialCounts[i] -= newCraftObject.GetComponent<Recipe>().materialCosts[j];
				}
			}
		}
		for (int k = 0; k < NanoInventory.instance.craftMaterials.Count; k++)
		{
			for (int l = 0; l < NanoStorage.nanoStorages.Count; l++)
			{
				while (NanoStorage.nanoStorages[l].materialCounts[k] > 0 && NanoInventory.instance.materialCounts[k] < 0)
				{
					NanoStorage.nanoStorages[l].materialCounts[k]--;
					NanoInventory.instance.materialCounts[k]++;
				}
				NanoStorage.nanoStorages[l].UpdateUI();
			}
		}
		Invoke("CancelCooldown", 0.5f);
	}

	public void PlayClickAudio()
	{
		audioSource.PlayOneShot(beep);
	}

	public void OnSelectCraft(CraftedObject craftedObject, CraftMaterial craftMaterial)
	{
		if ((bool)BaseCraftManager.instance)
		{
			ResetTargets();
		}
		resourceCost[0].transform.parent.parent.gameObject.SetActive(false);
		resourceCost[0].transform.parent.parent.gameObject.SetActive(true);
		if ((bool)oldProxy)
		{
			Object.Destroy(oldProxy.gameObject);
			oldProxy = null;
		}
		if (!craftMaterial)
		{
			craftObject = craftedObject.prefab;
			for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
			{
				if (NanoInventory.instance.craftedObjects[i] == craftedObject)
				{
					craftIndex = i;
					if (BaseManager.instance.power > (float)NanoInventory.instance.craftedObjects[i].prefab.GetComponent<Recipe>().powerCost)
					{
						powerCost.text = "<color=lime>" + BaseManager.instance.power.ToString("F0") + "</color>/<color=cyan>" + NanoInventory.instance.craftedObjects[i].prefab.GetComponent<Recipe>().powerCost + "</color>";
					}
					else
					{
						powerCost.text = "<color=red>" + BaseManager.instance.power.ToString("F0") + "</color>/<color=cyan>" + NanoInventory.instance.craftedObjects[i].prefab.GetComponent<Recipe>().powerCost + "</color>";
					}
				}
			}
			visual.sharedMesh = craftedObject.icon;
			visual.transform.localScale = Vector3.one * NanoInventory.instance.craftedObjects[craftIndex].iconSize;
			description.text = craftedObject.description;
			currentObjName.text = craftedObject.name;
			SpawnProxy(craftedObject);
		}
		else
		{
			craftObject = craftMaterial.prefab;
			for (int j = 0; j < NanoInventory.instance.craftMaterials.Count; j++)
			{
				if (NanoInventory.instance.craftMaterials[j] == craftMaterial)
				{
					craftIndex = j;
					if (BaseManager.instance.power > (float)NanoInventory.instance.craftMaterials[j].prefab.GetComponent<Recipe>().powerCost)
					{
						powerCost.text = "<color=lime>" + BaseManager.instance.power.ToString("F0") + "</color>/<color=cyan>" + NanoInventory.instance.craftMaterials[j].prefab.GetComponent<Recipe>().powerCost + "</color>";
					}
					else
					{
						powerCost.text = "<color=red>" + BaseManager.instance.power.ToString("F0") + "</color>/<color=cyan>" + NanoInventory.instance.craftMaterials[j].prefab.GetComponent<Recipe>().powerCost + "</color>";
					}
				}
			}
			visual.sharedMesh = craftObject.GetComponent<MeshFilter>().sharedMesh;
			visual.transform.localScale = Vector3.one * NanoInventory.instance.craftMaterials[craftIndex].iconSize;
			description.text = craftMaterial.description;
			currentObjName.text = craftMaterial.name;
			SpawnProxy(craftMaterial);
		}
		UpdateTypeCount();
		PlayClickAudio();
		UpdateUI();
	}

	public void UpdateUI()
	{
		UpdateIcons();
		UpdateTypeCount();
		for (int i = 0; i < NanoStorage.nanoStorages.Count; i++)
		{
			NanoStorage.nanoStorages[i].UpdateUI();
		}
		if (BaseManager.instance.power > (float)craftObject.GetComponent<Recipe>().powerCost)
		{
			powerCost.text = "<color=lime>" + BaseManager.instance.power.ToString("F0") + "</color>/<color=cyan>" + craftObject.GetComponent<Recipe>().powerCost + "</color>";
		}
		else
		{
			powerCost.text = "<color=red>" + BaseManager.instance.power.ToString("F0") + "</color>/<color=cyan>" + craftObject.GetComponent<Recipe>().powerCost + "</color>";
		}
	}

	private void UpdateIcons()
	{
		for (int i = 0; i < 6; i++)
		{
			if ((bool)craftObject && craftObject.GetComponent<Recipe>().craftMaterials.Count > i)
			{
				resourceCost[i].transform.parent.gameObject.SetActive(true);
				for (int j = 0; j < NanoInventory.instance.craftMaterials.Count; j++)
				{
					if (NanoInventory.instance.craftMaterials[j] == craftObject.GetComponent<Recipe>().craftMaterials[i])
					{
						string text = NanoInventory.instance.craftMaterials[j].name;
						if (NanoInventory.instance.GetGlobalMaterialCount(j) >= craftObject.GetComponent<Recipe>().materialCosts[i])
						{
							resourceCost[i].text = "<color=lime>" + NanoInventory.instance.GetGlobalMaterialCount(j) + "</color>/<color=cyan>" + craftObject.GetComponent<Recipe>().materialCosts[i] + "</color>";
						}
						else
						{
							resourceCost[i].text = "<color=red>" + NanoInventory.instance.GetGlobalMaterialCount(j) + "</color>/<color=cyan>" + craftObject.GetComponent<Recipe>().materialCosts[i] + "</color>";
						}
						resourceName[i].text = NanoInventory.instance.craftMaterials[j].name;
						currentComponentIcons[i].sprite = NanoInventory.instance.craftMaterials[j].icon;
					}
				}
			}
			else
			{
				resourceCost[i].transform.parent.gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator Crafting()
	{
		float time = 0f;
		craftQueue[0].gameObject.GetComponent<Collider>().enabled = false;
		anim.SetBool("Crafting", true);
		craftQueue[0].GetComponent<MeshRenderer>().material = GameManager.instance.craftMaterial;
		craftQueue[0].GetComponent<MeshRenderer>().material.EnableKeyword("_Cutoff");
		while (time < newCraftObject.GetComponent<Recipe>().craftTime)
		{
			time += 0.1f;
			craftQueue[0].transform.position = craftPosition.position;
			craftQueue[0].transform.rotation = craftPosition.rotation;
			craftQueue[0].GetComponent<MeshRenderer>().material.SetFloat("_Cutoff", 0.75f - time / newCraftObject.GetComponent<Recipe>().craftTime);
			yield return new WaitForSeconds(0.1f);
		}
		anim.SetBool("Crafting", false);
		craftQueue[0].gameObject.SetActive(false);
		craftQueue.RemoveAt(0);
		OnCraftComplete();
	}

	public void CancelCrafting()
	{
		if (!IsInvoking("CancelCooldown") && isCrafting)
		{
			anim.SetBool("Crafting", false);
			craftQueue[0].gameObject.SetActive(false);
			craftQueue.RemoveAt(0);
			ReturnMaterials();
			StopCoroutine("Crafting");
			isCrafting = false;
			Invoke("CancelCooldown", 0.5f);
			UpdateUI();
			ArmUIManager.instance.UpdateUI();
		}
	}

	private void OnCraftComplete()
	{
		if ((bool)newCraftObject.GetComponent<Recipe>())
		{
			if (!newCraftObject.GetComponent<Recipe>().addToNanoInventory)
			{
				GameObject gameObject = Object.Instantiate(newCraftObject, craftPosition.position, craftPosition.rotation);
				gameObject.name = newCraftObject.name;
				if ((bool)gameObject.GetComponent<CraftComponent>() && !gameObject.GetComponent<Food>())
				{
					gameObject.GetComponent<CraftComponent>().OnAdd(false);
					gameObject.SetActive(false);
					ArmUIManager.instance.UpdateUI();
				}
			}
			else
			{
				AddShipPart();
			}
			BaseManager.instance.power -= newCraftObject.GetComponent<Recipe>().powerCost;
			UpdateUI();
			ArmUIManager.instance.UpdateUI();
		}
		isCrafting = false;
		if (craftQueue.Count > 0)
		{
			if ((bool)craftQueue[0].craftedObject)
			{
				craftObject = craftQueue[0].craftedObject.prefab;
			}
			else if ((bool)craftQueue[0].craftMaterial)
			{
				craftObject = craftQueue[0].craftMaterial.prefab;
			}
			OnCraft();
		}
	}

	private void ReturnMaterials()
	{
		for (int i = 0; i < NanoInventory.instance.craftMaterials.Count; i++)
		{
			for (int j = 0; j < newCraftObject.GetComponent<Recipe>().craftMaterials.Count; j++)
			{
				if (NanoInventory.instance.craftMaterials[i] == newCraftObject.GetComponent<Recipe>().craftMaterials[j])
				{
					NanoInventory.instance.materialCounts[i] += newCraftObject.GetComponent<Recipe>().materialCosts[j];
				}
			}
		}
	}

	private void UpdateTypeCount()
	{
		typeCount.gameObject.SetActive(true);
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if (NanoInventory.instance.craftedObjects[i].prefab == craftObject)
			{
				typeCount.text = NanoInventory.instance.craftedObjectCounts[i].ToString();
			}
		}
		if ((bool)craftObject.GetComponent<Upgrade>())
		{
			typeCount.gameObject.SetActive(false);
		}
		else if ((bool)craftObject.GetComponent<CraftObject>())
		{
			typeCount.gameObject.SetActive(false);
		}
		else
		{
			if (!craftObject.GetComponent<CraftComponent>())
			{
				return;
			}
			for (int j = 0; j < NanoInventory.instance.craftMaterials.Count; j++)
			{
				if (NanoInventory.instance.craftMaterials[j].prefab != null && NanoInventory.instance.craftMaterials[j].prefab == craftObject)
				{
					typeCount.text = NanoInventory.instance.materialCounts[j].ToString();
				}
			}
		}
	}

	private void AddShipPart()
	{
		for (int i = 0; i < NanoInventory.instance.craftedObjects.Count; i++)
		{
			if (NanoInventory.instance.craftedObjects[i].prefab == newCraftObject)
			{
				if (NanoInventory.instance.craftedObjectCounts[i] == 0)
				{
					NanoInventory.instance.craftedObjects[i].newCraft = true;
				}
				NanoInventory.instance.craftedObjectCounts[i]++;
			}
		}
		EnableIndicator();
	}

	private void EnableIndicator()
	{
		CancelInvoke("DisableIndicator");
		craftIndicator.SetActive(true);
		craftIndicator.transform.position = craftPosition.position;
		craftIndicator.transform.SetParent(ArmUIManager.instance.craftIndicatorPos);
		StartCoroutine("LerpIndicator");
	}

	private IEnumerator LerpIndicator()
	{
		Transform indicator = craftIndicator.transform;
		float refreshRate = 0.01f;
		indicator.transform.localEulerAngles = Vector3.zero;
		while (indicator.transform.localPosition.magnitude > 0.05f)
		{
			indicator.transform.localPosition = Vector3.Lerp(indicator.transform.localPosition, Vector3.zero, refreshRate * 8f);
			yield return new WaitForSeconds(refreshRate);
		}
		Invoke("DisableIndicator", 5f);
	}

	private void DisableIndicator()
	{
		craftIndicator.SetActive(false);
	}

	private void ResetTargets()
	{
		BaseCraftManager.instance.currentRoom = null;
		BaseCraftManager.instance.targetGate = null;
		BaseCraftManager.instance.targetWall = null;
	}

	private IEnumerator UpdateState()
	{
		while (true)
		{
			UpdateQue();
			yield return new WaitForSeconds(5f);
		}
	}

	private void UpdateQue()
	{
		if (craftQueue.Count <= 0)
		{
			return;
		}
		if ((bool)craftQueue[0].craftedObject)
		{
			if (craftQueue[0].craftedObject.prefab.GetComponent<Recipe>().CanCraft() && (float)craftQueue[0].craftedObject.prefab.GetComponent<Recipe>().powerCost < BaseManager.instance.power)
			{
				craftObject = craftQueue[0].craftedObject.prefab;
				OnCraft();
			}
		}
		else if ((bool)craftQueue[0].craftMaterial && craftQueue[0].craftMaterial.prefab.GetComponent<Recipe>().CanCraft() && (float)craftQueue[0].craftMaterial.prefab.GetComponent<Recipe>().powerCost < BaseManager.instance.power)
		{
			craftObject = craftQueue[0].craftMaterial.prefab;
			OnCraft();
		}
	}

	public void OnToggleTab(int value)
	{
		for (int i = 0; i < tabLists.Length; i++)
		{
			tabLists[i].gameObject.SetActive(false);
		}
		switch (value)
		{
		case 0:
		{
			tabLists[0].gameObject.SetActive(true);
			for (int m = 0; m < craftTab1.Count; m++)
			{
				if (!createdButtons.Contains(craftTab1[m]))
				{
					GameObject gameObject4 = Object.Instantiate(craftButtonProxy, tabLists[0]);
					gameObject4.GetComponent<CraftStationButton>().OnSetup(craftTab1[m], this);
					createdButtons.Add(craftTab1[m]);
				}
			}
			break;
		}
		case 1:
		{
			tabLists[1].gameObject.SetActive(true);
			for (int k = 0; k < craftTab2.Count; k++)
			{
				if (!createdButtons.Contains(craftTab2[k]))
				{
					GameObject gameObject2 = Object.Instantiate(craftButtonProxy, tabLists[1]);
					gameObject2.GetComponent<CraftStationButton>().OnSetup(craftTab2[k], this);
					createdButtons.Add(craftTab2[k]);
				}
			}
			break;
		}
		case 2:
		{
			tabLists[2].gameObject.SetActive(true);
			for (int l = 0; l < craftTab3.Count; l++)
			{
				if (!createdButtons.Contains(craftTab3[l]))
				{
					GameObject gameObject3 = Object.Instantiate(craftButtonProxy, tabLists[2]);
					gameObject3.GetComponent<CraftStationButton>().OnSetup(craftTab3[l], this);
					createdButtons.Add(craftTab3[l]);
				}
			}
			break;
		}
		case 3:
		{
			tabLists[3].gameObject.SetActive(true);
			for (int j = 0; j < craftMaterials.Count; j++)
			{
				if (!createdButtons.Contains(craftMaterials[j]))
				{
					GameObject gameObject = Object.Instantiate(craftButtonProxy, tabLists[3]);
					gameObject.GetComponent<CraftStationButton>().OnSetup(craftMaterials[j], this);
					createdButtons.Add(craftMaterials[j]);
				}
			}
			break;
		}
		}
	}

	private void CancelCooldown()
	{
	}

	private void WarningCooldown()
	{
	}

	private void OnDisable()
	{
		craftStations.Remove(this);
		if ((bool)craftBound)
		{
			craftBound.gameObject.SetActive(false);
		}
	}
}
