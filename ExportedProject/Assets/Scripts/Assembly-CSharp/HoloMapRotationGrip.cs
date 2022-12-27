using UnityEngine;
using VRTK;

public class HoloMapRotationGrip : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	public Transform target;

	public Transform lod;

	private Rigidbody rigidbody;

	internal bool rotate;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
		rotate = true;
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		rotate = true;
		rigidbody.angularVelocity = Vector3.zero;
	}

	private void Update()
	{
		if (rotate)
		{
			float num = Vector3.Dot(base.transform.up, target.transform.right);
			int num2 = 1;
			if (num < 0f)
			{
				num2 = -1;
			}
			float num3 = Vector3.Angle(base.transform.up, target.transform.up);
			if (num3 < 179.5f)
			{
				target.RotateAround(target.position, Vector3.up, (float)(8 * num2) * Time.deltaTime);
				lod.rotation = target.rotation;
			}
			else
			{
				rotate = false;
			}
		}
	}
}
