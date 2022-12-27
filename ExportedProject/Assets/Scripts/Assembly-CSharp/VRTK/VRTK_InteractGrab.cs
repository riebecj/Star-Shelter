using UnityEngine;
using VRTK.GrabAttachMechanics;

namespace VRTK
{
	public class VRTK_InteractGrab : MonoBehaviour
	{
		[Header("Grab Settings")]
		[Tooltip("The button used to grab/release a touched object.")]
		public VRTK_ControllerEvents.ButtonAlias grabButton = VRTK_ControllerEvents.ButtonAlias.GripPress;

		[Tooltip("An amount of time between when the grab button is pressed to when the controller is touching something to grab it. For example, if an object is falling at a fast rate, then it is very hard to press the grab button in time to catch the object due to human reaction times. A higher number here will mean the grab button can be pressed before the controller touches the object and when the collision takes place, if the grab button is still being held down then the grab action will be successful.")]
		public float grabPrecognition;

		[Tooltip("An amount to multiply the velocity of any objects being thrown. This can be useful when scaling up the play area to simulate being able to throw items further.")]
		public float throwMultiplier = 1f;

		[Tooltip("If this is checked and the controller is not touching an Interactable Object when the grab button is pressed then a rigid body is added to the controller to allow the controller to push other rigid body objects around.")]
		public bool createRigidBodyWhenNotTouching;

		[Header("Custom Settings")]
		[Tooltip("The rigidbody point on the controller model to snap the grabbed object to. If blank it will be set to the SDK default.")]
		public Rigidbody controllerAttachPoint;

