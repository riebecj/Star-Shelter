using System;
using System.Collections.Generic;
using Oculus.Avatar;
using UnityEngine;

public class OvrAvatarRemoteDriver : OvrAvatarDriver
{
	private Queue<OvrAvatarPacket> packetQueue = new Queue<OvrAvatarPacket>();

	private IntPtr CurrentSDKPacket = IntPtr.Zero;

	private float CurrentSDKPacketTime;

	private const int MinPacketQueue = 1;

	private const int MaxPacketQueue = 4;

	private int CurrentSequence = -1;

	public void QueuePacket(int sequence, OvrAvatarPacket packet)
	{
		if (sequence > CurrentSequence)
		{
			CurrentSequence = sequence;
			packetQueue.Enqueue(packet);
		}
	}

	public override void UpdateTransforms(IntPtr sdkAvatar)
	{
		if (CurrentSDKPacket == IntPtr.Zero && packetQueue.Count >= 1)
		{
			CurrentSDKPacket = packetQueue.Dequeue().ovrNativePacket;
		}
		if (!(CurrentSDKPacket != IntPtr.Zero))
		{
			return;
		}
		float num = CAPI.ovrAvatarPacket_GetDurationSeconds(CurrentSDKPacket);
		CAPI.ovrAvatar_UpdatePoseFromPacket(sdkAvatar, CurrentSDKPacket, Mathf.Min(num, CurrentSDKPacketTime));
		CurrentSDKPacketTime += Time.deltaTime;
		if (CurrentSDKPacketTime > num)
		{
			CAPI.ovrAvatarPacket_Free(CurrentSDKPacket);
			CurrentSDKPacket = IntPtr.Zero;
			CurrentSDKPacketTime -= num;
			while (packetQueue.Count > 4)
			{
				packetQueue.Dequeue();
			}
		}
	}
}
