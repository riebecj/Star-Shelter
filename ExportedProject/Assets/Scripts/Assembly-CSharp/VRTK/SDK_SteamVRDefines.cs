using System;
using System.Reflection;

namespace VRTK
{
	public static class SDK_SteamVRDefines
	{
		public const string ScriptingDefineSymbol = "VRTK_DEFINE_SDK_STEAMVR";

		private const string BuildTargetGroupName = "Standalone";

		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_STEAMVR", "Standalone")]
		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_STEAMVR_PLUGIN_1_2_1_OR_NEWER", "Standalone")]
		private static bool IsPluginVersion121OrNewer()
		{
			Type type = typeof(SDK_SteamVRDefines).Assembly.GetType("SteamVR_Events");
			if (type == null)
			{
				return false;
			}
			MethodInfo method = type.GetMethod("System", BindingFlags.Static | BindingFlags.Public);
			if (method == null)
			{
				return false;
			}
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length != 1)
			{
				return false;
			}
			return parameters[0].ParameterType == Type.GetType("Valve.VR.EVREventType");
		}

		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_STEAMVR", "Standalone")]
		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_STEAMVR_PLUGIN_1_2_0", "Standalone")]
		private static bool IsPluginVersion120()
		{
			Type type = typeof(SDK_SteamVRDefines).Assembly.GetType("SteamVR_Events");
			if (type == null)
			{
				return false;
			}
			MethodInfo method = type.GetMethod("System", BindingFlags.Static | BindingFlags.Public);
			if (method == null)
			{
				return false;
			}
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length != 1)
			{
				return false;
			}
			return parameters[0].ParameterType == typeof(string);
		}

		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_STEAMVR", "Standalone")]
		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_STEAMVR_PLUGIN_1_1_1_OR_OLDER", "Standalone")]
		private static bool IsPluginVersion111OrOlder()
		{
			Type type = typeof(SDK_SteamVRDefines).Assembly.GetType("SteamVR_Utils");
			if (type == null)
			{
				return false;
			}
			Type nestedType = type.GetNestedType("Event");
			return nestedType != null && nestedType.GetMethod("Listen", BindingFlags.Static | BindingFlags.Public) != null;
		}
	}
}
