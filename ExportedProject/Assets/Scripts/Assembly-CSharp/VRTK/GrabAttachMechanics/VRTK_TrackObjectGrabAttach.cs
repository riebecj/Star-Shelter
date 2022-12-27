using UnityEngine;

namespace VRTK.GrabAttachMechanics
{
	[RequireComponent(typeof(VRTK_InteractableObject))]
	public class VRTK_TrackObjectGrabAttach : VRTK_BaseGrabAttach
	{
		[Header("Track Options", order = 2)]
		[Tooltip("The maximum distance the grabbing controller is away from the object before it is automatically dropped.")]
		public float detachDistance = 1f;

		[Tooltip("The maximum amount of velocity magnitude that can be applied to the object. Lowering this can prevent physics glitches if objects are moving too fast.")]
		public float velocityLimit = float.PositiveInfinity;

		[Tooltip("The maximum amount of angular velocity magnitude that can be applied to the object. Lowering this can prevent physics glitches if objects are moving too fast.")]
		public float angularVelocityLimit = float.PositiveInfinity;

		protected bool isReleasable = true;

		public override void StopGrab(bool applyGrabbingObjectVelocity)
		{
			if (isReleasable)
			{
				ReleaseObject(applyGrabbingObjectVelocity);
			}
			base.StopGrab(applyGrabbingObjectVelocity);
		}

		public override Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)
		{
			Transform transform = null;
			if (precisionGrab)
			{
				transform = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, currentGrabbedObject.name, "TrackObject", "PrecisionSnap", "AttachPoint")).transform;
				transform.parent = currentGrabbingObject.transform;
				SetTrackPointOrientation(ref transform, currentGrabbedObject.transform, controllerPoint);
				customTrackPoint = true;
			}
			else
			{
				transform = base.CreateTrackPoint(controllerPoint, currentGrabbedObject, currentGrabbingObject, ref customTrackPoint);
			}
			return transform;
		}

		public override void ProcessUpdate()
		{
			if ((bool)trackPoint && grabbedObjectScript.IsDroppable())
			{
				float num = Vector3.Distance(trackPoint.position, initialAttachPoint.position);
				if (num > detachDistance)
				{
					ForceReleaseGrab();
				}
			}
		}

		public override void ProcessFixedUpdate()
		{
			if (!grabbedObject)
			{
				return;
			}
			float maxDistanceDelta = 10f;
			Quaternion quaternion;
			Vector3 vector;
			if (grabbedSnapHandle != null)
			{
				quaternion = trackPoint.rotation * Quaternion.Inverse(grabbedSnapHandle.rotation);
				vector = trackPoint.position - grabbedSnapHandle.position;
			}
			else
			{
				quaternion = trackPoint.rotation * Quaternion.Inverse(grabbedObject.transform.rotation);
				vector = trackPoint.position - grabbedObject.transform.position;
			}
			Vector3 axis;
			float angle;
			quaternion.ToAngleAxis(out angle, out axis);
			angle = ((!(angle > 180f)) ? angle : (angle -= 360f));
			if (angle != 0f)
			{
				Vector3 target = angle * axis;
				Vector3 angularVelocity = Vector3.MoveTowards(grabbedObjectRigidBody.angularVelocity, target, maxDistanceDelta);
				if (angularVelocityLimit == float.PositiveInfinity || angularVelocity.sqrMagnitude < angularVelocityLimit)
				{
					grabbedObjectRigidBody.angularVelocity = angularVelocity;
				}
			}
			Vector3 target2 = vector / Time.fixedDeltaTime;
			Vector3 velocity = Vector3.MoveTowards(grabbedObjectRigidBody.velocity, target2, maxDistanceDelta);
			if (velocityLimit == float.PositiveInfinity || velocity.sqrMagnitude < velocityLimit)
			{
				grabbedObjectRigidBody.velocity = velocity;
			}
		}

		protected override void Initialise()
		{
			tracked = true;
			climbable = false;
			kinematic = false;
			FlipSnapHandles();
		}

		protected virtual void SetTrackPointOrientation(ref Transform trackPoint, Transform currentGrabbedObject, Transform controllerPoint)
		{
			trackPoint.position = currentGrabbedObject.position;
			trackPoint.rotation = currentGrabbedObject.rotation;
		}
	}
}
