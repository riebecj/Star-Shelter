using UnityEngine;
using UnityEngine.UI;

public class CraftedObjectIcon : MonoBehaviour
{
	public CraftedObject craftedObject;

	public MeshFilter filter;

	public Text nameLabel;

	public Text count;

	public Image unread;

	public void OnAssign(CraftedObject _object)
	{
		craftedObject = _object;
		if (_object is Structure)
		{
			filter.transform.localScale = new Vector3(5f, 5f, 5f);
		}
		else
		{
			filter.transform.localScale = new Vector3(_object.iconSize, _object.iconSize, _object.iconSize);
		}
		filter.mesh = craftedObject.icon;
		nameLabel.text = craftedObject.name;
	}

	public void OnClick()
	{
		ArmUIManager.instance.PlaceCraftedObject(this);
		unread.gameObject.SetActive(false);
		craftedObject.newCraft = false;
	}
}
