using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	[SDK_Description(typeof(SDK_OculusVRSystem))]
	public class SDK_OculusVRController : SDK_BaseController
	{
		private SDK_OculusVRBoundaries cachedBoundariesSDK;

		private VRTK_TrackedController cachedLeftController;

		private VRTK_TrackedController cachedRightController;

		private OVRInput.Controller[] touchControllers = new OVRInput.Controller[2]
		{
			OVRInput.Controller.LTouch,
			OVRInput.Controller.RTouch
		};

		private OVRInput.RawAxis2D[] touchpads = new OVRInput.RawAxis2D[2]
		{
			OVRInput.RawAxis2D.LThumbstick,
			OVRInput.RawAxis2D.RThumbstick
		};

		private OVRInput.RawAxis1D[] triggers = new OVRInput.RawAxis1D[2]
		{
			OVRInput.RawAxis1D.LIndexTrigger,
			OVRInput.RawAxis1D.RIndexTrigger
		};

		private OVRInput.RawAxis1D[] grips = new OVRInput.RawAxis1D[2]
		{
			OVRInput.RawAxis1D.LHandTrigger,
			OVRInput.RawAxis1D.RHandTrigger
		};

		private Quaternion[] previousControllerRotations = new Quaternion[2];

		private Quaternion[] currentControllerRotations = new Quaternion[2];

		private bool[] previousHairTriggerState = new bool[2];

		private bool[] currentHairTriggerState = new bool[2];

		private bool[] previousHairGripState = new bool[2];

		private bool[] currentHairGripState = new bool[2];

		private float[] hairTriggerLimit = new float[2];

		private float[] hairGripLimit = new float[2];

		private OVRHapticsClip hapticsProceduralClipLeft = new OVRHapticsClip();

		private OVRHapticsClip hapticsProceduralClipRight = new OVRHapticsClip();

		public override void ProcessUpdate(uint index, Dictionary<string, object> options)
		{
		}

		public override void ProcessFixedUpdate(uint index, Dictionary<string, object> options)
		{
			CalculateAngularVelocity(index);
		}

		public override string GetControllerDefaultColliderPath(ControllerHand hand)
		{
			if (HasAvatar())
			{
				return "ControllerColliders/OculusTouch_" + hand;
			}
			return "ControllerColliders/Fallback";
		}

		public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
		{
			if ((bool)GetAvatar())
			{
				string text = ((!fullPath) ? string.Empty : string.Empty);
				string text2 = "controller_" + ((hand != ControllerHand.Left) ? "right" : "left") + "_renderPart_0";
				string text3 = ((hand != ControllerHand.Left) ? "r" : "l") + "ctrl:";
				string text4 = text3 + ((hand != ControllerHand.Left) ? "right" : "left") + "_touch_controller_world";
				string text5 = text2 + "/" + text4 + "/" + text3 + "b_";
				switch (element)
				{
				case ControllerElements.AttachPoint:
					return null;
				case ControllerElements.Trigger:
					return text5 + "trigger" + text;
				case ControllerElements.GripLeft:
					return text5 + "hold" + text;
				case ControllerElements.GripRight:
					return text5 + "hold" + text;
				case ControllerElements.Touchpad:
					return text5 + "stick/" + text3 + "b_stick_IGNORE" + text;
				case ControllerElements.ButtonOne:
					return text5 + "button01" + text;
				case ControllerElements.ButtonTwo:
					return text5 + "button02" + text;
				case ControllerElements.SystemMenu:
					return text5 + "button03" + text;
				case ControllerElements.StartMenu:
					return text5 + "button03" + text;
				case ControllerElements.Body:
					return text2;
				}
			}
			return null;
		}

		public override uint GetControllerIndex(GameObject controller)
		{
			VRTK_TrackedController trackedObject = GetTrackedObject(controller);
			return (!(trackedObject != null)) ? uint.MaxValue : trackedObject.index;
		}

		public override GameObject GetControllerByIndex(uint index, bool actual = false)
		{
			SetTrackedControllerCaches();
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null)
			{
				if (cachedLeftController != null && cachedLeftController.index == index)
				{
					return (!actual) ? instance.scriptAliasLeftController : instance.actualLeftController;
				}
				if (cachedRightController != null && cachedRightController.index == index)
				{
					return (!actual) ? instance.scriptAliasRightController : instance.actualRightController;
				}
			}
			return null;
		}

		public override Transform GetControllerOrigin(GameObject controller)
		{
			return VRTK_SDK_Bridge.GetPlayArea();
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
				gameObject = VRTK_SharedMethods.FindEvenInactiveGameObject<OVRCameraRig>("TrackingSpace/LeftHandAnchor");
			}
			return gameObject;
		}

		public override GameObject GetControllerRightHand(bool actual = false)
		{
			GameObject gameObject = GetSDKManagerControllerRightHand(actual);
			if (!gameObject && actual)
			{
				gameObject = VRTK_SharedMethods.FindEvenInactiveGameObject<OVRCameraRig>("TrackingSpace/RightHandAnchor");
			}
			return gameObject;
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
			GameObject gameObject = GetSDKManagerControllerModelForHand(hand);
			if (gameObject == null)
			{
				GameObject avatar = GetAvatar();
				switch (hand)
				{
				case ControllerHand.Left:
					if (avatar != null)
					{
						gameObject = avatar.transform.Find("controller_left").gameObject;
						break;
					}
					gameObject = GetControllerLeftHand(true);
					gameObject = ((!(gameObject != null) || gameObject.transform.childCount <= 0) ? null : gameObject.transform.GetChild(0).gameObject);
					break;
				case ControllerHand.Right:
					if (avatar != null)
					{
						gameObject = avatar.transform.Find("controller_right").gameObject;
						break;
					}
					gameObject = GetControllerRightHand(true);
					gameObject = ((!(gameObject != null) || gameObject.transform.childCount <= 0) ? null : gameObject.transform.GetChild(0).gameObject);
					break;
				}
			}
			return gameObject;
		}

		public override GameObject GetControllerRenderModel(GameObject controller)
		{
			return null;
		}

		public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
		{
		}

		public override void HapticPulseOnIndex(uint index, float strength = 0.5f)
		{
			if (index < uint.MaxValue)
			{
				GameObject controllerByIndex = GetControllerByIndex(index);
				if (IsControllerLeftHand(controllerByIndex))
				{
					hapticsProceduralClipLeft.Reset();
					hapticsProceduralClipLeft.WriteSample((byte)(strength * 255f));
					OVRHaptics.LeftChannel.Preempt(hapticsProceduralClipLeft);
				}
				else if (IsControllerRightHand(controllerByIndex))
				{
					hapticsProceduralClipRight.Reset();
					hapticsProceduralClipRight.WriteSample((byte)(strength * 255f));
					OVRHaptics.RightChannel.Preempt(hapticsProceduralClipRight);
				}
			}
		}

		public override SDK_ControllerHapticModifiers GetHapticModifiers()
		{
			SDK_ControllerHapticModifiers sDK_ControllerHapticModifiers = new SDK_ControllerHapticModifiers();
			sDK_ControllerHapticModifiers.durationModifier = 0.8f;
			sDK_ControllerHapticModifiers.intervalModifier = 1f;
			return sDK_ControllerHapticModifiers;
		}

		public override Vector3 GetVelocityOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return Vector3.zero;
			}
			VRTK_TrackedController trackedObject = GetTrackedObject(GetControllerByIndex(index));
			return OVRInput.GetLocalControllerVelocity(touchControllers[trackedObject.index]);
		}

		public override Vector3 GetAngularVelocityOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return Vector3.zero;
			}
			Quaternion quaternion = currentControllerRotations[index] * Quaternion.Inverse(previousControllerRotations[index]);
			return new Vector3(Mathf.DeltaAngle(0f, quaternion.eulerAngles.x), Mathf.DeltaAngle(0f, quaternion.eulerAngles.y), Mathf.DeltaAngle(0f, quaternion.eulerAngles.z));
		}

		public override Vector2 GetTouchpadAxisOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return Vector2.zero;
			}
			VRTK_TrackedController trackedObject = GetTrackedObject(GetControllerByIndex(index));
			return (!trackedObject) ? Vector2.zero : OVRInput.Get(touchpads[index], touchControllers[index]);
		}

		public override Vector2 GetTriggerAxisOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return Vector2.zero;
			}
			VRTK_TrackedController trackedObject = GetTrackedObject(GetControllerByIndex(index));
			if ((bool)trackedObject)
			{
				float x = OVRInput.Get(triggers[index], touchControllers[index]);
				return new Vector2(x, 0f);
			}
			return Vector2.zero;
		}

		public override Vector2 GetGripAxisOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return Vector2.zero;
			}
			VRTK_TrackedController trackedObject = GetTrackedObject(GetControllerByIndex(index));
			if ((bool)trackedObject)
			{
				float x = OVRInput.Get(grips[index], touchControllers[index]);
				return new Vector2(x, 0f);
			}
			return Vector2.zero;
		}

		public override float GetTriggerHairlineDeltaOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return 0f;
			}
			return 0.1f;
		}

		public override float GetGripHairlineDeltaOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return 0f;
			}
			return 0.1f;
		}

		public override bool IsTriggerPressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.PrimaryIndexTrigger);
		}

		public override bool IsTriggerPressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.PrimaryIndexTrigger);
		}

		public override bool IsTriggerPressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.PrimaryIndexTrigger);
		}

		public override bool IsTriggerTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, OVRInput.Touch.PrimaryIndexTrigger);
		}

		public override bool IsTriggerTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, OVRInput.Touch.PrimaryIndexTrigger);
		}

		public override bool IsTriggerTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, OVRInput.Touch.PrimaryIndexTrigger);
		}

		public override bool IsHairTriggerDownOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return false;
			}
			return currentHairTriggerState[index] && !previousHairTriggerState[index];
		}

		public override bool IsHairTriggerUpOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return false;
			}
			return !currentHairTriggerState[index] && previousHairTriggerState[index];
		}

		public override bool IsGripPressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.PrimaryHandTrigger);
		}

		public override bool IsGripPressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.PrimaryHandTrigger);
		}

		public override bool IsGripPressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.PrimaryHandTrigger);
		}

		public override bool IsGripTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, OVRInput.Button.PrimaryHandTrigger);
		}

		public override bool IsGripTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, OVRInput.Button.PrimaryHandTrigger);
		}

		public override bool IsGripTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, OVRInput.Button.PrimaryHandTrigger);
		}

		public override bool IsHairGripDownOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return false;
			}
			return currentHairGripState[index] && !previousHairGripState[index];
		}

		public override bool IsHairGripUpOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return false;
			}
			return !currentHairGripState[index] && previousHairGripState[index];
		}

		public override bool IsTouchpadPressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.PrimaryThumbstick);
		}

		public override bool IsTouchpadPressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.PrimaryThumbstick);
		}

		public override bool IsTouchpadPressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.PrimaryThumbstick);
		}

		public override bool IsTouchpadTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, OVRInput.Touch.PrimaryThumbstick);
		}

		public override bool IsTouchpadTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, OVRInput.Touch.PrimaryThumbstick);
		}

		public override bool IsTouchpadTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, OVRInput.Touch.PrimaryThumbstick);
		}

		public override bool IsButtonOnePressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.One);
		}

		public override bool IsButtonOnePressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.One);
		}

		public override bool IsButtonOnePressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.One);
		}

		public override bool IsButtonOneTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, OVRInput.Touch.One);
		}

		public override bool IsButtonOneTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, OVRInput.Touch.One);
		}

		public override bool IsButtonOneTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, OVRInput.Touch.One);
		}

		public override bool IsButtonTwoPressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.Two);
		}

		public override bool IsButtonTwoPressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.Two);
		}

		public override bool IsButtonTwoPressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.Two);
		}

		public override bool IsButtonTwoTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, OVRInput.Touch.Two);
		}

		public override bool IsButtonTwoTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, OVRInput.Touch.Two);
		}

		public override bool IsButtonTwoTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, OVRInput.Touch.Two);
		}

		public override bool IsStartMenuPressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.Start);
		}

		public override bool IsStartMenuPressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.Start);
		}

		public override bool IsStartMenuPressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.Start);
		}

		public override bool IsStartMenuTouchedOnIndex(uint index)
		{
			return false;
		}

		public override bool IsStartMenuTouchedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsStartMenuTouchedUpOnIndex(uint index)
		{
			return false;
		}

		private void CalculateAngularVelocity(uint index)
		{
			if (index < uint.MaxValue)
			{
				VRTK_TrackedController trackedObject = GetTrackedObject(GetControllerByIndex(index));
				previousControllerRotations[index] = currentControllerRotations[index];
				currentControllerRotations[index] = trackedObject.transform.rotation;
				UpdateHairValues(index, GetTriggerAxisOnIndex(index).x, GetTriggerHairlineDeltaOnIndex(index), ref previousHairTriggerState[index], ref currentHairTriggerState[index], ref hairTriggerLimit[index]);
				UpdateHairValues(index, GetGripAxisOnIndex(index).x, GetGripHairlineDeltaOnIndex(index), ref previousHairGripState[index], ref currentHairGripState[index], ref hairGripLimit[index]);
			}
		}

		private void SetTrackedControllerCaches(bool forceRefresh = false)
		{
			if (forceRefresh)
			{
				cachedLeftController = null;
				cachedRightController = null;
			}
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null)
			{
				if (cachedLeftController == null && (bool)instance.actualLeftController)
				{
					cachedLeftController = instance.actualLeftController.GetComponent<VRTK_TrackedController>();
					cachedLeftController.index = 0u;
				}
				if (cachedRightController == null && (bool)instance.actualRightController)
				{
					cachedRightController = instance.actualRightController.GetComponent<VRTK_TrackedController>();
					cachedRightController.index = 1u;
				}
			}
		}

		private VRTK_TrackedController GetTrackedObject(GameObject controller)
		{
			SetTrackedControllerCaches();
			VRTK_TrackedController result = null;
			if (IsControllerLeftHand(controller))
			{
				result = cachedLeftController;
			}
			else if (IsControllerRightHand(controller))
			{
				result = cachedRightController;
			}
			return result;
		}

		private bool IsButtonPressed(uint index, ButtonPressTypes type, OVRInput.Button button)
		{
			if (index >= uint.MaxValue)
			{
				return false;
			}
			VRTK_TrackedController trackedObject = GetTrackedObject(GetControllerByIndex(index));
			if ((bool)trackedObject)
			{
				OVRInput.Controller controllerMask = touchControllers[index];
				switch (type)
				{
				case ButtonPressTypes.Press:
					return OVRInput.Get(button, controllerMask);
				case ButtonPressTypes.PressDown:
					return OVRInput.GetDown(button, controllerMask);
				case ButtonPressTypes.PressUp:
					return OVRInput.GetUp(button, controllerMask);
				}
			}
			return false;
		}

		private bool IsButtonPressed(uint index, ButtonPressTypes type, OVRInput.Touch button)
		{
			if (index >= uint.MaxValue)
			{
				return false;
			}
			VRTK_TrackedController trackedObject = GetTrackedObject(GetControllerByIndex(index));
			if ((bool)trackedObject)
			{
				OVRInput.Controller controllerMask = touchControllers[index];
				switch (type)
				{
				case ButtonPressTypes.Touch:
					return OVRInput.Get(button, controllerMask);
				case ButtonPressTypes.TouchDown:
					return OVRInput.GetDown(button, controllerMask);
				case ButtonPressTypes.TouchUp:
					return OVRInput.GetUp(button, controllerMask);
				}
			}
			return false;
		}

		private void UpdateHairValues(uint index, float axisValue, float hairDelta, ref bool previousState, ref bool currentState, ref float hairLimit)
		{
			previousState = currentState;
			if (currentState)
			{
				if (axisValue < hairLimit - hairDelta || axisValue <= 0f)
				{
					currentState = false;
				}
			}
			else if (axisValue > hairLimit + hairDelta || axisValue >= 1f)
			{
				currentState = true;
			}
			hairLimit = ((!currentState) ? Mathf.Min(hairLimit, axisValue) : Mathf.Max(hairLimit, axisValue));
		}

		private SDK_OculusVRBoundaries GetBoundariesSDK()
		{
			if (cachedBoundariesSDK == null)
			{
				cachedBoundariesSDK = ((!VRTK_SDKManager.instance) ? ScriptableObject.CreateInstance<SDK_OculusVRBoundaries>() : VRTK_SDKManager.instance.GetBoundariesSDK()) as SDK_OculusVRBoundaries;
			}
			return cachedBoundariesSDK;
		}

		private bool HasAvatar(bool controllersAreVisible = true)
		{
			GetBoundariesSDK();
			return false;
		}

		private GameObject GetAvatar()
		{
			GetBoundariesSDK();
			return null;
		}
	}
}
