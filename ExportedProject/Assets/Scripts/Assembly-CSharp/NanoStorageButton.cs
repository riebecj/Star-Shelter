using UnityEngine;
using UnityEngine.UI;

public class NanoStorageButton : MonoBehaviour
{
	internal NanoStorage nanoStorage;

	public Image icon;

	public Text count;

	internal CraftMaterial craftMaterial;

	internal int materialCount;

	public void OnSetup(NanoStorage storage, CraftMaterial _craftMaterial)
	{
		craftMaterial = _craftMaterial;
		nanoStorage = storage;
		icon.sprite = craftMaterial.icon;
	}

	public void UpdateCount(int index)
	{
		count.text = nanoStorage.materialCounts[index].ToString();
	}

	public void OnPress()
	{
		nanoStorage.OnTarget(this);
	}
}
