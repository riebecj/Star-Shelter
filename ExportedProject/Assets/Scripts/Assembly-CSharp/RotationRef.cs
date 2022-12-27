using UnityEngine;
using VRTK;

public class RotationRef : MonoBehaviour
{
	private Transform left;

	private Transform right;

	internal VRTK_ControllerEvents leftEvents;

	internal VRTK_ControllerEvents rightEvents;

	private Vector3 leftGrabPos;

	private Vector3 rightGrabPos;

	internal bool startGrabbing;

	private Quaternion grabbedRot;

	private void Start()
	{
		left = GameManager.instance.leftController.parent;
		right = GameManager.instance.rightController.parent;
		leftEvents = left.GetComponentInChildren<VRTK_ControllerEvents>();
		rightEvents = right.GetComponentInChildren<VRTK_ControllerEvents>();
	}

	private void Update()
	{
		if (leftEvents.triggerPressed && rightEvents.triggerPressed)
		{
			if (!startGrabbing)
			{
				startGrabbing = true;
				leftGrabPos = left.position;
				rightGrabPos = right.position;
				grabbedRot = GameManager.instance.CamRig.rotation;
			}
			Vector3 normalized = (leftGrabPos - rightGrabPos).normalized;
			Vector3 normalized2 = (left.transform.position - right.transform.position).normalized;
			Quaternion quaternion = Quaternion.FromToRotation(normalized, normalized2);
			base.transform.rotation = quaternion * grabbedRot;
			Quaternion quaternion2 = quaternion * grabbedRot;
			base.transform.position = GameManager.instance.Head.position;
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, quaternion * grabbedRot, 1.5f * Time.deltaTime);
			GameManager.instance.CamRig.rotation = Quaternion.Lerp(GameManager.instance.CamRig.rotation, base.transform.rotation, 5f * Time.deltaTime);
		}
		else
		{
			startGrabbing = false;
		}
	}
}
