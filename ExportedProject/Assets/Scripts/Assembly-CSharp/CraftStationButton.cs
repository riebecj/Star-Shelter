using UnityEngine;
using UnityEngine.UI;

public class CraftStationButton : MonoBehaviour
{
	private int index;

	public new Text name;

	public Text nameColor;

	private CraftStation craftStation;

	public CraftedObject craftedObject;

	public CraftMaterial craftMaterial;

	public void OnSetup(ScriptableObject obj, CraftStation station)
	{
		craftStation = station;
		if (obj is CraftMaterial)
		{
			craftMaterial = (CraftMaterial)obj;
			name.text = craftMaterial.name;
			nameColor.text = craftMaterial.name;
		}
		else if (obj is CraftedObject)
		{
			craftedObject = (CraftedObject)obj;
			name.text = craftedObject.name;
			nameColor.text = craftedObject.name;
		}
	}

	public void OnPress()
	{
		craftStation.OnSelectCraft(craftedObject, craftMaterial);
	}
}
