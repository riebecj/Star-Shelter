using UnityEngine;

public class NodeProxy : MonoBehaviour
{
	public GameObject turretRadius;

	private void OnEnable()
	{
		if ((bool)CraftingManager.instance.craftObject && CraftingManager.instance.craftObject.name == "SpaceshipTurretPlate")
		{
			turretRadius.SetActive(true);
		}
		else
		{
			turretRadius.SetActive(false);
		}
	}
}
