using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	[SDK_Description(typeof(SDK_SteamVRSystem))]
	public class SDK_SteamVRHeadset : SDK_BaseHeadset
	{
		public override void ProcessUpdate(Dictionary<string, object> options)
		{
		}

		public override void ProcessFixedUpdate(Dictionary<string, object> options)
		{
		}

		public override Transform GetHeadset()
		{
			cachedHeadset = GetSDKManagerHeadset();
			if (cachedHeadset == null)
			{
				SteamVR_Camera steamVR_Camera = VRTK_SharedMethods.FindEvenInactiveComponent<SteamVR_Camera>();
				if ((bool)steamVR_Camera)
				{
					cachedHeadset = steamVR_Camera.transform;
				}
			}
			return cachedHeadset;
		}

		public override Transform GetHeadsetCamera()
		{
			cachedHeadsetCamera = GetSDKManagerHeadset();
			if (cachedHeadsetCamera == null)
			{
				SteamVR_Camera steamVR_Camera = VRTK_SharedMethods.FindEvenInactiveComponent<SteamVR_Camera>();
				if ((bool)steamVR_Camera)
				{
					cachedHeadsetCamera = steamVR_Camera.transform;
				}
			}
			return cachedHeadsetCamera;
		}

		public override Vector3 GetHeadsetVelocity()
		{
			return SteamVR_Controller.Input(0).velocity;
		}

		public override Vector3 GetHeadsetAngularVelocity()
		{
			return SteamVR_Controller.Input(0).angularVelocity;
		}

		public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
		{
			SteamVR_Fade.Start(color, duration, fadeOverlay);
		}

		public override bool HasHeadsetFade(Transform obj)
		{
			if ((bool)obj.GetComponentInChildren<SteamVR_Fade>())
			{
				return true;
			}
			return false;
		}

		public override void AddHeadsetFade(Transform camera)
		{
			if ((bool)camera && !camera.GetComponent<SteamVR_Fade>())
			{
				camera.gameObject.AddComponent<SteamVR_Fade>();
			}
		}
	}
}
