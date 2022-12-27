using UnityEngine;

namespace VRTK
{
	public class VRTK_HipTracking : MonoBehaviour
	{
		[Tooltip("Distance underneath Player Head for hips to reside.")]
		public float HeadOffset = -0.35f;

		[Header("Optional")]
		[Tooltip("Optional Transform to use as the Head Object for calculating hip position. If none is given one will try to be found in the scene.")]
		public Transform headOverride;

		[Tooltip("Optional Transform to use for calculating which way is 'Up' relative to the player for hip positioning.")]
		public Transform ReferenceUp;

		protected Transform playerHead;

		protected virtual void OnEnable()
		{
			playerHead = ((!(headOverride != null)) ? VRTK_DeviceFinder.HeadsetTransform() : headOverride);
		}

		protected virtual void Update()
		{
			if (!(playerHead == null))
			{
				Vector3 up = Vector3.up;
				if (ReferenceUp != null)
				{
					up = ReferenceUp.up;
				}
				base.transform.position = playerHead.position + HeadOffset * up;
				Vector3 forward = playerHead.forward;
				Vector3 vector = forward;
				vector.y = 0f;
				vector.Normalize();
				Vector3 a = playerHead.up;
				if (forward.y > 0f)
				{
					a = -playerHead.up;
				}
				a.y = 0f;
				a.Normalize();
				float num = Mathf.Clamp(Vector3.Dot(vector, forward), 0f, 1f);
				Vector3 forward2 = Vector3.Lerp(a, vector, num * num);
				base.transform.rotation = Quaternion.LookRotation(forward2, up);
			}
		}
	}
}
