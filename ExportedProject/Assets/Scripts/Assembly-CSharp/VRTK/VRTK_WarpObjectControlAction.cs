using UnityEngine;

namespace VRTK
{
	public class VRTK_WarpObjectControlAction : VRTK_BaseObjectControlAction
	{
		[Tooltip("The distance to warp in the facing direction.")]
		public float warpDistance = 1f;

		[Tooltip("The multiplier to be applied to the warp when the modifier button is pressed.")]
		public float warpMultiplier = 2f;

		[Tooltip("The amount of time required to pass before another warp can be carried out.")]
		public float warpDelay = 0.5f;

		[Tooltip("The height different in floor allowed to be a valid warp.")]
		public float floorHeightTolerance = 1f;

		[Tooltip("The speed for the headset to fade out and back in. Having a blink between warps can reduce nausia.")]
		public float blinkTransitionSpeed = 0.6f;

		private float warpDelayTimer;

		private Transform headset;

		protected override void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive)
		{
			if (warpDelayTimer < Time.timeSinceLevelLoad && axis != 0f)
			{
				Warp(controlledGameObject, directionDevice, axisDirection, axis, modifierActive);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			headset = VRTK_DeviceFinder.HeadsetTransform();
		}

		protected virtual void Warp(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, bool modifierActive)
		{
			Vector3 objectCenter = GetObjectCenter(controlledGameObject.transform);
			Vector3 vector = controlledGameObject.transform.TransformPoint(objectCenter);
			float num = warpDistance * ((!modifierActive) ? 1f : warpMultiplier);
			int axisDirection2 = GetAxisDirection(axis);
			Vector3 vector2 = vector + axisDirection * num * axisDirection2;
			float num2 = 0.2f;
			Vector3 vector3 = axisDirection2 * axisDirection;
			Vector3 vector4 = ((!(controlledGameObject.transform == playArea)) ? controlledGameObject.transform.position : headset.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(vector4 + Vector3.up * num2, vector3, out hitInfo, num - colliderRadius))
			{
				vector2 = hitInfo.point - vector3 * colliderRadius;
			}
			if (Physics.Raycast(vector2 + Vector3.up * (floorHeightTolerance + num2), Vector3.down, out hitInfo, (floorHeightTolerance + num2) * 2f))
			{
				vector2.y = hitInfo.point.y + colliderHeight / 2f;
				Vector3 position = vector2 - vector + controlledGameObject.transform.position;
				warpDelayTimer = Time.timeSinceLevelLoad + warpDelay;
				controlledGameObject.transform.position = position;
				Blink(blinkTransitionSpeed);
			}
		}
	}
}
