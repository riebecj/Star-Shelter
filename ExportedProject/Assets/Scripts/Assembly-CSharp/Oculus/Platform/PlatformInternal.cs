using System;

namespace Oculus.Platform
{
	public static class PlatformInternal
	{
		public enum MessageTypeInternal : uint
		{
			Application_ExecuteCoordinatedLaunch = 645772532u,
			Application_GetInstalledApplications = 1376744524u,
			Avatar_UpdateMetaData = 2077219214u,
			GraphAPI_Get = 822018158u,
			GraphAPI_Post = 1990567876u,
			HTTP_Get = 1874211363u,
			HTTP_GetToFile = 1317133401u,
			HTTP_MultiPartPost = 1480774160u,
			HTTP_Post = 1798743375u,
			Livestreaming_IsAllowedForApplication = 191729014u,
			Livestreaming_StartPartyStream = 2066701532u,
			Livestreaming_StartStream = 1343932350u,
			Livestreaming_StopPartyStream = 661065560u,
			Livestreaming_StopStream = 1155796426u,
			Livestreaming_UpdateCommentsOverlayVisibility = 528318516u,
			Livestreaming_UpdateMicStatus = 475495815u,
			Party_Create = 450042703u,
			Party_GatherInApplication = 1921499523u,
			Party_Get = 1586058173u,
			Party_GetCurrentForUser = 1489764138u,
			Party_Invite = 901104867u,
			Party_Join = 1744993395u,
			Party_Leave = 848430801u,
			Room_CreateOrUpdateAndJoinNamed = 2089683601u,
			Room_GetNamedRooms = 125660812u,
			Room_GetSocialRooms = 1636310390u,
			SystemPermissions_GetStatus = 493497353u,
			SystemPermissions_LaunchDeeplink = 442139697u,
			User_NewEntitledTestUser = 292822787u,
			User_NewTestUser = 921194380u,
			User_NewTestUserFriends = 517416647u
		}

		public static class HTTP
		{
			public static void SetHttpTransferUpdateCallback(Message<Oculus.Platform.Models.HttpTransferUpdate>.Callback callback)
			{
				Callback.SetNotificationCallback(Message.MessageType.Notification_HTTP_Transfer, callback);
			}
		}

		public static void CrashApplication()
		{
			CAPI.ovr_CrashApplication();
		}

		internal static Message ParseMessageHandle(IntPtr messageHandle, Message.MessageType messageType)
		{
			Message result = null;
			switch (messageType)
			{
			case (Message.MessageType)475495815u:
			case (Message.MessageType)645772532u:
			case (Message.MessageType)661065560u:
			case (Message.MessageType)848430801u:
				result = new Message(messageHandle);
				break;
			case (Message.MessageType)1376744524u:
				result = new MessageWithInstalledApplicationList(messageHandle);
				break;
			case (Message.MessageType)191729014u:
				result = new MessageWithLivestreamingApplicationStatus(messageHandle);
				break;
			case (Message.MessageType)1343932350u:
			case (Message.MessageType)2066701532u:
				result = new MessageWithLivestreamingStartResult(messageHandle);
				break;
			case (Message.MessageType)528318516u:
				result = new MessageWithLivestreamingStatus(messageHandle);
				break;
			case (Message.MessageType)1155796426u:
				result = new MessageWithLivestreamingVideoStats(messageHandle);
				break;
			case (Message.MessageType)1586058173u:
				result = new MessageWithParty(messageHandle);
				break;
			case (Message.MessageType)1489764138u:
				result = new MessageWithPartyUnderCurrentParty(messageHandle);
				break;
			case (Message.MessageType)450042703u:
			case (Message.MessageType)901104867u:
			case (Message.MessageType)1744993395u:
			case (Message.MessageType)1921499523u:
				result = new MessageWithPartyID(messageHandle);
				break;
			case (Message.MessageType)2089683601u:
				result = new MessageWithRoomUnderViewerRoom(messageHandle);
				break;
			case (Message.MessageType)125660812u:
			case (Message.MessageType)1636310390u:
				result = new MessageWithRoomList(messageHandle);
				break;
			case (Message.MessageType)292822787u:
			case (Message.MessageType)517416647u:
			case (Message.MessageType)822018158u:
			case (Message.MessageType)921194380u:
			case (Message.MessageType)1317133401u:
			case (Message.MessageType)1480774160u:
			case (Message.MessageType)1798743375u:
			case (Message.MessageType)1874211363u:
			case (Message.MessageType)1990567876u:
			case (Message.MessageType)2077219214u:
				result = new MessageWithString(messageHandle);
				break;
			case (Message.MessageType)442139697u:
			case (Message.MessageType)493497353u:
				result = new MessageWithSystemPermission(messageHandle);
				break;
			}
			return result;
		}
	}
}
