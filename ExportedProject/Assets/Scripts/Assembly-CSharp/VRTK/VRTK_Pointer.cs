using UnityEngine;

namespace VRTK
{
	public class VRTK_Pointer : VRTK_DestinationMarker
	{
		[Header("Pointer Activation Settings")]
		[Tooltip("The specific renderer to use when the pointer is activated. The renderer also determines how the pointer reaches it's destination (e.g. straight line, bezier curve).")]
		public VRTK_BasePointerRenderer pointerRenderer;

		[Tooltip("The button used to activate/deactivate the pointer.")]
		public VRTK_ControllerEvents.ButtonAlias activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;

		[Tooltip("If this is checked then the Activation Button needs to be continuously held down to keep the pointer active. If this is unchecked then the Activation Button works as a toggle, the first press/release enables the pointer and the second press/release disables the pointer.")]
		public bool holdButtonToActivate = true;

		[Tooltip("If this is checked then the pointer will be toggled on when the script is enabled.")]
		public bool activateOnEnable;

		[Tooltip("The time in seconds to delay the pointer being able to be active again.")]
		public float activationDelay;

		[Header("Pointer Selection Settings")]
		[Tooltip("The button used to execute the select action at the pointer's target position.")]
		public VRTK_ControllerEvents.ButtonAlias selectionButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;

		[Tooltip("If this is checked then the pointer selection action is executed when the Selection Button is pressed down. If this is unchecked then the selection action is executed when the Selection Button is released.")]
		public bool selectOnPress;

		[Tooltip("The time in seconds to delay the pointer being able to execute the select action again.")]
		public float selectionDelay;

		[Tooltip("The amount of time the pointer can be over the same collider before it automatically attempts to select it. 0f means no selection attempt will be made.")]
		public float selectAfterHoverDuration;

		[Header("Pointer Interaction Settings")]
		[Tooltip("If this is checked then the pointer will be an extension of the controller and able to interact with Interactable Objects.")]
		public bool interactWithObjects;

		[Tooltip("If `Interact With Objects` is checked and this is checked then when an object is grabbed with the pointer touching it, the object will attach to the pointer tip and not snap to the controller.")]
		public bool grabToPointerTip;

