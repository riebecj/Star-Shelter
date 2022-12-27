using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "CraftObjects/Item")]
public class Item : ScriptableObject
{
	public new string name = "New Item";

	public virtual void Use()
	{
		Debug.Log("Using " + name);
	}
}
