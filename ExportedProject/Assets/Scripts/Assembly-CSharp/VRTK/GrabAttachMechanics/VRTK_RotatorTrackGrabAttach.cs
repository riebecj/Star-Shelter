using UnityEngine;

namespace VRTK.GrabAttachMechanics
{
	[RequireComponent(typeof(VRTK_InteractableObject))]
	public class VRTK_RotatorTrackGrabAttach : VRTK_TrackObjectGrabAttach
	{
		public override void StopGrab(bool applyGrabbingObjectVelocity)
		{
			isReleasable = false;
			base.StopGrab(applyGrabbingObjectVelocity);
		}

		public override void ProcessFixedUpdate()
		{
			Vector3 force = trackPoint.position - initialAttachPoint.position;
			grabbedObjectRigidBody.AddForceAtPosition(force, initialAttachPoint.position, ForceMode.VelocityChange);
		}

		protected override void SetTrackPointOrientation(ref Transform trackPoint, Transform currentGrabbedObject, Transform controllerPoint)
		{
			trackPoint.position = controllerPoint.position;
			trackPoint.rotation = controllerPoint.rotation;
		}
	}
}
