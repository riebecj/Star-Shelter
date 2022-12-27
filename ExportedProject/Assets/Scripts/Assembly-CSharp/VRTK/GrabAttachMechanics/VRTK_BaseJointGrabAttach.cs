using UnityEngine;

namespace VRTK.GrabAttachMechanics
{
	[RequireComponent(typeof(VRTK_InteractableObject))]
	public abstract class VRTK_BaseJointGrabAttach : VRTK_BaseGrabAttach
	{
		[Header("Joint Options", order = 2)]
		[Tooltip("Determines whether the joint should be destroyed immediately on release or whether to wait till the end of the frame before being destroyed.")]
		public bool destroyImmediatelyOnThrow = true;

		protected Joint givenJoint;

		protected Joint controllerAttachJoint;

		public override bool ValidGrab(Rigidbody checkAttachPoint)
		{
			return controllerAttachJoint == null || controllerAttachJoint.connectedBody != checkAttachPoint;
		}

		public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
		{
			if (base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint))
			{
				SnapObjectToGrabToController(givenGrabbedObject);
				return true;
			}
			return false;
		}

		public override void StopGrab(bool applyGrabbingObjectVelocity)
		{
			ReleaseObject(applyGrabbingObjectVelocity);
			base.StopGrab(applyGrabbingObjectVelocity);
		}

		protected override void Initialise()
		{
			tracked = false;
			climbable = false;
			kinematic = false;
			controllerAttachJoint = null;
		}

		protected override Rigidbody ReleaseFromController(bool applyGrabbingObjectVelocity)
		{
			if ((bool)controllerAttachJoint)
			{
				Rigidbody component = controllerAttachJoint.GetComponent<Rigidbody>();
				DestroyJoint(destroyImmediatelyOnThrow, applyGrabbingObjectVelocity);
				controllerAttachJoint = null;
				return component;
			}
			return null;
		}

		protected virtual void OnJointBreak(float force)
		{
			ForceReleaseGrab();
		}

		protected virtual void CreateJoint(GameObject obj)
		{
			if (precisionGrab)
			{
				givenJoint.anchor = obj.transform.InverseTransformPoint(controllerAttachPoint.position);
			}
			controllerAttachJoint = givenJoint;
			controllerAttachJoint.breakForce = ((!grabbedObjectScript.IsDroppable()) ? float.PositiveInfinity : controllerAttachJoint.breakForce);
			controllerAttachJoint.connectedBody = controllerAttachPoint;
		}

		protected virtual void DestroyJoint(bool withDestroyImmediate, bool applyGrabbingObjectVelocity)
		{
			controllerAttachJoint.connectedBody = null;
			if (withDestroyImmediate && applyGrabbingObjectVelocity)
			{
				Object.DestroyImmediate(controllerAttachJoint);
			}
			else
			{
				Object.Destroy(controllerAttachJoint);
			}
		}

		protected virtual void SetSnappedObjectPosition(GameObject obj)
		{
			if (grabbedSnapHandle == null)
			{
				obj.transform.position = controllerAttachPoint.transform.position;
				return;
			}
			obj.transform.rotation = controllerAttachPoint.transform.rotation * Quaternion.Euler(grabbedSnapHandle.transform.localEulerAngles);
			obj.transform.position = controllerAttachPoint.transform.position - (grabbedSnapHandle.transform.position - obj.transform.position);
		}

		protected virtual void SnapObjectToGrabToController(GameObject obj)
		{
			if (!precisionGrab)
			{
				SetSnappedObjectPosition(obj);
			}
			CreateJoint(obj);
		}
	}
}
