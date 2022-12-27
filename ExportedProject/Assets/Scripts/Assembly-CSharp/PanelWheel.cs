using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class PanelWheel : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	private VRTK_ControllerEvents holdControl;

	private Rigidbody rigidbody;

	public PanelHackA panel;

	internal float Vibration = 0.05f;

	private float PreviousRotZ;

	public Image wheelColor;

	public Image wheelFrame;

	public Color frameDefault;

	public Color frameTargeted;

	internal Transform targetPoint;

	internal float offset;

	internal float deltaAngle;

	internal float oldangle;

	public GameObject proxy;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectUngrabbed += DoObjectDrop;
		interact.InteractableObjectGrabbed += DoObjectGrab;
		rigidbody = GetComponent<Rigidbody>();
		base.gameObject.layer = 15;
	}

	public bool IsHeld()
	{
		return interact.IsGrabbed();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller")
		{
			if (!panel.hasTarget)
			{
				wheelFrame.color = frameTargeted;
				panel.audioSource.PlayOneShot(panel.targetWheelAudio);
			}
			targetPoint = other.transform;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Controller" && !IsHeld())
		{
			wheelFrame.color = frameDefault;
		}
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		if (!panel.hasTarget)
		{
			if (VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
			{
				holdControl = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>();
			}
			else
			{
				holdControl = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>();
			}
			panel.Activate(base.transform);
			panel.hasTarget = true;
			wheelFrame.color = frameTargeted;
		}
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
		panel.Deactivate();
		panel.hasTarget = false;
		wheelFrame.color = frameDefault;
		offset = 0f;
	}

	private void Update()
	{
		if (!IsHeld())
		{
			return;
		}
		float z = base.transform.localEulerAngles.z;
		if (!z.ToString("F0").Equals(PreviousRotZ.ToString("F0")))
		{
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), Vibration);
		}
		PreviousRotZ = z;
		if ((bool)targetPoint)
		{
			Vector3 normalized = (targetPoint.GetComponent<Collider>().bounds.center - base.transform.position).normalized;
			float num = Vector3.SignedAngle(base.transform.parent.up, normalized, base.transform.parent.forward);
			if (offset == 0f)
			{
				offset = num - base.transform.eulerAngles.z;
			}
			base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, num - offset);
			deltaAngle = num - offset - oldangle;
			oldangle = num - offset;
		}
	}
}
