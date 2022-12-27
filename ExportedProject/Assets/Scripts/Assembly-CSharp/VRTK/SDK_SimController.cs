using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	[SDK_Description(typeof(SDK_SimSystem))]
	public class SDK_SimController : SDK_BaseController
	{
		private SDK_ControllerSim rightController;

		private SDK_ControllerSim leftController;

		private Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode>
		{
			{
				"Trigger",
				KeyCode.Mouse1
			},
			{
				"Grip",
				KeyCode.Mouse0
			},
			{
				"TouchpadPress",
				KeyCode.Q
			},
			{
				"ButtonOne",
				KeyCode.E
			},
			{
				"ButtonTwo",
				KeyCode.R
			},
			{
				"StartMenu",
				KeyCode.F
			},
			{
				"TouchModifier",
				KeyCode.T
			},
			{
				"HairTouchModifier",
				KeyCode.H
			}
		};

		protected const string RIGHT_HAND_CONTROLLER_NAME = "RightHand";

		protected const string LEFT_HAND_CONTROLLER_NAME = "LeftHand";

		public virtual void SetKeyMappings(Dictionary<string, KeyCode> givenKeyMappings)
		{
			keyMappings = givenKeyMappings;
		}

		public override void ProcessUpdate(uint index, Dictionary<string, object> options)
		{
		}

		public override void ProcessFixedUpdate(uint index, Dictionary<string, object> options)
		{
		}

		public override string GetControllerDefaultColliderPath(ControllerHand hand)
		{
			return "ControllerColliders/Simulator";
		}

		public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
		{
			string text = ((!fullPath) ? string.Empty : "/attach");
			switch (element)
			{
			case ControllerElements.AttachPoint:
				return string.Empty;
			case ControllerElements.Trigger:
				return string.Empty + text;
			case ControllerElements.GripLeft:
				return string.Empty + text;
			case ControllerElements.GripRight:
				return string.Empty + text;
			case ControllerElements.Touchpad:
				return string.Empty + text;
			case ControllerElements.ButtonOne:
				return string.Empty + text;
			case ControllerElements.SystemMenu:
				return string.Empty + text;
			case ControllerElements.Body:
				return string.Empty;
			default:
				return null;
			}
		}

		public override uint GetControllerIndex(GameObject controller)
		{
			uint result = 0u;
			switch (controller.name)
			{
			case "Camera":
				result = 0u;
				break;
			case "RightController":
				result = 1u;
				break;
			case "LeftController":
				result = 2u;
				break;
			}
			return result;
		}

		public override GameObject GetControllerByIndex(uint index, bool actual = false)
		{
			switch (index)
			{
			case 1u:
				return rightController.gameObject;
			case 2u:
				return leftController.gameObject;
			default:
				return null;
			}
		}

		public override Transform GetControllerOrigin(GameObject controller)
		{
			return controller.transform;
		}

		public override Transform GenerateControllerPointerOrigin(GameObject parent)
		{
			return null;
		}

		public override GameObject GetControllerLeftHand(bool actual = false)
		{
			GameObject gameObject = GetSDKManagerControllerLeftHand(actual);
			if (!gameObject && actual)
			{
				gameObject = GetActualController(ControllerHand.Left);
			}
			return gameObject;
		}

		public override GameObject GetControllerRightHand(bool actual = false)
		{
			GameObject gameObject = GetSDKManagerControllerRightHand(actual);
			if (!gameObject && actual)
			{
				gameObject = GetActualController(ControllerHand.Right);
			}
			return gameObject;
		}

		private static GameObject GetActualController(ControllerHand hand)
		{
			GameObject gameObject = SDK_InputSimulator.FindInScene();
			GameObject result = null;
			if (gameObject != null)
			{
				switch (hand)
				{
				case ControllerHand.Right:
					result = gameObject.transform.Find("RightHand").gameObject;
					break;
				case ControllerHand.Left:
					result = gameObject.transform.Find("LeftHand").gameObject;
					break;
				}
			}
			return result;
		}

		public override bool IsControllerLeftHand(GameObject controller)
		{
			return CheckActualOrScriptAliasControllerIsLeftHand(controller);
		}

		public override bool IsControllerRightHand(GameObject controller)
		{
			return CheckActualOrScriptAliasControllerIsRightHand(controller);
		}

		public override bool IsControllerLeftHand(GameObject controller, bool actual)
		{
			return CheckControllerLeftHand(controller, actual);
		}

		public override bool IsControllerRightHand(GameObject controller, bool actual)
		{
			return CheckControllerRightHand(controller, actual);
		}

		public override GameObject GetControllerModel(GameObject controller)
		{
			return GetControllerModelFromController(controller);
		}

		public override GameObject GetControllerModel(ControllerHand hand)
		{
			GameObject result = null;
			GameObject gameObject = SDK_InputSimulator.FindInScene();
			if ((bool)gameObject)
			{
				switch (hand)
				{
				case ControllerHand.Left:
					result = gameObject.transform.Find(string.Format("{0}/Hand", "LeftHand")).gameObject;
					break;
				case ControllerHand.Right:
					result = gameObject.transform.Find(string.Format("{0}/Hand", "RightHand")).gameObject;
					break;
				}
			}
			return result;
		}

		public override GameObject GetControllerRenderModel(GameObject controller)
		{
			return controller.transform.parent.Find("Hand").gameObject;
		}

		public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
		{
		}

		public override void HapticPulseOnIndex(uint index, float strength = 0.5f)
		{
		}

		public override SDK_ControllerHapticModifiers GetHapticModifiers()
		{
			return new SDK_ControllerHapticModifiers();
		}

		public override Vector3 GetVelocityOnIndex(uint index)
		{
			switch (index)
			{
			case 1u:
				return rightController.GetVelocity();
			case 2u:
				return leftController.GetVelocity();
			default:
				return Vector3.zero;
			}
		}

		public override Vector3 GetAngularVelocityOnIndex(uint index)
		{
			switch (index)
			{
			case 1u:
				return rightController.GetAngularVelocity();
			case 2u:
				return leftController.GetAngularVelocity();
			default:
				return Vector3.zero;
			}
		}

		public override Vector2 GetTouchpadAxisOnIndex(uint index)
		{
			return Vector2.zero;
		}

		public override Vector2 GetTriggerAxisOnIndex(uint index)
		{
			return Vector2.zero;
		}

		public override Vector2 GetGripAxisOnIndex(uint index)
		{
			return Vector2.zero;
		}

		public override float GetTriggerHairlineDeltaOnIndex(uint index)
		{
			return 0f;
		}

		public override float GetGripHairlineDeltaOnIndex(uint index)
		{
			return 0f;
		}

		public override bool IsTriggerPressedOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsTriggerTouchedOnIndex(index);
		}

		public override bool IsTriggerPressedDownOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsTriggerTouchedDownOnIndex(index);
		}

		public override bool IsTriggerPressedUpOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsTriggerTouchedUpOnIndex(index);
		}

		public override bool IsTriggerTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["Trigger"]);
		}

		public override bool IsTriggerTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["Trigger"]);
		}

		public override bool IsTriggerTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["Trigger"]);
		}

		public override bool IsHairTriggerDownOnIndex(uint index)
		{
			return !IsButtonHairTouchIgnored() && IsTriggerTouchedDownOnIndex(index);
		}

		public override bool IsHairTriggerUpOnIndex(uint index)
		{
			return !IsButtonHairTouchIgnored() && IsTriggerTouchedUpOnIndex(index);
		}

		public override bool IsGripPressedOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsGripTouchedOnIndex(index);
		}

		public override bool IsGripPressedDownOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsGripTouchedDownOnIndex(index);
		}

		public override bool IsGripPressedUpOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsGripTouchedUpOnIndex(index);
		}

		public override bool IsGripTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["Grip"]);
		}

		public override bool IsGripTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["Grip"]);
		}

		public override bool IsGripTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["Grip"]);
		}

		public override bool IsHairGripDownOnIndex(uint index)
		{
			return !IsButtonHairTouchIgnored() && IsGripTouchedDownOnIndex(index);
		}

		public override bool IsHairGripUpOnIndex(uint index)
		{
			return !IsButtonHairTouchIgnored() && IsGripTouchedUpOnIndex(index);
		}

		public override bool IsTouchpadPressedOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsTouchpadTouchedOnIndex(index);
		}

		public override bool IsTouchpadPressedDownOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsTouchpadTouchedDownOnIndex(index);
		}

		public override bool IsTouchpadPressedUpOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsTouchpadTouchedUpOnIndex(index);
		}

		public override bool IsTouchpadTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["TouchpadPress"]);
		}

		public override bool IsTouchpadTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["TouchpadPress"]);
		}

		public override bool IsTouchpadTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["TouchpadPress"]);
		}

		public override bool IsButtonOnePressedOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsButtonOneTouchedOnIndex(index);
		}

		public override bool IsButtonOnePressedDownOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsButtonOneTouchedDownOnIndex(index);
		}

		public override bool IsButtonOnePressedUpOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsButtonOneTouchedUpOnIndex(index);
		}

		public override bool IsButtonOneTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["ButtonOne"]);
		}

		public override bool IsButtonOneTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["ButtonOne"]);
		}

		public override bool IsButtonOneTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["ButtonOne"]);
		}

		public override bool IsButtonTwoPressedOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsButtonTwoTouchedOnIndex(index);
		}

		public override bool IsButtonTwoPressedDownOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsButtonTwoTouchedDownOnIndex(index);
		}

		public override bool IsButtonTwoPressedUpOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsButtonTwoTouchedUpOnIndex(index);
		}

		public override bool IsButtonTwoTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["ButtonTwo"]);
		}

		public override bool IsButtonTwoTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["ButtonTwo"]);
		}

		public override bool IsButtonTwoTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["ButtonTwo"]);
		}

		public override bool IsStartMenuPressedOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsStartMenuTouchedOnIndex(index);
		}

		public override bool IsStartMenuPressedDownOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsStartMenuTouchedDownOnIndex(index);
		}

		public override bool IsStartMenuPressedUpOnIndex(uint index)
		{
			return !IsButtonPressIgnored() && IsStartMenuTouchedUpOnIndex(index);
		}

		public override bool IsStartMenuTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["StartMenu"]);
		}

		public override bool IsStartMenuTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["StartMenu"]);
		}

		public override bool IsStartMenuTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["StartMenu"]);
		}

		private void OnEnable()
		{
			GameObject gameObject = SDK_InputSimulator.FindInScene();
			if (gameObject != null)
			{
				rightController = gameObject.transform.Find("RightHand").GetComponent<SDK_ControllerSim>();
				leftController = gameObject.transform.Find("LeftHand").GetComponent<SDK_ControllerSim>();
			}
		}

		protected bool IsTouchModifierPressed()
		{
			return Input.GetKey(keyMappings["TouchModifier"]);
		}

		protected bool IsHairTouchModifierPressed()
		{
			return Input.GetKey(keyMappings["HairTouchModifier"]);
		}

		protected bool IsButtonPressIgnored()
		{
			return IsHairTouchModifierPressed() || IsTouchModifierPressed();
		}

		protected bool IsButtonHairTouchIgnored()
		{
			return IsTouchModifierPressed() && !IsHairTouchModifierPressed();
		}

		private bool IsButtonPressed(uint index, ButtonPressTypes type, KeyCode button)
		{
			switch (index)
			{
			case uint.MaxValue:
				return false;
			case 1u:
				if (!rightController.Selected)
				{
					return false;
				}
				break;
			case 2u:
				if (!leftController.Selected)
				{
					return false;
				}
				break;
			default:
				return false;
			}
			switch (type)
			{
			case ButtonPressTypes.Press:
				return Input.GetKey(button);
			case ButtonPressTypes.PressDown:
				return Input.GetKeyDown(button);
			case ButtonPressTypes.PressUp:
				return Input.GetKeyUp(button);
			default:
				return false;
			}
		}
	}
}
