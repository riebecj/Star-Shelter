using UnityEngine;

public class ToggleOnTime : MonoBehaviour
{
	public float waitForSeconds = 1f;

	public GameObject toggleObject;

	private void OnEnable()
	{
		Invoke("Toggle", waitForSeconds);
	}

	private void Toggle()
	{
		toggleObject.SetActive(!toggleObject.activeSelf);
	}
}
