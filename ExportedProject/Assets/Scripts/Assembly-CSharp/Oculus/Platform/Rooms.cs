using System;
using System.Collections.Generic;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Rooms
	{
		public static Request<Oculus.Platform.Models.Room> UpdateDataStore(ulong roomID, Dictionary<string, string> data)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovrKeyValuePair[] array = new CAPI.ovrKeyValuePair[data.Count];
				int num = 0;
				foreach (KeyValuePair<string, string> datum in data)
				{
					array[num++] = new CAPI.ovrKeyValuePair(datum.Key, datum.Value);
				}
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_UpdateDataStore(roomID, array));
			}
			return null;
		}

		public static void SetUpdateNotificationCallback(Message<Oculus.Platform.Models.Room>.Callback callback)
		{
			Callback.SetNotificationCallback(Message.MessageType.Notification_Room_RoomUpdate, callback);
		}

		[Obsolete("Deprecated in favor of SetRoomInviteAcceptedNotificationCallback")]
		public static void SetRoomInviteNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback(Message.MessageType.Notification_Room_InviteAccepted, callback);
		}

		public static void SetRoomInviteAcceptedNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback(Message.MessageType.Notification_Room_InviteAccepted, callback);
		}

		public static void SetRoomInviteReceivedNotificationCallback(Message<RoomInviteNotification>.Callback callback)
		{
			Callback.SetNotificationCallback(Message.MessageType.Notification_Room_InviteReceived, callback);
		}

		public static Request<Oculus.Platform.Models.Room> CreateAndJoinPrivate(RoomJoinPolicy joinPolicy, uint maxUsers, bool subscribeToUpdates = false)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_CreateAndJoinPrivate(joinPolicy, maxUsers, subscribeToUpdates));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> CreateAndJoinPrivate2(RoomJoinPolicy joinPolicy, uint maxUsers, RoomOptions roomOptions)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_CreateAndJoinPrivate2(joinPolicy, maxUsers, (IntPtr)roomOptions));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> Get(ulong roomID)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_Get(roomID));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> GetCurrent()
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_GetCurrent());
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> GetCurrentForUser(ulong userID)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_GetCurrentForUser(userID));
			}
			return null;
		}

		public static Request<UserList> GetInvitableUsers()
		{
			if (Core.IsInitialized())
			{
				return new Request<UserList>(CAPI.ovr_Room_GetInvitableUsers());
			}
			return null;
		}

		public static Request<UserList> GetInvitableUsers2(RoomOptions roomOptions = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<UserList>(CAPI.ovr_Room_GetInvitableUsers2((IntPtr)roomOptions));
			}
			return null;
		}

		public static Request<RoomList> GetModeratedRooms()
		{
			if (Core.IsInitialized())
			{
				return new Request<RoomList>(CAPI.ovr_Room_GetModeratedRooms());
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> InviteUser(ulong roomID, string inviteToken)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_InviteUser(roomID, inviteToken));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> Join(ulong roomID, bool subscribeToUpdates = false)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_Join(roomID, subscribeToUpdates));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> Join2(ulong roomID, RoomOptions roomOptions)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_Join2(roomID, (IntPtr)roomOptions));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> KickUser(ulong roomID, ulong userID, int kickDurationSeconds)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_KickUser(roomID, userID, kickDurationSeconds));
			}
			return null;
		}

		public static Request LaunchInvitableUserFlow(ulong roomID)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Room_LaunchInvitableUserFlow(roomID));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> Leave(ulong roomID)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_Leave(roomID));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> SetDescription(ulong roomID, string description)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_SetDescription(roomID, description));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> UpdateMembershipLockStatus(ulong roomID, RoomMembershipLockStatus membershipLockStatus)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_UpdateMembershipLockStatus(roomID, membershipLockStatus));
			}
			return null;
		}

		public static Request UpdateOwner(ulong roomID, ulong userID)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Room_UpdateOwner(roomID, userID));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> UpdatePrivateRoomJoinPolicy(ulong roomID, RoomJoinPolicy newJoinPolicy)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Room_UpdatePrivateRoomJoinPolicy(roomID, newJoinPolicy));
			}
			return null;
		}

		public static Request<RoomList> GetNextRoomListPage(RoomList list)
		{
			if (!list.HasNextPage)
			{
				Debug.LogWarning("Oculus.Platform.GetNextRoomListPage: List has no next page");
				return null;
			}
			if (Core.IsInitialized())
			{
				return new Request<RoomList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 1317239238));
			}
			return null;
		}
	}
}
