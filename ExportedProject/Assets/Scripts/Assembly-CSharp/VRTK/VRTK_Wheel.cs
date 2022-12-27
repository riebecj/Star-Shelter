using UnityEngine;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

namespace VRTK
{
	public class VRTK_Wheel : VRTK_Control
	{
		public enum GrabTypes
		{
			TrackObject = 0,
			RotatorTrack = 1
		}

		[Tooltip("An optional game object to which the wheel will be connected. If the game object moves the wheel will follow along.")]
		public GameObject connectedTo;

		[Tooltip("The grab attach mechanic to use. Track Object allows for rotations of the controller, Rotator Track allows for grabbing the wheel and spinning it.")]
		public GrabTypes grabType;

		[Tooltip("The maximum distance the grabbing controller is away from the wheel before it is automatically released.")]
		public float detatchDistance = 0.5f;

		[Tooltip("The minimum value the wheel can be set to.")]
		public float minimumValue;

		[Tooltip("The maximum value the wheel can be set to.")]
		public float maximumValue = 10f;

		[Tooltip("The increments in which values can change.")]
		public float stepSize = 1f;

		[Tooltip("If this is checked then when the wheel is released, it will snap to the step rotation.")]
		public bool snapToStep;

		[Tooltip("The amount of friction the wheel will have when it is grabbed.")]
		public float grabbedFriction = 25f;

		[Tooltip("The amount of friction the wheel will have when it is released.")]
		public float releasedFriction = 10f;

		[Range(0f, 359f)]
		[Tooltip("The maximum angle the wheel has to be turned to reach it's maximum value.")]
		public float maxAngle = 359f;

		[Tooltip("If this is checked then the wheel cannot be turned beyond the minimum and maximum value.")]
		public bool lockAtLimits;

		protected float angularVelocityLimit = 150f;

		protected float springStrengthValue = 150f;

		protected float springDamperValue = 5f;

		protected Quaternion initialLocalRotation;

		protected Rigidbody wheelRigidbody;

		protected HingeJoint wheelHinge;

		protected bool wheelHingeCreated;

		protected bool initialValueCalculated;

		protected float springAngle;

		protected override void InitRequiredComponents()
		{
			initialLocalRotation = base.transform.localRotation;
			InitWheel();
		}

		protected override bool DetectSetup()
		{
			if (wheelHingeCreated)
			{
				wheelHinge.anchor = Vector3.up;
				wheelHinge.axis = Vector3.up;
				if ((bool)connectedTo)
				{
					wheelHinge.connectedBody = connectedTo.GetComponent<Rigidbody>();
				}
			}
			return true;
		}

		protected override ControlValueRange RegisterValueRange()
		{
			ControlValueRange result = default(ControlValueRange);
			result.controlMin = minimumValue;
			result.controlMax = maximumValue;
			return result;
		}

		protected override void HandleUpdate()
		{
			CalculateValue();
			if (lockAtLimits && !initialValueCalculated)
			{
				base.transform.localRotation = initialLocalRotation;
				initialValueCalculated = true;
			}
		}

		protected virtual void InitWheel()
		{
			SetupRigidbody();
			SetupHinge();
			SetupInteractableObject();
		}

		protected virtual void SetupRigidbody()
		{
			wheelRigidbody = GetComponent<Rigidbody>();
			if (wheelRigidbody == null)
			{
				wheelRigidbody = base.gameObject.AddComponent<Rigidbody>();
				wheelRigidbody.angularDrag = releasedFriction;
			}
			wheelRigidbody.isKinematic = false;
			wheelRigidbody.useGravity = false;
			if ((bool)connectedTo)
			{
				Rigidbody component = connectedTo.GetComponent<Rigidbody>();
				if (component == null)
				{
					component = connectedTo.AddComponent<Rigidbody>();
					component.useGravity = false;
					component.isKinematic = true;
				}
			}
		}

		protected virtual void SetupHinge()
		{
			wheelHinge = GetComponent<HingeJoint>();
			if (wheelHinge == null)
			{
				wheelHinge = base.gameObject.AddComponent<HingeJoint>();
				wheelHingeCreated = true;
			}
			SetupHingeRestrictions();
		}

