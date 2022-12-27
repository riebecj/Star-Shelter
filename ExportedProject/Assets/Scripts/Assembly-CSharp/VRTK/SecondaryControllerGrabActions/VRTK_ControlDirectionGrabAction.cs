using System.Collections;
using UnityEngine;

namespace VRTK.SecondaryControllerGrabActions
{
	public class VRTK_ControlDirectionGrabAction : VRTK_BaseGrabAction
	{
		[Tooltip("The distance the secondary controller must move away from the original grab position before the secondary controller auto ungrabs the object.")]
		public float ungrabDistance = 1f;

		[Tooltip("The speed in which the object will snap back to it's original rotation when the secondary controller stops grabbing it. `0` for instant snap, `infinity` for no snap back.")]
		public float releaseSnapSpeed = 0.1f;

		[Tooltip("Prevent the secondary controller rotating the grabbed object through it's z-axis.")]
		public bool lockZRotation = true;

		protected Vector3 initialPosition;

		protected Quaternion initialRotation;

		protected Quaternion releaseRotation;

		protected Coroutine snappingOnRelease;

		public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
		{
			base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
			initialPosition = currentGrabbdObject.transform.localPosition;
			initialRotation = currentGrabbdObject.transform.localRotation;
			StopRealignOnRelease();
		}

		public override void ResetAction()
		{
			releaseRotation = base.transform.localRotation;
			if (!grabbedObject.grabAttachMechanicScript.precisionGrab)
			{
				if (releaseSnapSpeed < float.MaxValue && releaseSnapSpeed > 0f)
				{
					snappingOnRelease = StartCoroutine(RealignOnRelease());
				}
				else if (releaseSnapSpeed == 0f)
				{
					base.transform.localRotation = initialRotation;
				}
			}
			base.ResetAction();
		}

		public override void OnDropAction()
		{
			base.OnDropAction();
			StopRealignOnRelease();
		}

		public override void ProcessUpdate()
		{
			base.ProcessUpdate();
			CheckForceStopDistance(ungrabDistance);
		}

		public override void ProcessFixedUpdate()
		{
			base.ProcessFixedUpdate();
			if (initialised)
			{
				AimObject();
			}
		}

		protected virtual void StopRealignOnRelease()
		{
			if (snappingOnRelease != null)
			{
				StopCoroutine(snappingOnRelease);
			}
			snappingOnRelease = null;
		}

		protected virtual IEnumerator RealignOnRelease()
		{
			float elapsedTime = 0f;
			while (elapsedTime < releaseSnapSpeed)
			{
				base.transform.localRotation = Quaternion.Lerp(releaseRotation, initialRotation, elapsedTime / releaseSnapSpeed);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			base.transform.localRotation = initialRotation;
			base.transform.localPosition = initialPosition;
		}

		protected virtual void AimObject()
		{
			if (lockZRotation)
			{
				ZLockedAim();
			}
			else
			{
				base.transform.rotation = Quaternion.LookRotation(secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position, secondaryGrabbingObject.transform.TransformDirection(Vector3.forward));
			}
			if (grabbedObject.grabAttachMechanicScript.precisionGrab)
			{
				base.transform.Translate(primaryGrabbingObject.controllerAttachPoint.transform.position - primaryInitialGrabPoint.position, Space.World);
			}
		}

		protected virtual void ZLockedAim()
		{
			Vector3 normalized = (secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position).normalized;
			Quaternion quaternion = Quaternion.LookRotation(normalized, Vector3.Cross(-primaryGrabbingObject.transform.right, normalized).normalized);
			Vector3 axis;
			float angle;
			(Quaternion.Inverse(grabbedObject.transform.rotation) * quaternion).ToAngleAxis(out angle, out axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			angle = Mathf.Abs(angle);
			Quaternion quaternion2 = Quaternion.LookRotation(normalized, primaryGrabbingObject.transform.forward);
			Vector3 axis2;
			float angle2;
			(Quaternion.Inverse(grabbedObject.transform.rotation) * quaternion2).ToAngleAxis(out angle2, out axis2);
			if (angle2 > 180f)
			{
				angle2 -= 360f;
			}
			angle2 = Mathf.Abs(angle2);
			grabbedObject.transform.rotation = ((!(angle2 < angle)) ? quaternion : quaternion2);
		}
	}
}
