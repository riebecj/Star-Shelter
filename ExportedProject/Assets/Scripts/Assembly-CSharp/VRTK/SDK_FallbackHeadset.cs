using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	public class SDK_FallbackHeadset : SDK_BaseHeadset
	{
		public override void ProcessUpdate(Dictionary<string, object> options)
		{
		}

		public override void ProcessFixedUpdate(Dictionary<string, object> options)
		{
		}

		public override Transform GetHeadset()
		{
			return null;
		}

		public override Transform GetHeadsetCamera()
		{
			return null;
		}

		public override Vector3 GetHeadsetVelocity()
		{
			return Vector3.zero;
		}

		public override Vector3 GetHeadsetAngularVelocity()
		{
			return Vector3.zero;
		}

		public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
		{
		}

		public override bool HasHeadsetFade(Transform obj)
		{
			return false;
		}

		public override void AddHeadsetFade(Transform camera)
		{
		}
	}
}