		protected virtual void SetupHingeRestrictions()
		{
			float num = 0f;
			float max = maxAngle;
			float num2 = maxAngle - 180f;
			if (num2 > 0f)
			{
				num -= num2;
				max = 180f;
			}
			if (lockAtLimits)
			{
				wheelHinge.useLimits = true;
				JointLimits limits = default(JointLimits);
				limits.min = num;
				limits.max = max;
				wheelHinge.limits = limits;
				Vector3 localEulerAngles = base.transform.localEulerAngles;
				switch (Mathf.RoundToInt(initialLocalRotation.eulerAngles.z))
				{
				case 0:
					localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y - num, base.transform.localEulerAngles.z);
					break;
				case 90:
					localEulerAngles = new Vector3(base.transform.localEulerAngles.x + num, base.transform.localEulerAngles.y, base.transform.localEulerAngles.z);
					break;
				case 180:
					localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y + num, base.transform.localEulerAngles.z);
					break;
				}
				base.transform.localEulerAngles = localEulerAngles;
				initialValueCalculated = false;
			}
		}

		protected virtual void ConfigureHingeSpring()
		{
			JointSpring spring = default(JointSpring);
			spring.spring = springStrengthValue;
			spring.damper = springDamperValue;
			spring.targetPosition = springAngle + wheelHinge.limits.min;
			wheelHinge.spring = spring;
		}

		protected virtual void SetupInteractableObject()
		{
			VRTK_InteractableObject vRTK_InteractableObject = GetComponent<VRTK_InteractableObject>();
			if (vRTK_InteractableObject == null)
			{
				vRTK_InteractableObject = base.gameObject.AddComponent<VRTK_InteractableObject>();
			}
			vRTK_InteractableObject.isGrabbable = true;
			VRTK_TrackObjectGrabAttach vRTK_TrackObjectGrabAttach;
			if (grabType == GrabTypes.TrackObject)
			{
				vRTK_TrackObjectGrabAttach = base.gameObject.AddComponent<VRTK_TrackObjectGrabAttach>();
				if (lockAtLimits)
				{
					vRTK_TrackObjectGrabAttach.velocityLimit = 0f;
					vRTK_TrackObjectGrabAttach.angularVelocityLimit = angularVelocityLimit;
				}
			}
			else
			{
				vRTK_TrackObjectGrabAttach = base.gameObject.AddComponent<VRTK_RotatorTrackGrabAttach>();
			}
			vRTK_TrackObjectGrabAttach.precisionGrab = true;
			vRTK_TrackObjectGrabAttach.detachDistance = detatchDistance;
			vRTK_InteractableObject.grabAttachMechanicScript = vRTK_TrackObjectGrabAttach;
			vRTK_InteractableObject.secondaryGrabActionScript = base.gameObject.AddComponent<VRTK_SwapControllerGrabAction>();
			vRTK_InteractableObject.stayGrabbedOnTeleport = false;
			vRTK_InteractableObject.InteractableObjectGrabbed += WheelInteractableObjectGrabbed;
			vRTK_InteractableObject.InteractableObjectUngrabbed += WheelInteractableObjectUngrabbed;
		}

		protected virtual void WheelInteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
		{
			wheelRigidbody.angularDrag = grabbedFriction;
			wheelHinge.useSpring = false;
		}

		protected virtual void WheelInteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
		{
			wheelRigidbody.angularDrag = releasedFriction;
			if (snapToStep)
			{
				wheelHinge.useSpring = true;
				ConfigureHingeSpring();
			}
		}

		protected virtual void CalculateValue()
		{
			ControlValueRange controlValueRange = RegisterValueRange();
			float angle;
			Vector3 axis;
			(base.transform.localRotation * Quaternion.Inverse(initialLocalRotation)).ToAngleAxis(out angle, out axis);
			float num = Mathf.Round((controlValueRange.controlMin + Mathf.Clamp01(angle / maxAngle) * (controlValueRange.controlMax - controlValueRange.controlMin)) / stepSize) * stepSize;
			float num2 = num - controlValueRange.controlMin;
			float num3 = controlValueRange.controlMax - controlValueRange.controlMin;
			springAngle = num2 / num3 * maxAngle;
			value = num;
		}
	}
}
