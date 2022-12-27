using UnityEngine;
using VRTK;

public class BaseOxygenCharger : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	private VRTK_ControllerEvents holdControl;

	internal float Vibration = 0.062f;

	internal float oxygenValue;

	public float drainSpeed = 5f;

	private Collider collider;

	internal Transform target;

	private void Start()
	{
		collider = GetComponent<SphereCollider>();
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
	}

	public bool IsHeld()
	{
		return interact.IsGrabbed();
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		if (VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
		{
			holdControl = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>();
		}
		else
		{
			holdControl = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller")
		{
			target = other.transform;
			if (VRTK_DeviceFinder.IsControllerLeftHand(other.transform.parent.gameObject))
			{
				holdControl = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>();
			}
			else
			{
				holdControl = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform == target)
		{
			target = null;
		}
	}

	private void Update()
	{
		if (target != null && !collider.bounds.Contains(target.position))
		{
		}
	}
}
