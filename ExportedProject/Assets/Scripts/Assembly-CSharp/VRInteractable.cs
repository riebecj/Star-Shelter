using System.Collections.Generic;
using UnityEngine;

public class VRInteractable : ExposableMonobehaviour
{
	public bool Interactable = true;

	private Rigidbody rb3d;

	private List<Collider> mColliders3D;

	private bool mCache;

	private void Awake()
	{
		UpdateColliders3D();
		mCache = Interactable;
	}

	public void UpdateColliders3D()
	{
		rb3d = base.gameObject.GetComponent<Rigidbody>();
		mColliders3D = new List<Collider>();
		if (rb3d != null)
		{
			mColliders3D.AddRange(rb3d.gameObject.GetComponentsInChildren<Collider>());
			mColliders3D.Add(rb3d.gameObject.GetComponent<Collider>());
		}
		else
		{
			mColliders3D.AddRange(GetComponentsInChildren<Collider>());
			mColliders3D.Add(GetComponent<Collider>());
		}
	}

	public void IgnoreColliders(Rigidbody _rigidbody)
	{
		Collider[] componentsInChildren = _rigidbody.GetComponentsInChildren<Collider>();
		IgnoreColliders3D(componentsInChildren, mColliders3D.ToArray());
	}

	public void IgnoreColliders(Transform _object)
	{
		Collider[] componentsInChildren = _object.GetComponentsInChildren<Collider>();
		IgnoreColliders3D(componentsInChildren, mColliders3D.ToArray());
	}

	public void RemoveIgnoreColliders(Rigidbody _rigidbody)
	{
		Collider[] componentsInChildren = _rigidbody.GetComponentsInChildren<Collider>();
		IgnoreColliders3D(componentsInChildren, mColliders3D.ToArray(), false);
	}

	public void RemoveIgnoreColliders(Transform _object)
	{
		Collider[] componentsInChildren = _object.GetComponentsInChildren<Collider>();
		IgnoreColliders3D(componentsInChildren, mColliders3D.ToArray(), false);
	}

	public static void IgnoreColliders3D(Collider[] _colliders, Collider[] _otherColliders, bool _ignore = true)
	{
		foreach (Collider collider in _colliders)
		{
			foreach (Collider collider2 in _otherColliders)
			{
				if (!(collider2 == null) && !(collider == null))
				{
					Physics.IgnoreCollision(collider, collider2, _ignore);
				}
			}
		}
	}

	private void IgnoreAllControllerColliders()
	{
		foreach (SteamVR_TrackedObject controller in VRGripper.GetControllers())
		{
			IgnoreColliders(controller.transform);
		}
	}

	private void RemoveIgnoreAllControllerColliders()
	{
		foreach (SteamVR_TrackedObject controller in VRGripper.GetControllers())
		{
			RemoveIgnoreColliders(controller.transform);
		}
	}

	public void Update()
	{
		if (mCache != Interactable)
		{
			if (!Interactable)
			{
				IgnoreAllControllerColliders();
			}
			else
			{
				RemoveIgnoreAllControllerColliders();
			}
			mCache = Interactable;
		}
	}
}
