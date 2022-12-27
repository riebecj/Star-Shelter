using UnityEngine;
using UnityEngine.EventSystems;

namespace VRTK
{
	public class VRTK_UIPointer : MonoBehaviour
	{
		public enum ActivationMethods
		{
			HoldButton = 0,
			ToggleButton = 1,
			AlwaysOn = 2
		}

		public enum ClickMethods
		{
			ClickOnButtonUp = 0,
			ClickOnButtonDown = 1
		}

		[Header("Activation Settings")]
		[Tooltip("The button used to activate/deactivate the UI raycast for the pointer.")]
		public VRTK_ControllerEvents.ButtonAlias activationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;

		[Tooltip("Determines when the UI pointer should be active.")]
		public ActivationMethods activationMode;

		[Header("Selection Settings")]
		[Tooltip("The button used to execute the select action at the pointer's target position.")]
		public VRTK_ControllerEvents.ButtonAlias selectionButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;

		[Tooltip("Determines when the UI Click event action should happen.")]
		public ClickMethods clickMethod;

		[Tooltip("Determines whether the UI click action should be triggered when the pointer is deactivated. If the pointer is hovering over a clickable element then it will invoke the click action on that element. Note: Only works with `Click Method =  Click_On_Button_Up`")]
		public bool attemptClickOnDeactivate;

		[Tooltip("The amount of time the pointer can be over the same UI element before it automatically attempts to click it. 0f means no click attempt will be made.")]
		public float clickAfterHoverDuration;

		[Header("Customisation Settings")]
		[Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
		public VRTK_ControllerEvents controller;

		[Tooltip("A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.")]
		public Transform pointerOriginTransform;

		[HideInInspector]
		public PointerEventData pointerEventData;

		[HideInInspector]
		public GameObject hoveringElement;

		[HideInInspector]
		public GameObject controllerRenderModel;

		[HideInInspector]
		public float hoverDurationTimer;

		[HideInInspector]
		public bool canClickOnHover;

		[HideInInspector]
		public GameObject autoActivatingCanvas;

		[HideInInspector]
		public bool collisionClick;

		protected bool pointerClicked;

		protected bool beamEnabledState;

		protected bool lastPointerPressState;

		protected bool lastPointerClickState;

		protected GameObject currentTarget;

		protected EventSystem cachedEventSystem;

		protected VRTK_VRInputModule cachedVRInputModule;

		public event ControllerInteractionEventHandler ActivationButtonPressed;

		public event ControllerInteractionEventHandler ActivationButtonReleased;

		public event ControllerInteractionEventHandler SelectionButtonPressed;

		public event ControllerInteractionEventHandler SelectionButtonReleased;

		public event UIPointerEventHandler UIPointerElementEnter;

		public event UIPointerEventHandler UIPointerElementExit;

		public event UIPointerEventHandler UIPointerElementClick;

		public event UIPointerEventHandler UIPointerElementDragStart;

		public event UIPointerEventHandler UIPointerElementDragEnd;

		public virtual void OnUIPointerElementEnter(UIPointerEventArgs e)
		{
			if (e.currentTarget != currentTarget)
			{
				ResetHoverTimer();
			}
			if (clickAfterHoverDuration > 0f && hoverDurationTimer <= 0f)
			{
				canClickOnHover = true;
				hoverDurationTimer = clickAfterHoverDuration;
			}
			currentTarget = e.currentTarget;
			if (this.UIPointerElementEnter != null)
			{
				this.UIPointerElementEnter(this, e);
			}
		}

		public virtual void OnUIPointerElementExit(UIPointerEventArgs e)
		{
			if (e.previousTarget == currentTarget)
			{
				ResetHoverTimer();
			}
			if (this.UIPointerElementExit != null)
			{
				this.UIPointerElementExit(this, e);
				if (attemptClickOnDeactivate && !e.isActive && (bool)e.previousTarget)
				{
					pointerEventData.pointerPress = e.previousTarget;
				}
			}
		}

		public virtual void OnUIPointerElementClick(UIPointerEventArgs e)
		{
			if (e.currentTarget == currentTarget)
			{
				ResetHoverTimer();
			}
			if (this.UIPointerElementClick != null)
			{
				this.UIPointerElementClick(this, e);
			}
		}

		public virtual void OnUIPointerElementDragStart(UIPointerEventArgs e)
		{
			if (this.UIPointerElementDragStart != null)
			{
				this.UIPointerElementDragStart(this, e);
			}
		}

		public virtual void OnUIPointerElementDragEnd(UIPointerEventArgs e)
		{
			if (this.UIPointerElementDragEnd != null)
			{
				this.UIPointerElementDragEnd(this, e);
			}
		}

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

		public virtual UIPointerEventArgs SetUIPointerEvent(RaycastResult currentRaycastResult, GameObject currentTarget, GameObject lastTarget = null)
		{
			UIPointerEventArgs result = default(UIPointerEventArgs);
			result.controllerIndex = ((!(controller != null)) ? uint.MaxValue : VRTK_DeviceFinder.GetControllerIndex(controller.gameObject));
			result.isActive = PointerActive();
			result.currentTarget = currentTarget;
			result.previousTarget = lastTarget;
			result.raycastResult = currentRaycastResult;
			return result;
		}

		public virtual VRTK_VRInputModule SetEventSystem(EventSystem eventSystem)
		{
			if (!eventSystem)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_UIPointer", "EventSystem"));
				return null;
			}
			if (!(eventSystem is VRTK_EventSystem))
			{
				eventSystem = eventSystem.gameObject.AddComponent<VRTK_EventSystem>();
			}
			return eventSystem.GetComponent<VRTK_VRInputModule>();
		}

