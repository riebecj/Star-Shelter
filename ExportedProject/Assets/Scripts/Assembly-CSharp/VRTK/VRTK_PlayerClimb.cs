using UnityEngine;

namespace VRTK
{
	[RequireComponent(typeof(VRTK_BodyPhysics))]
	public class VRTK_PlayerClimb : MonoBehaviour
	{
		[Tooltip("Will scale movement up and down based on the player transform's scale.")]
		public bool usePlayerScale = true;

		protected Transform playArea;

		protected Vector3 startControllerScaledLocalPosition;

		protected Vector3 startGrabPointLocalPosition;

		protected Vector3 startPlayAreaWorldOffset;

		protected GameObject grabbingController;

		protected GameObject climbingObject;

		protected Quaternion climbingObjectLastRotation;

		protected VRTK_BodyPhysics bodyPhysics;

		protected bool isClimbing;

		protected bool useGrabbedObjectRotation;

		public bool movingMode;

		internal bool disabled;

		public static VRTK_PlayerClimb instance;

		public event PlayerClimbEventHandler PlayerClimbStarted;

		public event PlayerClimbEventHandler PlayerClimbEnded;

		protected virtual void Awake()
		{
			playArea = VRTK_DeviceFinder.PlayAreaTransform();
			bodyPhysics = GetComponent<VRTK_BodyPhysics>();
			instance = this;
		}

		protected virtual void OnEnable()
		{
			InitListeners(true);
		}

		protected virtual void OnDisable()
		{
			Ungrab(false, 0u, climbingObject);
			InitListeners(false);
		}

		protected virtual void FixedUpdate()
		{
			if (isClimbing && !disabled && (bool)climbingObject)
			{
				Vector3 vector = GetScaledLocalPosition(grabbingController.transform) - startControllerScaledLocalPosition;
				Vector3 vector2 = climbingObject.transform.TransformPoint(startGrabPointLocalPosition);
				if (movingMode)
				{
					playArea.position = Vector3.Lerp(playArea.position, vector2 + startPlayAreaWorldOffset - vector, 100f * Time.deltaTime);
					return;
				}
				Vector3 velocity = (vector2 + startPlayAreaWorldOffset - vector - playArea.position) * 5000f * Time.deltaTime;
				bodyPhysics.ApplyBodyVelocity(velocity, true, true);
			}
		}

		protected virtual void OnPlayerClimbStarted(PlayerClimbEventArgs e)
		{
			if (this.PlayerClimbStarted != null)
			{
				this.PlayerClimbStarted(this, e);
			}
		}

		protected virtual void OnPlayerClimbEnded(PlayerClimbEventArgs e)
		{
			if (this.PlayerClimbEnded != null)
			{
				this.PlayerClimbEnded(this, e);
			}
		}

		protected virtual PlayerClimbEventArgs SetPlayerClimbEvent(uint controllerIndex, GameObject target)
		{
			PlayerClimbEventArgs result = default(PlayerClimbEventArgs);
			result.controllerIndex = controllerIndex;
			result.target = target;
			return result;
		}

