using UnityEngine;

public class BaseRotationSwitch : MonoBehaviour
{
	internal bool rotate;

	public Transform target;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller")
		{
			rotate = true;
			GameManager.instance.CamRig.SetParent(base.transform);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Controller")
		{
			rotate = false;
			GameManager.instance.CamRig.SetParent(null, true);
		}
	}

	private void Update()
	{
		if (rotate)
		{
			target.RotateAround(target.position, Vector3.up, 5f * Time.deltaTime);
			GameManager.instance.CamRig.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}
}
