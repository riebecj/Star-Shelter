using System;
using UnityEngine;

namespace VRTK
{
	public class VRTK_ControllerEvents : MonoBehaviour
	{
		public enum ButtonAlias
		{
			Undefined = 0,
			TriggerHairline = 1,
			TriggerTouch = 2,
			TriggerPress = 3,
			TriggerClick = 4,
			GripHairline = 5,
			GripTouch = 6,
			GripPress = 7,
			GripClick = 8,
			TouchpadTouch = 9,
			TouchpadPress = 10,
			ButtonOneTouch = 11,
			ButtonOnePress = 12,
			ButtonTwoTouch = 13,
			ButtonTwoPress = 14,
			StartMenuPress = 15
		}

		[Header("Action Alias Buttons")]
		[Tooltip("**OBSOLETE [use VRTK_Pointer.activationButton]** The button to use for the action of turning a laser pointer on / off.")]
		[Obsolete("`VRTK_ControllerEvents.pointerToggleButton` is no longer used in the new `VRTK_Pointer` class, use `VRTK_Pointer.activationButton` instead. This parameter will be removed in a future version of VRTK.")]
		public ButtonAlias pointerToggleButton = ButtonAlias.TouchpadPress;

		[Tooltip("**OBSOLETE [use VRTK_Pointer.selectionButton]** The button to use for the action of setting a destination marker from the cursor position of the pointer.")]
		[Obsolete("`VRTK_ControllerEvents.pointerSetButton` is no longer used in the new `VRTK_Pointer` class, use `VRTK_Pointer.selectionButton` instead. This parameter will be removed in a future version of VRTK.")]
		public ButtonAlias pointerSetButton = ButtonAlias.TouchpadPress;

		[Tooltip("**OBSOLETE [use VRTK_InteractGrab.grabButton]** The button to use for the action of grabbing game objects.")]
		[Obsolete("`VRTK_ControllerEvents.grabToggleButton` is no longer used in the `VRTK_InteractGrab` class, use `VRTK_InteractGrab.grabButton` instead. This parameter will be removed in a future version of VRTK.")]
		public ButtonAlias grabToggleButton = ButtonAlias.GripPress;

		[Tooltip("**OBSOLETE [use VRTK_InteractUse.useButton]** The button to use for the action of using game objects.")]
		[Obsolete("`VRTK_ControllerEvents.useToggleButton` is no longer used in the `VRTK_InteractUse` class, use `VRTK_InteractUse.useButton` instead. This parameter will be removed in a future version of VRTK.")]
		public ButtonAlias useToggleButton = ButtonAlias.TriggerPress;

		[Tooltip("**OBSOLETE [use VRTK_UIPointer.selectionButton]** The button to use for the action of clicking a UI element.")]
		[Obsolete("`VRTK_ControllerEvents.uiClickButton` is no longer used in the `VRTK_UIPointer` class, use `VRTK_UIPointer.selectionButton` instead. This parameter will be removed in a future version of VRTK.")]
		public ButtonAlias uiClickButton = ButtonAlias.TriggerPress;

		[Tooltip("**OBSOLETE [use VRTK_ControllerEvents.buttonTwoPressed]** The button to use for the action of bringing up an in-game menu.")]
		[Obsolete("`VRTK_ControllerEvents.menuToggleButton` is no longer used, use `VRTK_ControllerEvents.buttonTwoPressed` instead. This parameter will be removed in a future version of VRTK.")]
		public ButtonAlias menuToggleButton = ButtonAlias.ButtonTwoPress;

		[Header("Axis Refinement")]
		[Tooltip("The amount of fidelity in the changes on the axis, which is defaulted to 1. Any number higher than 2 will probably give too sensitive results.")]
		public int axisFidelity = 1;

		[Tooltip("The level on the trigger axis to reach before a click is registered.")]
		public float triggerClickThreshold = 1f;

		[Tooltip("The level on the trigger axis to reach before the axis is forced to 0f.")]
		public float triggerForceZeroThreshold = 0.01f;

		[Tooltip("If this is checked then the trigger axis will be forced to 0f when the trigger button reports an untouch event.")]
		public bool triggerAxisZeroOnUntouch;

		[Tooltip("The level on the grip axis to reach before a click is registered.")]
		public float gripClickThreshold = 1f;

		[Tooltip("The level on the grip axis to reach before the axis is forced to 0f.")]
		public float gripForceZeroThreshold = 0.01f;

		[Tooltip("If this is checked then the grip axis will be forced to 0f when the grip button reports an untouch event.")]
		public bool gripAxisZeroOnUntouch;

		[HideInInspector]
		public bool triggerPressed;

		[HideInInspector]
		public bool triggerTouched;

		[HideInInspector]
		public bool triggerHairlinePressed;

		[HideInInspector]
		public bool triggerClicked;

		[HideInInspector]
		public bool triggerAxisChanged;

		[HideInInspector]
		public bool gripPressed;

		[HideInInspector]
		public bool gripTouched;

		[HideInInspector]
		public bool gripHairlinePressed;

		[HideInInspector]
		public bool gripClicked;

		[HideInInspector]
		public bool gripAxisChanged;

		[HideInInspector]
		public bool touchpadPressed;

		[HideInInspector]
		public bool touchpadTouched;

		[HideInInspector]
		public bool touchpadAxisChanged;

		[HideInInspector]
		public bool buttonOnePressed;

		[HideInInspector]
		public bool buttonOneTouched;

		[HideInInspector]
		public bool buttonTwoPressed;

		[HideInInspector]
		public bool buttonTwoTouched;

		[HideInInspector]
		public bool startMenuPressed;

