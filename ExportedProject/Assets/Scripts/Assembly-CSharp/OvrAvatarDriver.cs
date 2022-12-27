using System;
using UnityEngine;

public abstract class OvrAvatarDriver : MonoBehaviour
{
	public struct ControllerPose
	{
		public ovrAvatarButton buttons;

		public ovrAvatarTouch touches;

		public Vector2 joystickPosition;

		public float indexTrigger;

		public float handTrigger;

		public bool isActive;

		public static ControllerPose Interpolate(ControllerPose a, ControllerPose b, float t)
		{
			ControllerPose result = default(ControllerPose);
			result.buttons = ((!(t < 0.5f)) ? b.buttons : a.buttons);
			result.touches = ((!(t < 0.5f)) ? b.touches : a.touches);
			result.joystickPosition = Vector2.Lerp(a.joystickPosition, b.joystickPosition, t);
			result.indexTrigger = Mathf.Lerp(a.indexTrigger, b.indexTrigger, t);
			result.handTrigger = Mathf.Lerp(a.handTrigger, b.handTrigger, t);
			result.isActive = ((!(t < 0.5f)) ? b.isActive : a.isActive);
			return result;
		}
	}

	public struct PoseFrame
	{
		public Vector3 headPosition;

		public Quaternion headRotation;

		public Vector3 handLeftPosition;

		public Quaternion handLeftRotation;

		public Vector3 handRightPosition;

		public Quaternion handRightRotation;

		public float voiceAmplitude;

		public ControllerPose controllerLeftPose;

		public ControllerPose controllerRightPose;

		public static PoseFrame Interpolate(PoseFrame a, PoseFrame b, float t)
		{
			PoseFrame result = default(PoseFrame);
			result.headPosition = Vector3.Lerp(a.headPosition, b.headPosition, t);
			result.headRotation = Quaternion.Slerp(a.headRotation, b.headRotation, t);
			result.handLeftPosition = Vector3.Lerp(a.handLeftPosition, b.handLeftPosition, t);
			result.handLeftRotation = Quaternion.Slerp(a.handLeftRotation, b.handLeftRotation, t);
			result.handRightPosition = Vector3.Lerp(a.handRightPosition, b.handRightPosition, t);
			result.handRightRotation = Quaternion.Slerp(a.handRightRotation, b.handRightRotation, t);
			result.voiceAmplitude = Mathf.Lerp(a.voiceAmplitude, b.voiceAmplitude, t);
			result.controllerLeftPose = ControllerPose.Interpolate(a.controllerLeftPose, b.controllerLeftPose, t);
			result.controllerRightPose = ControllerPose.Interpolate(a.controllerRightPose, b.controllerRightPose, t);
			return result;
		}
	}

	public abstract void UpdateTransforms(IntPtr avatar);
}
