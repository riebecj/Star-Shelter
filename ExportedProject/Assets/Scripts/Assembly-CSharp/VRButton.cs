using System.Collections.Generic;
using UnityEngine;

public class VRButton : VRInteractable
{
	public VRButtonEvent ButtonListeners;

	private List<VRGripper> ActiveControllers = new List<VRGripper>();

	public float TriggerHapticStrength = 0.5f;

	private void OnTriggerEnter(Collider _collider)
	{
		if (Interactable && _collider.name == "Switch")
		{
			TriggerButton();
		}
	}

	private void OnCollisionEnter(Collision _collision)
	{
		if (Interactable && _collision.collider.name == "Switch")
		{
			TriggerButton();
		}
		else if (_collision.rigidbody == null)
		{
			return;
		}
		VRGripper component = _collision.rigidbody.GetComponent<VRGripper>();
		if (component != null)
		{
			ActiveControllers.Add(component);
		}
	}

	private void OnCollisionExit(Collision _collision)
	{
		if (!(_collision.rigidbody == null))
		{
			VRGripper component = _collision.rigidbody.GetComponent<VRGripper>();
			if (component != null)
			{
				ActiveControllers.Remove(component);
			}
		}
	}

	private void TriggerButton()
	{
		if (!Interactable)
		{
			return;
		}
		if (ButtonListeners != null)
		{
			ButtonListeners.Invoke(this);
		}
		foreach (VRGripper activeController in ActiveControllers)
		{
			activeController.HapticVibration(0.112f, TriggerHapticStrength);
		}
	}
}
