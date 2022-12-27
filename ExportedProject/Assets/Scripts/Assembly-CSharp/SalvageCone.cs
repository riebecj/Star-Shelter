using UnityEngine;

public class SalvageCone : MonoBehaviour
{
	private Collider col;

	private void Start()
	{
		col = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponentInParent<CraftComponent>())
		{
			other.GetComponentInParent<CraftComponent>().SetTarget(col);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponentInParent<CraftComponent>())
		{
			other.GetComponentInParent<CraftComponent>().OnExitTarget(col, false);
		}
	}
}
