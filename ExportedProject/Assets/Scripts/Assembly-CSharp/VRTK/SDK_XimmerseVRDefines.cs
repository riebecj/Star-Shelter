namespace VRTK
{
	public static class SDK_XimmerseVRDefines
	{
		public const string ScriptingDefineSymbol = "VRTK_DEFINE_SDK_XIMMERSEVR";

		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_XIMMERSEVR", "Standalone")]
		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_XIMMERSEVR", "Android")]
		private static bool IsXimmerseVRAvailable()
		{
			return typeof(SDK_XimmerseVRDefines).Assembly.GetType("Ximmerse.InputSystem.XDevicePlugin") != null;
		}
	}
}
