using System;
using System.Reflection;

namespace VRTK
{
	public static class SDK_OculusVRDefines
	{
		public const string ScriptingDefineSymbol = "VRTK_DEFINE_SDK_OCULUSVR";

		public const string AvatarScriptingDefineSymbol = "VRTK_DEFINE_SDK_OCULUSVR_AVATAR";

		private const string BuildTargetGroupName = "Standalone";

		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_OCULUSVR", "Standalone")]
		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_OCULUSVR_UTILITIES_1_12_0_OR_NEWER", "Standalone")]
		private static bool IsUtilitiesVersion1120OrNewer()
		{
			Version oculusWrapperVersion = GetOculusWrapperVersion();
			return oculusWrapperVersion != null && oculusWrapperVersion >= new Version(1, 12, 0);
		}

		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_OCULUSVR", "Standalone")]
		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_OCULUSVR_UTILITIES_1_11_0_OR_OLDER", "Standalone")]
		private static bool IsUtilitiesVersion1110OrOlder()
		{
			Version oculusWrapperVersion = GetOculusWrapperVersion();
			return oculusWrapperVersion != null && oculusWrapperVersion < new Version(1, 12, 0);
		}

		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_OCULUSVR_AVATAR", "Standalone")]
		private static bool IsAvatarAvailable()
		{
			return (IsUtilitiesVersion1120OrNewer() || IsUtilitiesVersion1110OrOlder()) && typeof(SDK_OculusVRDefines).Assembly.GetType("OvrAvatar") != null;
		}

		private static Version GetOculusWrapperVersion()
		{
			Type type = typeof(SDK_OculusVRDefines).Assembly.GetType("OVRPlugin");
			if (type == null)
			{
				return null;
			}
			FieldInfo field = type.GetField("wrapperVersion", BindingFlags.Static | BindingFlags.Public);
			if (field == null)
			{
				return null;
			}
			return (Version)field.GetValue(null);
		}

		private static Version GetOculusRuntimeVersion()
		{
			Type type = typeof(SDK_OculusVRDefines).Assembly.GetType("OVRPlugin");
			if (type == null)
			{
				return null;
			}
			PropertyInfo property = type.GetProperty("version", BindingFlags.Static | BindingFlags.Public);
			if (property == null)
			{
				return null;
			}
			return (Version)property.GetGetMethod().Invoke(null, null);
		}
	}
}
