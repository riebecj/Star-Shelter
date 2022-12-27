using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace VRTK
{
	[SDK_Description(typeof(SDK_SteamVRSystem))]
	public class SDK_SteamVRController : SDK_BaseController
	{
		private SteamVR_TrackedObject cachedLeftTrackedObject;

		private SteamVR_TrackedObject cachedRightTrackedObject;

		private Dictionary<GameObject, SteamVR_TrackedObject> cachedTrackedObjectsByGameObject = new Dictionary<GameObject, SteamVR_TrackedObject>();

		private Dictionary<uint, SteamVR_TrackedObject> cachedTrackedObjectsByIndex = new Dictionary<uint, SteamVR_TrackedObject>();

		private ushort maxHapticVibration = 3999;

		public override void ProcessUpdate(uint index, Dictionary<string, object> options)
		{
		}

		public override void ProcessFixedUpdate(uint index, Dictionary<string, object> options)
		{
		}

		public override string GetControllerDefaultColliderPath(ControllerHand hand)
		{
			string result = "ControllerColliders/Fallback";
			switch (VRTK_DeviceFinder.GetHeadsetType(true))
			{
			case VRTK_DeviceFinder.Headsets.OculusRift:
				result = ((hand != ControllerHand.Left) ? "ControllerColliders/SteamVROculusTouch_Right" : "ControllerColliders/SteamVROculusTouch_Left");
				break;
			case VRTK_DeviceFinder.Headsets.Vive:
				result = "ControllerColliders/HTCVive";
				break;
			}
			return result;
		}

		public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
		{
			string text = ((!fullPath) ? string.Empty : "/attach");
			switch (element)
			{
			case ControllerElements.AttachPoint:
				return "tip/attach";
			case ControllerElements.Trigger:
				return "trigger" + text;
			case ControllerElements.GripLeft:
				return GetControllerGripPath(hand, text, ControllerHand.Left);
			case ControllerElements.GripRight:
				return GetControllerGripPath(hand, text, ControllerHand.Right);
			case ControllerElements.Touchpad:
				return GetControllerTouchpadPath(hand, text);
			case ControllerElements.ButtonOne:
				return GetControllerButtonOnePath(hand, text);
			case ControllerElements.ButtonTwo:
				return GetControllerButtonTwoPath(hand, text);
			case ControllerElements.SystemMenu:
				return GetControllerSystemMenuPath(hand, text);
			case ControllerElements.StartMenu:
				return GetControllerStartMenuPath(hand, text);
			case ControllerElements.Body:
				return "body";
			default:
				return null;
			}
		}

		public override uint GetControllerIndex(GameObject controller)
		{
			SteamVR_TrackedObject trackedObject = GetTrackedObject(controller);
			return (!trackedObject) ? uint.MaxValue : ((uint)trackedObject.index);
		}

		public override GameObject GetControllerByIndex(uint index, bool actual = false)
		{
			SetTrackedControllerCaches();
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null)
			{
				if (cachedLeftTrackedObject != null && cachedLeftTrackedObject.index == (SteamVR_TrackedObject.EIndex)index)
				{
					return (!actual) ? instance.scriptAliasLeftController : instance.actualLeftController;
				}
				if (cachedRightTrackedObject != null && cachedRightTrackedObject.index == (SteamVR_TrackedObject.EIndex)index)
				{
					return (!actual) ? instance.scriptAliasRightController : instance.actualRightController;
				}
			}
			if (cachedTrackedObjectsByIndex.ContainsKey(index) && cachedTrackedObjectsByIndex[index] != null)
			{
				return cachedTrackedObjectsByIndex[index].gameObject;
			}
			return null;
		}

		public override Transform GetControllerOrigin(GameObject controller)
		{
			SteamVR_TrackedObject trackedObject = GetTrackedObject(controller);
			if ((bool)trackedObject)
			{
				return (!trackedObject.origin) ? trackedObject.transform.parent : trackedObject.origin;
			}
			return null;
		}

		public override Transform GenerateControllerPointerOrigin(GameObject parent)
		{
			VRTK_DeviceFinder.Headsets headsetType = VRTK_DeviceFinder.GetHeadsetType(true);
			if (headsetType == VRTK_DeviceFinder.Headsets.OculusRift && (IsControllerLeftHand(parent) || IsControllerRightHand(parent)))
			{
				GameObject gameObject = new GameObject(parent.name + " _CustomPointerOrigin");
				gameObject.transform.SetParent(parent.transform);
				gameObject.transform.localEulerAngles = new Vector3(40f, 0f, 0f);
				gameObject.transform.localPosition = new Vector3((!IsControllerLeftHand(parent)) ? (-0.0081f) : 0.0081f, -0.0273f, -0.0311f);
				return gameObject.transform;
			}
			return null;
		}

		public override GameObject GetControllerLeftHand(bool actual = false)
		{
			GameObject gameObject = GetSDKManagerControllerLeftHand(actual);
			if (!gameObject && actual)
			{
				gameObject = VRTK_SharedMethods.FindEvenInactiveGameObject<SteamVR_ControllerManager>("Controller (left)");
			}
			return gameObject;
		}

		public override GameObject GetControllerRightHand(bool actual = false)
		{
			GameObject gameObject = GetSDKManagerControllerRightHand(actual);
			if (!gameObject && actual)
			{
				gameObject = VRTK_SharedMethods.FindEvenInactiveGameObject<SteamVR_ControllerManager>("Controller (right)");
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
			if (!gameObject)
			{
				GameObject gameObject2 = null;
				switch (hand)
				{
				case ControllerHand.Left:
					gameObject2 = GetControllerLeftHand(true);
					break;
				case ControllerHand.Right:
					gameObject2 = GetControllerRightHand(true);
					break;
				}
				if (gameObject2 != null)
				{
					gameObject = gameObject2.transform.Find("ControllerMesh").gameObject;
				}
			}
			return gameObject;
		}

		public override GameObject GetControllerRenderModel(GameObject controller)
		{
			SteamVR_RenderModel steamVR_RenderModel = ((!controller.GetComponent<SteamVR_RenderModel>()) ? controller.GetComponentInChildren<SteamVR_RenderModel>() : controller.GetComponent<SteamVR_RenderModel>());
			return (!steamVR_RenderModel) ? null : steamVR_RenderModel.gameObject;
		}

		public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
		{
			SteamVR_RenderModel component = renderModel.GetComponent<SteamVR_RenderModel>();
			if ((bool)component)
			{
				component.controllerModeState.bScrollWheelVisible = state;
			}
		}

		public override void HapticPulseOnIndex(uint index, float strength = 0.5f)
		{
			if (index < uint.MaxValue)
			{
				float num = (float)(int)maxHapticVibration * strength;
				SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
				device.TriggerHapticPulse((ushort)num);
			}
		}

		public override SDK_ControllerHapticModifiers GetHapticModifiers()
		{
			return new SDK_ControllerHapticModifiers();
		}

		public override Vector3 GetVelocityOnIndex(uint index)
		{
			if (index == 0 || index >= uint.MaxValue)
			{
				return Vector3.zero;
			}
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
			return device.velocity;
		}

		public override Vector3 GetAngularVelocityOnIndex(uint index)
		{
			if (index == 0 || index >= uint.MaxValue)
			{
				return Vector3.zero;
			}
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
			return device.angularVelocity;
		}

		public override Vector2 GetTouchpadAxisOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return Vector2.zero;
			}
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
			return device.GetAxis();
		}

		public override Vector2 GetTriggerAxisOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return Vector2.zero;
			}
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
			return device.GetAxis(EVRButtonId.k_EButton_Axis1);
		}

		public override Vector2 GetGripAxisOnIndex(uint index)
		{
			return Vector2.zero;
		}

		public override float GetTriggerHairlineDeltaOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return 0f;
			}
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
			return device.hairTriggerDelta;
		}

		public override float GetGripHairlineDeltaOnIndex(uint index)
		{
			return 0f;
		}

		public override bool IsTriggerPressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, 8589934592uL);
		}

		public override bool IsTriggerPressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, 8589934592uL);
		}

		public override bool IsTriggerPressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, 8589934592uL);
		}

		public override bool IsTriggerTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, 8589934592uL);
		}

		public override bool IsTriggerTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, 8589934592uL);
		}

		public override bool IsTriggerTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, 8589934592uL);
		}

		public override bool IsHairTriggerDownOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return false;
			}
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
			return device.GetHairTriggerDown();
		}

		public override bool IsHairTriggerUpOnIndex(uint index)
		{
			if (index >= uint.MaxValue)
			{
				return false;
			}
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
			return device.GetHairTriggerUp();
		}

		public override bool IsGripPressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, 4uL);
		}

		public override bool IsGripPressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, 4uL);
		}

		public override bool IsGripPressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, 4uL);
		}

		public override bool IsGripTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, 4uL);
		}

		public override bool IsGripTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, 4uL);
		}

		public override bool IsGripTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, 4uL);
		}

		public override bool IsHairGripDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsHairGripUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsTouchpadPressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, 4294967296uL);
		}

		public override bool IsTouchpadPressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, 4294967296uL);
		}

		public override bool IsTouchpadPressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, 4294967296uL);
		}

		public override bool IsTouchpadTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, 4294967296uL);
		}

		public override bool IsTouchpadTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, 4294967296uL);
		}

		public override bool IsTouchpadTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, 4294967296uL);
		}

		public override bool IsButtonOnePressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, 128uL);
		}

		public override bool IsButtonOnePressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, 128uL);
		}

		public override bool IsButtonOnePressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, 128uL);
		}

		public override bool IsButtonOneTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, 128uL);
		}

		public override bool IsButtonOneTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, 128uL);
		}

		public override bool IsButtonOneTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, 128uL);
		}

		public override bool IsButtonTwoPressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, 2uL);
		}

		public override bool IsButtonTwoPressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, 2uL);
		}

		public override bool IsButtonTwoPressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, 2uL);
		}

		public override bool IsButtonTwoTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, 2uL);
		}

		public override bool IsButtonTwoTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, 2uL);
		}

		public override bool IsButtonTwoTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, 2uL);
		}

		public override bool IsStartMenuPressedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Press, 1uL);
		}

		public override bool IsStartMenuPressedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressDown, 1uL);
		}

		public override bool IsStartMenuPressedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.PressUp, 1uL);
		}

		public override bool IsStartMenuTouchedOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.Touch, 1uL);
		}

		public override bool IsStartMenuTouchedDownOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchDown, 1uL);
		}

		public override bool IsStartMenuTouchedUpOnIndex(uint index)
		{
			return IsButtonPressed(index, ButtonPressTypes.TouchUp, 1uL);
		}

		private void Awake()
		{
			SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceRoleChanged).Listen(OnTrackedDeviceRoleChanged);
			SetTrackedControllerCaches(true);
		}

		private void OnTrackedDeviceRoleChanged<T>(T ignoredArgument)
		{
			SetTrackedControllerCaches(true);
		}

		private void SetTrackedControllerCaches(bool forceRefresh = false)
		{
			if (forceRefresh)
			{
				cachedLeftTrackedObject = null;
				cachedRightTrackedObject = null;
				cachedTrackedObjectsByGameObject.Clear();
				cachedTrackedObjectsByIndex.Clear();
			}
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null)
			{
				if (cachedLeftTrackedObject == null && (bool)instance.actualLeftController)
				{
					cachedLeftTrackedObject = instance.actualLeftController.GetComponent<SteamVR_TrackedObject>();
				}
				if (cachedRightTrackedObject == null && (bool)instance.actualRightController)
				{
					cachedRightTrackedObject = instance.actualRightController.GetComponent<SteamVR_TrackedObject>();
				}
			}
		}

		private SteamVR_TrackedObject GetTrackedObject(GameObject controller)
		{
			SetTrackedControllerCaches();
			if (IsControllerLeftHand(controller))
			{
				return cachedLeftTrackedObject;
			}
			if (IsControllerRightHand(controller))
			{
				return cachedRightTrackedObject;
			}
			if (cachedTrackedObjectsByGameObject.ContainsKey(controller) && cachedTrackedObjectsByGameObject[controller] != null)
			{
				return cachedTrackedObjectsByGameObject[controller];
			}
			SteamVR_TrackedObject component = controller.GetComponent<SteamVR_TrackedObject>();
			if (component != null)
			{
				cachedTrackedObjectsByGameObject.Add(controller, component);
				cachedTrackedObjectsByIndex.Add((uint)component.index, component);
			}
			return component;
		}

		private bool IsButtonPressed(uint index, ButtonPressTypes type, ulong button)
		{
			if (index >= uint.MaxValue)
			{
				return false;
			}
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
			switch (type)
			{
			case ButtonPressTypes.Press:
				return device.GetPress(button);
			case ButtonPressTypes.PressDown:
				return device.GetPressDown(button);
			case ButtonPressTypes.PressUp:
				return device.GetPressUp(button);
			case ButtonPressTypes.Touch:
				return device.GetTouch(button);
			case ButtonPressTypes.TouchDown:
				return device.GetTouchDown(button);
			case ButtonPressTypes.TouchUp:
				return device.GetTouchUp(button);
			default:
				return false;
			}
		}

		private string GetControllerGripPath(ControllerHand hand, string suffix, ControllerHand forceHand)
		{
			switch (VRTK_DeviceFinder.GetHeadsetType(true))
			{
			case VRTK_DeviceFinder.Headsets.Vive:
				return ((forceHand != ControllerHand.Left) ? "rgrip" : "lgrip") + suffix;
			case VRTK_DeviceFinder.Headsets.OculusRift:
				return "grip" + suffix;
			default:
				return null;
			}
		}

		private string GetControllerTouchpadPath(ControllerHand hand, string suffix)
		{
			switch (VRTK_DeviceFinder.GetHeadsetType(true))
			{
			case VRTK_DeviceFinder.Headsets.Vive:
				return "trackpad" + suffix;
			case VRTK_DeviceFinder.Headsets.OculusRift:
				return "thumbstick" + suffix;
			default:
				return null;
			}
		}

		private string GetControllerButtonOnePath(ControllerHand hand, string suffix)
		{
			switch (VRTK_DeviceFinder.GetHeadsetType(true))
			{
			case VRTK_DeviceFinder.Headsets.Vive:
				return null;
			case VRTK_DeviceFinder.Headsets.OculusRift:
				return ((hand != ControllerHand.Left) ? "a_button" : "x_button") + suffix;
			default:
				return null;
			}
		}

		private string GetControllerButtonTwoPath(ControllerHand hand, string suffix)
		{
			switch (VRTK_DeviceFinder.GetHeadsetType(true))
			{
			case VRTK_DeviceFinder.Headsets.Vive:
				return "button" + suffix;
			case VRTK_DeviceFinder.Headsets.OculusRift:
				return ((hand != ControllerHand.Left) ? "b_button" : "y_button") + suffix;
			default:
				return null;
			}
		}

		private string GetControllerSystemMenuPath(ControllerHand hand, string suffix)
		{
			switch (VRTK_DeviceFinder.GetHeadsetType(true))
			{
			case VRTK_DeviceFinder.Headsets.Vive:
				return "sys_button" + suffix;
			case VRTK_DeviceFinder.Headsets.OculusRift:
				return ((hand != ControllerHand.Left) ? "home_button" : "enter_button") + suffix;
			default:
				return null;
			}
		}

		private string GetControllerStartMenuPath(ControllerHand hand, string suffix)
		{
			switch (VRTK_DeviceFinder.GetHeadsetType(true))
			{
			case VRTK_DeviceFinder.Headsets.Vive:
				return null;
			case VRTK_DeviceFinder.Headsets.OculusRift:
				return ((hand != ControllerHand.Left) ? "home_button" : "enter_button") + suffix;
			default:
				return null;
			}
		}
	}
}
