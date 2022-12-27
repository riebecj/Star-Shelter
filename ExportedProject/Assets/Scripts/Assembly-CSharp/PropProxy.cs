using UnityEngine;

public class PropProxy : MonoBehaviour
{
	internal Collider target;

	public GameObject visualCone;

	private void OnTriggerStay(Collider other)
	{
		if (!other.isTrigger)
		{
			target = other;
			CraftingManager.instance.colliding = true;
			CancelInvoke("Exit");
			Invoke("Exit", 0.5f);
		}
	}

	private void Exit()
	{
		target = null;
		CraftingManager.instance.colliding = false;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other == target)
		{
			target = null;
			CraftingManager.instance.colliding = false;
		}
	}

	private void OnDisable()
	{
		target = null;
	}
}
