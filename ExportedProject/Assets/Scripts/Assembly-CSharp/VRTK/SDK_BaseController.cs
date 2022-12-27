using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	public abstract class SDK_BaseController : SDK_Base
	{
		public enum ButtonPressTypes
		{
			Press = 0,
			PressDown = 1,
			PressUp = 2,
			Touch = 3,
			TouchDown = 4,
			TouchUp = 5
		}

		public enum ControllerElements
		{
			AttachPoint = 0,
			Trigger = 1,
			GripLeft = 2,
			GripRight = 3,
			Touchpad = 4,
			ButtonOne = 5,
			ButtonTwo = 6,
			SystemMenu = 7,
			Body = 8,
			StartMenu = 9
		}

		public enum ControllerHand
		{
			None = 0,
			Left = 1,
			Right = 2
		}

		public abstract void ProcessUpdate(uint index, Dictionary<string, object> options);

		public abstract void ProcessFixedUpdate(uint index, Dictionary<string, object> options);

		public abstract string GetControllerDefaultColliderPath(ControllerHand hand);

		public abstract string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false);

		public abstract uint GetControllerIndex(GameObject controller);

		public abstract GameObject GetControllerByIndex(uint index, bool actual = false);

		public abstract Transform GetControllerOrigin(GameObject controller);

		public abstract Transform GenerateControllerPointerOrigin(GameObject parent);

		public abstract GameObject GetControllerLeftHand(bool actual = false);

		public abstract GameObject GetControllerRightHand(bool actual = false);

		public abstract bool IsControllerLeftHand(GameObject controller);

		public abstract bool IsControllerRightHand(GameObject controller);

		public abstract bool IsControllerLeftHand(GameObject controller, bool actual);

		public abstract bool IsControllerRightHand(GameObject controller, bool actual);

		public abstract GameObject GetControllerModel(GameObject controller);

		public abstract GameObject GetControllerModel(ControllerHand hand);

		public abstract GameObject GetControllerRenderModel(GameObject controller);

		public abstract void SetControllerRenderModelWheel(GameObject renderModel, bool state);

		public abstract void HapticPulseOnIndex(uint index, float strength = 0.5f);

		public abstract SDK_ControllerHapticModifiers GetHapticModifiers();

		public abstract Vector3 GetVelocityOnIndex(uint index);

		public abstract Vector3 GetAngularVelocityOnIndex(uint index);

		public abstract Vector2 GetTouchpadAxisOnIndex(uint index);

		public abstract Vector2 GetTriggerAxisOnIndex(uint index);

		public abstract Vector2 GetGripAxisOnIndex(uint index);

		public abstract float GetTriggerHairlineDeltaOnIndex(uint index);

		public abstract float GetGripHairlineDeltaOnIndex(uint index);

		public abstract bool IsTriggerPressedOnIndex(uint index);

		public abstract bool IsTriggerPressedDownOnIndex(uint index);

		public abstract bool IsTriggerPressedUpOnIndex(uint index);

		public abstract bool IsTriggerTouchedOnIndex(uint index);

		public abstract bool IsTriggerTouchedDownOnIndex(uint index);

		public abstract bool IsTriggerTouchedUpOnIndex(uint index);

		public abstract bool IsHairTriggerDownOnIndex(uint index);

		public abstract bool IsHairTriggerUpOnIndex(uint index);

		public abstract bool IsGripPressedOnIndex(uint index);

		public abstract bool IsGripPressedDownOnIndex(uint index);

		public abstract bool IsGripPressedUpOnIndex(uint index);

		public abstract bool IsGripTouchedOnIndex(uint index);

		public abstract bool IsGripTouchedDownOnIndex(uint index);

		public abstract bool IsGripTouchedUpOnIndex(uint index);

		public abstract bool IsHairGripDownOnIndex(uint index);

		public abstract bool IsHairGripUpOnIndex(uint index);

		public abstract bool IsTouchpadPressedOnIndex(uint index);

		public abstract bool IsTouchpadPressedDownOnIndex(uint index);

		public abstract bool IsTouchpadPressedUpOnIndex(uint index);

		public abstract bool IsTouchpadTouchedOnIndex(uint index);

		public abstract bool IsTouchpadTouchedDownOnIndex(uint index);

		public abstract bool IsTouchpadTouchedUpOnIndex(uint index);

		public abstract bool IsButtonOnePressedOnIndex(uint index);

		public abstract bool IsButtonOnePressedDownOnIndex(uint index);

		public abstract bool IsButtonOnePressedUpOnIndex(uint index);

		public abstract bool IsButtonOneTouchedOnIndex(uint index);

		public abstract bool IsButtonOneTouchedDownOnIndex(uint index);

		public abstract bool IsButtonOneTouchedUpOnIndex(uint index);

		public abstract bool IsButtonTwoPressedOnIndex(uint index);

		public abstract bool IsButtonTwoPressedDownOnIndex(uint index);

		public abstract bool IsButtonTwoPressedUpOnIndex(uint index);

		public abstract bool IsButtonTwoTouchedOnIndex(uint index);

		public abstract bool IsButtonTwoTouchedDownOnIndex(uint index);

		public abstract bool IsButtonTwoTouchedUpOnIndex(uint index);

		public abstract bool IsStartMenuPressedOnIndex(uint index);

		public abstract bool IsStartMenuPressedDownOnIndex(uint index);

		public abstract bool IsStartMenuPressedUpOnIndex(uint index);

		public abstract bool IsStartMenuTouchedOnIndex(uint index);

		public abstract bool IsStartMenuTouchedDownOnIndex(uint index);

		public abstract bool IsStartMenuTouchedUpOnIndex(uint index);

		protected GameObject GetSDKManagerControllerLeftHand(bool actual = false)
		{
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null)
			{
				return (!actual) ? instance.scriptAliasLeftController : instance.actualLeftController;
			}
			return null;
		}

		protected GameObject GetSDKManagerControllerRightHand(bool actual = false)
		{
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null)
			{
				return (!actual) ? instance.scriptAliasRightController : instance.actualRightController;
			}
			return null;
		}

		protected bool CheckActualOrScriptAliasControllerIsLeftHand(GameObject controller)
		{
			return IsControllerLeftHand(controller, true) || IsControllerLeftHand(controller, false);
		}

		protected bool CheckActualOrScriptAliasControllerIsRightHand(GameObject controller)
		{
			return IsControllerRightHand(controller, true) || IsControllerRightHand(controller, false);
		}

		protected bool CheckControllerLeftHand(GameObject controller, bool actual)
		{
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null && (bool)controller)
			{
				return (!actual) ? controller.Equals(instance.scriptAliasLeftController) : controller.Equals(instance.actualLeftController);
			}
			return false;
		}

		protected bool CheckControllerRightHand(GameObject controller, bool actual)
		{
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null && (bool)controller)
			{
				return (!actual) ? controller.Equals(instance.scriptAliasRightController) : controller.Equals(instance.actualRightController);
			}
			return false;
		}

		protected GameObject GetControllerModelFromController(GameObject controller)
		{
			return GetControllerModel(VRTK_DeviceFinder.GetControllerHand(controller));
		}

		protected GameObject GetSDKManagerControllerModelForHand(ControllerHand hand)
		{
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null)
			{
				switch (hand)
				{
				case ControllerHand.Left:
					return instance.modelAliasLeftController;
				case ControllerHand.Right:
					return instance.modelAliasRightController;
				}
			}
			return null;
		}

		protected GameObject GetActualController(GameObject controller)
		{
			GameObject result = null;
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null)
			{
				if (IsControllerLeftHand(controller))
				{
					result = instance.actualLeftController;
				}
				else if (IsControllerRightHand(controller))
				{
					result = instance.actualRightController;
				}
			}
			return result;
		}
	}
}
