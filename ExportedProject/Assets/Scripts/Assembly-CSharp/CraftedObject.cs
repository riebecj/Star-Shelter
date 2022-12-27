using UnityEngine;

[CreateAssetMenu(fileName = "New CraftedObject", menuName = "CraftObjects/CraftedObject")]
public class CraftedObject : ScriptableObject
{
	public new string name = "New CraftedObject";

	public GameObject prefab;

	public Mesh icon;

	public int iconSize = 5;

	public string description;

	public float iconOffsetY;

	public Vector3 iconRot;

	internal bool newCraft;
}