		public virtual void RemoveEventSystem()
		{
			VRTK_EventSystem vRTK_EventSystem = Object.FindObjectOfType<VRTK_EventSystem>();
			if (!vRTK_EventSystem)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_UIPointer", "EventSystem"));
			}
			else
			{
				Object.Destroy(vRTK_EventSystem);
			}
		}

		public virtual bool PointerActive()
		{
			if (activationMode == ActivationMethods.AlwaysOn || autoActivatingCanvas != null)
			{
				return true;
			}
			if (activationMode == ActivationMethods.HoldButton)
			{
				return IsActivationButtonPressed();
			}
			pointerClicked = false;
			if (IsActivationButtonPressed() && !lastPointerPressState)
			{
				pointerClicked = true;
			}
			lastPointerPressState = controller != null && controller.IsButtonPressed(activationButton);
			if (pointerClicked)
			{
				beamEnabledState = !beamEnabledState;
			}
			return beamEnabledState;
		}

		public virtual bool IsActivationButtonPressed()
		{
			return controller != null && controller.IsButtonPressed(activationButton);
		}

		public virtual bool IsSelectionButtonPressed()
		{
			return controller != null && controller.IsButtonPressed(selectionButton);
		}

		public virtual bool ValidClick(bool checkLastClick, bool lastClickState = false)
		{
			bool flag = ((!collisionClick) ? IsSelectionButtonPressed() : collisionClick);
			bool result = ((!checkLastClick) ? flag : (flag && lastPointerClickState == lastClickState));
			lastPointerClickState = flag;
			return result;
		}

		public virtual Vector3 GetOriginPosition()
		{
			return (!pointerOriginTransform) ? base.transform.position : pointerOriginTransform.position;
		}

		public virtual Vector3 GetOriginForward()
		{
			return (!pointerOriginTransform) ? base.transform.forward : pointerOriginTransform.forward;
		}

		protected virtual void OnEnable()
		{
			pointerOriginTransform = ((!(pointerOriginTransform == null)) ? pointerOriginTransform : VRTK_SDK_Bridge.GenerateControllerPointerOrigin(base.gameObject));
			controller = ((!(controller != null)) ? GetComponent<VRTK_ControllerEvents>() : controller);
			ConfigureEventSystem();
			pointerClicked = false;
			lastPointerPressState = false;
			lastPointerClickState = false;
			beamEnabledState = false;
			if (controller != null)
			{
				controllerRenderModel = VRTK_SDK_Bridge.GetControllerRenderModel(controller.gameObject);
				controller.SubscribeToButtonAliasEvent(activationButton, true, DoActivationButtonPressed);
				controller.SubscribeToButtonAliasEvent(activationButton, false, DoActivationButtonReleased);
				controller.SubscribeToButtonAliasEvent(selectionButton, true, DoSelectionButtonPressed);
				controller.SubscribeToButtonAliasEvent(selectionButton, false, DoSelectionButtonReleased);
			}
		}

		protected virtual void OnDisable()
		{
			if ((bool)cachedVRInputModule && cachedVRInputModule.pointers.Contains(this))
			{
				cachedVRInputModule.pointers.Remove(this);
			}
			if (controller != null)
			{
				controller.UnsubscribeToButtonAliasEvent(activationButton, true, DoActivationButtonPressed);
				controller.UnsubscribeToButtonAliasEvent(activationButton, false, DoActivationButtonReleased);
				controller.UnsubscribeToButtonAliasEvent(selectionButton, true, DoSelectionButtonPressed);
				controller.UnsubscribeToButtonAliasEvent(selectionButton, false, DoSelectionButtonReleased);
			}
		}

		protected virtual void LateUpdate()
		{
			if (controller != null)
			{
				pointerEventData.pointerId = (int)VRTK_DeviceFinder.GetControllerIndex(controller.gameObject);
			}
		}

		protected virtual void DoActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
		{
			OnActivationButtonPressed(controller.SetControllerEvent());
		}

		protected virtual void DoActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
		{
			OnActivationButtonReleased(controller.SetControllerEvent());
		}

		protected virtual void DoSelectionButtonPressed(object sender, ControllerInteractionEventArgs e)
		{
			OnSelectionButtonPressed(controller.SetControllerEvent());
		}

		protected virtual void DoSelectionButtonReleased(object sender, ControllerInteractionEventArgs e)
		{
			OnSelectionButtonReleased(controller.SetControllerEvent());
		}

		protected virtual void ResetHoverTimer()
		{
			hoverDurationTimer = 0f;
			canClickOnHover = false;
		}

		protected virtual void ConfigureEventSystem()
		{
			if (!cachedEventSystem)
			{
				cachedEventSystem = Object.FindObjectOfType<EventSystem>();
			}
			if (!cachedVRInputModule)
			{
				cachedVRInputModule = SetEventSystem(cachedEventSystem);
			}
			if ((bool)cachedEventSystem && (bool)cachedVRInputModule)
			{
				if (pointerEventData == null)
				{
					pointerEventData = new PointerEventData(cachedEventSystem);
				}
				if (!cachedVRInputModule.pointers.Contains(this))
				{
					cachedVRInputModule.pointers.Add(this);
				}
			}
		}
	}
}
