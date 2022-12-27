using UnityEngine;

[CreateAssetMenu(fileName = "New CraftMaterial", menuName = "CraftObjects/CraftMaterial")]
public class CraftMaterial : ScriptableObject
{
	public new string name = "New CraftMaterial";

	[AssetIcon]
	public Sprite icon;

	public GameObject prefab;

	public int iconSize = 1;

	public string description;
}
