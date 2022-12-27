using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public static class Matchmaking
	{
		public class CustomQuery
		{
			public struct Criterion
			{
				public string key;

				public MatchmakingCriterionImportance importance;

				public Dictionary<string, object> parameters;

				public Criterion(string key_, MatchmakingCriterionImportance importance_)
				{
					key = key_;
					importance = importance_;
					parameters = null;
				}
			}

			public Dictionary<string, object> data;

			public Criterion[] criteria;

			public IntPtr ToUnmanaged()
			{
				CAPI.ovrMatchmakingCustomQueryData ovrMatchmakingCustomQueryData = default(CAPI.ovrMatchmakingCustomQueryData);
				if (criteria != null && criteria.Length > 0)
				{
					ovrMatchmakingCustomQueryData.criterionArrayCount = (uint)criteria.Length;
					CAPI.ovrMatchmakingCriterion[] array = new CAPI.ovrMatchmakingCriterion[criteria.Length];
					for (int i = 0; i < criteria.Length; i++)
					{
						array[i].importance_ = criteria[i].importance;
						array[i].key_ = criteria[i].key;
						if (criteria[i].parameters != null && criteria[i].parameters.Count > 0)
						{
							array[i].parameterArrayCount = (uint)criteria[i].parameters.Count;
							array[i].parameterArray = CAPI.ArrayOfStructsToIntPtr(CAPI.DictionaryToOVRKeyValuePairs(criteria[i].parameters));
						}
						else
						{
							array[i].parameterArrayCount = 0u;
							array[i].parameterArray = IntPtr.Zero;
						}
					}
					ovrMatchmakingCustomQueryData.criterionArray = CAPI.ArrayOfStructsToIntPtr(array);
				}
				else
				{
					ovrMatchmakingCustomQueryData.criterionArrayCount = 0u;
					ovrMatchmakingCustomQueryData.criterionArray = IntPtr.Zero;
				}
				if (data != null && data.Count > 0)
				{
					ovrMatchmakingCustomQueryData.dataArrayCount = (uint)data.Count;
					ovrMatchmakingCustomQueryData.dataArray = CAPI.ArrayOfStructsToIntPtr(CAPI.DictionaryToOVRKeyValuePairs(data));
				}
				else
				{
					ovrMatchmakingCustomQueryData.dataArrayCount = 0u;
					ovrMatchmakingCustomQueryData.dataArray = IntPtr.Zero;
				}
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ovrMatchmakingCustomQueryData));
				Marshal.StructureToPtr(ovrMatchmakingCustomQueryData, intPtr, true);
				return intPtr;
			}
		}

		public static Request ReportResultsInsecure(ulong roomID, Dictionary<string, int> data)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovrKeyValuePair[] array = new CAPI.ovrKeyValuePair[data.Count];
				int num = 0;
				foreach (KeyValuePair<string, int> datum in data)
				{
					array[num++] = new CAPI.ovrKeyValuePair(datum.Key, datum.Value);
				}
				return new Request(CAPI.ovr_Matchmaking_ReportResultInsecure(roomID, array));
			}
			return null;
		}

		public static void SetMatchFoundNotificationCallback(Message<Oculus.Platform.Models.Room>.Callback callback)
		{
			Callback.SetNotificationCallback(Message.MessageType.Notification_Matchmaking_MatchFound, callback);
		}

		public static Request<MatchmakingStats> GetStats(string pool, uint maxLevel, MatchmakingStatApproach approach = MatchmakingStatApproach.Trailing)
		{
			if (Core.IsInitialized())
			{
				return new Request<MatchmakingStats>(CAPI.ovr_Matchmaking_GetStats(pool, maxLevel, approach));
			}
			return null;
		}

		public static Request<MatchmakingBrowseResult> Browse(string pool, CustomQuery customQueryData = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<MatchmakingBrowseResult>(CAPI.ovr_Matchmaking_Browse(pool, (customQueryData == null) ? IntPtr.Zero : customQueryData.ToUnmanaged()));
			}
			return null;
		}

		public static Request<MatchmakingBrowseResult> Browse2(string pool, MatchmakingOptions matchmakingOptions = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<MatchmakingBrowseResult>(CAPI.ovr_Matchmaking_Browse2(pool, (IntPtr)matchmakingOptions));
			}
			return null;
		}

		public static Request Cancel(string pool, string requestHash)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Matchmaking_Cancel(pool, requestHash));
			}
			return null;
		}

		public static Request Cancel()
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Matchmaking_Cancel2());
			}
			return null;
		}

		public static Request<MatchmakingEnqueueResultAndRoom> CreateAndEnqueueRoom(string pool, uint maxUsers, bool subscribeToUpdates = false, CustomQuery customQueryData = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<MatchmakingEnqueueResultAndRoom>(CAPI.ovr_Matchmaking_CreateAndEnqueueRoom(pool, maxUsers, subscribeToUpdates, (customQueryData == null) ? IntPtr.Zero : customQueryData.ToUnmanaged()));
			}
			return null;
		}

		public static Request<MatchmakingEnqueueResultAndRoom> CreateAndEnqueueRoom2(string pool, MatchmakingOptions matchmakingOptions = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<MatchmakingEnqueueResultAndRoom>(CAPI.ovr_Matchmaking_CreateAndEnqueueRoom2(pool, (IntPtr)matchmakingOptions));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> CreateRoom(string pool, uint maxUsers, bool subscribeToUpdates = false)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Matchmaking_CreateRoom(pool, maxUsers, subscribeToUpdates));
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> CreateRoom2(string pool, MatchmakingOptions matchmakingOptions = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Matchmaking_CreateRoom2(pool, (IntPtr)matchmakingOptions));
			}
			return null;
		}

		public static Request<MatchmakingEnqueueResult> Enqueue(string pool, CustomQuery customQueryData = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<MatchmakingEnqueueResult>(CAPI.ovr_Matchmaking_Enqueue(pool, (customQueryData == null) ? IntPtr.Zero : customQueryData.ToUnmanaged()));
			}
			return null;
		}

		public static Request<MatchmakingEnqueueResult> Enqueue2(string pool, MatchmakingOptions matchmakingOptions = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<MatchmakingEnqueueResult>(CAPI.ovr_Matchmaking_Enqueue2(pool, (IntPtr)matchmakingOptions));
			}
			return null;
		}

		public static Request<MatchmakingEnqueueResult> EnqueueRoom(ulong roomID, CustomQuery customQueryData = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<MatchmakingEnqueueResult>(CAPI.ovr_Matchmaking_EnqueueRoom(roomID, (customQueryData == null) ? IntPtr.Zero : customQueryData.ToUnmanaged()));
			}
			return null;
		}

		public static Request<MatchmakingEnqueueResult> EnqueueRoom2(ulong roomID, MatchmakingOptions matchmakingOptions = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<MatchmakingEnqueueResult>(CAPI.ovr_Matchmaking_EnqueueRoom2(roomID, (IntPtr)matchmakingOptions));
			}
			return null;
		}

		public static Request<MatchmakingAdminSnapshot> GetAdminSnapshot()
		{
			if (Core.IsInitialized())
			{
				return new Request<MatchmakingAdminSnapshot>(CAPI.ovr_Matchmaking_GetAdminSnapshot());
			}
			return null;
		}

		public static Request<Oculus.Platform.Models.Room> JoinRoom(ulong roomID, bool subscribeToUpdates = false)
		{
			if (Core.IsInitialized())
			{
				return new Request<Oculus.Platform.Models.Room>(CAPI.ovr_Matchmaking_JoinRoom(roomID, subscribeToUpdates));
			}
			return null;
		}

		public static Request StartMatch(ulong roomID)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Matchmaking_StartMatch(roomID));
			}
			return null;
		}
	}
}
