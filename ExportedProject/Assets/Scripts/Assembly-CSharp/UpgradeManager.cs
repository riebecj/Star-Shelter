using System.Collections;
using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class UpgradeManager : MonoBehaviour
{
	public static List<UpgradeManager> upgradeManagers = new List<UpgradeManager>();

	public List<Image> upgradeRadials = new List<Image>();

	public List<Text> upgradInfo = new List<Text>();

	public AudioSource audioSource;

	public AudioClip upgradeBeep;

	public static UpgradeManager instance;

	public GameObject researchIcon;

	internal int upgradeIndex;

	internal bool returnIcon;

	public Text[] countLabels;

	private Vector3 startPos;

	internal static int InventoryCapacity;

	internal static int ConstructionSpeed;

	internal static int SalvageSpeed;

	internal static int AmmoCapacity;

	internal static int RadiationResistance;

	internal static int NanoStorageCapacity;

	internal static int SuitNanoCapacity;

	public static bool loaded;

	private void Awake()
	{
		upgradeManagers.Add(this);
	}

	private void Start()
	{
		if (!loaded)
		{
			LoadUpgrades();
		}
		RefreshVisuals();
		Text[] array = countLabels;
		foreach (Text text in array)
		{
			text.text = BaseManager.researchPoints.ToString();
		}
		if (BaseManager.researchPoints == 0)
		{
			researchIcon.SetActive(false);
		}
		startPos = researchIcon.transform.position;
		if ((bool)base.transform.parent.GetComponent<Rigidbody>())
		{
			Object.Destroy(base.transform.parent.GetComponent<Rigidbody>());
		}
	}

	public static void Unload()
	{
		InventoryCapacity = 0;
		ConstructionSpeed = 0;
		SalvageSpeed = 0;
		AmmoCapacity = 0;
		RadiationResistance = 0;
		NanoStorageCapacity = 0;
		SuitNanoCapacity = 0;
		loaded = false;
	}

	public void UpdateState()
	{
		if (BaseManager.researchPoints == 0)
		{
			researchIcon.SetActive(false);
		}
		else
		{
			researchIcon.SetActive(true);
		}
		Text[] array = countLabels;
		foreach (Text text in array)
		{
			text.text = BaseManager.researchPoints.ToString();
		}
	}

	public void AddResearchPoint()
	{
		researchIcon.SetActive(true);
		Text[] array = countLabels;
		foreach (Text text in array)
		{
			text.text = BaseManager.researchPoints.ToString();
		}
	}

	public void AttemptUpgrade(int index)
	{
		if (!CheckIfCanUpgrade(index))
		{
			return;
		}
		if (BaseManager.researchPoints >= 1)
		{
			researchIcon.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
			researchIcon.transform.position = startPos;
			BaseManager.researchPoints--;
		}
		if (BaseManager.researchPoints <= 0)
		{
			researchIcon.SetActive(false);
		}
		upgradeIndex = index;
		CompleteUpdate();
		for (int i = 0; i < upgradeManagers.Count; i++)
		{
			Text[] array = upgradeManagers[i].countLabels;
			foreach (Text text in array)
			{
				text.text = BaseManager.researchPoints.ToString();
			}
		}
	}

	public void OnYes()
	{
		CompleteUpdate();
	}

	public void OnNo()
	{
	}

	private bool CheckIfCanUpgrade(int index)
	{
		if (index == 0 && InventoryCapacity < 4)
		{
			return true;
		}
		if (index == 1 && ConstructionSpeed < 5)
		{
			return true;
		}
		if (index == 2 && SalvageSpeed < 5)
		{
			return true;
		}
		if (index == 3 && AmmoCapacity < 3)
		{
			return true;
		}
		if (index == 4 && RadiationResistance < 3)
		{
			return true;
		}
		if (index == 5 && NanoStorageCapacity < 5)
		{
			return true;
		}
		if (index == 6 && SuitNanoCapacity < 5)
		{
			return true;
		}
		return false;
	}

	public void CompleteUpdate()
	{
		if (upgradeIndex == 0)
		{
			UpgradeInventoryCapacity();
		}
		else if (upgradeIndex == 1)
		{
			UpgradeConstructionSpeed();
		}
		else if (upgradeIndex == 2)
		{
			UpgradeSalvageSpeed();
		}
		else if (upgradeIndex == 3)
		{
			UpgradeAmmoCapacity();
		}
		else if (upgradeIndex == 4)
		{
			UpgradeRadiationResistance();
		}
		else if (upgradeIndex == 5)
		{
			UpgradeNanoStorageCapacity();
		}
		else if (upgradeIndex == 6)
		{
			UpgradeSuitNanoCapacity();
		}
		audioSource.PlayOneShot(upgradeBeep);
	}

	internal void UpgradeInventoryCapacity()
	{
		if (InventoryCapacity > 3)
		{
			return;
		}
		InventoryCapacity++;
		float x = Inventory.instance.inventoryBubble.transform.localScale.x;
		GameObject[] array = Inventory.inventory.ToArray();
		foreach (GameObject item in Inventory.inventory)
		{
			item.transform.SetParent(null);
		}
		Inventory.instance.inventoryBubble.transform.localScale = new Vector3(x + 0.15f, x + 0.15f, x + 0.15f);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.transform.SetParent(Inventory.instance.inventoryBubble.transform);
		}
		for (int j = 0; j < upgradeManagers.Count; j++)
		{
			upgradeManagers[j].UpdateVisual(0, (float)InventoryCapacity / 4f, InventoryCapacity + "/4");
		}
	}

	internal void UpgradeConstructionSpeed()
	{
		ConstructionSpeed++;
		CraftingManager.instance.craftSpeedMultiplier += 0.25f;
		for (int i = 0; i < upgradeManagers.Count; i++)
		{
			upgradeManagers[i].UpdateVisual(1, (float)ConstructionSpeed / 5f, ConstructionSpeed + "/5");
		}
	}

	internal void UpgradeSalvageSpeed()
	{
		SalvageSpeed++;
		CraftingManager.instance.salvageSpeedMultiplier += 0.25f;
		for (int i = 0; i < upgradeManagers.Count; i++)
		{
			upgradeManagers[i].UpdateVisual(2, (float)SalvageSpeed / 5f, SalvageSpeed + "/5");
		}
	}

	internal void UpgradeAmmoCapacity()
	{
		AmmoCapacity++;
		Gun.instance.OnUpgrade(4 + AmmoCapacity * 2);
		for (int i = 0; i < upgradeManagers.Count; i++)
		{
			upgradeManagers[i].UpdateVisual(3, (float)AmmoCapacity / 3f, AmmoCapacity + "/3");
		}
	}

	internal void UpgradeRadiationResistance()
	{
		RadiationResistance++;
		SuitManager.instance.UpgradeRadiationResistance();
		for (int i = 0; i < upgradeManagers.Count; i++)
		{
			upgradeManagers[i].UpdateVisual(4, (float)RadiationResistance / 3f, RadiationResistance + "/3");
		}
	}

	internal void UpgradeNanoStorageCapacity()
	{
		NanoStorageCapacity++;
		BaseManager.instance.UpgradeNanoStorage();
		for (int i = 0; i < upgradeManagers.Count; i++)
		{
			upgradeManagers[i].UpdateVisual(5, (float)NanoStorageCapacity / 5f, NanoStorageCapacity + "/5");
		}
	}

	internal void UpgradeSuitNanoCapacity()
	{
		SuitNanoCapacity++;
		NanoInventory.instance.UpgradeSuitNanoCapacity();
		for (int i = 0; i < upgradeManagers.Count; i++)
		{
			upgradeManagers[i].UpdateVisual(6, (float)SuitNanoCapacity / 5f, SuitNanoCapacity + "/5");
		}
	}

	internal void UpdateVisual(int index, float value, string info)
	{
		upgradeRadials[index].fillAmount = value;
		upgradInfo[index].text = info;
		upgradInfo[index].transform.GetChild(0).GetComponent<Text>().text = info;
	}

	internal void RefreshVisuals()
	{
		for (int i = 0; i < upgradeManagers.Count; i++)
		{
			upgradeManagers[i].UpdateVisual(0, (float)InventoryCapacity / 4f, InventoryCapacity + "/4");
		}
		for (int j = 0; j < upgradeManagers.Count; j++)
		{
			upgradeManagers[j].UpdateVisual(1, (float)ConstructionSpeed / 5f, ConstructionSpeed + "/5");
		}
		for (int k = 0; k < upgradeManagers.Count; k++)
		{
			upgradeManagers[k].UpdateVisual(2, (float)SalvageSpeed / 5f, SalvageSpeed + "/5");
		}
		for (int l = 0; l < upgradeManagers.Count; l++)
		{
			upgradeManagers[l].UpdateVisual(3, (float)AmmoCapacity / 3f, AmmoCapacity + "/3");
		}
		for (int m = 0; m < upgradeManagers.Count; m++)
		{
			upgradeManagers[m].UpdateVisual(4, (float)RadiationResistance / 3f, RadiationResistance + "/3");
		}
		for (int n = 0; n < upgradeManagers.Count; n++)
		{
			upgradeManagers[n].UpdateVisual(5, (float)NanoStorageCapacity / 5f, NanoStorageCapacity + "/5");
		}
		for (int num = 0; num < upgradeManagers.Count; num++)
		{
			upgradeManagers[num].UpdateVisual(6, (float)SuitNanoCapacity / 5f, SuitNanoCapacity + "/5");
		}
	}

	private IEnumerator ReturnIcon()
	{
		while (returnIcon)
		{
			if (Vector3.Distance(researchIcon.transform.position, startPos) > 0.03f)
			{
				researchIcon.transform.position = Vector3.MoveTowards(researchIcon.transform.position, startPos, 10f * Time.deltaTime);
			}
			else
			{
				researchIcon.GetComponent<Rigidbody>().velocity = Vector3.zero;
				returnIcon = false;
			}
			yield return new WaitForSeconds(0.02f);
		}
	}

	public void SaveUpgrades()
	{
		PreviewLabs.PlayerPrefs.SetInt("InventoryCapacity", InventoryCapacity);
		PreviewLabs.PlayerPrefs.SetInt("ConstructionSpeed", ConstructionSpeed);
		PreviewLabs.PlayerPrefs.SetInt("SalvageSpeed", SalvageSpeed);
		PreviewLabs.PlayerPrefs.SetInt("AmmoCapacity", AmmoCapacity);
		PreviewLabs.PlayerPrefs.SetInt("RadiationResistance", RadiationResistance);
		PreviewLabs.PlayerPrefs.SetInt("NanoStorageCapacity", NanoStorageCapacity);
		PreviewLabs.PlayerPrefs.SetInt("SuitNanoCapacity", SuitNanoCapacity);
	}

	public void LoadUpgrades()
	{
		loaded = true;
		int @int = PreviewLabs.PlayerPrefs.GetInt("InventoryCapacity");
		for (int i = 0; i < @int; i++)
		{
			UpgradeInventoryCapacity();
		}
		int num = PreviewLabs.PlayerPrefs.GetInt("ConstructionSpeed");
		if (Mathf.Abs(num) > 10)
		{
			num = 1;
		}
		for (int j = 0; j < num; j++)
		{
			UpgradeConstructionSpeed();
		}
		int num2 = PreviewLabs.PlayerPrefs.GetInt("SalvageSpeed");
		if (Mathf.Abs(num2) > 10)
		{
			num2 = 1;
		}
		for (int k = 0; k < num2; k++)
		{
			UpgradeSalvageSpeed();
		}
		int num3 = PreviewLabs.PlayerPrefs.GetInt("AmmoCapacity");
		if (Mathf.Abs(num3) > 10)
		{
			num3 = 1;
		}
		if (Application.isEditor)
		{
			num3 = 3;
		}
		for (int l = 0; l < num3; l++)
		{
			UpgradeAmmoCapacity();
		}
		int num4 = PreviewLabs.PlayerPrefs.GetInt("RadiationResistance");
		if (Mathf.Abs(num4) > 10)
		{
			num4 = 1;
		}
		for (int m = 0; m < num4; m++)
		{
			UpgradeRadiationResistance();
		}
		int num5 = PreviewLabs.PlayerPrefs.GetInt("NanoStorageCapacity");
		if (Mathf.Abs(num5) > 10)
		{
			num5 = 1;
		}
		for (int n = 0; n < num5; n++)
		{
			UpgradeNanoStorageCapacity();
		}
		int num6 = PreviewLabs.PlayerPrefs.GetInt("SuitNanoCapacity");
		if (Mathf.Abs(num6) > 10)
		{
			num6 = 1;
		}
		for (int num7 = 0; num7 < num6; num7++)
		{
			UpgradeSuitNanoCapacity();
		}
	}
}
