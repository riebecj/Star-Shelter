using UnityEngine;

public class ResearchCore : MonoBehaviour
{
	public int value = 1;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller")
		{
			BaseManager.researchPoints += value;
			base.gameObject.SetActive(false);
		}
	}
}
