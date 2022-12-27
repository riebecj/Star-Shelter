using UnityEngine;

namespace VRTK.Examples
{
	public class VRTK_ControllerEvents_ListenerExample : MonoBehaviour
	{
		private void Start()
		{
			if (GetComponent<VRTK_ControllerEvents>() == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerEvents_ListenerExample", "VRTK_ControllerEvents", "the same"));
				return;
			}
			GetComponent<VRTK_ControllerEvents>().TriggerPressed += DoTriggerPressed;
			GetComponent<VRTK_ControllerEvents>().TriggerReleased += DoTriggerReleased;
			GetComponent<VRTK_ControllerEvents>().TriggerTouchStart += DoTriggerTouchStart;
			GetComponent<VRTK_ControllerEvents>().TriggerTouchEnd += DoTriggerTouchEnd;
			GetComponent<VRTK_ControllerEvents>().TriggerHairlineStart += DoTriggerHairlineStart;
			GetComponent<VRTK_ControllerEvents>().TriggerHairlineEnd += DoTriggerHairlineEnd;
			GetComponent<VRTK_ControllerEvents>().TriggerClicked += DoTriggerClicked;
			GetComponent<VRTK_ControllerEvents>().TriggerUnclicked += DoTriggerUnclicked;
			GetComponent<VRTK_ControllerEvents>().TriggerAxisChanged += DoTriggerAxisChanged;
			GetComponent<VRTK_ControllerEvents>().GripPressed += DoGripPressed;
			GetComponent<VRTK_ControllerEvents>().GripReleased += DoGripReleased;
			GetComponent<VRTK_ControllerEvents>().GripTouchStart += DoGripTouchStart;
			GetComponent<VRTK_ControllerEvents>().GripTouchEnd += DoGripTouchEnd;
			GetComponent<VRTK_ControllerEvents>().GripHairlineStart += DoGripHairlineStart;
			GetComponent<VRTK_ControllerEvents>().GripHairlineEnd += DoGripHairlineEnd;
			GetComponent<VRTK_ControllerEvents>().GripClicked += DoGripClicked;
			GetComponent<VRTK_ControllerEvents>().GripUnclicked += DoGripUnclicked;
			GetComponent<VRTK_ControllerEvents>().GripAxisChanged += DoGripAxisChanged;
			GetComponent<VRTK_ControllerEvents>().TouchpadPressed += DoTouchpadPressed;
			GetComponent<VRTK_ControllerEvents>().TouchpadReleased += DoTouchpadReleased;
			GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart += DoTouchpadTouchStart;
			GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += DoTouchpadTouchEnd;
			GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += DoTouchpadAxisChanged;
			GetComponent<VRTK_ControllerEvents>().ButtonOnePressed += DoButtonOnePressed;
			GetComponent<VRTK_ControllerEvents>().ButtonOneReleased += DoButtonOneReleased;
			GetComponent<VRTK_ControllerEvents>().ButtonOneTouchStart += DoButtonOneTouchStart;
			GetComponent<VRTK_ControllerEvents>().ButtonOneTouchEnd += DoButtonOneTouchEnd;
			GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += DoButtonTwoPressed;
			GetComponent<VRTK_ControllerEvents>().ButtonTwoReleased += DoButtonTwoReleased;
			GetComponent<VRTK_ControllerEvents>().ButtonTwoTouchStart += DoButtonTwoTouchStart;
			GetComponent<VRTK_ControllerEvents>().ButtonTwoTouchEnd += DoButtonTwoTouchEnd;
			GetComponent<VRTK_ControllerEvents>().StartMenuPressed += DoStartMenuPressed;
			GetComponent<VRTK_ControllerEvents>().StartMenuReleased += DoStartMenuReleased;
			GetComponent<VRTK_ControllerEvents>().ControllerEnabled += DoControllerEnabled;
			GetComponent<VRTK_ControllerEvents>().ControllerDisabled += DoControllerDisabled;
			GetComponent<VRTK_ControllerEvents>().ControllerIndexChanged += DoControllerIndexChanged;
		}

		private void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e)
		{
			VRTK_Logger.Info(string.Concat("Controller on index '", index, "' ", button, " has been ", action, " with a pressure of ", e.buttonPressure, " / trackpad axis at: ", e.touchpadAxis, " (", e.touchpadAngle, " degrees)"));
		}

		private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TRIGGER", "pressed", e);
		}

		private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TRIGGER", "released", e);
		}

		private void DoTriggerTouchStart(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TRIGGER", "touched", e);
		}

		private void DoTriggerTouchEnd(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TRIGGER", "untouched", e);
		}

		private void DoTriggerHairlineStart(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TRIGGER", "hairline start", e);
		}

		private void DoTriggerHairlineEnd(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TRIGGER", "hairline end", e);
		}

		private void DoTriggerClicked(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TRIGGER", "clicked", e);
		}

		private void DoTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TRIGGER", "unclicked", e);
		}

		private void DoTriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TRIGGER", "axis changed", e);
		}

		private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "GRIP", "pressed", e);
		}

		private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "GRIP", "released", e);
		}

		private void DoGripTouchStart(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "GRIP", "touched", e);
		}

		private void DoGripTouchEnd(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "GRIP", "untouched", e);
		}

		private void DoGripHairlineStart(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "GRIP", "hairline start", e);
		}

		private void DoGripHairlineEnd(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "GRIP", "hairline end", e);
		}

		private void DoGripClicked(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "GRIP", "clicked", e);
		}

		private void DoGripUnclicked(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "GRIP", "unclicked", e);
		}

		private void DoGripAxisChanged(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "GRIP", "axis changed", e);
		}

		private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TOUCHPAD", "pressed down", e);
		}

		private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TOUCHPAD", "released", e);
		}

		private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TOUCHPAD", "touched", e);
		}

		private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TOUCHPAD", "untouched", e);
		}

		private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "TOUCHPAD", "axis changed", e);
		}

		private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "BUTTON ONE", "pressed down", e);
		}

		private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "BUTTON ONE", "released", e);
		}

		private void DoButtonOneTouchStart(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "BUTTON ONE", "touched", e);
		}

		private void DoButtonOneTouchEnd(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "BUTTON ONE", "untouched", e);
		}

		private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "BUTTON TWO", "pressed down", e);
		}

		private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "BUTTON TWO", "released", e);
		}

		private void DoButtonTwoTouchStart(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "BUTTON TWO", "touched", e);
		}

		private void DoButtonTwoTouchEnd(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "BUTTON TWO", "untouched", e);
		}

		private void DoStartMenuPressed(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "START MENU", "pressed down", e);
		}

		private void DoStartMenuReleased(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "START MENU", "released", e);
		}

		private void DoControllerEnabled(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "CONTROLLER STATE", "ENABLED", e);
		}

		private void DoControllerDisabled(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "CONTROLLER STATE", "DISABLED", e);
		}

		private void DoControllerIndexChanged(object sender, ControllerInteractionEventArgs e)
		{
			DebugLogger(e.controllerIndex, "CONTROLLER STATE", "INDEX CHANGED", e);
		}
	}
}
