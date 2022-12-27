using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;

public class NanoStorage : MonoBehaviour
{
	public static List<NanoStorage> nanoStorages = new List<NanoStorage>();

	internal bool transferAll;

	internal CraftMaterial targetmaterial;

	public Text suitCount;

	public Text storageCount;

	public Text storageCapacity;

	public Text materialName;

	public Image suitMatIcon;

	public Image storageMatIcon;

	public int nanoCap = 25;

	public List<CraftMaterial> craftMaterials = new List<CraftMaterial>();

	public List<int> materialCounts = new List<int>();

	public GameObject buttonRef;

	public Transform grid;

	private NanoStorageButton oldTarget;

	public List<NanoStorageButton> buttons = new List<NanoStorageButton>();

	private void Awake()
	{
		nanoStorages.Add(this);
	}

	private void Start()
	{
		materialName.text = string.Empty;
		craftMaterials = NanoInventory.instance.craftMaterials;
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			materialCounts.Add(0);
		}
		OnLoad();
		GenerateButtons();
		if (nanoCap < nanoCap + BaseManager.instance.nanoCapacity * 15)
		{
			nanoCap += BaseManager.instance.nanoCapacity * 15;
		}
		UpdateUI();
	}

	public void ToggleTransferAll(ToggleButton toggleButton)
	{
		transferAll = toggleButton.On;
	}

	public void OnTarget(NanoStorageButton newTarget)
	{
		if ((bool)oldTarget)
		{
			oldTarget.transform.localScale = Vector3.one;
		}
		targetmaterial = newTarget.craftMaterial;
		newTarget.transform.localScale = Vector3.one * 1.25f;
		suitMatIcon.sprite = targetmaterial.icon;
		storageMatIcon.sprite = targetmaterial.icon;
		oldTarget = newTarget;
		materialName.text = targetmaterial.name;
		UpdateUI();
	}

	private void GenerateButtons()
	{
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			GameObject gameObject = Object.Instantiate(buttonRef, grid);
			NanoStorageButton component = gameObject.GetComponent<NanoStorageButton>();
			component.OnSetup(this, craftMaterials[i]);
			component.UpdateCount(i);
			buttons.Add(component);
		}
		buttons[0].OnPress();
	}

	public void Transfer(bool toSuit)
	{
		if (toSuit)
		{
			for (int i = 0; i < craftMaterials.Count; i++)
			{
				if (!(craftMaterials[i] == targetmaterial) || materialCounts[i] <= 0)
				{
					continue;
				}
				if (NanoInventory.instance.GetNanoMass() < NanoInventory.instance.nanoCap)
				{
					if (transferAll)
					{
						int num = Mathf.Clamp(materialCounts[i], 0, NanoInventory.instance.nanoCap - NanoInventory.instance.GetNanoMass());
						NanoInventory.instance.materialCounts[i] += num;
						materialCounts[i] -= num;
					}
					else
					{
						NanoInventory.instance.materialCounts[i]++;
						materialCounts[i]--;
					}
					if (materialCounts[i] < 0)
					{
						materialCounts[i] = 0;
					}
				}
				else
				{
					Debug.Log("Full");
				}
			}
		}
		else
		{
			for (int j = 0; j < craftMaterials.Count; j++)
			{
				if (!(craftMaterials[j] == targetmaterial) || NanoInventory.instance.materialCounts[j] <= 0)
				{
					continue;
				}
				if (GetNanoMass() < nanoCap)
				{
					if (transferAll)
					{
						int num2 = Mathf.Clamp(NanoInventory.instance.materialCounts[j], 0, nanoCap - GetNanoMass());
						materialCounts[j] += num2;
						NanoInventory.instance.materialCounts[j] -= num2;
					}
					else
					{
						NanoInventory.instance.materialCounts[j]--;
						materialCounts[j]++;
					}
					if (NanoInventory.instance.materialCounts[j] < 0)
					{
						NanoInventory.instance.materialCounts[j] = 0;
					}
				}
				else
				{
					Debug.Log("Full");
				}
			}
		}
		UpdateUI();
	}

	public void OnSave()
	{
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			PreviewLabs.PlayerPrefs.SetInt(craftMaterials[i].name + "Count" + base.transform.position, materialCounts[i]);
		}
	}

	public void OnLoad()
	{
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			materialCounts[i] = PreviewLabs.PlayerPrefs.GetInt(craftMaterials[i].name + "Count" + base.transform.position);
		}
	}

	public int GetNanoMass()
	{
		int num = 0;
		for (int i = 0; i < materialCounts.Count; i++)
		{
			num += materialCounts[i];
		}
		return num;
	}

	public void UpdateUI()
	{
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			buttons[i].UpdateCount(i);
			if (craftMaterials[i] == targetmaterial)
			{
				suitCount.text = NanoInventory.instance.materialCounts[i].ToString();
				storageCount.text = materialCounts[i].ToString();
			}
		}
		storageCapacity.text = GetNanoMass() + "/" + nanoCap;
		NanoInventory.instance.GetNanoMass();
		if ((bool)ArmUIManager.instance)
		{
			ArmUIManager.instance.UpdateUI();
		}
	}

	private void OnDisable()
	{
		nanoStorages.Remove(this);
	}
}
