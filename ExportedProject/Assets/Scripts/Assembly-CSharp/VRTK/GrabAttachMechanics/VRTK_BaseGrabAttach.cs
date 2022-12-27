using UnityEngine;

namespace VRTK.GrabAttachMechanics
{
	public abstract class VRTK_BaseGrabAttach : MonoBehaviour
	{
		[Header("Base Options", order = 1)]
		[Tooltip("If this is checked then when the controller grabs the object, it will grab it with precision and pick it up at the particular point on the object the controller is touching.")]
		public bool precisionGrab = true;

		[Tooltip("A Transform provided as an empty game object which must be the child of the item being grabbed and serves as an orientation point to rotate and position the grabbed item in relation to the right handed controller. If no Right Snap Handle is provided but a Left Snap Handle is provided, then the Left Snap Handle will be used in place. If no Snap Handle is provided then the object will be grabbed at its central point. Not required for `Precision Snap`.")]
		public Transform rightSnapHandle;

		[Tooltip("A Transform provided as an empty game object which must be the child of the item being grabbed and serves as an orientation point to rotate and position the grabbed item in relation to the left handed controller. If no Left Snap Handle is provided but a Right Snap Handle is provided, then the Right Snap Handle will be used in place. If no Snap Handle is provided then the object will be grabbed at its central point. Not required for `Precision Snap`.")]
		public Transform leftSnapHandle;

		[Tooltip("If checked then when the object is thrown, the distance between the object's attach point and the controller's attach point will be used to calculate a faster throwing velocity.")]
		public bool throwVelocityWithAttachDistance;

		[Tooltip("An amount to multiply the velocity of the given object when it is thrown. This can also be used in conjunction with the Interact Grab Throw Multiplier to have certain objects be thrown even further than normal (or thrown a shorter distance if a number below 1 is entered).")]
		public float throwMultiplier = 1f;

		[Tooltip("The amount of time to delay collisions affecting the object when it is first grabbed. This is useful if a game object may get stuck inside another object when it is being grabbed.")]
		public float onGrabCollisionDelay;

		protected bool tracked;

		protected bool climbable;

		protected bool kinematic;

		protected GameObject grabbedObject;

		protected Rigidbody grabbedObjectRigidBody;

		protected VRTK_InteractableObject grabbedObjectScript;

		protected Transform trackPoint;

		protected Transform grabbedSnapHandle;

		protected Transform initialAttachPoint;

		protected Rigidbody controllerAttachPoint;

		public virtual bool IsTracked()
		{
			return tracked;
		}

		public virtual bool IsClimbable()
		{
			return climbable;
		}

		public virtual bool IsKinematic()
		{
			return kinematic;
		}

		public virtual bool ValidGrab(Rigidbody checkAttachPoint)
		{
			return true;
		}

		public virtual void SetTrackPoint(Transform givenTrackPoint)
		{
			trackPoint = givenTrackPoint;
		}

		public virtual void SetInitialAttachPoint(Transform givenInitialAttachPoint)
		{
			initialAttachPoint = givenInitialAttachPoint;
		}

		public virtual bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
		{
			grabbedObject = givenGrabbedObject;
			if (grabbedObject == null)
			{
				return false;
			}
			grabbedObjectScript = grabbedObject.GetComponent<VRTK_InteractableObject>();
			grabbedObjectRigidBody = grabbedObject.GetComponent<Rigidbody>();
			controllerAttachPoint = givenControllerAttachPoint;
			grabbedSnapHandle = GetSnapHandle(grabbingObject);
			grabbedObjectScript.PauseCollisions(onGrabCollisionDelay);
			return true;
		}

		public virtual void StopGrab(bool applyGrabbingObjectVelocity)
		{
			grabbedObject = null;
			grabbedObjectScript = null;
			trackPoint = null;
			grabbedSnapHandle = null;
			initialAttachPoint = null;
			controllerAttachPoint = null;
		}

