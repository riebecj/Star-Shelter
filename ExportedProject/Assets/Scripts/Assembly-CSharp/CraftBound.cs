using UnityEngine;

public class CraftBound : MonoBehaviour
{
	public CraftStation craftStation;

	private void Start()
	{
		craftStation.craftBound = this;
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<CraftProxy>())
		{
			other.GetComponent<CraftProxy>().craftBound = this;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<CraftProxy>() && craftStation.craftQueue.Count > 0 && craftStation.craftQueue[0] == other.GetComponent<CraftProxy>())
		{
			craftStation.CancelCrafting();
		}
	}
}
