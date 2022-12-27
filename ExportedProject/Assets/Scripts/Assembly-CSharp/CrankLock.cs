using UnityEngine;
using VRTK;

public class CrankLock : MonoBehaviour
{
	private VRTK_InteractableObject interact;

	private HingeJoint joint;

	internal bool closed;

	internal bool squeecked;

	public Rigidbody door;

	internal bool isHeld;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		joint = GetComponent<HingeJoint>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
	}

	public bool IsHeld()
	{
		return interact.IsGrabbed();
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		GetComponent<Rigidbody>().isKinematic = false;
		isHeld = false;
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		GetComponent<Rigidbody>().isKinematic = true;
		isHeld = true;
	}

	public void OnGravityGrab()
	{
		isHeld = true;
	}

	private void NotHeld()
	{
		isHeld = false;
	}

	public void OnGravityDrop()
	{
		Invoke("NotHeld", 1f);
	}

	private void Update()
	{
		if (!closed && joint.angle > 89f)
		{
			Unlock();
		}
	}

	private void Unlock()
	{
		door.isKinematic = false;
	}
}
