using System.Collections;
using UnityEngine;

namespace VRTK
{
	public class VRTK_InteractControllerAppearance : MonoBehaviour
	{
		[Header("Touch Visibility")]
		[Tooltip("Hides the controller model when a valid touch occurs.")]
		public bool hideControllerOnTouch;

		[Tooltip("The amount of seconds to wait before hiding the controller on touch.")]
		public float hideDelayOnTouch;

		[Header("Grab Visibility")]
		[Tooltip("Hides the controller model when a valid grab occurs.")]
		public bool hideControllerOnGrab;

		[Tooltip("The amount of seconds to wait before hiding the controller on grab.")]
		public float hideDelayOnGrab;

		[Header("Use Visibility")]
		[Tooltip("Hides the controller model when a valid use occurs.")]
		public bool hideControllerOnUse;

		[Tooltip("The amount of seconds to wait before hiding the controller on use.")]
		public float hideDelayOnUse;

		protected bool touchControllerShow = true;

		protected bool grabControllerShow = true;

		protected Coroutine hideControllerRoutine;

		public virtual void ToggleControllerOnTouch(bool showController, GameObject touchingObject, GameObject ignoredObject)
		{
			if (hideControllerOnTouch)
			{
				touchControllerShow = showController;
				ToggleController(showController, touchingObject, ignoredObject, hideDelayOnTouch);
			}
		}

		public virtual void ToggleControllerOnGrab(bool showController, GameObject grabbingObject, GameObject ignoredObject)
		{
			if (hideControllerOnGrab)
			{
				VRTK_InteractableObject vRTK_InteractableObject = ((!(ignoredObject != null)) ? null : ignoredObject.GetComponentInParent<VRTK_InteractableObject>());
				if (!showController || touchControllerShow || !vRTK_InteractableObject || !vRTK_InteractableObject.IsTouched())
				{
					grabControllerShow = showController;
					ToggleController(showController, grabbingObject, ignoredObject, hideDelayOnGrab);
				}
			}
		}

		public virtual void ToggleControllerOnUse(bool showController, GameObject usingObject, GameObject ignoredObject)
		{
			if (hideControllerOnUse)
			{
				VRTK_InteractableObject vRTK_InteractableObject = ((!(ignoredObject != null)) ? null : ignoredObject.GetComponentInParent<VRTK_InteractableObject>());
				if (!showController || ((grabControllerShow || !vRTK_InteractableObject || !vRTK_InteractableObject.IsGrabbed()) && (touchControllerShow || !vRTK_InteractableObject || !vRTK_InteractableObject.IsTouched())))
				{
					ToggleController(showController, usingObject, ignoredObject, hideDelayOnUse);
				}
			}
		}

		protected virtual void OnEnable()
		{
			if (!GetComponent<VRTK_InteractableObject>())
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractControllerAppearance", "VRTK_InteractableObject", "the same"));
			}
		}

		protected virtual void OnDisable()
		{
			if (hideControllerRoutine != null)
			{
				StopCoroutine(hideControllerRoutine);
			}
		}

		protected virtual void ToggleController(bool showController, GameObject interactingObject, GameObject ignoredObject, float delayTime)
		{
			if (showController)
			{
				ShowController(interactingObject, ignoredObject);
			}
			else
			{
				hideControllerRoutine = StartCoroutine(HideController(interactingObject, ignoredObject, delayTime));
			}
		}

		protected virtual void ShowController(GameObject interactingObject, GameObject ignoredObject)
		{
			if (hideControllerRoutine != null)
			{
				StopCoroutine(hideControllerRoutine);
			}
			VRTK_SharedMethods.SetRendererVisible(interactingObject, ignoredObject);
		}

		protected virtual IEnumerator HideController(GameObject interactingObject, GameObject ignoredObject, float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			VRTK_SharedMethods.SetRendererHidden(interactingObject, ignoredObject);
		}
	}
}
