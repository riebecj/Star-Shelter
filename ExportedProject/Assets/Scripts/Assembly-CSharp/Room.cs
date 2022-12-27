using System;
using System.Collections;
using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
	public float Oxygen;

	public float OxgenCapacity;

	public float oxygenGenerationRate;

	public float leakRate;

	public List<RoomNode> nodes = new List<RoomNode>();

	public WallNode[] walls;

	public RoomNode lastConnectedNode;

	private float distance = 25f;

	private float gridSize = 1f;

	public int health = 10;

	private int maxHealth;

	internal bool blocked;

	public Text info;

	public OxygenGroup group;

	internal GameObject doorButton;

	internal int roomIndex;

	public List<Transform> leakSource = new List<Transform>();

	internal List<MeshRenderer> visuals = new List<MeshRenderer>();

	public List<GameObject> props = new List<GameObject>();

	public List<RoomLeak> holes = new List<RoomLeak>();

	public List<OxygenPlant> oxygenPlants = new List<OxygenPlant>();

	public List<FruitPlant> fruitPlants = new List<FruitPlant>();

	public float craftTime = 15f;

	internal float startTime;

	public Image oxygenBar;

	public Text integrity;

	public Text oxygenText;

	public Text leakText;

	internal Transform leakUIPoint;

	private GameObject leakUI;

	private void Awake()
	{
		foreach (RoomNode node in nodes)
		{
			node.room = this;
		}
	}

	private void Start()
	{
		maxHealth = health;
		startTime = craftTime;
		if ((bool)GetComponent<RoomSave>() || (bool)GetComponent<StartCaps>())
		{
			craftTime = 0f;
		}
		StartCoroutine("Constructing");
		if (!BaseLoader.instance.AllRooms.Contains(this))
		{
			BaseLoader.instance.AllRooms.Add(this);
		}
		AddToHoloMap();
		AddLeakUI();
		Transform transform = base.transform.Find("LeakPoints");
		if (!transform)
		{
			return;
		}
		IEnumerator enumerator = transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform2 = (Transform)enumerator.Current;
				if ((transform2.tag == "LeakPoint" || transform2.tag == "LeakPointGlass") && !BaseLoader.instance.leakHoles.Contains(transform2.gameObject))
				{
					BaseLoader.instance.leakHoles.Add(transform2.gameObject);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private void AddToHoloMap()
	{
		for (int i = 0; i < HoloMap.holoMaps.Count; i++)
		{
			HoloMap.holoMaps[i].OnAddRoom(base.gameObject);
		}
	}

	private void AddLeakUI()
	{
		IEnumerator enumerator = base.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				if (transform.tag == "LeakUI")
				{
					leakUIPoint = transform;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		if ((bool)leakUIPoint)
		{
			leakUI = UnityEngine.Object.Instantiate(BaseLoader.instance.leakUI, leakUIPoint);
			leakUI.transform.localPosition = Vector3.zero;
			leakText = leakUI.transform.Find("RoomLeak Ammount").GetComponent<Text>();
		}
	}

	public void OnAddProp(GameObject prop)
	{
		props.Add(prop);
		prop.transform.SetParent(base.transform);
		prop.transform.SetParent(null);
		prop.transform.SetParent(base.transform);
	}

	public void OnSave()
	{
		for (int i = 0; i < walls.Length; i++)
		{
			if (walls[i].crafted)
			{
				PreviewLabs.PlayerPrefs.SetString(i + "WallType" + base.transform.position, walls[i].wallStructure.name);
			}
			else if (PreviewLabs.PlayerPrefs.HasKey(i + "WallType" + base.transform.position))
			{
				PreviewLabs.PlayerPrefs.SetString(i + "WallType" + base.transform.position, string.Empty);
			}
		}
		PreviewLabs.PlayerPrefs.SetInt("RoomHealth" + base.transform.position, health);
		SaveProps();
		SaveHoles();
	}

	public void SaveProps()
	{
		for (int num = props.Count - 1; num > -1; num--)
		{
			if (props[num] == null)
			{
				props.RemoveAt(num);
			}
		}
		for (int i = 0; i < props.Count; i++)
		{
			PreviewLabs.PlayerPrefs.SetString(i + "Prop" + base.transform.position, props[i].name);
			PreviewLabs.PlayerPrefs.SetFloat(i + "PropPosX" + base.transform.position, props[i].transform.position.x);
			PreviewLabs.PlayerPrefs.SetFloat(i + "PropPosY" + base.transform.position, props[i].transform.position.y);
			PreviewLabs.PlayerPrefs.SetFloat(i + "PropPosZ" + base.transform.position, props[i].transform.position.z);
			PreviewLabs.PlayerPrefs.SetFloat(i + "PropRotX" + base.transform.position, props[i].transform.eulerAngles.x);
			PreviewLabs.PlayerPrefs.SetFloat(i + "PropRotY" + base.transform.position, props[i].transform.eulerAngles.y);
			PreviewLabs.PlayerPrefs.SetFloat(i + "PropRotZ" + base.transform.position, props[i].transform.eulerAngles.z);
		}
		PreviewLabs.PlayerPrefs.SetInt("PropCount" + base.transform.position, props.Count);
	}

	public void SaveHoles()
	{
		for (int i = 0; i < holes.Count; i++)
		{
			PreviewLabs.PlayerPrefs.SetString(i + "Hole" + base.transform.position, holes[i].name);
			PreviewLabs.PlayerPrefs.SetFloat(i + "HolePosX" + base.transform.position, holes[i].transform.position.x);
			PreviewLabs.PlayerPrefs.SetFloat(i + "HolePosY" + base.transform.position, holes[i].transform.position.y);
			PreviewLabs.PlayerPrefs.SetFloat(i + "HolePosZ" + base.transform.position, holes[i].transform.position.z);
			PreviewLabs.PlayerPrefs.SetFloat(i + "HoleRotX" + base.transform.position, holes[i].transform.eulerAngles.x);
			PreviewLabs.PlayerPrefs.SetFloat(i + "HoleRotY" + base.transform.position, holes[i].transform.eulerAngles.y);
			PreviewLabs.PlayerPrefs.SetFloat(i + "HoleRotZ" + base.transform.position, holes[i].transform.eulerAngles.z);
		}
		PreviewLabs.PlayerPrefs.SetInt("HoleCount" + base.transform.position, holes.Count);
	}

	public void OnLoad()
	{
		for (int i = 0; i < walls.Length; i++)
		{
			if (!(PreviewLabs.PlayerPrefs.GetString(i + "WallType" + base.transform.position) != string.Empty))
			{
				continue;
			}
			for (int j = 0; j < NanoInventory.instance.craftedObjects.Count; j++)
			{
				if (PreviewLabs.PlayerPrefs.GetString(i + "WallType" + base.transform.position) == NanoInventory.instance.craftedObjects[j].name)
				{
					walls[i].gameObject.SetActive(true);
					walls[i].OnCraft(NanoInventory.instance.craftedObjects[j] as WallStructure);
				}
			}
		}
		health = PreviewLabs.PlayerPrefs.GetInt("RoomHealth" + base.transform.position);
		LoadProps();
		LoadHoles();
	}

	public void LoadProps()
	{
		for (int i = 0; i < PreviewLabs.PlayerPrefs.GetInt("PropCount" + base.transform.position); i++)
		{
			for (int j = 0; j < NanoInventory.instance.craftedObjects.Count; j++)
			{
				if (PreviewLabs.PlayerPrefs.GetString(i + "Prop" + base.transform.position) == NanoInventory.instance.craftedObjects[j].prefab.name)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(NanoInventory.instance.craftedObjects[j].prefab, base.transform.position, base.transform.rotation);
					gameObject.name = NanoInventory.instance.craftedObjects[j].prefab.name;
					OnAddProp(gameObject);
					gameObject.transform.position = new Vector3(PreviewLabs.PlayerPrefs.GetFloat(i + "PropPosX" + base.transform.position), PreviewLabs.PlayerPrefs.GetFloat(i + "PropPosY" + base.transform.position), PreviewLabs.PlayerPrefs.GetFloat(i + "PropPosZ" + base.transform.position));
					gameObject.transform.eulerAngles = new Vector3(PreviewLabs.PlayerPrefs.GetFloat(i + "PropRotX" + base.transform.position), PreviewLabs.PlayerPrefs.GetFloat(i + "PropRotY" + base.transform.position), PreviewLabs.PlayerPrefs.GetFloat(i + "PropRotZ" + base.transform.position));
				}
			}
		}
	}

	public void LoadHoles()
	{
		for (int i = 0; i < PreviewLabs.PlayerPrefs.GetInt("HoleCount" + base.transform.position); i++)
		{
			GameObject gameObject = null;
			for (int j = 0; j < BaseLoader.instance.HoleTypes.Count; j++)
			{
				if (PreviewLabs.PlayerPrefs.GetString(i + "Hole" + base.transform.position) == BaseLoader.instance.HoleTypes[j].name)
				{
					gameObject = BaseLoader.instance.HoleTypes[j];
				}
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, base.transform.position, base.transform.rotation);
			gameObject2.name = gameObject.name;
			gameObject2.GetComponent<RoomLeak>().Setup(this);
			gameObject2.transform.position = new Vector3(PreviewLabs.PlayerPrefs.GetFloat(i + "HolePosX" + base.transform.position), PreviewLabs.PlayerPrefs.GetFloat(i + "HolePosY" + base.transform.position), PreviewLabs.PlayerPrefs.GetFloat(i + "HolePosZ" + base.transform.position));
			gameObject2.transform.eulerAngles = new Vector3(PreviewLabs.PlayerPrefs.GetFloat(i + "HoleRotX" + base.transform.position), PreviewLabs.PlayerPrefs.GetFloat(i + "HoleRotY" + base.transform.position), PreviewLabs.PlayerPrefs.GetFloat(i + "HoleRotZ" + base.transform.position));
		}
	}

	public void OnSalvage()
	{
		OnRemove();
	}

	public void UpdateState(float refreshRate)
	{
		if (group.TotalOxygen < group.MaxOxygen)
		{
			Oxygen += oxygenGenerationRate * refreshRate;
		}
		else
		{
			Oxygen = OxgenCapacity;
		}
		if (Oxygen - leakRate * refreshRate > 0f)
		{
			Oxygen -= leakRate * refreshRate;
		}
		else
		{
			Oxygen = 0f;
		}
		UpdateUI();
		if (leakRate > 0f)
		{
			DamagePlants(refreshRate);
		}
	}

	private void UpdateUI()
	{
		if ((bool)oxygenBar)
		{
			oxygenBar.fillAmount = Oxygen / OxgenCapacity;
			if ((bool)integrity)
			{
				integrity.text = health / maxHealth + "00%";
			}
			if ((bool)oxygenText)
			{
				oxygenText.text = Oxygen.ToString("F0") + "/" + OxgenCapacity.ToString("F0");
			}
		}
		if (!leakUI)
		{
			return;
		}
		if (leakRate > 0f)
		{
			if (!leakUI.activeSelf)
			{
				leakUI.SetActive(true);
			}
			leakText.text = "- " + leakRate.ToString("F0");
		}
		else if (leakUI.activeSelf)
		{
			leakUI.SetActive(false);
		}
	}

	private void DamagePlants(float refreshRate)
	{
		for (int i = 0; i < fruitPlants.Count; i++)
		{
			fruitPlants[i].OnTakeDamage(leakRate / 5f * refreshRate);
		}
		for (int j = 0; j < oxygenPlants.Count; j++)
		{
			oxygenPlants[j].OnTakeDamage(leakRate / 5f * refreshRate);
		}
	}

	public void OnRemove()
	{
		foreach (RoomNode node in nodes)
		{
			node.OnRemove();
		}
		WallNode[] array = walls;
		foreach (WallNode wallNode in array)
		{
			if (wallNode.isActiveAndEnabled)
			{
				wallNode.GetComponent<CraftComponent>().OnAdd(true);
				WallNode.wallNodes.Remove(wallNode);
			}
		}
		for (int j = 0; j < holes.Count; j++)
		{
			if (holes[j].isActiveAndEnabled)
			{
				holes[j].gameObject.SetActive(false);
			}
		}
		Transform transform = base.transform.Find("LeakPoints");
		if ((bool)transform)
		{
			IEnumerator enumerator2 = transform.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Transform transform2 = (Transform)enumerator2.Current;
					if ((transform2.tag == "LeakPoint" || transform2.tag == "LeakPointGlass") && BaseLoader.instance.leakHoles.Contains(transform2.gameObject))
					{
						BaseLoader.instance.leakHoles.Remove(transform2.gameObject);
					}
					if (transform2 != base.transform)
					{
						transform2.SendMessage("OnRemove", SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator2 as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
		}
		foreach (GameObject prop in props)
		{
			prop.SetActive(false);
		}
		BaseLoader.instance.AllRooms.Remove(this);
		OxygenManager.instance.UpdateState();
		base.gameObject.SetActive(false);
	}

	public void OnLoad(int index, float oxygen)
	{
		roomIndex = index;
		Oxygen = oxygen;
		craftTime = 0.1f;
		foreach (RoomNode node in nodes)
		{
			node.CheckConnections();
		}
		base.transform.SetParent(CenterRoom.instance.transform);
		OnLoad();
	}

	private void SetSalvageValue()
	{
		CraftComponent component = GetComponent<CraftComponent>();
		if ((bool)component)
		{
			for (int i = 0; i < component.materialCounts.Length; i++)
			{
				float num = component.materialCounts[i];
				num *= 0.8f;
				component.materialCounts[i] = (int)num;
			}
		}
	}

	private IEnumerator Constructing()
	{
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = renderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = true;
			if (meshRenderer.material.name.StartsWith(GameManager.instance.atlasMaterial.name) && (!meshRenderer.GetComponent<Recipe>() || meshRenderer.transform == base.transform) && !meshRenderer.GetComponent<RoomLeak>())
			{
				meshRenderer.material = GameManager.instance.craftMaterial;
				visuals.Add(meshRenderer);
			}
		}
		foreach (MeshRenderer visual in visuals)
		{
			visual.material.EnableKeyword("_Cutoff");
			visual.material.SetFloat("_Cutoff", 0.75f);
		}
		float waitTime = 0.05f;
		while (craftTime > 0f)
		{
			craftTime -= waitTime;
			float value = 0.25f + (startTime - craftTime) / startTime / 2f;
			foreach (MeshRenderer visual2 in visuals)
			{
				if (visual2 != null)
				{
					visual2.material.SetFloat("_Cutoff", 1f - value);
				}
			}
			yield return new WaitForSeconds(waitTime);
		}
		foreach (MeshRenderer visual3 in visuals)
		{
			if (visual3 != null)
			{
				visual3.material = GameManager.instance.atlasMaterial;
			}
		}
	}

	public void TakeDamage(int damage)
	{
	}

	public void OnCometImpact(GameObject comet)
	{
		if (!IsInvoking("ImpactCooldown"))
		{
			GameObject gameObject = GetClosestLeakNode(comet.transform).gameObject;
			Invoke("ImpactCooldown", 0.5f);
			GameObject gameObject2 = ((comet == CometManager.instance.Comet_S1) ? ((!(gameObject.tag == "LeakPointGlass")) ? BaseLoader.instance.HoleTypes[2] : BaseLoader.instance.HoleTypes[5]) : ((comet == CometManager.instance.Comet_S2) ? ((!(gameObject.tag == "LeakPointGlass")) ? BaseLoader.instance.HoleTypes[1] : BaseLoader.instance.HoleTypes[4]) : ((!(gameObject.tag == "LeakPointGlass")) ? BaseLoader.instance.HoleTypes[0] : BaseLoader.instance.HoleTypes[3])));
			BaseManager.instance.HullBreachWarning();
			GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2, base.transform);
			gameObject3.transform.position = gameObject.transform.position;
			gameObject3.transform.rotation = gameObject.transform.rotation;
			gameObject3.name = gameObject2.name;
			gameObject3.GetComponent<RoomLeak>().Setup(this);
			BaseLoader.instance.leakHoles.Remove(gameObject);
			GameObject gameObject4 = UnityEngine.Object.Instantiate(GameManager.instance.cometImpactParticle, gameObject.transform.position, Quaternion.identity);
		}
	}

	public void OnImpact(GameObject impactObject, int damage)
	{
		if (!IsInvoking("ImpactCooldown"))
		{
			Invoke("ImpactCooldown", 0.5f);
			GameObject gameObject = GetClosestLeakNode(impactObject.transform).gameObject;
			GameObject gameObject2;
			if (damage < 3)
			{
				gameObject2 = ((!(gameObject.tag == "LeakPointGlass")) ? BaseLoader.instance.HoleTypes[2] : BaseLoader.instance.HoleTypes[5]);
				health -= 3;
			}
			else if (damage < 5)
			{
				gameObject2 = ((!(gameObject.tag == "LeakPointGlass")) ? BaseLoader.instance.HoleTypes[1] : BaseLoader.instance.HoleTypes[4]);
				health -= damage - 5;
			}
			else
			{
				gameObject2 = ((!(gameObject.tag == "LeakPointGlass")) ? BaseLoader.instance.HoleTypes[0] : BaseLoader.instance.HoleTypes[3]);
				health -= damage - 8;
			}
			BaseManager.instance.HullBreachWarning();
			GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2, base.transform);
			gameObject3.transform.position = gameObject.transform.position;
			gameObject3.transform.rotation = gameObject.transform.rotation;
			gameObject3.name = gameObject2.name;
			Debug.Log("damageObject " + gameObject2.name);
			gameObject3.GetComponent<RoomLeak>().Setup(gameObject.GetComponentInParent<Room>());
			BaseLoader.instance.leakHoles.Remove(gameObject);
		}
	}

	public void OnImpact(Vector3 hitPoint, int damage)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.position = hitPoint;
		gameObject.AddComponent<DestroyOnTime>();
		OnImpact(gameObject, damage);
	}

	private Transform GetClosestLeakNode(Transform leakPoint)
	{
		float num = float.PositiveInfinity;
		float num2 = 0f;
		Transform result = null;
		for (int i = 0; i < BaseLoader.instance.leakHoles.Count; i++)
		{
			num2 = Vector3.Distance(BaseLoader.instance.leakHoles[i].transform.position, leakPoint.position);
			if (num2 < num)
			{
				num = num2;
				result = BaseLoader.instance.leakHoles[i].transform;
			}
		}
		return result;
	}

	private void OnBreak()
	{
		base.gameObject.SetActive(false);
		foreach (RoomNode node in nodes)
		{
			node.OnRemove();
		}
		BaseLoader.instance.AllRooms.Remove(this);
		OxygenManager.instance.UpdateState();
		if ((bool)BaseLoader.instance.RoomWreckage)
		{
			UnityEngine.Object.Instantiate(BaseLoader.instance.RoomWreckage, base.transform.position, Quaternion.identity);
		}
	}

	private void ImpactCooldown()
	{
	}
}
