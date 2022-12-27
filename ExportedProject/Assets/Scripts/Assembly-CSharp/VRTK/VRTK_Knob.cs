using UnityEngine;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

namespace VRTK
{
	public class VRTK_Knob : VRTK_Control
	{
		public enum KnobDirection
		{
			x = 0,
			y = 1,
			z = 2
		}

		[Tooltip("An optional game object to which the knob will be connected. If the game object moves the knob will follow along.")]
		public GameObject connectedTo;

		[Tooltip("The axis on which the knob should rotate. All other axis will be frozen.")]
		public KnobDirection direction;

		[Tooltip("The minimum value of the knob.")]
		public float min;

		[Tooltip("The maximum value of the knob.")]
		public float max = 100f;

		[Tooltip("The increments in which knob values can change.")]
		public float stepSize = 1f;

		protected const float MAX_AUTODETECT_KNOB_WIDTH = 3f;

		protected KnobDirection finalDirection;

		protected KnobDirection subDirection;

		protected bool subDirectionFound;

		protected Quaternion initialRotation;

		protected Vector3 initialLocalRotation;

		protected ConfigurableJoint knobJoint;

		protected bool knobJointCreated;

		protected override void InitRequiredComponents()
		{
			initialRotation = base.transform.rotation;
			initialLocalRotation = base.transform.localRotation.eulerAngles;
			InitKnob();
		}

		protected override bool DetectSetup()
		{
			finalDirection = direction;
			if (knobJointCreated)
			{
				knobJoint.angularXMotion = ConfigurableJointMotion.Locked;
				knobJoint.angularYMotion = ConfigurableJointMotion.Locked;
				knobJoint.angularZMotion = ConfigurableJointMotion.Locked;
				switch (finalDirection)
				{
				case KnobDirection.x:
					knobJoint.angularXMotion = ConfigurableJointMotion.Free;
					break;
				case KnobDirection.y:
					knobJoint.angularYMotion = ConfigurableJointMotion.Free;
					break;
				case KnobDirection.z:
					knobJoint.angularZMotion = ConfigurableJointMotion.Free;
					break;
				}
			}
			if ((bool)knobJoint)
			{
				knobJoint.xMotion = ConfigurableJointMotion.Locked;
				knobJoint.yMotion = ConfigurableJointMotion.Locked;
				knobJoint.zMotion = ConfigurableJointMotion.Locked;
				if ((bool)connectedTo)
				{
					knobJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
				}
			}
			return true;
		}

		protected override ControlValueRange RegisterValueRange()
		{
			ControlValueRange result = default(ControlValueRange);
			result.controlMin = min;
			result.controlMax = max;
			return result;
		}

		protected override void HandleUpdate()
		{
			value = CalculateValue();
		}