		public virtual Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)
		{
			customTrackPoint = false;
			return controllerPoint;
		}

		public virtual void ProcessUpdate()
		{
		}

		public virtual void ProcessFixedUpdate()
		{
		}

		protected abstract void Initialise();

		protected virtual Rigidbody ReleaseFromController(bool applyGrabbingObjectVelocity)
		{
			return grabbedObjectRigidBody;
		}

		protected virtual void ForceReleaseGrab()
		{
			if ((bool)grabbedObjectScript)
			{
				GameObject grabbingObject = grabbedObjectScript.GetGrabbingObject();
				if ((bool)grabbingObject)
				{
					grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
				}
			}
		}

		protected virtual void ReleaseObject(bool applyGrabbingObjectVelocity)
		{
			Rigidbody rigidbody = ReleaseFromController(applyGrabbingObjectVelocity);
			if ((bool)rigidbody && applyGrabbingObjectVelocity)
			{
				ThrowReleasedObject(rigidbody);
			}
		}

		protected virtual void FlipSnapHandles()
		{
			FlipSnapHandle(rightSnapHandle);
			FlipSnapHandle(leftSnapHandle);
		}

		protected virtual void Awake()
		{
			Initialise();
		}

		protected virtual void ThrowReleasedObject(Rigidbody objectRigidbody)
		{
			if (!grabbedObjectScript)
			{
				return;
			}
			GameObject grabbingObject = grabbedObjectScript.GetGrabbingObject();
			if (!grabbingObject)
			{
				return;
			}
			VRTK_InteractGrab component = grabbingObject.GetComponent<VRTK_InteractGrab>();
			float num = component.throwMultiplier;
			Transform controllerOrigin = VRTK_DeviceFinder.GetControllerOrigin(grabbingObject);
			Vector3 controllerVelocity = VRTK_DeviceFinder.GetControllerVelocity(grabbingObject);
			Vector3 controllerAngularVelocity = VRTK_DeviceFinder.GetControllerAngularVelocity(grabbingObject);
			if (controllerOrigin != null)
			{
				objectRigidbody.velocity = controllerOrigin.TransformVector(controllerVelocity) * (num * throwMultiplier);
				objectRigidbody.angularVelocity = controllerOrigin.TransformDirection(controllerAngularVelocity);
			}
			else
			{
				objectRigidbody.velocity = controllerVelocity * (num * throwMultiplier);
				objectRigidbody.angularVelocity = controllerAngularVelocity;
			}
			if (throwVelocityWithAttachDistance)
			{
				Collider componentInChildren = objectRigidbody.GetComponentInChildren<Collider>();
				if ((bool)componentInChildren)
				{
					Vector3 center = componentInChildren.bounds.center;
					objectRigidbody.velocity = objectRigidbody.GetPointVelocity(center + (center - base.transform.position));
				}
				else
				{
					objectRigidbody.velocity = objectRigidbody.GetPointVelocity(objectRigidbody.position + (objectRigidbody.position - base.transform.position));
				}
			}
		}

		protected virtual Transform GetSnapHandle(GameObject grabbingObject)
		{
			if (rightSnapHandle == null && leftSnapHandle != null)
			{
				rightSnapHandle = leftSnapHandle;
			}
			if (leftSnapHandle == null && rightSnapHandle != null)
			{
				leftSnapHandle = rightSnapHandle;
			}
			if (VRTK_DeviceFinder.IsControllerRightHand(grabbingObject))
			{
				return rightSnapHandle;
			}
			if (VRTK_DeviceFinder.IsControllerLeftHand(grabbingObject))
			{
				return leftSnapHandle;
			}
			return null;
		}

		protected virtual void FlipSnapHandle(Transform snapHandle)
		{
			if ((bool)snapHandle)
			{
				snapHandle.localRotation = Quaternion.Inverse(snapHandle.localRotation);
			}
		}
	}
}
