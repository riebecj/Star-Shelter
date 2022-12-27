using UnityEngine;
using VRTK;

public class ForceHook : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	internal VRTK_ControllerEvents holdControl;

	internal VRTK_InteractGrab grab;

	private AudioSource audioSource;

	public GameObject grabProxy;

	public GameObject beam;

	internal bool lockedObject;

	private Rigidbody rigidbody;

	private bool flinging;

	private Vector3 grabPointWorldPosition;

	private Vector3 startPlayAreaWorldOffset;

	private Vector3 grabOffset;

	private Vector3 areaOffset;

	private Vector3 hitOffset;

	public Transform barrel;

	public Transform snapPoint;

	private Transform playArea;

	private Vector3 OldVelocity;

	public LayerMask layerMask;

	private Rigidbody heldBody;

	private Rigidbody wreckage;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		audioSource = GetComponent<AudioSource>();
		rigidbody = GetComponent<Rigidbody>();
		playArea = VRTK_DeviceFinder.PlayAreaTransform();
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		if (VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
		{
			holdControl = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>();
			grab = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractGrab>();
			if ((bool)ArmUIManager.instance && !ArmUIManager.instance.gripSwap)
			{
				VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
				VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
			}
			GunRadial.instance.OnGrab(VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_ControllerEvents>());
			interact.allowedTouchControllers = VRTK_InteractableObject.AllowedController.LeftOnly;
		}
		else
		{
			holdControl = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>();
			grab = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractGrab>();
			if ((bool)ArmUIManager.instance && !ArmUIManager.instance.gripSwap)
			{
				VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
				VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
			}
			GunRadial.instance.OnGrab(VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_ControllerEvents>());
			interact.allowedTouchControllers = VRTK_InteractableObject.AllowedController.RightOnly;
		}
		holdControl.TriggerPressed += OnTriggerPressed;
		holdControl.TriggerReleased += OnTriggerReleased;
		holdControl.TriggerUnclicked += OnTriggerUnclicked;
		holdControl.GetComponentInParent<HandController>().anim.SetBool("Gun", true);
		Invoke("Cooldown", 1f);
	}

	private void OnTriggerPressed(object sender, ControllerInteractionEventArgs e)
	{
		RaycastHit hitInfo;
		if (!Physics.Raycast(new Ray(barrel.position, barrel.forward), out hitInfo, 20f, layerMask))
		{
			return;
		}
		Rigidbody componentInParent = hitInfo.collider.GetComponentInParent<Rigidbody>();
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent.isKinematic || (bool)componentInParent.GetComponent<Wreckage>())
		{
			if ((bool)componentInParent.GetComponent<Wreckage>())
			{
				wreckage = componentInParent;
				grabOffset = componentInParent.position - barrel.position;
				hitOffset = hitInfo.point - componentInParent.position;
				areaOffset = componentInParent.position - playArea.position;
			}
			grabProxy.SetActive(true);
			grabProxy.transform.SetParent(null);
			grabProxy.transform.position = hitInfo.point;
			OnLock();
		}
		else
		{
			OnPickup(hitInfo.collider);
		}
	}

	private void OnLock()
	{
		startPlayAreaWorldOffset = playArea.transform.position - barrel.position;
		grabPointWorldPosition = barrel.position;
		flinging = true;
		beam.SetActive(true);
	}

	private void OnPickup(Collider col)
	{
		snapPoint.position = col.bounds.center;
		heldBody = col.GetComponentInParent<Rigidbody>();
		beam.SetActive(true);
	}

	private void FixedUpdate()
	{
		if ((bool)heldBody)
		{
			grabProxy.transform.position = heldBody.transform.position;
			heldBody.velocity = (snapPoint.transform.position - heldBody.transform.position) * 500f * Time.deltaTime;
		}
		if (flinging)
		{
			if ((bool)wreckage)
			{
				grabPointWorldPosition = wreckage.transform.position - grabOffset;
			}
			if ((bool)wreckage)
			{
				grabProxy.transform.position = wreckage.transform.position + hitOffset;
			}
			Vector3 vector = barrel.position - grabPointWorldPosition;
			Vector3 velocity = (grabPointWorldPosition + startPlayAreaWorldOffset - vector - playArea.position) * 1200f * Time.deltaTime;
			VRTK_BodyPhysics.instance.ApplyBodyVelocity(velocity, true, true);
		}
	}

	private void OnTriggerReleased(object sender, ControllerInteractionEventArgs e)
	{
		OnRelease();
	}

	private void OnTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
	{
		OnRelease();
	}

	private void OnRelease()
	{
		flinging = false;
		beam.SetActive(false);
		heldBody = null;
		wreckage = null;
		grabProxy.SetActive(false);
	}

	private void Cooldown()
	{
	}
}