		[HideInInspector]
		[Obsolete("`VRTK_ControllerEvents.pointerPressed` is no longer used, use `VRTK_Pointer.IsActivationButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
		public bool pointerPressed;

		[HideInInspector]
		[Obsolete("`VRTK_ControllerEvents.grabPressed` is no longer used, use `VRTK_InteractGrab.IsGrabButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
		public bool grabPressed;

		[HideInInspector]
		[Obsolete("`VRTK_ControllerEvents.usePressed` is no longer used, use `VRTK_InteractUse.IsUseButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
		public bool usePressed;

		[HideInInspector]
		[Obsolete("`VRTK_ControllerEvents.uiClickPressed` is no longer used, use `VRTK_UIPointer.IsSelectionButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
		public bool uiClickPressed;

		[HideInInspector]
		[Obsolete("`VRTK_ControllerEvents.menuPressed` is no longer used, use `VRTK_ControllerEvents.buttonTwoPressed` instead. This parameter will be removed in a future version of VRTK.")]
		public bool menuPressed;

		[HideInInspector]
		public bool controllerVisible = true;

		protected Vector2 touchpadAxis = Vector2.zero;

		protected Vector2 triggerAxis = Vector2.zero;

		protected Vector2 gripAxis = Vector2.zero;

		protected float hairTriggerDelta;

		protected float hairGripDelta;

		public event ControllerInteractionEventHandler TriggerPressed;

		public event ControllerInteractionEventHandler TriggerReleased;

		public event ControllerInteractionEventHandler TriggerTouchStart;

		public event ControllerInteractionEventHandler TriggerTouchEnd;

		public event ControllerInteractionEventHandler TriggerHairlineStart;

		public event ControllerInteractionEventHandler TriggerHairlineEnd;

		public event ControllerInteractionEventHandler TriggerClicked;

		public event ControllerInteractionEventHandler TriggerUnclicked;

		public event ControllerInteractionEventHandler TriggerAxisChanged;

		public event ControllerInteractionEventHandler GripPressed;

		public event ControllerInteractionEventHandler GripReleased;

		public event ControllerInteractionEventHandler GripTouchStart;

		public event ControllerInteractionEventHandler GripTouchEnd;

		public event ControllerInteractionEventHandler GripHairlineStart;

		public event ControllerInteractionEventHandler GripHairlineEnd;

		public event ControllerInteractionEventHandler GripClicked;

		public event ControllerInteractionEventHandler GripUnclicked;

		public event ControllerInteractionEventHandler GripAxisChanged;

		public event ControllerInteractionEventHandler TouchpadPressed;

		public event ControllerInteractionEventHandler TouchpadReleased;

		public event ControllerInteractionEventHandler TouchpadTouchStart;

		public event ControllerInteractionEventHandler TouchpadTouchEnd;

		public event ControllerInteractionEventHandler TouchpadAxisChanged;

		public event ControllerInteractionEventHandler ButtonOneTouchStart;

		public event ControllerInteractionEventHandler ButtonOneTouchEnd;

		public event ControllerInteractionEventHandler ButtonOnePressed;

		public event ControllerInteractionEventHandler ButtonOneReleased;

		public event ControllerInteractionEventHandler ButtonTwoTouchStart;

		public event ControllerInteractionEventHandler ButtonTwoTouchEnd;

		public event ControllerInteractionEventHandler ButtonTwoPressed;

		public event ControllerInteractionEventHandler ButtonTwoReleased;

		public event ControllerInteractionEventHandler StartMenuPressed;

		public event ControllerInteractionEventHandler StartMenuReleased;

		[Obsolete("`VRTK_ControllerEvents.AliasPointerOn` has been replaced with `VRTK_Pointer.ActivationButtonPressed`. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasPointerOn;

		[Obsolete("`VRTK_ControllerEvents.AliasPointerOff` has been replaced with `VRTK_Pointer.ActivationButtonReleased`. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasPointerOff;

		[Obsolete("`VRTK_ControllerEvents.AliasPointerSet` has been replaced with `VRTK_Pointer.SelectionButtonReleased`. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasPointerSet;

		[Obsolete("`VRTK_ControllerEvents.AliasGrabOn` has been replaced with `VRTK_InteractGrab.GrabButtonPressed`. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasGrabOn;

		[Obsolete("`VRTK_ControllerEvents.AliasGrabOff` has been replaced with `VRTK_InteractGrab.GrabButtonReleased`. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasGrabOff;

		[Obsolete("`VRTK_ControllerEvents.AliasUseOn` has been replaced with `VRTK_InteractUse.UseButtonPressed`. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasUseOn;

		[Obsolete("`VRTK_ControllerEvents.AliasUseOff` has been replaced with `VRTK_InteractUse.UseButtonReleased`. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasUseOff;

		[Obsolete("`VRTK_ControllerEvents.AliasMenuOn` is no longer used, use `VRTK_ControllerEvents.ButtonTwoPressed` instead. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasMenuOn;

		[Obsolete("`VRTK_ControllerEvents.AliasMenuOff` is no longer used, use `VRTK_ControllerEvents.ButtonTwoReleased` instead. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasMenuOff;

		[Obsolete("`VRTK_ControllerEvents.AliasUIClickOn` has been replaced with `VRTK_UIPointer.SelectionButtonPressed`. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasUIClickOn;

		[Obsolete("`VRTK_ControllerEvents.AliasUIClickOff` has been replaced with `VRTK_UIPointer.SelectionButtonReleased`. This parameter will be removed in a future version of VRTK.")]
		public event ControllerInteractionEventHandler AliasUIClickOff;

		public event ControllerInteractionEventHandler ControllerEnabled;

		public event ControllerInteractionEventHandler ControllerDisabled;

		public event ControllerInteractionEventHandler ControllerIndexChanged;

		public event ControllerInteractionEventHandler ControllerVisible;

		public event ControllerInteractionEventHandler ControllerHidden;

		public virtual void OnTriggerPressed(ControllerInteractionEventArgs e)
		{
			if (this.TriggerPressed != null)
			{
				this.TriggerPressed(this, e);
			}
		}

		public virtual void OnTriggerReleased(ControllerInteractionEventArgs e)
		{
			if (this.TriggerReleased != null)
			{
				this.TriggerReleased(this, e);
			}
		}

		public virtual void OnTriggerTouchStart(ControllerInteractionEventArgs e)
		{
			if (this.TriggerTouchStart != null)
			{
				this.TriggerTouchStart(this, e);
			}
		}

		public virtual void OnTriggerTouchEnd(ControllerInteractionEventArgs e)
		{
			if (this.TriggerTouchEnd != null)
			{
				this.TriggerTouchEnd(this, e);
			}
		}

		public virtual void OnTriggerHairlineStart(ControllerInteractionEventArgs e)
		{
			if (this.TriggerHairlineStart != null)
			{
				this.TriggerHairlineStart(this, e);
			}
		}

		public virtual void OnTriggerHairlineEnd(ControllerInteractionEventArgs e)
		{
			if (this.TriggerHairlineEnd != null)
			{
				this.TriggerHairlineEnd(this, e);
			}
		}

		public virtual void OnTriggerClicked(ControllerInteractionEventArgs e)
		{
			if (this.TriggerClicked != null)
			{
				this.TriggerClicked(this, e);
			}
		}

		public virtual void OnTriggerUnclicked(ControllerInteractionEventArgs e)
		{
			if (this.TriggerUnclicked != null)
			{
				this.TriggerUnclicked(this, e);
			}
		}

		public virtual void OnTriggerAxisChanged(ControllerInteractionEventArgs e)
		{
			if (this.TriggerAxisChanged != null)
			{
				this.TriggerAxisChanged(this, e);
			}
		}

		public virtual void OnGripPressed(ControllerInteractionEventArgs e)
		{
			if (this.GripPressed != null)
			{
				this.GripPressed(this, e);
			}
		}

		public virtual void OnGripReleased(ControllerInteractionEventArgs e)
		{
			if (this.GripReleased != null)
			{
				this.GripReleased(this, e);
			}
		}

		public virtual void OnGripTouchStart(ControllerInteractionEventArgs e)
		{
			if (this.GripTouchStart != null)
			{
				this.GripTouchStart(this, e);
			}
		}

		public virtual void OnGripTouchEnd(ControllerInteractionEventArgs e)
		{
			if (this.GripTouchEnd != null)
			{
				this.GripTouchEnd(this, e);
			}
		}

		public virtual void OnGripHairlineStart(ControllerInteractionEventArgs e)
		{
			if (this.GripHairlineStart != null)
			{
				this.GripHairlineStart(this, e);
			}
		}

		public virtual void OnGripHairlineEnd(ControllerInteractionEventArgs e)
		{
			if (this.GripHairlineEnd != null)
			{
				this.GripHairlineEnd(this, e);
			}
		}

		public virtual void OnGripClicked(ControllerInteractionEventArgs e)
		{
			if (this.GripClicked != null)
			{
				this.GripClicked(this, e);
			}
		}

		public virtual void OnGripUnclicked(ControllerInteractionEventArgs e)
		{
			if (this.GripUnclicked != null)
			{
				this.GripUnclicked(this, e);
			}
		}

		public virtual void OnGripAxisChanged(ControllerInteractionEventArgs e)
		{
			if (this.GripAxisChanged != null)
			{
				this.GripAxisChanged(this, e);
			}
		}

		public virtual void OnTouchpadPressed(ControllerInteractionEventArgs e)
		{
			if (this.TouchpadPressed != null)
			{
				this.TouchpadPressed(this, e);
			}
		}

		public virtual void OnTouchpadReleased(ControllerInteractionEventArgs e)
		{
			if (this.TouchpadReleased != null)
			{
				this.TouchpadReleased(this, e);
			}
		}

		public virtual void OnTouchpadTouchStart(ControllerInteractionEventArgs e)
		{
			if (this.TouchpadTouchStart != null)
			{
				this.TouchpadTouchStart(this, e);
			}
		}

		public virtual void OnTouchpadTouchEnd(ControllerInteractionEventArgs e)
		{
			if (this.TouchpadTouchEnd != null)
			{
				this.TouchpadTouchEnd(this, e);
			}
		}

		public virtual void OnTouchpadAxisChanged(ControllerInteractionEventArgs e)
		{
			if (this.TouchpadAxisChanged != null)
			{
				this.TouchpadAxisChanged(this, e);
			}
		}

		public virtual void OnButtonOneTouchStart(ControllerInteractionEventArgs e)
		{
			if (this.ButtonOneTouchStart != null)
			{
				this.ButtonOneTouchStart(this, e);
			}
		}

		public virtual void OnButtonOneTouchEnd(ControllerInteractionEventArgs e)
		{
			if (this.ButtonOneTouchEnd != null)
			{
				this.ButtonOneTouchEnd(this, e);
			}
		}

		public virtual void OnButtonOnePressed(ControllerInteractionEventArgs e)
		{
			if (this.ButtonOnePressed != null)
			{
				this.ButtonOnePressed(this, e);
			}
		}

		public virtual void OnButtonOneReleased(ControllerInteractionEventArgs e)
		{
			if (this.ButtonOneReleased != null)
			{
				this.ButtonOneReleased(this, e);
			}
		}

		public virtual void OnButtonTwoTouchStart(ControllerInteractionEventArgs e)
		{
			if (this.ButtonTwoTouchStart != null)
			{
				this.ButtonTwoTouchStart(this, e);
			}
		}

		public virtual void OnButtonTwoTouchEnd(ControllerInteractionEventArgs e)
		{
			if (this.ButtonTwoTouchEnd != null)
			{
				this.ButtonTwoTouchEnd(this, e);
			}
		}

		public virtual void OnButtonTwoPressed(ControllerInteractionEventArgs e)
		{
			if (this.ButtonTwoPressed != null)
			{
				this.ButtonTwoPressed(this, e);
			}
		}

		public virtual void OnButtonTwoReleased(ControllerInteractionEventArgs e)
		{
			if (this.ButtonTwoReleased != null)
			{
				this.ButtonTwoReleased(this, e);
			}
		}

		public virtual void OnStartMenuPressed(ControllerInteractionEventArgs e)
		{
			if (this.StartMenuPressed != null)
			{
				this.StartMenuPressed(this, e);
			}
		}

		public virtual void OnStartMenuReleased(ControllerInteractionEventArgs e)
		{
			if (this.StartMenuReleased != null)
			{
				this.StartMenuReleased(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasPointerOn` has been replaced with `VRTK_Pointer.OnActivationButtonPressed`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasPointerOn(ControllerInteractionEventArgs e)
		{
			if (this.AliasPointerOn != null)
			{
				this.AliasPointerOn(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasPointerOff` has been replaced with `VRTK_Pointer.OnActivationButtonReleased`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasPointerOff(ControllerInteractionEventArgs e)
		{
			if (this.AliasPointerOff != null)
			{
				this.AliasPointerOff(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasPointerSet` has been replaced with `VRTK_Pointer.OnSelectionButtonReleased`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasPointerSet(ControllerInteractionEventArgs e)
		{
			if (this.AliasPointerSet != null)
			{
				this.AliasPointerSet(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasGrabOn` has been replaced with `VRTK_InteractGrab.OnGrabButtonPressed`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasGrabOn(ControllerInteractionEventArgs e)
		{
			if (this.AliasGrabOn != null)
			{
				this.AliasGrabOn(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasGrabOff` has been replaced with `VRTK_InteractGrab.OnGrabButtonReleased`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasGrabOff(ControllerInteractionEventArgs e)
		{
			if (this.AliasGrabOff != null)
			{
				this.AliasGrabOff(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasUseOn` has been replaced with `VRTK_InteractUse.OnUseButtonPressed`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasUseOn(ControllerInteractionEventArgs e)
		{
			if (this.AliasUseOn != null)
			{
				this.AliasUseOn(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasUseOff` has been replaced with `VRTK_InteractUse.OnUseButtonReleased`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasUseOff(ControllerInteractionEventArgs e)
		{
			if (this.AliasUseOff != null)
			{
				this.AliasUseOff(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasUIClickOn` has been replaced with `VRTK_UIPointer.OnSelectionButtonPressed`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasUIClickOn(ControllerInteractionEventArgs e)
		{
			if (this.AliasUIClickOn != null)
			{
				this.AliasUIClickOn(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasUIClickOff` has been replaced with `VRTK_UIPointer.OnSelectionButtonReleased`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasUIClickOff(ControllerInteractionEventArgs e)
		{
			if (this.AliasUIClickOff != null)
			{
				this.AliasUIClickOff(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasMenuOn` has been replaced with `VRTK_ControllerEvents.OnButtonTwoPressed`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasMenuOn(ControllerInteractionEventArgs e)
		{
			if (this.AliasMenuOn != null)
			{
				this.AliasMenuOn(this, e);
			}
		}

		[Obsolete("`VRTK_ControllerEvents.OnAliasMenuOff` has been replaced with `VRTK_ControllerEvents.OnButtonTwoReleased`. This method will be removed in a future version of VRTK.")]
		public virtual void OnAliasMenuOff(ControllerInteractionEventArgs e)
		{
			if (this.AliasMenuOff != null)
			{
				this.AliasMenuOff(this, e);
			}
		}

		public virtual void OnControllerEnabled(ControllerInteractionEventArgs e)
		{
			if (this.ControllerEnabled != null)
			{
				this.ControllerEnabled(this, e);
			}
		}

		public virtual void OnControllerDisabled(ControllerInteractionEventArgs e)
		{
			if (this.ControllerDisabled != null)
			{
				this.ControllerDisabled(this, e);
			}
		}

		public virtual void OnControllerIndexChanged(ControllerInteractionEventArgs e)
		{
			if (this.ControllerIndexChanged != null)
			{
				this.ControllerIndexChanged(this, e);
			}
		}

		public virtual void OnControllerVisible(ControllerInteractionEventArgs e)
		{
			controllerVisible = true;
			if (this.ControllerVisible != null)
			{
				this.ControllerVisible(this, e);
			}
		}

		public virtual void OnControllerHidden(ControllerInteractionEventArgs e)
		{
			controllerVisible = false;
			if (this.ControllerHidden != null)
			{
				this.ControllerHidden(this, e);
			}
		}

		public virtual ControllerInteractionEventArgs SetControllerEvent()
		{
			bool buttonBool = false;
			return SetControllerEvent(ref buttonBool);
		}

		public virtual ControllerInteractionEventArgs SetControllerEvent(ref bool buttonBool, bool value = false, float buttonPressure = 0f)
		{
			uint controllerIndex = VRTK_DeviceFinder.GetControllerIndex(base.gameObject);
			buttonBool = value;
			ControllerInteractionEventArgs result = default(ControllerInteractionEventArgs);
			result.controllerIndex = controllerIndex;
			result.buttonPressure = buttonPressure;
			result.touchpadAxis = VRTK_SDK_Bridge.GetTouchpadAxisOnIndex(controllerIndex);
			result.touchpadAngle = CalculateTouchpadAxisAngle(result.touchpadAxis);
			return result;
		}

		[Obsolete("`VRTK_ControllerEvents.GetVelocity()` has been replaced with `VRTK_DeviceFinder.GetControllerVelocity(givenController)`. This method will be removed in a future version of VRTK.")]
		public virtual Vector3 GetVelocity()
		{
			return VRTK_DeviceFinder.GetControllerVelocity(base.gameObject);
		}

		[Obsolete("`VRTK_ControllerEvents.GetAngularVelocity()` has been replaced with `VRTK_DeviceFinder.GetControllerAngularVelocity(givenController)`. This method will be removed in a future version of VRTK.")]
		public virtual Vector3 GetAngularVelocity()
		{
			return VRTK_DeviceFinder.GetControllerAngularVelocity(base.gameObject);
		}

		public virtual Vector2 GetTouchpadAxis()
		{
			return touchpadAxis;
		}

		public virtual float GetTouchpadAxisAngle()
		{
			return CalculateTouchpadAxisAngle(touchpadAxis);
		}

		public virtual float GetTriggerAxis()
		{
			return triggerAxis.x;
		}

		public virtual float GetGripAxis()
		{
			return gripAxis.x;
		}

		public virtual float GetHairTriggerDelta()
		{
			return hairTriggerDelta;
		}

		public virtual float GetHairGripDelta()
		{
			return hairGripDelta;
		}

		public virtual bool AnyButtonPressed()
		{
			return triggerPressed || gripPressed || touchpadPressed || buttonOnePressed || buttonTwoPressed || startMenuPressed;
		}

		public virtual bool IsButtonPressed(ButtonAlias button)
		{
			switch (button)
			{
			case ButtonAlias.TriggerHairline:
				return triggerHairlinePressed;
			case ButtonAlias.TriggerTouch:
				return triggerTouched;
			case ButtonAlias.TriggerPress:
				return triggerPressed;
			case ButtonAlias.TriggerClick:
				return triggerClicked;
			case ButtonAlias.GripHairline:
				return gripHairlinePressed;
			case ButtonAlias.GripTouch:
				return gripTouched;
			case ButtonAlias.GripPress:
				return gripPressed;
			case ButtonAlias.GripClick:
				return gripClicked;
			case ButtonAlias.TouchpadTouch:
				return touchpadTouched;
			case ButtonAlias.TouchpadPress:
				return touchpadPressed;
			case ButtonAlias.ButtonOnePress:
				return buttonOnePressed;
			case ButtonAlias.ButtonOneTouch:
				return buttonOneTouched;
			case ButtonAlias.ButtonTwoPress:
				return buttonTwoPressed;
			case ButtonAlias.ButtonTwoTouch:
				return buttonTwoTouched;
			case ButtonAlias.StartMenuPress:
				return startMenuPressed;
			default:
				return false;
			}
		}

		public virtual void SubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
		{
			ButtonAliasEventSubscription(true, givenButton, startEvent, callbackMethod);
		}

		public virtual void UnsubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
		{
			ButtonAliasEventSubscription(false, givenButton, startEvent, callbackMethod);
		}

		protected virtual void OnEnable()
		{
			GameObject actualController = VRTK_DeviceFinder.GetActualController(base.gameObject);
			if ((bool)actualController)
			{
				VRTK_TrackedController component = actualController.GetComponent<VRTK_TrackedController>();
				if ((bool)component)
				{
					component.ControllerEnabled += TrackedControllerEnabled;
					component.ControllerDisabled += TrackedControllerDisabled;
					component.ControllerIndexChanged += TrackedControllerIndexChanged;
				}
			}
		}

		protected virtual void OnDisable()
		{
			Invoke("DisableEvents", 0f);
			GameObject actualController = VRTK_DeviceFinder.GetActualController(base.gameObject);
			if ((bool)actualController)
			{
				VRTK_TrackedController component = actualController.GetComponent<VRTK_TrackedController>();
				if ((bool)component)
				{
					component.ControllerEnabled -= TrackedControllerEnabled;
					component.ControllerDisabled -= TrackedControllerDisabled;
				}
			}
		}

		protected virtual void Update()
		{
			uint controllerIndex = VRTK_DeviceFinder.GetControllerIndex(base.gameObject);
			if (controllerIndex < uint.MaxValue)
			{
				Vector2 triggerAxisOnIndex = VRTK_SDK_Bridge.GetTriggerAxisOnIndex(controllerIndex);
				Vector2 gripAxisOnIndex = VRTK_SDK_Bridge.GetGripAxisOnIndex(controllerIndex);
				Vector2 touchpadAxisOnIndex = VRTK_SDK_Bridge.GetTouchpadAxisOnIndex(controllerIndex);
				if (VRTK_SDK_Bridge.IsTriggerTouchedDownOnIndex(controllerIndex))
				{
					OnTriggerTouchStart(SetControllerEvent(ref triggerTouched, true, triggerAxisOnIndex.x));
					EmitAlias(ButtonAlias.TriggerTouch, true, triggerAxisOnIndex.x, ref triggerTouched);
				}
				if (VRTK_SDK_Bridge.IsHairTriggerDownOnIndex(controllerIndex))
				{
					OnTriggerHairlineStart(SetControllerEvent(ref triggerHairlinePressed, true, triggerAxisOnIndex.x));
					EmitAlias(ButtonAlias.TriggerHairline, true, triggerAxisOnIndex.x, ref triggerHairlinePressed);
				}
				if (VRTK_SDK_Bridge.IsTriggerPressedDownOnIndex(controllerIndex))
				{
					OnTriggerPressed(SetControllerEvent(ref triggerPressed, true, triggerAxisOnIndex.x));
					EmitAlias(ButtonAlias.TriggerPress, true, triggerAxisOnIndex.x, ref triggerPressed);
				}
				if (!triggerClicked && triggerAxisOnIndex.x >= triggerClickThreshold)
				{
					OnTriggerClicked(SetControllerEvent(ref triggerClicked, true, triggerAxisOnIndex.x));
					EmitAlias(ButtonAlias.TriggerClick, true, triggerAxisOnIndex.x, ref triggerClicked);
				}
				else if (triggerClicked && triggerAxisOnIndex.x < triggerClickThreshold)
				{
					OnTriggerUnclicked(SetControllerEvent(ref triggerClicked));
					EmitAlias(ButtonAlias.TriggerClick, false, 0f, ref triggerClicked);
				}
				if (VRTK_SDK_Bridge.IsTriggerPressedUpOnIndex(controllerIndex))
				{
					OnTriggerReleased(SetControllerEvent(ref triggerPressed));
					EmitAlias(ButtonAlias.TriggerPress, false, 0f, ref triggerPressed);
				}
				if (VRTK_SDK_Bridge.IsHairTriggerUpOnIndex(controllerIndex))
				{
					OnTriggerHairlineEnd(SetControllerEvent(ref triggerHairlinePressed));
					EmitAlias(ButtonAlias.TriggerHairline, false, 0f, ref triggerHairlinePressed);
				}
				if (VRTK_SDK_Bridge.IsTriggerTouchedUpOnIndex(controllerIndex))
				{
					OnTriggerTouchEnd(SetControllerEvent(ref triggerTouched));
					EmitAlias(ButtonAlias.TriggerTouch, false, 0f, ref triggerTouched);
				}
				triggerAxisOnIndex.x = (((triggerTouched || !triggerAxisZeroOnUntouch) && !(triggerAxisOnIndex.x < triggerForceZeroThreshold)) ? triggerAxisOnIndex.x : 0f);
				if (Vector2ShallowEquals(triggerAxis, triggerAxisOnIndex))
				{
					triggerAxisChanged = false;
				}
				else
				{
					OnTriggerAxisChanged(SetControllerEvent(ref triggerAxisChanged, true, triggerAxisOnIndex.x));
				}
				if (VRTK_SDK_Bridge.IsGripTouchedDownOnIndex(controllerIndex))
				{
					OnGripTouchStart(SetControllerEvent(ref gripTouched, true, gripAxisOnIndex.x));
					EmitAlias(ButtonAlias.GripTouch, true, gripAxisOnIndex.x, ref gripTouched);
				}
				if (VRTK_SDK_Bridge.IsHairGripDownOnIndex(controllerIndex))
				{
					OnGripHairlineStart(SetControllerEvent(ref gripHairlinePressed, true, gripAxisOnIndex.x));
					EmitAlias(ButtonAlias.GripHairline, true, gripAxisOnIndex.x, ref gripHairlinePressed);
				}
				if (VRTK_SDK_Bridge.IsGripPressedDownOnIndex(controllerIndex))
				{
					OnGripPressed(SetControllerEvent(ref gripPressed, true, gripAxisOnIndex.x));
					EmitAlias(ButtonAlias.GripPress, true, gripAxisOnIndex.x, ref gripPressed);
				}
				if (!gripClicked && gripAxisOnIndex.x >= gripClickThreshold)
				{
					OnGripClicked(SetControllerEvent(ref gripClicked, true, gripAxisOnIndex.x));
					EmitAlias(ButtonAlias.GripClick, true, gripAxisOnIndex.x, ref gripClicked);
				}
				else if (gripClicked && gripAxisOnIndex.x < gripClickThreshold)
				{
					OnGripUnclicked(SetControllerEvent(ref gripClicked));
					EmitAlias(ButtonAlias.GripClick, false, 0f, ref gripClicked);
				}
				if (VRTK_SDK_Bridge.IsGripPressedUpOnIndex(controllerIndex))
				{
					OnGripReleased(SetControllerEvent(ref gripPressed));
					EmitAlias(ButtonAlias.GripPress, false, 0f, ref gripPressed);
				}
				if (VRTK_SDK_Bridge.IsHairGripUpOnIndex(controllerIndex))
				{
					OnGripHairlineEnd(SetControllerEvent(ref gripHairlinePressed));
					EmitAlias(ButtonAlias.GripHairline, false, 0f, ref gripHairlinePressed);
				}
				if (VRTK_SDK_Bridge.IsGripTouchedUpOnIndex(controllerIndex))
				{
					OnGripTouchEnd(SetControllerEvent(ref gripTouched));
					EmitAlias(ButtonAlias.GripTouch, false, 0f, ref gripTouched);
				}
				gripAxisOnIndex.x = (((gripTouched || !gripAxisZeroOnUntouch) && !(gripAxisOnIndex.x < gripForceZeroThreshold)) ? gripAxisOnIndex.x : 0f);
				if (Vector2ShallowEquals(gripAxis, gripAxisOnIndex))
				{
					gripAxisChanged = false;
				}
				else
				{
					OnGripAxisChanged(SetControllerEvent(ref gripAxisChanged, true, gripAxisOnIndex.x));
				}
				if (VRTK_SDK_Bridge.IsTouchpadTouchedDownOnIndex(controllerIndex))
				{
					OnTouchpadTouchStart(SetControllerEvent(ref touchpadTouched, true, 1f));
					EmitAlias(ButtonAlias.TouchpadTouch, true, 1f, ref touchpadTouched);
				}
				if (VRTK_SDK_Bridge.IsTouchpadPressedDownOnIndex(controllerIndex))
				{
					OnTouchpadPressed(SetControllerEvent(ref touchpadPressed, true, 1f));
					EmitAlias(ButtonAlias.TouchpadPress, true, 1f, ref touchpadPressed);
				}
				else if (VRTK_SDK_Bridge.IsTouchpadPressedUpOnIndex(controllerIndex))
				{
					OnTouchpadReleased(SetControllerEvent(ref touchpadPressed));
					EmitAlias(ButtonAlias.TouchpadPress, false, 0f, ref touchpadPressed);
				}
				if (VRTK_SDK_Bridge.IsTouchpadTouchedUpOnIndex(controllerIndex))
				{
					OnTouchpadTouchEnd(SetControllerEvent(ref touchpadTouched));
					EmitAlias(ButtonAlias.TouchpadTouch, false, 0f, ref touchpadTouched);
					touchpadAxis = Vector2.zero;
				}
				if (!touchpadTouched || Vector2ShallowEquals(touchpadAxis, touchpadAxisOnIndex))
				{
					touchpadAxisChanged = false;
				}
				else
				{
					OnTouchpadAxisChanged(SetControllerEvent(ref touchpadAxisChanged, true, 1f));
				}
				if (VRTK_SDK_Bridge.IsButtonOneTouchedDownOnIndex(controllerIndex))
				{
					OnButtonOneTouchStart(SetControllerEvent(ref buttonOneTouched, true, 1f));
					EmitAlias(ButtonAlias.ButtonOneTouch, true, 1f, ref buttonOneTouched);
				}
				if (VRTK_SDK_Bridge.IsButtonOnePressedDownOnIndex(controllerIndex))
				{
					OnButtonOnePressed(SetControllerEvent(ref buttonOnePressed, true, 1f));
					EmitAlias(ButtonAlias.ButtonOnePress, true, 1f, ref buttonOnePressed);
				}
				else if (VRTK_SDK_Bridge.IsButtonOnePressedUpOnIndex(controllerIndex))
				{
					OnButtonOneReleased(SetControllerEvent(ref buttonOnePressed));
					EmitAlias(ButtonAlias.ButtonOnePress, false, 0f, ref buttonOnePressed);
				}
				if (VRTK_SDK_Bridge.IsButtonOneTouchedUpOnIndex(controllerIndex))
				{
					OnButtonOneTouchEnd(SetControllerEvent(ref buttonOneTouched));
					EmitAlias(ButtonAlias.ButtonOneTouch, false, 0f, ref buttonOneTouched);
				}
				if (VRTK_SDK_Bridge.IsButtonTwoTouchedDownOnIndex(controllerIndex))
				{
					OnButtonTwoTouchStart(SetControllerEvent(ref buttonTwoTouched, true, 1f));
					EmitAlias(ButtonAlias.ButtonTwoTouch, true, 1f, ref buttonTwoTouched);
				}
				if (VRTK_SDK_Bridge.IsButtonTwoPressedDownOnIndex(controllerIndex))
				{
					OnButtonTwoPressed(SetControllerEvent(ref buttonTwoPressed, true, 1f));
					EmitAlias(ButtonAlias.ButtonTwoPress, true, 1f, ref buttonTwoPressed);
				}
				else if (VRTK_SDK_Bridge.IsButtonTwoPressedUpOnIndex(controllerIndex))
				{
					OnButtonTwoReleased(SetControllerEvent(ref buttonTwoPressed));
					EmitAlias(ButtonAlias.ButtonTwoPress, false, 0f, ref buttonTwoPressed);
				}
				if (VRTK_SDK_Bridge.IsButtonTwoTouchedUpOnIndex(controllerIndex))
				{
					OnButtonTwoTouchEnd(SetControllerEvent(ref buttonTwoTouched));
					EmitAlias(ButtonAlias.ButtonTwoTouch, false, 0f, ref buttonTwoTouched);
				}
				if (VRTK_SDK_Bridge.IsStartMenuPressedDownOnIndex(controllerIndex))
				{
					OnStartMenuPressed(SetControllerEvent(ref startMenuPressed, true, 1f));
					EmitAlias(ButtonAlias.StartMenuPress, true, 1f, ref startMenuPressed);
				}
				else if (VRTK_SDK_Bridge.IsStartMenuPressedUpOnIndex(controllerIndex))
				{
					OnStartMenuReleased(SetControllerEvent(ref startMenuPressed));
					EmitAlias(ButtonAlias.StartMenuPress, false, 0f, ref startMenuPressed);
				}
				touchpadAxis = ((!touchpadAxisChanged) ? touchpadAxis : new Vector2(touchpadAxisOnIndex.x, touchpadAxisOnIndex.y));
				triggerAxis = ((!triggerAxisChanged) ? triggerAxis : new Vector2(triggerAxisOnIndex.x, triggerAxisOnIndex.y));
				gripAxis = ((!gripAxisChanged) ? gripAxis : new Vector2(gripAxisOnIndex.x, gripAxisOnIndex.y));
				hairTriggerDelta = VRTK_SDK_Bridge.GetTriggerHairlineDeltaOnIndex(controllerIndex);
				hairGripDelta = VRTK_SDK_Bridge.GetGripHairlineDeltaOnIndex(controllerIndex);
			}
		}

		protected virtual void ButtonAliasEventSubscription(bool subscribe, ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
		{
			switch (givenButton)
			{
			case ButtonAlias.TriggerClick:
				if (subscribe)
				{
					if (startEvent)
					{
						TriggerClicked += callbackMethod;
					}
					else
					{
						TriggerUnclicked += callbackMethod;
					}
				}
				else if (startEvent)
				{
					TriggerClicked -= callbackMethod;
				}
				else
				{
					TriggerUnclicked -= callbackMethod;
				}
				break;
			case ButtonAlias.TriggerHairline:
				if (subscribe)
				{
					if (startEvent)
					{
						TriggerHairlineStart += callbackMethod;
					}
					else
					{
						TriggerHairlineEnd += callbackMethod;
					}
				}
				else if (startEvent)
				{
					TriggerHairlineStart -= callbackMethod;
				}
				else
				{
					TriggerHairlineEnd -= callbackMethod;
				}
				break;
			case ButtonAlias.TriggerPress:
				if (subscribe)
				{
					if (startEvent)
					{
						TriggerPressed += callbackMethod;
					}
					else
					{
						TriggerReleased += callbackMethod;
					}
				}
				else if (startEvent)
				{
					TriggerPressed -= callbackMethod;
				}
				else
				{
					TriggerReleased -= callbackMethod;
				}
				break;
			case ButtonAlias.TriggerTouch:
				if (subscribe)
				{
					if (startEvent)
					{
						TriggerTouchStart += callbackMethod;
					}
					else
					{
						TriggerTouchEnd += callbackMethod;
					}
				}
				else if (startEvent)
				{
					TriggerTouchStart -= callbackMethod;
				}
				else
				{
					TriggerTouchEnd -= callbackMethod;
				}
				break;
			case ButtonAlias.GripClick:
				if (subscribe)
				{
					if (startEvent)
					{
						GripClicked += callbackMethod;
					}
					else
					{
						GripUnclicked += callbackMethod;
					}
				}
				else if (startEvent)
				{
					GripClicked -= callbackMethod;
				}
				else
				{
					GripUnclicked -= callbackMethod;
				}
				break;
			case ButtonAlias.GripHairline:
				if (subscribe)
				{
					if (startEvent)
					{
						GripHairlineStart += callbackMethod;
					}
					else
					{
						GripHairlineEnd += callbackMethod;
					}
				}
				else if (startEvent)
				{
					GripHairlineStart -= callbackMethod;
				}
				else
				{
					GripHairlineEnd -= callbackMethod;
				}
				break;
			case ButtonAlias.GripPress:
				if (subscribe)
				{
					if (startEvent)
					{
						GripPressed += callbackMethod;
					}
					else
					{
						GripReleased += callbackMethod;
					}
				}
				else if (startEvent)
				{
					GripPressed -= callbackMethod;
				}
				else
				{
					GripReleased -= callbackMethod;
				}
				break;
			case ButtonAlias.GripTouch:
				if (subscribe)
				{
					if (startEvent)
					{
						GripTouchStart += callbackMethod;
					}
					else
					{
						GripTouchEnd += callbackMethod;
					}
				}
				else if (startEvent)
				{
					GripTouchStart -= callbackMethod;
				}
				else
				{
					GripTouchEnd -= callbackMethod;
				}
				break;
			case ButtonAlias.TouchpadPress:
				if (subscribe)
				{
					if (startEvent)
					{
						TouchpadPressed += callbackMethod;
					}
					else
					{
						TouchpadReleased += callbackMethod;
					}
				}
				else if (startEvent)
				{
					TouchpadPressed -= callbackMethod;
				}
				else
				{
					TouchpadReleased -= callbackMethod;
				}
				break;
			case ButtonAlias.TouchpadTouch:
				if (subscribe)
				{
					if (startEvent)
					{
						TouchpadTouchStart += callbackMethod;
					}
					else
					{
						TouchpadTouchEnd += callbackMethod;
					}
				}
				else if (startEvent)
				{
					TouchpadTouchStart -= callbackMethod;
				}
				else
				{
					TouchpadTouchEnd -= callbackMethod;
				}
				break;
			case ButtonAlias.ButtonOnePress:
				if (subscribe)
				{
					if (startEvent)
					{
						ButtonOnePressed += callbackMethod;
					}
					else
					{
						ButtonOneReleased += callbackMethod;
					}
				}
				else if (startEvent)
				{
					ButtonOnePressed -= callbackMethod;
				}
				else
				{
					ButtonOneReleased -= callbackMethod;
				}
				break;
			case ButtonAlias.ButtonOneTouch:
				if (subscribe)
				{
					if (startEvent)
					{
						ButtonOneTouchStart += callbackMethod;
					}
					else
					{
						ButtonOneTouchEnd += callbackMethod;
					}
				}
				else if (startEvent)
				{
					ButtonOneTouchStart -= callbackMethod;
				}
				else
				{
					ButtonOneTouchEnd -= callbackMethod;
				}
				break;
			case ButtonAlias.ButtonTwoPress:
				if (subscribe)
				{
					if (startEvent)
					{
						ButtonTwoPressed += callbackMethod;
					}
					else
					{
						ButtonTwoReleased += callbackMethod;
					}
				}
				else if (startEvent)
				{
					ButtonTwoPressed -= callbackMethod;
				}
				else
				{
					ButtonTwoReleased -= callbackMethod;
				}
				break;
			case ButtonAlias.ButtonTwoTouch:
				if (subscribe)
				{
					if (startEvent)
					{
						ButtonTwoTouchStart += callbackMethod;
					}
					else
					{
						ButtonTwoTouchEnd += callbackMethod;
					}
				}
				else if (startEvent)
				{
					ButtonTwoTouchStart -= callbackMethod;
				}
				else
				{
					ButtonTwoTouchEnd -= callbackMethod;
				}
				break;
			case ButtonAlias.StartMenuPress:
				if (subscribe)
				{
					if (startEvent)
					{
						StartMenuPressed += callbackMethod;
					}
					else
					{
						StartMenuReleased += callbackMethod;
					}
				}
				else if (startEvent)
				{
					StartMenuPressed -= callbackMethod;
				}
				else
				{
					StartMenuReleased -= callbackMethod;
				}
				break;
			}
		}

		protected virtual void TrackedControllerEnabled(object sender, VRTKTrackedControllerEventArgs e)
		{
			OnControllerEnabled(SetControllerEvent());
		}

		protected virtual void TrackedControllerDisabled(object sender, VRTKTrackedControllerEventArgs e)
		{
			DisableEvents();
			OnControllerDisabled(SetControllerEvent());
		}

		protected virtual void TrackedControllerIndexChanged(object sender, VRTKTrackedControllerEventArgs e)
		{
			OnControllerIndexChanged(SetControllerEvent());
		}

		protected virtual float CalculateTouchpadAxisAngle(Vector2 axis)
		{
			float num = Mathf.Atan2(axis.y, axis.x) * 57.29578f;
			num = 90f - num;
			if (num < 0f)
			{
				num += 360f;
			}
			return num;
		}

		protected virtual void EmitAlias(ButtonAlias type, bool touchDown, float buttonPressure, ref bool buttonBool)
		{
			if (pointerToggleButton == type)
			{
				if (touchDown)
				{
					pointerPressed = true;
					OnAliasPointerOn(SetControllerEvent(ref buttonBool, true, buttonPressure));
				}
				else
				{
					pointerPressed = false;
					OnAliasPointerOff(SetControllerEvent(ref buttonBool, false, buttonPressure));
				}
			}
			if (pointerSetButton == type && !touchDown)
			{
				OnAliasPointerSet(SetControllerEvent(ref buttonBool, false, buttonPressure));
			}
			if (grabToggleButton == type)
			{
				if (touchDown)
				{
					grabPressed = true;
					OnAliasGrabOn(SetControllerEvent(ref buttonBool, true, buttonPressure));
				}
				else
				{
					grabPressed = false;
					OnAliasGrabOff(SetControllerEvent(ref buttonBool, false, buttonPressure));
				}
			}
			if (useToggleButton == type)
			{
				if (touchDown)
				{
					usePressed = true;
					OnAliasUseOn(SetControllerEvent(ref buttonBool, true, buttonPressure));
				}
				else
				{
					usePressed = false;
					OnAliasUseOff(SetControllerEvent(ref buttonBool, false, buttonPressure));
				}
			}
			if (uiClickButton == type)
			{
				if (touchDown)
				{
					uiClickPressed = true;
					OnAliasUIClickOn(SetControllerEvent(ref buttonBool, true, buttonPressure));
				}
				else
				{
					uiClickPressed = false;
					OnAliasUIClickOff(SetControllerEvent(ref buttonBool, false, buttonPressure));
				}
			}
			if (menuToggleButton == type)
			{
				if (touchDown)
				{
					menuPressed = true;
					OnAliasMenuOn(SetControllerEvent(ref buttonBool, true, buttonPressure));
				}
				else
				{
					menuPressed = false;
					OnAliasMenuOff(SetControllerEvent(ref buttonBool, false, buttonPressure));
				}
			}
		}

		protected virtual bool Vector2ShallowEquals(Vector2 vectorA, Vector2 vectorB)
		{
			Vector2 vector = vectorA - vectorB;
			return Math.Round(Mathf.Abs(vector.x), axisFidelity, MidpointRounding.AwayFromZero) < 1.401298464324817E-45 && Math.Round(Mathf.Abs(vector.y), axisFidelity, MidpointRounding.AwayFromZero) < 1.401298464324817E-45;
		}

		protected virtual void DisableEvents()
		{
			if (triggerPressed)
			{
				OnTriggerReleased(SetControllerEvent(ref triggerPressed));
				EmitAlias(ButtonAlias.TriggerPress, false, 0f, ref triggerPressed);
			}
			if (triggerTouched)
			{
				OnTriggerTouchEnd(SetControllerEvent(ref triggerTouched));
				EmitAlias(ButtonAlias.TriggerTouch, false, 0f, ref triggerTouched);
			}
			if (triggerHairlinePressed)
			{
				OnTriggerHairlineEnd(SetControllerEvent(ref triggerHairlinePressed));
				EmitAlias(ButtonAlias.TriggerHairline, false, 0f, ref triggerHairlinePressed);
			}
			if (triggerClicked)
			{
				OnTriggerUnclicked(SetControllerEvent(ref triggerClicked));
				EmitAlias(ButtonAlias.TriggerClick, false, 0f, ref triggerClicked);
			}
			if (gripPressed)
			{
				OnGripReleased(SetControllerEvent(ref gripPressed));
				EmitAlias(ButtonAlias.GripPress, false, 0f, ref gripPressed);
			}
			if (gripTouched)
			{
				OnGripTouchEnd(SetControllerEvent(ref gripTouched));
				EmitAlias(ButtonAlias.GripTouch, false, 0f, ref gripTouched);
			}
			if (gripHairlinePressed)
			{
				OnGripHairlineEnd(SetControllerEvent(ref gripHairlinePressed));
				EmitAlias(ButtonAlias.GripHairline, false, 0f, ref gripHairlinePressed);
			}
			if (gripClicked)
			{
				OnGripUnclicked(SetControllerEvent(ref gripClicked));
				EmitAlias(ButtonAlias.GripClick, false, 0f, ref gripClicked);
			}
			if (touchpadPressed)
			{
				OnTouchpadReleased(SetControllerEvent(ref touchpadPressed));
				EmitAlias(ButtonAlias.TouchpadPress, false, 0f, ref touchpadPressed);
			}
			if (touchpadTouched)
			{
				OnTouchpadTouchEnd(SetControllerEvent(ref touchpadTouched));
				EmitAlias(ButtonAlias.TouchpadTouch, false, 0f, ref touchpadTouched);
			}
			if (buttonOnePressed)
			{
				OnButtonOneReleased(SetControllerEvent(ref buttonOnePressed));
				EmitAlias(ButtonAlias.ButtonOnePress, false, 0f, ref buttonOnePressed);
			}
			if (buttonOneTouched)
			{
				OnButtonOneTouchEnd(SetControllerEvent(ref buttonOneTouched));
				EmitAlias(ButtonAlias.ButtonOneTouch, false, 0f, ref buttonOneTouched);
			}
			if (buttonTwoPressed)
			{
				OnButtonTwoReleased(SetControllerEvent(ref buttonTwoPressed));
				EmitAlias(ButtonAlias.ButtonTwoPress, false, 0f, ref buttonTwoPressed);
			}
			if (buttonTwoTouched)
			{
				OnButtonTwoTouchEnd(SetControllerEvent(ref buttonTwoTouched));
				EmitAlias(ButtonAlias.ButtonTwoTouch, false, 0f, ref buttonTwoTouched);
			}
			if (startMenuPressed)
			{
				OnStartMenuReleased(SetControllerEvent(ref startMenuPressed));
				EmitAlias(ButtonAlias.StartMenuPress, false, 0f, ref startMenuPressed);
			}
			triggerAxisChanged = false;
			gripAxisChanged = false;
			touchpadAxisChanged = false;
			uint controllerIndex = VRTK_DeviceFinder.GetControllerIndex(base.gameObject);
			if (controllerIndex < uint.MaxValue)
			{
				Vector2 triggerAxisOnIndex = VRTK_SDK_Bridge.GetTriggerAxisOnIndex(controllerIndex);
				Vector2 gripAxisOnIndex = VRTK_SDK_Bridge.GetGripAxisOnIndex(controllerIndex);
				Vector2 touchpadAxisOnIndex = VRTK_SDK_Bridge.GetTouchpadAxisOnIndex(controllerIndex);
				touchpadAxis = new Vector2(touchpadAxisOnIndex.x, touchpadAxisOnIndex.y);
				triggerAxis = new Vector2(triggerAxisOnIndex.x, triggerAxisOnIndex.y);
				gripAxis = new Vector2(gripAxisOnIndex.x, gripAxisOnIndex.y);
				hairTriggerDelta = VRTK_SDK_Bridge.GetTriggerHairlineDeltaOnIndex(controllerIndex);
				hairGripDelta = VRTK_SDK_Bridge.GetGripHairlineDeltaOnIndex(controllerIndex);
			}
		}
	}
}
