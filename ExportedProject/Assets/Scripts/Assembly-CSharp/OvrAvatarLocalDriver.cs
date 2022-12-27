using System;
using Oculus.Avatar;
using UnityEngine;
using UnityEngine.VR;

public class OvrAvatarLocalDriver : OvrAvatarDriver
{
	private const float mobileBaseHeadHeight = 1.7f;

	private float voiceAmplitude;

	private ControllerPose GetControllerPose(OVRInput.Controller controller)
	{
		ovrAvatarButton ovrAvatarButton2 = (ovrAvatarButton)0;
		if (OVRInput.Get(OVRInput.Button.One, controller))
		{
			ovrAvatarButton2 |= ovrAvatarButton.One;
		}
		if (OVRInput.Get(OVRInput.Button.Two, controller))
		{
			ovrAvatarButton2 |= ovrAvatarButton.Two;
		}
		if (OVRInput.Get(OVRInput.Button.Start, controller))
		{
			ovrAvatarButton2 |= ovrAvatarButton.Three;
		}
		if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick, controller))
		{
			ovrAvatarButton2 |= ovrAvatarButton.Joystick;
		}
		ovrAvatarTouch ovrAvatarTouch2 = (ovrAvatarTouch)0;
		if (OVRInput.Get(OVRInput.Touch.One, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.One;
		}
		if (OVRInput.Get(OVRInput.Touch.Two, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.Two;
		}
		if (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.Joystick;
		}
		if (OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.ThumbRest;
		}
		if (OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.Index;
		}
		if (!OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.Pointing;
		}
		if (!OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.ThumbUp;
		}
		ControllerPose result = default(ControllerPose);
		result.buttons = ovrAvatarButton2;
		result.touches = ovrAvatarTouch2;
		result.joystickPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller);
		result.indexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);
		result.handTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
		result.isActive = (OVRInput.GetActiveController() & controller) != 0;
		return result;
	}

	private PoseFrame GetCurrentPose()
	{
		Vector3 localPosition = InputTracking.GetLocalPosition(VRNode.CenterEye);
		PoseFrame result = default(PoseFrame);
		result.voiceAmplitude = voiceAmplitude;
		result.headPosition = localPosition;
		result.headRotation = InputTracking.GetLocalRotation(VRNode.CenterEye);
		result.handLeftPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
		result.handLeftRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
		result.handRightPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
		result.handRightRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
		result.controllerLeftPose = GetControllerPose(OVRInput.Controller.LTouch);
		result.controllerRightPose = GetControllerPose(OVRInput.Controller.RTouch);
		return result;
	}

	public override void UpdateTransforms(IntPtr sdkAvatar)
	{
		if (sdkAvatar != IntPtr.Zero)
		{
			PoseFrame currentPose = GetCurrentPose();
			ovrAvatarTransform headPose = OvrAvatar.CreateOvrAvatarTransform(currentPose.headPosition, currentPose.headRotation);
			ovrAvatarHandInputState inputStateLeft = OvrAvatar.CreateInputState(OvrAvatar.CreateOvrAvatarTransform(currentPose.handLeftPosition, currentPose.handLeftRotation), currentPose.controllerLeftPose);
			ovrAvatarHandInputState inputStateRight = OvrAvatar.CreateInputState(OvrAvatar.CreateOvrAvatarTransform(currentPose.handRightPosition, currentPose.handRightRotation), currentPose.controllerRightPose);
			CAPI.ovrAvatarPose_UpdateBody(sdkAvatar, headPose);
			CAPI.ovrAvatarPose_UpdateHands(sdkAvatar, inputStateLeft, inputStateRight);
		}
	}
}
