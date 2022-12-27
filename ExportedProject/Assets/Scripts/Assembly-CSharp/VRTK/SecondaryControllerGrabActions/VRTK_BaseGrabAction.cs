using UnityEngine;

namespace VRTK.SecondaryControllerGrabActions
{
	public abstract class VRTK_BaseGrabAction : MonoBehaviour
	{
		protected VRTK_InteractableObject grabbedObject;

		protected VRTK_InteractGrab primaryGrabbingObject;

		protected VRTK_InteractGrab secondaryGrabbingObject;

		protected Transform primaryInitialGrabPoint;

		protected Transform secondaryInitialGrabPoint;

		protected bool initialised;

		protected bool isActionable = true;

		protected bool isSwappable;

		public virtual void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
		{
			grabbedObject = currentGrabbdObject;
			primaryGrabbingObject = currentPrimaryGrabbingObject;
			secondaryGrabbingObject = currentSecondaryGrabbingObject;
			primaryInitialGrabPoint = primaryGrabPoint;
			secondaryInitialGrabPoint = secondaryGrabPoint;
			initialised = true;
		}

		public virtual void ResetAction()
		{
			grabbedObject = null;
			primaryGrabbingObject = null;
			secondaryGrabbingObject = null;
			primaryInitialGrabPoint = null;
			secondaryInitialGrabPoint = null;
			initialised = false;
		}

		public virtual bool IsActionable()
		{
			return isActionable;
		}

		public virtual bool IsSwappable()
		{
			return isSwappable;
		}

		public virtual void ProcessUpdate()
		{
		}

		public virtual void ProcessFixedUpdate()
		{
		}

		public virtual void OnDropAction()
		{
		}

		protected virtual void CheckForceStopDistance(float ungrabDistance)
		{
			if (initialised && Vector3.Distance(secondaryGrabbingObject.transform.position, secondaryInitialGrabPoint.position) > ungrabDistance)
			{
				grabbedObject.ForceStopSecondaryGrabInteraction();
			}
		}
	}
}
