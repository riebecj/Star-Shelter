using UnityEngine;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

namespace VRTK
{
	public class VRTK_Lever : VRTK_Control
	{
		public enum LeverDirection
		{
			x = 0,
			y = 1,
			z = 2
		}

		[Tooltip("An optional game object to which the lever will be connected. If the game object moves the lever will follow along.")]
		public GameObject connectedTo;

		[Tooltip("The axis on which the lever should rotate. All other axis will be frozen.")]
		public LeverDirection direction = LeverDirection.y;

		[Tooltip("The minimum angle of the lever counted from its initial position.")]
		public float minAngle;

		[Tooltip("The maximum angle of the lever counted from its initial position.")]
		public float maxAngle = 130f;

		[Tooltip("The increments in which lever values can change.")]
		public float stepSize = 1f;

		[Tooltip("The amount of friction the lever will have whilst swinging when it is not grabbed.")]
		public float releasedFriction = 30f;

		[Tooltip("The amount of friction the lever will have whilst swinging when it is grabbed.")]
		public float grabbedFriction = 60f;

		protected HingeJoint leverHingeJoint;

		protected bool leverHingeJointCreated;

		protected Rigidbody leverRigidbody;

		protected override void InitRequiredComponents()
		{
			if (GetComponentInChildren<Collider>() == null)
			{
				VRTK_SharedMethods.CreateColliders(base.gameObject);
			}
			InitRigidbody();
			InitInteractableObject();
			InitHingeJoint();
		}

		protected override bool DetectSetup()
		{
			if (leverHingeJointCreated)
			{
				Bounds bounds = VRTK_SharedMethods.GetBounds(base.transform, base.transform);
				switch (direction)
				{
				case LeverDirection.x:
					leverHingeJoint.anchor = ((!(bounds.extents.y > bounds.extents.z)) ? new Vector3(0f, 0f, bounds.extents.z / base.transform.lossyScale.z) : new Vector3(0f, bounds.extents.y / base.transform.lossyScale.y, 0f));
					break;
				case LeverDirection.y:
					leverHingeJoint.axis = new Vector3(0f, 1f, 0f);
					leverHingeJoint.anchor = ((!(bounds.extents.x > bounds.extents.z)) ? new Vector3(0f, 0f, bounds.extents.z / base.transform.lossyScale.z) : new Vector3(bounds.extents.x / base.transform.lossyScale.x, 0f, 0f));
					break;
				case LeverDirection.z:
					leverHingeJoint.axis = new Vector3(0f, 0f, 1f);
					leverHingeJoint.anchor = ((!(bounds.extents.y > bounds.extents.x)) ? new Vector3(bounds.extents.x / base.transform.lossyScale.x, 0f) : new Vector3(0f, bounds.extents.y / base.transform.lossyScale.y, 0f));
					break;
				}
				leverHingeJoint.anchor *= -1f;
			}
			if ((bool)leverHingeJoint)
			{
				leverHingeJoint.useLimits = true;
				JointLimits limits = leverHingeJoint.limits;
				limits.min = minAngle;
				limits.max = maxAngle;
				leverHingeJoint.limits = limits;
				if ((bool)connectedTo)
				{
					leverHingeJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
				}
			}
			return true;
		}

		protected override ControlValueRange RegisterValueRange()
		{
			ControlValueRange result = default(ControlValueRange);
			result.controlMin = minAngle;
			result.controlMax = maxAngle;
			return result;
		}

		protected override void HandleUpdate()
		{
			value = CalculateValue();
			SnapToValue(value);
		}

		protected virtual void InitRigidbody()
		{
			leverRigidbody = GetComponent<Rigidbody>();
			if (leverRigidbody == null)
			{
				leverRigidbody = base.gameObject.AddComponent<Rigidbody>();
				leverRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				leverRigidbody.angularDrag = releasedFriction;
			}
			leverRigidbody.isKinematic = false;
			leverRigidbody.useGravity = false;
		}

		protected virtual void InitInteractableObject()
		{
			VRTK_InteractableObject vRTK_InteractableObject = GetComponent<VRTK_InteractableObject>();
			if (vRTK_InteractableObject == null)
			{
				vRTK_InteractableObject = base.gameObject.AddComponent<VRTK_InteractableObject>();
			}
			vRTK_InteractableObject.isGrabbable = true;
			vRTK_InteractableObject.grabAttachMechanicScript = base.gameObject.AddComponent<VRTK_RotatorTrackGrabAttach>();
			vRTK_InteractableObject.secondaryGrabActionScript = base.gameObject.AddComponent<VRTK_SwapControllerGrabAction>();
			vRTK_InteractableObject.grabAttachMechanicScript.precisionGrab = true;
			vRTK_InteractableObject.stayGrabbedOnTeleport = false;
			vRTK_InteractableObject.InteractableObjectGrabbed += InteractableObjectGrabbed;
			vRTK_InteractableObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
		}

		protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
		{
			leverRigidbody.angularDrag = grabbedFriction;
		}

		protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
		{
			leverRigidbody.angularDrag = releasedFriction;
		}

		protected virtual void InitHingeJoint()
		{
			leverHingeJoint = GetComponent<HingeJoint>();
			if (leverHingeJoint == null)
			{
				leverHingeJoint = base.gameObject.AddComponent<HingeJoint>();
				leverHingeJointCreated = true;
			}
			if ((bool)connectedTo)
			{
				Rigidbody rigidbody = connectedTo.GetComponent<Rigidbody>();
				if (rigidbody == null)
				{
					rigidbody = connectedTo.AddComponent<Rigidbody>();
				}
				rigidbody.useGravity = false;
			}
		}

		protected virtual float CalculateValue()
		{
			return Mathf.Round(leverHingeJoint.angle / stepSize) * stepSize;
		}

		protected virtual void SnapToValue(float value)
		{
			float num = (value - minAngle) / (maxAngle - minAngle) * (leverHingeJoint.limits.max - leverHingeJoint.limits.min);
			JointLimits limits = leverHingeJoint.limits;
			JointLimits limits2 = leverHingeJoint.limits;
			limits2.min = num;
			limits2.max = num;
			leverHingeJoint.limits = limits2;
			leverHingeJoint.limits = limits;
		}
	}
}