		[Header("Pointer Customisation Settings")]
		[Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
		public VRTK_ControllerEvents controller;

		[Tooltip("A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.")]
		public Transform customOrigin;

		[Tooltip("A custom VRTK_PointerDirectionIndicator to use to determine the rotation given to the destination set event.")]
		public VRTK_PointerDirectionIndicator directionIndicator;

		protected VRTK_ControllerEvents.ButtonAlias subscribedActivationButton;

		protected VRTK_ControllerEvents.ButtonAlias subscribedSelectionButton;

		protected bool currentSelectOnPress;

		protected float activateDelayTimer;

		protected float selectDelayTimer;

		protected float hoverDurationTimer;

		internal int currentActivationState;

		protected bool willDeactivate;

		protected bool wasActivated;

		protected uint controllerIndex;

		protected VRTK_InteractableObject pointerInteractableObject;

		protected Collider currentCollider;

		protected bool canClickOnHover;

		protected bool activationButtonPressed;

		protected bool selectionButtonPressed;

		public event ControllerInteractionEventHandler ActivationButtonPressed;

		public event ControllerInteractionEventHandler ActivationButtonReleased;

		public event ControllerInteractionEventHandler SelectionButtonPressed;

		public event ControllerInteractionEventHandler SelectionButtonReleased;

		public virtual void OnActivationButtonPressed(ControllerInteractionEventArgs e)
		{
			if (this.ActivationButtonPressed != null)
			{
				this.ActivationButtonPressed(this, e);
			}
		}

		public virtual void OnActivationButtonReleased(ControllerInteractionEventArgs e)
		{
			if (this.ActivationButtonReleased != null)
			{
				this.ActivationButtonReleased(this, e);
			}
		}

		public virtual void OnSelectionButtonPressed(ControllerInteractionEventArgs e)
		{
			if (this.SelectionButtonPressed != null)
			{
				this.SelectionButtonPressed(this, e);
			}
		}

		public virtual void OnSelectionButtonReleased(ControllerInteractionEventArgs e)
		{
			if (this.SelectionButtonReleased != null)
			{
				this.SelectionButtonReleased(this, e);
			}
		}

		public virtual bool IsActivationButtonPressed()
		{
			return activationButtonPressed;
		}

		public virtual bool IsSelectionButtonPressed()
		{
			return selectionButtonPressed;
		}

		public virtual void PointerEnter(RaycastHit givenHit)
		{
			if (base.enabled && (bool)givenHit.transform && controllerIndex < uint.MaxValue)
			{
				SetHoverSelectionTimer(givenHit.collider);
				OnDestinationMarkerEnter(SetDestinationMarkerEvent(givenHit.distance, givenHit.transform, givenHit, givenHit.point, controllerIndex, false, GetCursorRotation()));
				StartUseAction(givenHit.transform);
				HandController.currentHand = base.transform.GetComponentInParent<HandController>();
				HandController.currentHand.OnPointerEnter(givenHit);
				if ((bool)givenHit.collider.GetComponent<WallNode>())
				{
					givenHit.collider.GetComponent<WallNode>().OnEnter();
				}
				else if ((bool)givenHit.collider.GetComponent<GateNode>())
				{
					givenHit.collider.GetComponent<GateNode>().OnEnter();
				}
			}
		}

		public virtual void PointerExit(RaycastHit givenHit)
		{
			ResetHoverSelectionTimer(givenHit.collider);
			if ((bool)givenHit.transform && controllerIndex < uint.MaxValue)
			{
				OnDestinationMarkerExit(SetDestinationMarkerEvent(givenHit.distance, givenHit.transform, givenHit, givenHit.point, controllerIndex, false, GetCursorRotation()));
				StopUseAction();
				HandController.currentHand = base.transform.GetComponentInParent<HandController>();
				HandController.currentHand.OnPointerExit(givenHit);
				if ((bool)givenHit.collider.GetComponent<WallNode>())
				{
					givenHit.collider.GetComponent<WallNode>().OnExit();
				}
				else if ((bool)givenHit.collider.GetComponent<GateNode>())
				{
					givenHit.collider.GetComponent<GateNode>().OnExit();
				}
			}
		}

		public virtual bool CanActivate()
		{
			return Time.time >= activateDelayTimer;
		}

		public virtual bool CanSelect()
		{
			return Time.time >= selectDelayTimer;
		}

		public virtual bool IsPointerActive()
		{
			return currentActivationState != 0;
		}

		public virtual void ResetActivationTimer(bool forceZero = false)
		{
			activateDelayTimer = ((!forceZero) ? (Time.time + activationDelay) : 0f);
		}

		public virtual void ResetSelectionTimer(bool forceZero = false)
		{
			selectDelayTimer = ((!forceZero) ? (Time.time + selectionDelay) : 0f);
		}

		public virtual void Toggle(bool state)
		{
			if ((!state || !(GetComponent<VRTK_InteractGrab>().GetGrabbedObject() != null)) && (!ArmUIManager.instance || ArmUIManager.instance.tabs[1].activeSelf) && CanActivate() && !NoPointerRenderer() && !CanActivateOnToggleButton(state))
			{
				ManageActivationState(willDeactivate || state);
				pointerRenderer.Toggle(IsPointerActive(), state);
				willDeactivate = false;
				if (!state)
				{
					StopUseAction();
				}
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			VRTK_PlayerObject.SetPlayerObject(base.gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);
			customOrigin = ((!(customOrigin == null)) ? customOrigin : VRTK_SDK_Bridge.GenerateControllerPointerOrigin(base.gameObject));
			SetupController();
			SetupRenderer();
			activateDelayTimer = 0f;
			selectDelayTimer = 0f;
			hoverDurationTimer = 0f;
			currentActivationState = 0;
			wasActivated = false;
			willDeactivate = false;
			canClickOnHover = false;
			if (NoPointerRenderer())
			{
				VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_PARAMETER, "VRTK_Pointer", "VRTK_BasePointerRenderer", "Pointer Renderer"));
			}
			if (activateOnEnable)
			{
				Toggle(true);
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			UnsubscribeActivationButton();
			UnsubscribeSelectionButton();
		}

		protected virtual void Start()
		{
			FindController();
		}

		protected virtual void Update()
		{
			CheckButtonSubscriptions();
			if (EnabledPointerRenderer())
			{
				pointerRenderer.InitalizePointer(this, invalidListPolicy, navMeshCheckDistance, headsetPositionCompensation);
				pointerRenderer.UpdateRenderer();
				if (!IsPointerActive())
				{
					bool state = pointerRenderer.IsVisible();
					pointerRenderer.ToggleInteraction(state);
				}
				CheckHoverSelect();
			}
			UpdateDirectionIndicator();
		}

		protected virtual void UpdateDirectionIndicator()
		{
			if (directionIndicator != null && pointerRenderer != null)
			{
				RaycastHit destinationHit = pointerRenderer.GetDestinationHit();
				directionIndicator.SetPosition(IsPointerActive() && destinationHit.collider != null, destinationHit.point);
			}
		}

		protected virtual Quaternion? GetCursorRotation()
		{
			if (directionIndicator != null)
			{
				return directionIndicator.GetRotation();
			}
			return null;
		}

		protected virtual bool EnabledPointerRenderer()
		{
			return (bool)pointerRenderer && pointerRenderer.enabled;
		}

		protected virtual bool NoPointerRenderer()
		{
			return !pointerRenderer || !pointerRenderer.enabled;
		}

		protected virtual bool CanActivateOnToggleButton(bool state)
		{
			bool flag = state && !holdButtonToActivate && IsPointerActive();
			if (flag)
			{
				willDeactivate = true;
			}
			return flag;
		}

		protected virtual void FindController()
		{
			if (controller == null)
			{
				controller = GetComponentInParent<VRTK_ControllerEvents>();
				SetupController();
			}
			if (controller == null && (activationButton != 0 || selectionButton != 0))
			{
				VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_Pointer", "VRTK_ControllerEvents", "the Controller Alias", ". To omit this warning, set the `Activation Button` and `Selection Button` to `Undefined`"));
			}
			if (directionIndicator != null)
			{
				directionIndicator.Initialize(controller);
			}
		}

		protected virtual void SetupController()
		{
			if ((bool)controller)
			{
				CheckButtonMappingConflict();
				SubscribeSelectionButton();
				SubscribeActivationButton();
			}
		}

		protected virtual void SetupRenderer()
		{
			if (EnabledPointerRenderer())
			{
				pointerRenderer.InitalizePointer(this, invalidListPolicy, navMeshCheckDistance, headsetPositionCompensation);
			}
		}

		protected virtual bool ButtonMappingIsUndefined(VRTK_ControllerEvents.ButtonAlias givenButton, VRTK_ControllerEvents.ButtonAlias givenSubscribedButton)
		{
			return givenSubscribedButton != 0 && givenButton == VRTK_ControllerEvents.ButtonAlias.Undefined;
		}

		protected virtual void CheckButtonMappingConflict()
		{
			if (activationButton == selectionButton)
			{
				if (selectOnPress && holdButtonToActivate)
				{
					VRTK_Logger.Warn("`Hold Button To Activate` and `Select On Press` cannot both be checked when using the same button for Activation and Selection. Fixing by setting `Select On Press` to `false`.");
				}
				if (!selectOnPress && !holdButtonToActivate)
				{
					VRTK_Logger.Warn("`Hold Button To Activate` and `Select On Press` cannot both be unchecked when using the same button for Activation and Selection. Fixing by setting `Select On Press` to `true`.");
				}
				selectOnPress = !holdButtonToActivate;
			}
		}

		protected virtual void CheckButtonSubscriptions()
		{
			CheckButtonMappingConflict();
			if (ButtonMappingIsUndefined(selectionButton, subscribedSelectionButton) || selectOnPress != currentSelectOnPress)
			{
				UnsubscribeSelectionButton();
			}
			if (selectionButton != subscribedSelectionButton)
			{
				SubscribeSelectionButton();
				UnsubscribeActivationButton();
			}
			if (ButtonMappingIsUndefined(activationButton, subscribedActivationButton))
			{
				UnsubscribeActivationButton();
			}
			if (activationButton != subscribedActivationButton)
			{
				SubscribeActivationButton();
			}
		}

		protected virtual void SubscribeActivationButton()
		{
			if (subscribedActivationButton != 0)
			{
				UnsubscribeActivationButton();
			}
			if ((bool)controller)
			{
				controller.SubscribeToButtonAliasEvent(activationButton, true, DoActivationButtonPressed);
				controller.SubscribeToButtonAliasEvent(activationButton, false, DoActivationButtonReleased);
				subscribedActivationButton = activationButton;
			}
		}

		protected virtual void UnsubscribeActivationButton()
		{
			if ((bool)controller && subscribedActivationButton != 0)
			{
				controller.UnsubscribeToButtonAliasEvent(subscribedActivationButton, true, DoActivationButtonPressed);
				controller.UnsubscribeToButtonAliasEvent(subscribedActivationButton, false, DoActivationButtonReleased);
				subscribedActivationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
			}
		}

		protected virtual void DoActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
		{
			if (!(Mathf.Abs(e.touchpadAxis.magnitude) > 0.8f))
			{
				OnActivationButtonPressed(controller.SetControllerEvent(ref activationButtonPressed, true));
				if (EnabledPointerRenderer())
				{
					controllerIndex = e.controllerIndex;
					Toggle(true);
				}
			}
		}

		protected virtual void DoActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
		{
			if (EnabledPointerRenderer())
			{
				controllerIndex = e.controllerIndex;
				if (IsPointerActive())
				{
					Toggle(false);
				}
			}
			OnActivationButtonReleased(controller.SetControllerEvent(ref activationButtonPressed));
		}

		protected virtual void SubscribeSelectionButton()
		{
			if (subscribedSelectionButton != 0)
			{
				UnsubscribeSelectionButton();
			}
			if ((bool)controller)
			{
				controller.SubscribeToButtonAliasEvent(selectionButton, true, DoSelectionButtonPressed);
				controller.SubscribeToButtonAliasEvent(selectionButton, false, DoSelectionButtonReleased);
				controller.SubscribeToButtonAliasEvent(selectionButton, selectOnPress, SelectionButtonAction);
				subscribedSelectionButton = selectionButton;
				currentSelectOnPress = selectOnPress;
			}
		}

		protected virtual void UnsubscribeSelectionButton()
		{
			if ((bool)controller && subscribedSelectionButton != 0)
			{
				controller.UnsubscribeToButtonAliasEvent(selectionButton, true, DoSelectionButtonPressed);
				controller.UnsubscribeToButtonAliasEvent(selectionButton, false, DoSelectionButtonReleased);
				controller.UnsubscribeToButtonAliasEvent(subscribedSelectionButton, currentSelectOnPress, SelectionButtonAction);
				subscribedSelectionButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
			}
		}

		protected virtual void DoSelectionButtonPressed(object sender, ControllerInteractionEventArgs e)
		{
			OnSelectionButtonPressed(controller.SetControllerEvent(ref selectionButtonPressed, true));
		}

		protected virtual void DoSelectionButtonReleased(object sender, ControllerInteractionEventArgs e)
		{
			OnSelectionButtonReleased(controller.SetControllerEvent(ref selectionButtonPressed));
		}

		protected virtual void SelectionButtonAction(object sender, ControllerInteractionEventArgs e)
		{
			controllerIndex = e.controllerIndex;
			ExecuteSelectionButtonAction();
		}

		protected virtual void ExecuteSelectionButtonAction()
		{
			if (EnabledPointerRenderer() && CanSelect() && (IsPointerActive() || wasActivated))
			{
				wasActivated = false;
				RaycastHit destinationHit = pointerRenderer.GetDestinationHit();
				AttemptUseOnSet(destinationHit.transform);
				if ((bool)destinationHit.transform && IsPointerActive() && pointerRenderer.ValidPlayArea() && !PointerActivatesUseAction(pointerInteractableObject) && pointerRenderer.IsValidCollision())
				{
					ResetHoverSelectionTimer(destinationHit.collider);
					ResetSelectionTimer();
					OnDestinationMarkerSet(SetDestinationMarkerEvent(destinationHit.distance, destinationHit.transform, destinationHit, destinationHit.point, controllerIndex, false, GetCursorRotation()));
				}
			}
		}

		protected virtual bool CanResetActivationState(bool givenState)
		{
			return (!givenState && holdButtonToActivate) || (givenState && !holdButtonToActivate && currentActivationState >= 2);
		}

		protected virtual void ManageActivationState(bool state)
		{
			if (state)
			{
				currentActivationState++;
			}
			wasActivated = currentActivationState == 2;
			if (CanResetActivationState(state))
			{
				currentActivationState = 0;
			}
		}

		protected virtual bool PointerActivatesUseAction(VRTK_InteractableObject givenInteractableObject)
		{
			return (bool)givenInteractableObject && givenInteractableObject.pointerActivatesUseAction && givenInteractableObject.IsValidInteractableController(controller.gameObject, givenInteractableObject.allowedUseControllers);
		}

		protected virtual void StartUseAction(Transform target)
		{
			pointerInteractableObject = target.GetComponent<VRTK_InteractableObject>();
			bool flag = (bool)pointerInteractableObject && pointerInteractableObject.useOnlyIfGrabbed && !pointerInteractableObject.IsGrabbed();
			if (PointerActivatesUseAction(pointerInteractableObject) && pointerInteractableObject.holdButtonToUse && !flag && pointerInteractableObject.usingState == 0)
			{
				pointerInteractableObject.StartUsing(controller.gameObject);
				pointerInteractableObject.usingState++;
			}
		}

		protected virtual void StopUseAction()
		{
			if (PointerActivatesUseAction(pointerInteractableObject) && pointerInteractableObject.holdButtonToUse && pointerInteractableObject.IsUsing())
			{
				pointerInteractableObject.StopUsing(controller.gameObject);
				pointerInteractableObject.usingState = 0;
			}
		}

		protected virtual void AttemptUseOnSet(Transform target)
		{
			if ((bool)pointerInteractableObject && (bool)target && PointerActivatesUseAction(pointerInteractableObject))
			{
				if (pointerInteractableObject.IsUsing())
				{
					pointerInteractableObject.StopUsing(controller.gameObject);
					pointerInteractableObject.usingState = 0;
				}
				else if (!pointerInteractableObject.holdButtonToUse)
				{
					pointerInteractableObject.StartUsing(controller.gameObject);
					pointerInteractableObject.usingState++;
				}
			}
		}

		protected virtual void SetHoverSelectionTimer(Collider collider)
		{
			if (collider != currentCollider)
			{
				hoverDurationTimer = 0f;
			}
			if (selectAfterHoverDuration > 0f && hoverDurationTimer <= 0f)
			{
				canClickOnHover = true;
				hoverDurationTimer = selectAfterHoverDuration;
			}
			currentCollider = collider;
		}

		protected virtual void ResetHoverSelectionTimer(Collider collider)
		{
			canClickOnHover = false;
			hoverDurationTimer = ((!(collider == currentCollider)) ? hoverDurationTimer : 0f);
		}

		protected virtual void CheckHoverSelect()
		{
			if (hoverDurationTimer > 0f)
			{
				hoverDurationTimer -= Time.deltaTime;
			}
			if (canClickOnHover && hoverDurationTimer <= 0f)
			{
				canClickOnHover = false;
				ExecuteSelectionButtonAction();
			}
		}
	}
}
