using System.Collections;
using UnityEngine;

namespace VRTK
{
	public class VRTK_ObjectAutoGrab : MonoBehaviour
	{
		[Tooltip("A game object (either within the scene or a prefab) that will be grabbed by the controller on game start.")]
		public VRTK_InteractableObject objectToGrab;

		[Tooltip("If the `Object To Grab` is a prefab then this needs to be checked, if the `Object To Grab` already exists in the scene then this needs to be unchecked.")]
		public bool objectIsPrefab;

		[Tooltip("If this is checked then the Object To Grab will be cloned into a new object and attached to the controller leaving the existing object in the scene. This is required if the same object is to be grabbed to both controllers as a single object cannot be grabbed by different controllers at the same time. It is also required to clone a grabbed object if it is a prefab as it needs to exist within the scene to be grabbed.")]
		public bool cloneGrabbedObject;

		[Tooltip("If `Clone Grabbed Object` is checked and this is checked, then whenever this script is disabled and re-enabled, it will always create a new clone of the object to grab. If this is false then the original cloned object will attempt to be grabbed again. If the original cloned object no longer exists then a new clone will be created.")]
		public bool alwaysCloneOnEnable;

		[Header("Custom Settings")]
		[Tooltip("The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
		public VRTK_InteractTouch interactTouch;

		[Tooltip("The Interact Grab to listen for grab actions on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
		public VRTK_InteractGrab interactGrab;

		protected VRTK_InteractableObject previousClonedObject;

		public virtual void ClearPreviousClone()
		{
			previousClonedObject = null;
		}

		protected virtual void OnEnable()
		{
			if (objectIsPrefab)
			{
				cloneGrabbedObject = true;
			}
			StartCoroutine(AutoGrab());
		}

		protected virtual IEnumerator AutoGrab()
		{
			yield return new WaitForEndOfFrame();
			interactTouch = ((!(interactTouch != null)) ? GetComponentInParent<VRTK_InteractTouch>() : interactTouch);
			interactGrab = ((!(interactGrab != null)) ? GetComponentInParent<VRTK_InteractGrab>() : interactGrab);
			if (interactTouch == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_ObjectAutoGrab", "VRTK_InteractTouch", "interactTouch", "the same or parent"));
			}
			if (interactGrab == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_ObjectAutoGrab", "VRTK_InteractGrab", "interactGrab", "the same or parent"));
			}
			if (objectToGrab == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.NOT_DEFINED, "objectToGrab"));
				yield break;
			}
			while (interactGrab.controllerAttachPoint == null)
			{
				yield return true;
			}
			bool grabbableObjectDisableState = objectToGrab.disableWhenIdle;
			if (objectIsPrefab)
			{
				objectToGrab.disableWhenIdle = false;
			}
			VRTK_InteractableObject grabbableObject = objectToGrab;
			if (alwaysCloneOnEnable)
			{
				ClearPreviousClone();
			}
			if (!interactGrab.GetGrabbedObject())
			{
				if (cloneGrabbedObject)
				{
					grabbableObject = ((!(previousClonedObject == null)) ? previousClonedObject : (previousClonedObject = Object.Instantiate(objectToGrab)));
				}
				if (grabbableObject.isGrabbable && !grabbableObject.IsGrabbed())
				{
					grabbableObject.transform.position = base.transform.position;
					interactTouch.ForceStopTouching();
					interactTouch.ForceTouch(grabbableObject.gameObject);
					interactGrab.AttemptGrab();
				}
			}
			objectToGrab.disableWhenIdle = grabbableObjectDisableState;
			grabbableObject.disableWhenIdle = grabbableObjectDisableState;
		}
	}
}