		protected virtual void InitKnob()
		{
			Rigidbody rigidbody = GetComponent<Rigidbody>();
			if (rigidbody == null)
			{
				rigidbody = base.gameObject.AddComponent<Rigidbody>();
				rigidbody.angularDrag = 10f;
			}
			rigidbody.isKinematic = false;
			rigidbody.useGravity = false;
			VRTK_InteractableObject vRTK_InteractableObject = GetComponent<VRTK_InteractableObject>();
			if (vRTK_InteractableObject == null)
			{
				vRTK_InteractableObject = base.gameObject.AddComponent<VRTK_InteractableObject>();
			}
			vRTK_InteractableObject.isGrabbable = true;
			vRTK_InteractableObject.grabAttachMechanicScript = base.gameObject.AddComponent<VRTK_TrackObjectGrabAttach>();
			vRTK_InteractableObject.grabAttachMechanicScript.precisionGrab = true;
			vRTK_InteractableObject.secondaryGrabActionScript = base.gameObject.AddComponent<VRTK_SwapControllerGrabAction>();
			vRTK_InteractableObject.stayGrabbedOnTeleport = false;
			knobJoint = GetComponent<ConfigurableJoint>();
			if (knobJoint == null)
			{
				knobJoint = base.gameObject.AddComponent<ConfigurableJoint>();
				knobJoint.configuredInWorldSpace = false;
				knobJointCreated = true;
			}
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

		protected virtual KnobDirection DetectDirection()
		{
			KnobDirection result = KnobDirection.x;
			Bounds bounds = VRTK_SharedMethods.GetBounds(base.transform);
			RaycastHit hitInfo;
			Physics.Raycast(bounds.center, Vector3.forward, out hitInfo, bounds.extents.z * 3f, -5, QueryTriggerInteraction.UseGlobal);
			RaycastHit hitInfo2;
			Physics.Raycast(bounds.center, Vector3.back, out hitInfo2, bounds.extents.z * 3f, -5, QueryTriggerInteraction.UseGlobal);
			RaycastHit hitInfo3;
			Physics.Raycast(bounds.center, Vector3.left, out hitInfo3, bounds.extents.x * 3f, -5, QueryTriggerInteraction.UseGlobal);
			RaycastHit hitInfo4;
			Physics.Raycast(bounds.center, Vector3.right, out hitInfo4, bounds.extents.x * 3f, -5, QueryTriggerInteraction.UseGlobal);
			RaycastHit hitInfo5;
			Physics.Raycast(bounds.center, Vector3.up, out hitInfo5, bounds.extents.y * 3f, -5, QueryTriggerInteraction.UseGlobal);
			RaycastHit hitInfo6;
			Physics.Raycast(bounds.center, Vector3.down, out hitInfo6, bounds.extents.y * 3f, -5, QueryTriggerInteraction.UseGlobal);
			float num = ((!(hitInfo4.collider != null)) ? float.MaxValue : hitInfo4.distance);
			float num2 = ((!(hitInfo6.collider != null)) ? float.MaxValue : hitInfo6.distance);
			float num3 = ((!(hitInfo2.collider != null)) ? float.MaxValue : hitInfo2.distance);
			float num4 = ((!(hitInfo3.collider != null)) ? float.MaxValue : hitInfo3.distance);
			float num5 = ((!(hitInfo5.collider != null)) ? float.MaxValue : hitInfo5.distance);
			float num6 = ((!(hitInfo.collider != null)) ? float.MaxValue : hitInfo.distance);
			if (VRTK_SharedMethods.IsLowest(num, new float[5] { num2, num3, num4, num5, num6 }))
			{
				result = KnobDirection.z;
			}
			else if (VRTK_SharedMethods.IsLowest(num2, new float[5] { num, num3, num4, num5, num6 }))
			{
				result = KnobDirection.y;
			}
			else if (VRTK_SharedMethods.IsLowest(num3, new float[5] { num, num2, num4, num5, num6 }))
			{
				result = KnobDirection.x;
			}
			else if (VRTK_SharedMethods.IsLowest(num4, new float[5] { num, num2, num3, num5, num6 }))
			{
				result = KnobDirection.z;
			}
			else if (VRTK_SharedMethods.IsLowest(num5, new float[5] { num, num2, num3, num4, num6 }))
			{
				result = KnobDirection.y;
			}
			else if (VRTK_SharedMethods.IsLowest(num6, new float[5] { num, num2, num3, num4, num5 }))
			{
				result = KnobDirection.x;
			}
			return result;
		}

		protected virtual float CalculateValue()
		{
			if (!subDirectionFound)
			{
				float num = Mathf.Abs(base.transform.localRotation.eulerAngles.x - initialLocalRotation.x) % 90f;
				float num2 = Mathf.Abs(base.transform.localRotation.eulerAngles.y - initialLocalRotation.y) % 90f;
				float num3 = Mathf.Abs(base.transform.localRotation.eulerAngles.z - initialLocalRotation.z) % 90f;
				num = ((Mathf.RoundToInt(num) < 89) ? num : 0f);
				num2 = ((Mathf.RoundToInt(num2) < 89) ? num2 : 0f);
				num3 = ((Mathf.RoundToInt(num3) < 89) ? num3 : 0f);
				if (Mathf.RoundToInt(num) != 0 || Mathf.RoundToInt(num2) != 0 || Mathf.RoundToInt(num3) != 0)
				{
					subDirection = ((num < num2) ? ((!(num2 < num3)) ? KnobDirection.y : KnobDirection.z) : ((num < num3) ? KnobDirection.z : KnobDirection.x));
					subDirectionFound = true;
				}
			}
			float num4 = 0f;
			switch (subDirection)
			{
			case KnobDirection.x:
				num4 = base.transform.localRotation.eulerAngles.x - initialLocalRotation.x;
				break;
			case KnobDirection.y:
				num4 = base.transform.localRotation.eulerAngles.y - initialLocalRotation.y;
				break;
			case KnobDirection.z:
				num4 = base.transform.localRotation.eulerAngles.z - initialLocalRotation.z;
				break;
			}
			num4 = Mathf.Round(num4 * 1000f) / 1000f;
			float num5 = 0f;
			num5 = ((!(num4 > 0f) || !(num4 <= 180f)) ? Quaternion.Angle(initialRotation, base.transform.rotation) : (360f - Quaternion.Angle(initialRotation, base.transform.rotation)));
			num5 = Mathf.Round((min + Mathf.Clamp01(num5 / 360f) * (max - min)) / stepSize) * stepSize;
			if (min > max && num4 != 0f)
			{
				num5 = max + min - num5;
			}
			return num5;
		}
	}
}
