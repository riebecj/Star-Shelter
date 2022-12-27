using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	public abstract class SDK_BaseHeadset : SDK_Base
	{
		protected Transform cachedHeadset;

		protected Transform cachedHeadsetCamera;

		public abstract void ProcessUpdate(Dictionary<string, object> options);

		public abstract void ProcessFixedUpdate(Dictionary<string, object> options);

		public abstract Transform GetHeadset();

		public abstract Transform GetHeadsetCamera();

		public abstract Vector3 GetHeadsetVelocity();

		public abstract Vector3 GetHeadsetAngularVelocity();

		public abstract void HeadsetFade(Color color, float duration, bool fadeOverlay = false);

		public abstract bool HasHeadsetFade(Transform obj);

		public abstract void AddHeadsetFade(Transform camera);

		protected Transform GetSDKManagerHeadset()
		{
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null && instance.actualHeadset != null)
			{
				cachedHeadset = ((!instance.actualHeadset) ? null : instance.actualHeadset.transform);
				return cachedHeadset;
			}
			return null;
		}
	}
}
