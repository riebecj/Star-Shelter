using UnityEngine;

public class ToggleOnTrigger : MonoBehaviour
{
	public GameObject toggleObject;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller")
		{
			toggleObject.SetActive(!toggleObject.activeSelf);
		}
	}
}
