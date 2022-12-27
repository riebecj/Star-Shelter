namespace VRTK
{
	public static class SDK_DaydreamDefines
	{
		public const string ScriptingDefineSymbol = "VRTK_DEFINE_SDK_DAYDREAM";

		[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_DAYDREAM", "Android")]
		private static bool IsDaydreamAvailable()
		{
			return typeof(SDK_DaydreamDefines).Assembly.GetType("GvrController") != null;
		}
	}
}
