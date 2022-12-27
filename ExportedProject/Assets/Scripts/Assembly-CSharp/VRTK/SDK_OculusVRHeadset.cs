using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	[SDK_Description(typeof(SDK_OculusVRSystem))]
	public class SDK_OculusVRHeadset : SDK_BaseHeadset
	{
		private Quaternion previousHeadsetRotation;

		private Quaternion currentHeadsetRotation;

		public override void ProcessUpdate(Dictionary<string, object> options)
		{
		}

		public override void ProcessFixedUpdate(Dictionary<string, object> options)
		{
			CalculateAngularVelocity();
		}

		public override Transform GetHeadset()
		{
			cachedHeadset = GetSDKManagerHeadset();
			if (cachedHeadset == null)
			{
				cachedHeadset = VRTK_SharedMethods.FindEvenInactiveGameObject<OVRCameraRig>("TrackingSpace/CenterEyeAnchor").transform;
			}
			return cachedHeadset;
		}

		public override Transform GetHeadsetCamera()
		{
			cachedHeadsetCamera = GetSDKManagerHeadset();
			if (cachedHeadsetCamera == null)
			{
				cachedHeadsetCamera = GetHeadset();
			}
			return cachedHeadsetCamera;
		}

		public override Vector3 GetHeadsetVelocity()
		{
			return (!OVRManager.isHmdPresent) ? Vector3.zero : OVRPlugin.GetNodeVelocity(OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render).FromFlippedZVector3f();
		}

		public override Vector3 GetHeadsetAngularVelocity()
		{
			Quaternion quaternion = currentHeadsetRotation * Quaternion.Inverse(previousHeadsetRotation);
			return new Vector3(Mathf.DeltaAngle(0f, quaternion.eulerAngles.x), Mathf.DeltaAngle(0f, quaternion.eulerAngles.y), Mathf.DeltaAngle(0f, quaternion.eulerAngles.z));
		}

		public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
		{
			VRTK_ScreenFade.Start(color, duration);
		}

		public override bool HasHeadsetFade(Transform obj)
		{
			if ((bool)obj.GetComponentInChildren<VRTK_ScreenFade>())
			{
				return true;
			}
			return false;
		}

		public override void AddHeadsetFade(Transform camera)
		{
			if ((bool)camera && !camera.GetComponent<VRTK_ScreenFade>())
			{
				camera.gameObject.AddComponent<VRTK_ScreenFade>();
			}
		}

		private void CalculateAngularVelocity()
		{
			previousHeadsetRotation = currentHeadsetRotation;
			currentHeadsetRotation = GetHeadset().transform.rotation;
		}
	}
}
