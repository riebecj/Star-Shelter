using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	public class VRTK_ObjectTouchAutoInteract : MonoBehaviour
	{
		public enum AutoInteractions
		{
			Never = 0,
			NoButtonHeld = 1,
			ButtonHeld = 2
		}

		[Header("Auto Grab")]
		[Tooltip("Determines when a grab on touch should occur.")]
		public AutoInteractions grabOnTouchWhen;

		[Tooltip("After being ungrabbed, another auto grab on touch can only occur after this time.")]
		public float regrabDelay = 0.1f;

		[Tooltip("If this is checked then the grab on touch check will happen every frame and not only on the first touch of the object.")]
		public bool continuousGrabCheck;

		[Header("Auto Use")]
		[Tooltip("Determines when a use on touch should occur.")]
		public AutoInteractions useOnTouchWhen;

		[Tooltip("After being unused, another auto use on touch can only occur after this time.")]
		public float reuseDelay = 0.1f;

		[Tooltip("If this is checked then the use on touch check will happen every frame and not only on the first touch of the object.")]
		public bool continuousUseCheck;

		[Header("Custom Settings")]
		[Tooltip("The interactable object that the auto interaction will occur on. If this is blank then the script must be on the same GameObject as the Interactable Object script.")]
		public VRTK_InteractableObject interactableObject;

		protected float regrabTimer;

		protected float reuseTimer;

		protected List<GameObject> touchers;

		protected virtual void OnEnable()
		{
			regrabTimer = 0f;
			reuseTimer = 0f;
			touchers = new List<GameObject>();
			interactableObject = ((!(interactableObject != null)) ? GetComponent<VRTK_InteractableObject>() : interactableObject);
			if (interactableObject != null)
			{
				interactableObject.InteractableObjectTouched += InteractableObjectTouched;
				interactableObject.InteractableObjectUntouched += InteractableObjectUntouched;
				interactableObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
				interactableObject.InteractableObjectUnused += InteractableObjectUnused;
			}
		}

		protected virtual void OnDisable()
		{
			if (interactableObject != null)
			{
				interactableObject.InteractableObjectTouched -= InteractableObjectTouched;
				interactableObject.InteractableObjectUntouched -= InteractableObjectUntouched;
				interactableObject.InteractableObjectUngrabbed -= InteractableObjectUngrabbed;
				interactableObject.InteractableObjectUnused -= InteractableObjectUnused;
			}
		}

		protected virtual void Update()
		{
			if (!(interactableObject != null) || (!continuousGrabCheck && !continuousUseCheck))
			{
				return;
			}
			for (int i = 0; i < touchers.Count; i++)
			{
				if (continuousGrabCheck)
				{
					CheckGrab(touchers[i]);
				}
				if (continuousUseCheck)
				{
					CheckUse(touchers[i]);
				}
			}
		}

		protected virtual void InteractableObjectTouched(object sender, InteractableObjectEventArgs e)
		{
			ManageTouchers(e.interactingObject, true);
			CheckGrab(e.interactingObject);
			CheckUse(e.interactingObject);
		}

		protected virtual void InteractableObjectUntouched(object sender, InteractableObjectEventArgs e)
		{
			ManageTouchers(e.interactingObject, false);
		}

		protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
		{
			regrabTimer = regrabDelay + Time.time;
		}

		protected virtual void InteractableObjectUnused(object sender, InteractableObjectEventArgs e)
		{
			reuseTimer = reuseDelay + Time.time;
		}

		protected virtual void ManageTouchers(GameObject interactingObject, bool add)
		{
			if (add && !touchers.Contains(interactingObject))
			{
				touchers.Add(interactingObject);
			}
			else if (!add && touchers.Contains(interactingObject))
			{
				touchers.Remove(interactingObject);
			}
		}

		protected virtual void CheckGrab(GameObject interactingObject)
		{
			if (grabOnTouchWhen != 0 && regrabTimer < Time.time)
			{
				VRTK_InteractGrab component = interactingObject.GetComponent<VRTK_InteractGrab>();
				if (component != null && (grabOnTouchWhen == AutoInteractions.NoButtonHeld || (grabOnTouchWhen == AutoInteractions.ButtonHeld && component.IsGrabButtonPressed())))
				{
					component.AttemptGrab();
				}
			}
		}

		protected virtual void CheckUse(GameObject interactingObject)
		{
			if (useOnTouchWhen == AutoInteractions.Never || !(reuseTimer < Time.time))
			{
				return;
			}
			VRTK_InteractUse component = interactingObject.GetComponent<VRTK_InteractUse>();
			if (component != null && (useOnTouchWhen == AutoInteractions.NoButtonHeld || (useOnTouchWhen == AutoInteractions.ButtonHeld && component.IsUseButtonPressed())))
			{
				if (!interactableObject.holdButtonToUse && interactableObject.IsUsing())
				{
					interactableObject.ForceStopInteracting();
				}
				else
				{
					component.AttemptUse();
				}
			}
		}
	}
}