		[Tooltip("The controller to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
		public VRTK_ControllerEvents controllerEvents;

		[Tooltip("The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
		public VRTK_InteractTouch interactTouch;

		protected VRTK_ControllerEvents.ButtonAlias subscribedGrabButton;

		protected VRTK_ControllerEvents.ButtonAlias savedGrabButton;

		protected bool grabPressed;

		protected GameObject grabbedObject;

		protected bool influencingGrabbedObject;

		protected int grabEnabledState;

		protected float grabPrecognitionTimer;

		protected GameObject undroppableGrabbedObject;

		public GameObject deteriorationCone;

		internal int storedLayer;

		public event ControllerInteractionEventHandler GrabButtonPressed;

		public event ControllerInteractionEventHandler GrabButtonReleased;

		public event ObjectInteractEventHandler ControllerGrabInteractableObject;

		public event ObjectInteractEventHandler ControllerUngrabInteractableObject;

		public virtual void OnControllerGrabInteractableObject(ObjectInteractEventArgs e)
		{
			if (this.ControllerGrabInteractableObject != null)
			{
				this.ControllerGrabInteractableObject(this, e);
			}
		}

		public virtual void OnControllerUngrabInteractableObject(ObjectInteractEventArgs e)
		{
			if (this.ControllerUngrabInteractableObject != null)
			{
				this.ControllerUngrabInteractableObject(this, e);
			}
		}

		public virtual void OnGrabButtonPressed(ControllerInteractionEventArgs e)
		{
			if (this.GrabButtonPressed != null)
			{
				this.GrabButtonPressed(this, e);
			}
		}

		public virtual void OnGrabButtonReleased(ControllerInteractionEventArgs e)
		{
			if (this.GrabButtonReleased != null)
			{
				this.GrabButtonReleased(this, e);
			}
		}

		public virtual bool IsGrabButtonPressed()
		{
			return grabPressed;
		}

		public virtual void ForceRelease(bool applyGrabbingObjectVelocity = false)
		{
			InitUngrabbedObject(applyGrabbingObjectVelocity);
		}

		public virtual void AttemptGrab()
		{
			AttemptGrabObject();
		}

		public virtual GameObject GetGrabbedObject()
		{
			return grabbedObject;
		}

		protected virtual void OnEnable()
		{
			controllerEvents = ((!(controllerEvents != null)) ? GetComponentInParent<VRTK_ControllerEvents>() : controllerEvents);
			interactTouch = ((!(interactTouch != null)) ? GetComponentInParent<VRTK_InteractTouch>() : interactTouch);
			if (interactTouch == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_InteractGrab", "VRTK_InteractTouch", "interactTouch", "the same or parent"));
			}
			RegrabUndroppableObject();
			ManageGrabListener(true);
			ManageInteractTouchListener(true);
			SetControllerAttachPoint();
		}

		protected virtual void OnDisable()
		{
			SetUndroppableObject();
			ForceRelease();
			ManageGrabListener(false);
			ManageInteractTouchListener(false);
		}

		protected virtual void Update()
		{
			ManageGrabListener(true);
			CheckControllerAttachPointSet();
			CreateNonTouchingRigidbody();
			CheckPrecognitionGrab();
		}

		protected virtual void ManageInteractTouchListener(bool state)
		{
			if (interactTouch != null && !state)
			{
				interactTouch.ControllerTouchInteractableObject -= ControllerTouchInteractableObject;
				interactTouch.ControllerUntouchInteractableObject -= ControllerUntouchInteractableObject;
			}
			if (interactTouch != null && state)
			{
				interactTouch.ControllerTouchInteractableObject += ControllerTouchInteractableObject;
				interactTouch.ControllerUntouchInteractableObject += ControllerUntouchInteractableObject;
			}
		}

		protected virtual void ControllerTouchInteractableObject(object sender, ObjectInteractEventArgs e)
		{
			if (!(e.target != null))
			{
				return;
			}
			VRTK_InteractableObject component = e.target.GetComponent<VRTK_InteractableObject>();
			if (component != null && component.grabOverrideButton != 0 && !component.IsGrabbed())
			{
				savedGrabButton = subscribedGrabButton;
				if ((bool)component.GetComponent<Gun>())
				{
					grabButton = component.grabOverrideButton;
				}
			}
		}

		protected virtual void ControllerUntouchInteractableObject(object sender, ObjectInteractEventArgs e)
		{
			if (e.target != null)
			{
				VRTK_InteractableObject component = e.target.GetComponent<VRTK_InteractableObject>();
				if (!component.IsGrabbed() && savedGrabButton != 0)
				{
					grabButton = savedGrabButton;
					savedGrabButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
				}
			}
		}

		protected virtual void ManageGrabListener(bool state)
		{
			if (controllerEvents != null && subscribedGrabButton != 0 && (!state || !grabButton.Equals(subscribedGrabButton)))
			{
				controllerEvents.UnsubscribeToButtonAliasEvent(subscribedGrabButton, true, DoGrabObject);
				controllerEvents.UnsubscribeToButtonAliasEvent(subscribedGrabButton, false, DoReleaseObject);
				subscribedGrabButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
			}
			if (controllerEvents != null && state && grabButton != 0 && !grabButton.Equals(subscribedGrabButton))
			{
				controllerEvents.SubscribeToButtonAliasEvent(grabButton, true, DoGrabObject);
				controllerEvents.SubscribeToButtonAliasEvent(grabButton, false, DoReleaseObject);
				subscribedGrabButton = grabButton;
			}
		}

		protected virtual void RegrabUndroppableObject()
		{
			if (undroppableGrabbedObject != null && !undroppableGrabbedObject.GetComponent<VRTK_InteractableObject>().IsGrabbed())
			{
				undroppableGrabbedObject.SetActive(true);
				interactTouch.ForceTouch(undroppableGrabbedObject);
				AttemptGrab();
			}
			else
			{
				undroppableGrabbedObject = null;
			}
		}

		protected virtual void SetUndroppableObject()
		{
			if (undroppableGrabbedObject != null)
			{
				if (undroppableGrabbedObject.GetComponent<VRTK_InteractableObject>().IsDroppable())
				{
					undroppableGrabbedObject = null;
				}
				else
				{
					undroppableGrabbedObject.SetActive(false);
				}
			}
		}

		public void ForceGrab(GameObject newGrab)
		{
			interactTouch.ForceTouch(newGrab);
			AttemptGrab();
		}

		protected virtual void SetControllerAttachPoint()
		{
			GameObject modelAliasController = VRTK_DeviceFinder.GetModelAliasController(interactTouch.gameObject);
			if (!(modelAliasController != null) || !(controllerAttachPoint == null))
			{
				return;
			}
			Transform transform = modelAliasController.transform.Find(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.AttachPoint, VRTK_DeviceFinder.GetControllerHand(interactTouch.gameObject)));
			if (transform != null)
			{
				controllerAttachPoint = transform.GetComponent<Rigidbody>();
				if (controllerAttachPoint == null)
				{
					Rigidbody rigidbody = transform.gameObject.AddComponent<Rigidbody>();
					rigidbody.isKinematic = true;
					controllerAttachPoint = rigidbody;
				}
			}
		}

		protected virtual bool IsObjectGrabbable(GameObject obj)
		{
			if (!obj)
			{
				return false;
			}
			VRTK_InteractableObject component = obj.GetComponent<VRTK_InteractableObject>();
			return interactTouch.IsObjectInteractable(obj) && component != null && (component.isGrabbable || component.PerformSecondaryAction());
		}

		protected virtual bool IsObjectHoldOnGrab(GameObject obj)
		{
			if (obj != null)
			{
				VRTK_InteractableObject component = obj.GetComponent<VRTK_InteractableObject>();
				return (bool)component && component.holdButtonToGrab;
			}
			return false;
		}

		protected virtual void ChooseGrabSequence(VRTK_InteractableObject grabbedObjectScript)
		{
			if (!grabbedObjectScript.IsGrabbed() || grabbedObjectScript.IsSwappable())
			{
				InitPrimaryGrab(grabbedObjectScript);
			}
			else
			{
				InitSecondaryGrab(grabbedObjectScript);
			}
		}

		protected virtual void ToggleControllerVisibility(bool visible)
		{
			GameObject modelAliasController = VRTK_DeviceFinder.GetModelAliasController(interactTouch.gameObject);
			if (grabbedObject != null)
			{
				VRTK_InteractControllerAppearance[] componentsInParent = grabbedObject.GetComponentsInParent<VRTK_InteractControllerAppearance>(true);
				if (componentsInParent.Length > 0)
				{
					componentsInParent[0].ToggleControllerOnGrab(visible, modelAliasController, grabbedObject);
				}
			}
			else if (visible)
			{
				VRTK_SharedMethods.SetRendererVisible(modelAliasController, grabbedObject);
			}
		}

		protected virtual void InitGrabbedObject()
		{
			grabbedObject = interactTouch.GetTouchedObject();
			if (grabbedObject != null)
			{
				VRTK_InteractableObject component = grabbedObject.GetComponent<VRTK_InteractableObject>();
				ChooseGrabSequence(component);
				ToggleControllerVisibility(false);
				OnControllerGrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
			}
		}

		protected virtual void InitPrimaryGrab(VRTK_InteractableObject currentGrabbedObject)
		{
			GameObject gameObject = interactTouch.gameObject;
			if ((bool)currentGrabbedObject.GetComponent<Gun>() && (bool)currentGrabbedObject.GetComponent<FixedJoint>())
			{
				Object.Destroy(currentGrabbedObject.GetComponent<FixedJoint>());
			}
			if (!currentGrabbedObject.IsValidInteractableController(gameObject, currentGrabbedObject.allowedGrabControllers))
			{
				grabbedObject = null;
				if (currentGrabbedObject.IsGrabbed(gameObject))
				{
					interactTouch.ForceStopTouching();
				}
				return;
			}
			influencingGrabbedObject = false;
			currentGrabbedObject.SaveCurrentState();
			currentGrabbedObject.Grabbed(gameObject);
			currentGrabbedObject.ZeroVelocity();
			currentGrabbedObject.ToggleHighlight(false);
			storedLayer = currentGrabbedObject.gameObject.layer;
			GetComponentInParent<HandController>().OnGrab();
			if (currentGrabbedObject.gameObject.layer != 15 && (bool)currentGrabbedObject.GetComponent<Rigidbody>() && !currentGrabbedObject.GetComponent<Rigidbody>().isKinematic)
			{
				if ((bool)currentGrabbedObject.GetComponent<AddToInventory>())
				{
					currentGrabbedObject.gameObject.layer = 21;
				}
				else if (currentGrabbedObject.gameObject.layer != 17)
				{
					currentGrabbedObject.gameObject.layer = 10;
					Debug.Log(currentGrabbedObject);
				}
			}
		}

		protected virtual void InitSecondaryGrab(VRTK_InteractableObject currentGrabbedObject)
		{
			GameObject gameObject = interactTouch.gameObject;
			if (!currentGrabbedObject.IsValidInteractableController(gameObject, currentGrabbedObject.allowedGrabControllers))
			{
				grabbedObject = null;
				influencingGrabbedObject = false;
				currentGrabbedObject.Ungrabbed(gameObject);
			}
			else
			{
				influencingGrabbedObject = true;
				currentGrabbedObject.Grabbed(gameObject);
			}
		}

		protected virtual void CheckInfluencingObjectOnRelease()
		{
			if (!influencingGrabbedObject && (bool)interactTouch)
			{
				interactTouch.ForceStopTouching();
				ToggleControllerVisibility(true);
			}
			influencingGrabbedObject = false;
		}

		protected virtual void InitUngrabbedObject(bool applyGrabbingObjectVelocity)
		{
			if (grabbedObject != null)
			{
				GameObject previousGrabbingObject = interactTouch.gameObject;
				VRTK_InteractableObject component = grabbedObject.GetComponent<VRTK_InteractableObject>();
				if (!influencingGrabbedObject)
				{
					component.grabAttachMechanicScript.StopGrab(applyGrabbingObjectVelocity);
				}
				component.Ungrabbed(previousGrabbingObject);
				component.ToggleHighlight(false);
				ToggleControllerVisibility(true);
				OnControllerUngrabInteractableObject(interactTouch.SetControllerInteractEvent(grabbedObject));
			}
			CheckInfluencingObjectOnRelease();
			grabEnabledState = 0;
			if ((bool)grabbedObject && (bool)grabbedObject.GetComponent<AddToInventory>())
			{
				grabbedObject.GetComponent<AddToInventory>().OnRelease();
				Inventory.instance.CheckDropInInventory(grabbedObject);
				for (int i = 0; i < StorageBox.storageBoxes.Count; i++)
				{
					StorageBox.storageBoxes[i].CheckDropInInventory(grabbedObject);
				}
			}
			if (grabbedObject != null)
			{
				if (grabbedObject.layer != 15 && (bool)grabbedObject.GetComponent<Rigidbody>() && !grabbedObject.GetComponent<Rigidbody>().isKinematic)
				{
					grabbedObject.layer = storedLayer;
				}
				grabbedObject = null;
			}
		}

		protected virtual GameObject GetGrabbableObject()
		{
			GameObject touchedObject = interactTouch.GetTouchedObject();
			if (touchedObject != null && interactTouch.IsObjectInteractable(touchedObject))
			{
				return touchedObject;
			}
			return grabbedObject;
		}

		protected virtual void IncrementGrabState()
		{
			if (!IsObjectHoldOnGrab(interactTouch.GetTouchedObject()))
			{
				grabEnabledState++;
			}
		}

		protected virtual GameObject GetUndroppableObject()
		{
			if (grabbedObject != null)
			{
				VRTK_InteractableObject component = grabbedObject.GetComponent<VRTK_InteractableObject>();
				return (!component || component.IsDroppable()) ? null : grabbedObject;
			}
			return null;
		}

		protected virtual void AttemptHaptics(bool initialGrabAttempt)
		{
			if (grabbedObject != null && initialGrabAttempt)
			{
				VRTK_InteractHaptics componentInParent = grabbedObject.GetComponentInParent<VRTK_InteractHaptics>();
				if ((bool)componentInParent)
				{
					componentInParent.HapticsOnGrab(VRTK_DeviceFinder.GetControllerIndex(interactTouch.gameObject));
				}
			}
		}

		protected virtual void AttemptGrabObject()
		{
			GameObject grabbableObject = GetGrabbableObject();
			if (deteriorationCone.activeSelf && grabbableObject != null)
			{
				if ((bool)grabbableObject.GetComponent<CraftComponent>() || CraftingManager.instance.scrapObject != null)
				{
					return;
				}
				base.transform.parent.GetComponent<HandController>().ExitSalvageMode();
			}
			if (grabbableObject != null)
			{
				PerformGrabAttempt(grabbableObject);
			}
			else
			{
				grabPrecognitionTimer = Time.time + grabPrecognition;
			}
		}

		protected virtual void PerformGrabAttempt(GameObject objectToGrab)
		{
			IncrementGrabState();
			bool initialGrabAttempt = IsValidGrabAttempt(objectToGrab);
			undroppableGrabbedObject = GetUndroppableObject();
			AttemptHaptics(initialGrabAttempt);
		}

		protected virtual bool IsValidGrabAttempt(GameObject objectToGrab)
		{
			GameObject grabbingObject = interactTouch.gameObject;
			bool result = false;
			VRTK_InteractableObject component = objectToGrab.GetComponent<VRTK_InteractableObject>();
			if ((bool)objectToGrab.GetComponent<VRTK_ClimbableGrabAttach>())
			{
				objectToGrab.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
			}
			if (interactTouch != null && IsObjectGrabbable(interactTouch.GetTouchedObject()) && grabbedObject == null && component.grabAttachMechanicScript != null && component.grabAttachMechanicScript.ValidGrab(controllerAttachPoint))
			{
				InitGrabbedObject();
				if (!influencingGrabbedObject)
				{
					result = component.grabAttachMechanicScript.StartGrab(grabbingObject, grabbedObject, controllerAttachPoint);
				}
			}
			return result;
		}

		protected virtual bool CanRelease()
		{
			return grabbedObject != null && grabbedObject.GetComponent<VRTK_InteractableObject>().IsDroppable();
		}

		protected virtual void AttemptReleaseObject()
		{
			if (CanRelease() && (IsObjectHoldOnGrab(grabbedObject) || grabEnabledState >= 2))
			{
				InitUngrabbedObject(true);
			}
		}

		protected virtual void DoGrabObject(object sender, ControllerInteractionEventArgs e)
		{
			OnGrabButtonPressed(controllerEvents.SetControllerEvent(ref grabPressed, true));
			AttemptGrabObject();
		}

		protected virtual void DoReleaseObject(object sender, ControllerInteractionEventArgs e)
		{
			AttemptReleaseObject();
			OnGrabButtonReleased(controllerEvents.SetControllerEvent(ref grabPressed));
		}

		protected virtual void CheckControllerAttachPointSet()
		{
			if (controllerAttachPoint == null)
			{
				SetControllerAttachPoint();
			}
		}

		protected virtual void CreateNonTouchingRigidbody()
		{
			if (createRigidBodyWhenNotTouching && grabbedObject == null && !interactTouch.IsRigidBodyForcedActive() && interactTouch.IsRigidBodyActive() != grabPressed)
			{
				interactTouch.ToggleControllerRigidBody(grabPressed);
			}
		}

		protected virtual void CheckPrecognitionGrab()
		{
			if (grabPrecognitionTimer >= Time.time && GetGrabbableObject() != null)
			{
				AttemptGrabObject();
				if (GetGrabbedObject() != null)
				{
					grabPrecognitionTimer = 0f;
				}
			}
		}
	}
}
