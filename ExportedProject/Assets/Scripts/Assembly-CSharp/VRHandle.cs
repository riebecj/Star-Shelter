using System.Collections.Generic;
using UnityEngine;

public class VRHandle : MonoBehaviour
{
	public string Button;

	private CharacterJoint HandJoint;

	private Rigidbody OldConnection;

	private bool bAttached;

	private SteamVR_TrackedObject AttachedController;

	public Transform HandlePosition;

	public Transform HandleJointPrefab;

	private Transform JointObject;

	private List<SteamVR_TrackedObject> ActiveControllers = new List<SteamVR_TrackedObject>();

	public float breakForces = 10f;

	private void OnCollisionEnter(Collision _collision)
	{
		Debug.Log("Collision entered" + _collision.collider.gameObject.name);
		AttachTo(_collision.collider.attachedRigidbody);
	}

	private void OnTriggerEnter(Collider _collider)
	{
		Rigidbody attachedRigidbody = _collider.attachedRigidbody;
		if (!(attachedRigidbody == null))
		{
			SteamVR_TrackedObject component = attachedRigidbody.gameObject.GetComponent<SteamVR_TrackedObject>();
			if (!ActiveControllers.Contains(component))
			{
				ActiveControllers.Add(component);
			}
			AttachTo(_collider.attachedRigidbody);
		}
	}

	private void OnCollisionExit(Collision _collision)
	{
		HandleExit(_collision.collider.attachedRigidbody);
	}

	private void OnTriggerExit(Collider _collider)
	{
		HandleExit(_collider.attachedRigidbody);
	}

	private void HandleExit(Rigidbody _controllerBody)
	{
		if (!(_controllerBody == null))
		{
			SteamVR_TrackedObject component = _controllerBody.gameObject.GetComponent<SteamVR_TrackedObject>();
			if (ActiveControllers.Contains(component))
			{
				ActiveControllers.Remove(component);
			}
		}
	}

	public void AttachTo(Rigidbody _controllerBody)
	{
		if (_controllerBody == null)
		{
			return;
		}
		SteamVR_TrackedObject component = _controllerBody.gameObject.GetComponent<SteamVR_TrackedObject>();
		if (!(component == null))
		{
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)component.index);
			if (device.GetHairTrigger() && !bAttached)
			{
				AttachTo(component);
			}
		}
	}

	public void AttachTo(SteamVR_TrackedObject _controller)
	{
		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)_controller.index);
		device.TriggerHapticPulse(500);
		AddNewJoint(_controller);
	}

	public void AddNewJoint(SteamVR_TrackedObject _controller)
	{
		JointObject = Object.Instantiate(HandleJointPrefab, HandlePosition.position, Quaternion.identity);
		JointObject.parent = base.transform;
		ConfigurableJoint component = JointObject.GetComponent<ConfigurableJoint>();
		component.connectedBody = _controller.gameObject.GetComponent<Rigidbody>();
		FixedJoint component2 = JointObject.GetComponent<FixedJoint>();
		component2.connectedBody = base.transform.GetComponent<Rigidbody>();
		AttachedController = _controller;
		bAttached = true;
	}

	private void Update()
	{
		if (bAttached)
		{
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)AttachedController.index);
			if (!device.GetHairTrigger())
			{
				Disconnect();
			}
		}
		else
		{
			if (bAttached)
			{
				return;
			}
			foreach (SteamVR_TrackedObject activeController in ActiveControllers)
			{
				SteamVR_Controller.Device device2 = SteamVR_Controller.Input((int)activeController.index);
				if (device2.GetHairTrigger())
				{
					AttachTo(activeController);
				}
			}
		}
	}

	public void Disconnect()
	{
		Object.Destroy(JointObject.gameObject);
		AttachedController = null;
		bAttached = false;
	}
}
