using UnityEngine;

namespace VRTK
{
	public class VRTK_SnapRotateObjectControlAction : VRTK_BaseObjectControlAction
	{
		[Tooltip("The angle to rotate for each snap.")]
		public float anglePerSnap = 30f;

		[Tooltip("The snap angle multiplier to be applied when the modifier button is pressed.")]
		public float angleMultiplier = 1.5f;

		[Tooltip("The amount of time required to pass before another snap rotation can be carried out.")]
		public float snapDelay = 0.5f;

		[Tooltip("The speed for the headset to fade out and back in. Having a blink between rotations can reduce nausia.")]
		public float blinkTransitionSpeed = 0.6f;

		[Range(-1f, 1f)]
		[Tooltip("The threshold the listened axis needs to exceed before the action occurs. This can be used to limit the snap rotate to a single axis direction (e.g. pull down to flip rotate). The threshold is ignored if it is 0.")]
		public float axisThreshold;

		private float snapDelayTimer;

		protected override void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
		{
			if (snapDelayTimer < Time.timeSinceLevelLoad && ValidThreshold(axis))
			{
				float num = Rotate(axis, modifierActive);
				if (num != 0f)
				{
					Blink(blinkTransitionSpeed);
					RotateAroundPlayer(controlledGameObject, num);
				}
			}
		}

		protected virtual bool ValidThreshold(float axis)
		{
			return axisThreshold == 0f || (axisThreshold > 0f && axis >= axisThreshold) || (axisThreshold < 0f && axis <= axisThreshold);
		}

		protected virtual float Rotate(float axis, bool modifierActive)
		{
			snapDelayTimer = Time.timeSinceLevelLoad + snapDelay;
			int axisDirection = GetAxisDirection(axis);
			return anglePerSnap * ((!modifierActive) ? 1f : angleMultiplier) * (float)axisDirection;
		}
	}
}
