using UnityEngine;

namespace VRTK
{
	public class VRTK_SlideObjectControlAction : VRTK_BaseObjectControlAction
	{
		[Tooltip("The maximum speed the controlled object can be moved in based on the position of the axis.")]
		public float maximumSpeed = 3f;

		[Tooltip("The rate of speed deceleration when the axis is no longer being changed.")]
		public float deceleration = 0.1f;

		[Tooltip("The rate of speed deceleration when the axis is no longer being changed and the object is falling.")]
		public float fallingDeceleration = 0.01f;

		[Tooltip("The speed multiplier to be applied when the modifier button is pressed.")]
		public float speedMultiplier = 1.5f;

		private float currentSpeed;

		protected override void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
		{
			currentSpeed = CalculateSpeed(axis, currentlyFalling, modifierActive);
			Move(controlledGameObject, directionDevice, axisDirection);
		}

		protected virtual float CalculateSpeed(float inputValue, bool currentlyFalling, bool modifierActive)
		{
			float speed = currentSpeed;
			if (inputValue != 0f)
			{
				speed = maximumSpeed * inputValue;
				return (!modifierActive) ? speed : (speed * speedMultiplier);
			}
			return Decelerate(speed, currentlyFalling);
		}

		protected virtual float Decelerate(float speed, bool currentlyFalling)
		{
			float num = ((!currentlyFalling) ? deceleration : fallingDeceleration);
			speed = ((speed > 0f) ? (speed - Mathf.Lerp(num, maximumSpeed, 0f)) : ((!(speed < 0f)) ? 0f : (speed + Mathf.Lerp(num, 0f - maximumSpeed, 0f))));
			if (speed < num && speed > 0f - num)
			{
				speed = 0f;
			}
			return speed;
		}

		protected virtual void Move(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection)
		{
			if ((bool)directionDevice && directionDevice.gameObject.activeInHierarchy && controlledGameObject.activeInHierarchy)
			{
				float y = controlledGameObject.transform.position.y;
				Vector3 vector = axisDirection * currentSpeed * Time.deltaTime;
				controlledGameObject.transform.position += vector;
				controlledGameObject.transform.position = new Vector3(controlledGameObject.transform.position.x, y, controlledGameObject.transform.position.z);
			}
		}
	}
}
