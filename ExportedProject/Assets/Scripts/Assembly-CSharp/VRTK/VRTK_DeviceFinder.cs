using UnityEngine;
using UnityEngine.VR;

namespace VRTK
{
	public static class VRTK_DeviceFinder
	{
		public enum Devices
		{
			Headset = 0,
			LeftController = 1,
			RightController = 2
		}

		public enum Headsets
		{
			Unknown = 0,
			OculusRift = 1,
			OculusRiftCV1 = 2,
			Vive = 3,
			ViveMV = 4
		}

		public static uint GetControllerIndex(GameObject controller)
		{
			return VRTK_SDK_Bridge.GetControllerIndex(controller);
		}

		public static GameObject GetControllerByIndex(uint index, bool getActual)
		{
			return VRTK_SDK_Bridge.GetControllerByIndex(index, getActual);
		}

		public static Transform GetControllerOrigin(GameObject controller)
		{
			return VRTK_SDK_Bridge.GetControllerOrigin(controller);
		}

		public static Transform DeviceTransform(Devices device)
		{
			switch (device)
			{
			case Devices.Headset:
				return HeadsetTransform();
			case Devices.LeftController:
				return GetControllerLeftHand().transform;
			case Devices.RightController:
				return GetControllerRightHand().transform;
			default:
				return null;
			}
		}

		public static SDK_BaseController.ControllerHand GetControllerHandType(string hand)
		{
			switch (hand.ToLower())
			{
			case "left":
				return SDK_BaseController.ControllerHand.Left;
			case "right":
				return SDK_BaseController.ControllerHand.Right;
			default:
				return SDK_BaseController.ControllerHand.None;
			}
		}

		public static SDK_BaseController.ControllerHand GetControllerHand(GameObject controller)
		{
			if (VRTK_SDK_Bridge.IsControllerLeftHand(controller))
			{
				return SDK_BaseController.ControllerHand.Left;
			}
			if (VRTK_SDK_Bridge.IsControllerRightHand(controller))
			{
				return SDK_BaseController.ControllerHand.Right;
			}
			return SDK_BaseController.ControllerHand.None;
		}

		public static GameObject GetControllerLeftHand(bool getActual = false)
		{
			return VRTK_SDK_Bridge.GetControllerLeftHand(getActual);
		}

		public static GameObject GetControllerRightHand(bool getActual = false)
		{
			return VRTK_SDK_Bridge.GetControllerRightHand(getActual);
		}

		public static bool IsControllerOfHand(GameObject checkController, SDK_BaseController.ControllerHand hand)
		{
			switch (hand)
			{
			case SDK_BaseController.ControllerHand.Left:
				return IsControllerLeftHand(checkController);
			case SDK_BaseController.ControllerHand.Right:
				return IsControllerRightHand(checkController);
			default:
				return false;
			}
		}

		public static bool IsControllerLeftHand(GameObject checkController)
		{
			return VRTK_SDK_Bridge.IsControllerLeftHand(checkController);
		}

		public static bool IsControllerRightHand(GameObject checkController)
		{
			return VRTK_SDK_Bridge.IsControllerRightHand(checkController);
		}

		public static GameObject GetActualController(GameObject givenController)
		{
			if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, true) || VRTK_SDK_Bridge.IsControllerRightHand(givenController, true))
			{
				return givenController;
			}
			if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, false))
			{
				return VRTK_SDK_Bridge.GetControllerLeftHand(true);
			}
			if (VRTK_SDK_Bridge.IsControllerRightHand(givenController, false))
			{
				return VRTK_SDK_Bridge.GetControllerRightHand(true);
			}
			return null;
		}

		public static GameObject GetScriptAliasController(GameObject givenController)
		{
			if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, false) || VRTK_SDK_Bridge.IsControllerRightHand(givenController, false))
			{
				return givenController;
			}
			if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, true))
			{
				return VRTK_SDK_Bridge.GetControllerLeftHand(false);
			}
			if (VRTK_SDK_Bridge.IsControllerRightHand(givenController, true))
			{
				return VRTK_SDK_Bridge.GetControllerRightHand(false);
			}
			return null;
		}

		public static GameObject GetModelAliasController(GameObject givenController)
		{
			return VRTK_SDK_Bridge.GetControllerModel(givenController);
		}

		public static SDK_BaseController.ControllerHand GetModelAliasControllerHand(GameObject givenObject)
		{
			if (GetModelAliasController(GetControllerLeftHand()) == givenObject)
			{
				return SDK_BaseController.ControllerHand.Left;
			}
			if (GetModelAliasController(GetControllerRightHand()) == givenObject)
			{
				return SDK_BaseController.ControllerHand.Right;
			}
			return SDK_BaseController.ControllerHand.None;
		}

		public static Vector3 GetControllerVelocity(GameObject givenController)
		{
			uint controllerIndex = GetControllerIndex(givenController);
			return VRTK_SDK_Bridge.GetVelocityOnIndex(controllerIndex);
		}

		public static Vector3 GetControllerAngularVelocity(GameObject givenController)
		{
			uint controllerIndex = GetControllerIndex(givenController);
			return VRTK_SDK_Bridge.GetAngularVelocityOnIndex(controllerIndex);
		}

		public static Vector3 GetHeadsetVelocity()
		{
			return VRTK_SDK_Bridge.GetHeadsetVelocity();
		}

		public static Vector3 GetHeadsetAngularVelocity()
		{
			return VRTK_SDK_Bridge.GetHeadsetAngularVelocity();
		}

		public static Transform HeadsetTransform()
		{
			return VRTK_SDK_Bridge.GetHeadset();
		}

		public static Transform HeadsetCamera()
		{
			return VRTK_SDK_Bridge.GetHeadsetCamera();
		}

		public static Headsets GetHeadsetType(bool summary = false)
		{
			Headsets result = Headsets.Unknown;
			switch (VRDevice.model)
			{
			case "Oculus Rift CV1":
				result = (summary ? Headsets.OculusRift : Headsets.OculusRiftCV1);
				break;
			case "Vive MV":
				result = ((!summary) ? Headsets.ViveMV : Headsets.Vive);
				break;
			}
			return result;
		}

		public static Transform PlayAreaTransform()
		{
			return VRTK_SDK_Bridge.GetPlayArea();
		}
	}
}
