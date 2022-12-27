using System.Collections;
using UnityEngine;

public class UIBillboard : MonoBehaviour
{
	internal Transform cam;

	private void OnEnable()
	{
		Invoke("Setup", 0.1f);
	}

	private void Setup()
	{
		cam = Camera.main.transform;
		if (base.isActiveAndEnabled)
		{
			StartCoroutine("UpdateUI");
		}
	}

	private IEnumerator UpdateUI()
	{
		while (true)
		{
			base.transform.LookAt(base.transform.position + cam.rotation * Vector3.forward, Vector3.up);
			yield return new WaitForSeconds(0.025f);
		}
	}
}