		protected virtual void InitListeners(bool state)
		{
			InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), state);
			InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), state);
			InitTeleportListener(state);
		}

		protected virtual void InitTeleportListener(bool state)
		{
			VRTK_BasicTeleport component = GetComponent<VRTK_BasicTeleport>();
			if ((bool)component)
			{
				if (state)
				{
					component.Teleporting += OnTeleport;
				}
				else
				{
					component.Teleporting -= OnTeleport;
				}
			}
		}

		protected virtual void OnTeleport(object sender, DestinationMarkerEventArgs e)
		{
			Ungrab(false, e.controllerIndex, e.target.gameObject);
		}

		protected virtual Vector3 GetScaledLocalPosition(Transform objTransform)
		{
			if (usePlayerScale)
			{
				return playArea.localRotation * Vector3.Scale(objTransform.localPosition, playArea.localScale);
			}
			return playArea.localRotation * objTransform.localPosition;
		}

		protected virtual void OnGrabObject(object sender, ObjectInteractEventArgs e)
		{
			if (IsClimbableObject(e.target))
			{
				GameObject givenController = ((VRTK_InteractGrab)sender).gameObject;
				GameObject actualController = VRTK_DeviceFinder.GetActualController(givenController);
				Grab(actualController, e.controllerIndex, e.target);
				if ((bool)IntroManager.instance)
				{
					IntroManager.instance.ClimbComplete();
				}
			}
		}

		protected virtual void OnUngrabObject(object sender, ObjectInteractEventArgs e)
		{
			GameObject givenController = ((VRTK_InteractGrab)sender).gameObject;
			GameObject actualController = VRTK_DeviceFinder.GetActualController(givenController);
			if ((bool)e.target && IsClimbableObject(e.target) && IsActiveClimbingController(actualController))
			{
				Ungrab(true, e.controllerIndex, e.target);
			}
		}

		protected virtual void Grab(GameObject currentGrabbingController, uint controllerIndex, GameObject target)
		{
			isClimbing = true;
			climbingObject = target;
			grabbingController = currentGrabbingController;
			startControllerScaledLocalPosition = GetScaledLocalPosition(grabbingController.transform);
			startGrabPointLocalPosition = climbingObject.transform.InverseTransformPoint(grabbingController.transform.position);
			startPlayAreaWorldOffset = playArea.transform.position - grabbingController.transform.position;
			OnPlayerClimbStarted(SetPlayerClimbEvent(controllerIndex, climbingObject));
			if ((bool)target.GetComponentInParent<Rigidbody>() && (bool)target.GetComponentInParent<Wreckage>())
			{
				movingMode = true;
			}
			GameManager.instance.CamRig.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}

		protected virtual void Ungrab(bool carryMomentum, uint controllerIndex, GameObject target)
		{
			if (carryMomentum && !disabled)
			{
				Vector3 velocity = Vector3.zero;
				GameObject controllerByIndex = VRTK_DeviceFinder.GetControllerByIndex(controllerIndex, false);
				if ((bool)controllerByIndex)
				{
					velocity = -VRTK_DeviceFinder.GetControllerVelocity(controllerByIndex);
					velocity = ((!usePlayerScale) ? playArea.TransformDirection(velocity) : playArea.TransformVector(velocity));
				}
				if ((bool)target.GetComponentInParent<Rigidbody>() && (bool)target.GetComponentInParent<Wreckage>())
				{
					velocity += target.GetComponentInParent<Rigidbody>().velocity;
				}
				bodyPhysics.ApplyBodyVelocity(velocity, true, true);
			}
			isClimbing = false;
			grabbingController = null;
			climbingObject = null;
			OnPlayerClimbEnded(SetPlayerClimbEvent(controllerIndex, target));
		}

		protected virtual bool IsActiveClimbingController(GameObject controller)
		{
			return controller == grabbingController;
		}

		protected virtual bool IsClimbableObject(GameObject obj)
		{
			if (!obj)
			{
				return false;
			}
			VRTK_InteractableObject component = obj.GetComponent<VRTK_InteractableObject>();
			return (bool)component && (bool)component.grabAttachMechanicScript && component.grabAttachMechanicScript.IsClimbable();
		}

		protected virtual void InitControllerListeners(GameObject controller, bool state)
		{
			if (!controller)
			{
				return;
			}
			VRTK_InteractGrab component = controller.GetComponent<VRTK_InteractGrab>();
			if ((bool)component)
			{
				if (state)
				{
					component.ControllerGrabInteractableObject += OnGrabObject;
					component.ControllerUngrabInteractableObject += OnUngrabObject;
				}
				else
				{
					component.ControllerGrabInteractableObject -= OnGrabObject;
					component.ControllerUngrabInteractableObject -= OnUngrabObject;
				}
			}
		}
	}
}
