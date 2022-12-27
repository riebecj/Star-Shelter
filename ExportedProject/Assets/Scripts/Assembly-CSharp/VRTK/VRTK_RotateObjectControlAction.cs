using UnityEngine;

namespace VRTK
{
	public class VRTK_RotateObjectControlAction : VRTK_BaseObjectControlAction
	{
		[Tooltip("The maximum speed the controlled object can be rotated based on the position of the axis.")]
		public float maximumRotationSpeed = 3f;

		[Tooltip("The rotation multiplier to be applied when the modifier button is pressed.")]
		public float rotationMultiplier = 1.5f;

		protected override void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
		{
			float num = Rotate(axis, modifierActive);
			if (num != 0f)
			{
				RotateAroundPlayer(controlledGameObject, num);
			}
		}

		protected virtual float Rotate(float axis, bool modifierActive)
		{
			return axis * maximumRotationSpeed * Time.deltaTime * ((!modifierActive) ? 1f : rotationMultiplier) * 10f;
		}
	}
}
