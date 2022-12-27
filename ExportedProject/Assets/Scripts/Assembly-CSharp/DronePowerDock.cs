using UnityEngine;
using VRTK;

public class DronePowerDock : MonoBehaviour
{
	internal Transform target;

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.parent != base.transform && (bool)other.GetComponent<PowerContainer>() && !other.GetComponent<PowerContainer>().IsInvoking("SnapCooldown") && !IsInvoking("Snap"))
		{
			other.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
			other.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
			other.isTrigger = true;
			target = other.transform;
			Invoke("Snap", 0.1f);
		}
	}

	private void Snap()
	{
		target.SetParent(base.transform);
		if (target.parent != null)
		{
			target.localPosition = new Vector3(0f, 0f, -0.01f);
			target.localEulerAngles = new Vector3(180f, 0f, 0f);
			target.GetComponent<VRTK_InteractableObject>().previousParent = base.transform;
			target.GetComponent<VRTK_InteractableObject>().previousKinematicState = true;
			target.GetComponent<Rigidbody>().isKinematic = true;
			if ((bool)target.GetComponent<PowerContainer>())
			{
				target.GetComponent<PowerContainer>().StartCoroutine("DrainToDrone");
				target.GetComponent<PowerContainer>().locked = true;
			}
		}
		target.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
		target.gameObject.layer = 21;
	}
}
