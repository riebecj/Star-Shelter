using UnityEngine;
using UnityEngine.UI;

public class CraftMaterialIcon : MonoBehaviour
{
	public CraftMaterial craftedObject;

	public Image icon;

	public Text count;

	private int index;

	public void OnAssign(CraftMaterial _object)
	{
		craftedObject = _object;
		icon.sprite = craftedObject.icon;
		for (int i = 0; i < NanoInventory.instance.craftMaterials.Count; i++)
		{
			if (craftedObject == NanoInventory.instance.craftMaterials[i])
			{
				count.text = NanoInventory.instance.materialCounts[i].ToString();
				index = i;
			}
		}
	}

	public void UpdateCount()
	{
		count.text = NanoInventory.instance.materialCounts[index].ToString();
	}

	public void OnTarget()
	{
		ArmUIManager.instance.TargetCraftMaterial(this);
	}
}
