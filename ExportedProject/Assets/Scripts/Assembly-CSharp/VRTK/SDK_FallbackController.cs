using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	public class SDK_FallbackController : SDK_BaseController
	{
		public override void ProcessUpdate(uint index, Dictionary<string, object> options)
		{
		}

		public override void ProcessFixedUpdate(uint index, Dictionary<string, object> options)
		{
		}

		public override string GetControllerDefaultColliderPath(ControllerHand hand)
		{
			return string.Empty;
		}

		public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
		{
			return string.Empty;
		}

		public override uint GetControllerIndex(GameObject controller)
		{
			return uint.MaxValue;
		}

		public override GameObject GetControllerByIndex(uint index, bool actual = false)
		{
			return null;
		}

		public override Transform GetControllerOrigin(GameObject controller)
		{
			return null;
		}

		public override Transform GenerateControllerPointerOrigin(GameObject parent)
		{
			return null;
		}

		public override GameObject GetControllerLeftHand(bool actual = false)
		{
			return null;
		}

		public override GameObject GetControllerRightHand(bool actual = false)
		{
			return null;
		}

		public override bool IsControllerLeftHand(GameObject controller)
		{
			return false;
		}

		public override bool IsControllerRightHand(GameObject controller)
		{
			return false;
		}

		public override bool IsControllerLeftHand(GameObject controller, bool actual)
		{
			return false;
		}

		public override bool IsControllerRightHand(GameObject controller, bool actual)
		{
			return false;
		}

		public override GameObject GetControllerModel(GameObject controller)
		{
			return null;
		}

		public override GameObject GetControllerModel(ControllerHand hand)
		{
			return null;
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
		}

		public override SDK_ControllerHapticModifiers GetHapticModifiers()
		{
			return new SDK_ControllerHapticModifiers();
		}

		public override Vector3 GetVelocityOnIndex(uint index)
		{
			return Vector3.zero;
		}

		public override Vector3 GetAngularVelocityOnIndex(uint index)
		{
			return Vector3.zero;
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
			return false;
		}

		public override bool IsTriggerPressedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsTriggerPressedUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsTriggerTouchedOnIndex(uint index)
		{
			return false;
		}

		public override bool IsTriggerTouchedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsTriggerTouchedUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsHairTriggerDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsHairTriggerUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsGripPressedOnIndex(uint index)
		{
			return false;
		}

		public override bool IsGripPressedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsGripPressedUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsGripTouchedOnIndex(uint index)
		{
			return false;
		}

		public override bool IsGripTouchedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsGripTouchedUpOnIndex(uint index)
		{
			return false;
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
			return false;
		}

		public override bool IsTouchpadPressedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsTouchpadPressedUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsTouchpadTouchedOnIndex(uint index)
		{
			return false;
		}

		public override bool IsTouchpadTouchedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsTouchpadTouchedUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonOnePressedOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonOnePressedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonOnePressedUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonOneTouchedOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonOneTouchedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonOneTouchedUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonTwoPressedOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonTwoPressedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonTwoPressedUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonTwoTouchedOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonTwoTouchedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsButtonTwoTouchedUpOnIndex(uint index)
		{
			return false;
		}

		public override bool IsStartMenuPressedOnIndex(uint index)
		{
			return false;
		}

		public override bool IsStartMenuPressedDownOnIndex(uint index)
		{
			return false;
		}

		public override bool IsStartMenuPressedUpOnIndex(uint index)
		{
			return false;
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
	}
}
