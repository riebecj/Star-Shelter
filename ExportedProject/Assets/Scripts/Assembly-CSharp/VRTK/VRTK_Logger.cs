using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

namespace VRTK
{
	public class VRTK_Logger : MonoBehaviour
	{
		public enum LogLevels
		{
			Trace = 0,
			Debug = 1,
			Info = 2,
			Warn = 3,
			Error = 4,
			Fatal = 5,
			None = 6
		}

		public enum CommonMessageKeys
		{
			NOT_DEFINED = 0,
			REQUIRED_COMPONENT_MISSING_FROM_SCENE = 1,
			REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT = 2,
			REQUIRED_COMPONENT_MISSING_FROM_PARAMETER = 3,
			REQUIRED_COMPONENT_MISSING_NOT_INJECTED = 4,
			COULD_NOT_FIND_OBJECT_FOR_ACTION = 5,
			SDK_OBJECT_NOT_FOUND = 6,
			SDK_NOT_FOUND = 7,
			SDK_MANAGER_ERRORS = 8
		}

		public static VRTK_Logger instance = null;

		public static Dictionary<CommonMessageKeys, string> commonMessages = new Dictionary<CommonMessageKeys, string>
		{
			{
				CommonMessageKeys.NOT_DEFINED,
				"`{0}` not defined{1}."
			},
			{
				CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE,
				"`{0}` requires the `{1}` component to be available in the scene{2}."
			},
			{
				CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT,
				"`{0}` requires the `{1}` component to be attached to {2} GameObject{3}."
			},
			{
				CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_PARAMETER,
				"`{0}` requires a `{1}` component to be specified as the `{2}` parameter{3}."
			},
			{
				CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED,
				"`{0}` requires the `{1}` component. Either the `{2}` parameter is not set or no `{1}` component is attached to {3} GameObject{4}."
			},
			{
				CommonMessageKeys.COULD_NOT_FIND_OBJECT_FOR_ACTION,
				"The `{0}` could not automatically find {1} to {2}."
			},
			{
				CommonMessageKeys.SDK_OBJECT_NOT_FOUND,
				"No {0} could be found. Have you selected a valid {1} in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a {1} from the dropdown."
			},
			{
				CommonMessageKeys.SDK_NOT_FOUND,
				"The SDK '{0}' doesn't exist anymore. The fallback SDK '{1}' will be used instead."
			},
			{
				CommonMessageKeys.SDK_MANAGER_ERRORS,
				"The current SDK Manager setup is causing the following errors:\n\n{0}"
			}
		};

		public static Dictionary<CommonMessageKeys, int> commonMessageParts = new Dictionary<CommonMessageKeys, int>();

		public LogLevels minLevel;

		public bool throwExceptions = true;

		[CompilerGenerated]
		private static Func<Match, int> _003C_003Ef__am_0024cache0;

		public static void CreateIfNotExists()
		{
			if (instance == null)
			{
				GameObject gameObject = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, "Logger"));
				gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor;
				GameObject gameObject2 = gameObject;
				instance = gameObject2.AddComponent<VRTK_Logger>();
			}
			if (commonMessageParts.Count == commonMessages.Count)
			{
				return;
			}
			commonMessageParts.Clear();
			foreach (KeyValuePair<CommonMessageKeys, string> commonMessage in commonMessages)
			{
				IEnumerable<Match> source = Regex.Matches(commonMessage.Value, "(?<!\\{)\\{([0-9]+).*?\\}(?!})").Cast<Match>().DefaultIfEmpty();
				if (_003C_003Ef__am_0024cache0 == null)
				{
					_003C_003Ef__am_0024cache0 = _003CCreateIfNotExists_003Em__0;
				}
				int value = source.Max(_003C_003Ef__am_0024cache0) + 1;
				commonMessageParts.Add(commonMessage.Key, value);
			}
		}

		public static string GetCommonMessage(CommonMessageKeys messageKey, params object[] parameters)
		{
			CreateIfNotExists();
			string result = string.Empty;
			if (commonMessages.ContainsKey(messageKey))
			{
				if (parameters.Length != commonMessageParts[messageKey])
				{
					Array.Resize(ref parameters, commonMessageParts[messageKey]);
				}
				result = string.Format(commonMessages[messageKey], parameters);
			}
			return result;
		}

		public static void Trace(string message)
		{
			Log(LogLevels.Trace, message);
		}

		public static void Debug(string message)
		{
			Log(LogLevels.Debug, message);
		}

		public static void Info(string message)
		{
			Log(LogLevels.Info, message);
		}

		public static void Warn(string message)
		{
			Log(LogLevels.Warn, message);
		}

		public static void Error(string message)
		{
			Log(LogLevels.Error, message);
		}

		public static void Fatal(string message)
		{
			Log(LogLevels.Fatal, message);
		}

		public static void Log(LogLevels level, string message)
		{
			CreateIfNotExists();
			if (instance.minLevel > level)
			{
				return;
			}
			switch (level)
			{
			case LogLevels.Trace:
			case LogLevels.Debug:
			case LogLevels.Info:
				UnityEngine.Debug.Log(message);
				break;
			case LogLevels.Warn:
				UnityEngine.Debug.LogWarning(message);
				break;
			case LogLevels.Error:
			case LogLevels.Fatal:
				if (instance.throwExceptions)
				{
					throw new Exception(message);
				}
				UnityEngine.Debug.LogError(message);
				break;
			}
		}

		protected virtual void Awake()
		{
			if (instance == null)
			{
				instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			else if (instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		[CompilerGenerated]
		private static int _003CCreateIfNotExists_003Em__0(Match m)
		{
			return (m != null) ? int.Parse(m.Groups[1].Value) : (-1);
		}
	}
}
