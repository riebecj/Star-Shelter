using UnityEngine;

namespace VRTK.GrabAttachMechanics
{
	public class VRTK_ChildOfControllerGrabAttach : VRTK_BaseGrabAttach
	{
		public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
		{
			if (base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint))
			{
				SnapObjectToGrabToController(givenGrabbedObject);
				grabbedObjectScript.isKinematic = true;
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
			kinematic = true;
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
			obj.transform.SetParent(controllerAttachPoint.transform);
		}
	}
}
