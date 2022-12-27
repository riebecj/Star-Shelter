using UnityEngine;

namespace VRTK
{
	public class VRTK_TouchpadControl : VRTK_ObjectControl
	{
		[Header("Touchpad Control Settings")]
		[Tooltip("An optional button that has to be engaged to allow the touchpad control to activate.")]
		public VRTK_ControllerEvents.ButtonAlias primaryActivationButton = VRTK_ControllerEvents.ButtonAlias.TouchpadTouch;

		[Tooltip("An optional button that when engaged will activate the modifier on the touchpad control action.")]
		public VRTK_ControllerEvents.ButtonAlias actionModifierButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;

		[Tooltip("Any input on the axis will be ignored if it is within this deadzone threshold. Between `0f` and `1f`.")]
		public Vector2 axisDeadzone = new Vector2(0.2f, 0.2f);

		protected bool touchpadFirstChange;

		protected bool otherTouchpadControlEnabledState;

		protected override void OnEnable()
		{
			base.OnEnable();
			touchpadFirstChange = true;
		}

		protected override void ControlFixedUpdate()
		{
			ModifierButtonActive();
			if (OutsideDeadzone(currentAxis.x, axisDeadzone.x) || currentAxis.x == 0f)
			{
				OnXAxisChanged(SetEventArguements(directionDevice.right, currentAxis.x, axisDeadzone.x));
			}
			if (OutsideDeadzone(currentAxis.y, axisDeadzone.y) || currentAxis.y == 0f)
			{
				OnYAxisChanged(SetEventArguements(directionDevice.forward, currentAxis.y, axisDeadzone.y));
			}
		}

		protected override VRTK_ObjectControl GetOtherControl()
		{
			GameObject gameObject = ((!VRTK_DeviceFinder.IsControllerLeftHand(base.gameObject)) ? VRTK_DeviceFinder.GetControllerLeftHand() : VRTK_DeviceFinder.GetControllerRightHand());
			if ((bool)gameObject)
			{
				return gameObject.GetComponent<VRTK_TouchpadControl>();
			}
			return null;
		}

		protected override void SetListeners(bool state)
		{
			if ((bool)controllerEvents)
			{
				if (state)
				{
					controllerEvents.TouchpadAxisChanged += TouchpadAxisChanged;
					controllerEvents.TouchpadTouchEnd += TouchpadTouchEnd;
				}
				else
				{
					controllerEvents.TouchpadAxisChanged -= TouchpadAxisChanged;
					controllerEvents.TouchpadTouchEnd -= TouchpadTouchEnd;
				}
			}
		}

		protected override bool IsInAction()
		{
			return ValidPrimaryButton() && TouchpadTouched();
		}

		protected virtual bool OutsideDeadzone(float axisValue, float deadzoneThreshold)
		{
			return axisValue > deadzoneThreshold || axisValue < 0f - deadzoneThreshold;
		}

		protected virtual bool ValidPrimaryButton()
		{
			return (bool)controllerEvents && (primaryActivationButton == VRTK_ControllerEvents.ButtonAlias.Undefined || controllerEvents.IsButtonPressed(primaryActivationButton));
		}

		protected virtual void ModifierButtonActive()
		{
			modifierActive = (bool)controllerEvents && actionModifierButton != 0 && controllerEvents.IsButtonPressed(actionModifierButton);
		}

		protected virtual bool TouchpadTouched()
		{
			return (bool)controllerEvents && controllerEvents.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.TouchpadTouch);
		}

		protected virtual void TouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
		{
			if (touchpadFirstChange && (bool)otherObjectControl && disableOtherControlsOnActive && e.touchpadAxis != Vector2.zero)
			{
				otherTouchpadControlEnabledState = otherObjectControl.enabled;
				otherObjectControl.enabled = false;
			}
			currentAxis = ((!ValidPrimaryButton()) ? Vector2.zero : e.touchpadAxis);
			if (currentAxis != Vector2.zero)
			{
				touchpadFirstChange = false;
			}
		}

		protected virtual void TouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
		{
			if ((bool)otherObjectControl && disableOtherControlsOnActive)
			{
				otherObjectControl.enabled = otherTouchpadControlEnabledState;
			}
			currentAxis = Vector2.zero;
			touchpadFirstChange = true;
		}
	}
}
