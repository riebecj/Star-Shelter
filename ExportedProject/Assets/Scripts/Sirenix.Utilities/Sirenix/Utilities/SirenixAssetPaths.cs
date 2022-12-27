using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sirenix.Utilities
{
	public static class SirenixAssetPaths
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass10_0
		{
			public char[] invalids;

			public char replace;

			internal char _003CToPathSafeString_003Eb__0(char c)
			{
				if (!invalids.Contains(c))
				{
					return c;
				}
				return replace;
			}
		}

		private static readonly string DefaultSirenixPluginPath;

		public static readonly string OdinPath;

		public static readonly string SirenixAssetsPath;

		public static readonly string SirenixPluginPath;

		public static readonly string SirenixAssembliesPath;

		public static readonly string OdinResourcesPath;

		public static readonly string OdinEditorConfigsPath;

		public static readonly string OdinResourcesConfigsPath;

		public static readonly string OdinTempPath;

		static SirenixAssetPaths()
		{
			DefaultSirenixPluginPath = "Plugins/Sirenix/";
			SirenixPluginPath = DefaultSirenixPluginPath;
			OdinPath = SirenixPluginPath + "Odin Inspector/";
			SirenixAssetsPath = SirenixPluginPath + "Assets/";
			SirenixAssembliesPath = SirenixPluginPath + "Assemblies/";
			OdinResourcesPath = OdinPath + "Config/Resources/Sirenix/";
			OdinEditorConfigsPath = OdinPath + "Config/Editor/";
			OdinResourcesConfigsPath = OdinResourcesPath;
		}

		private static string ToPathSafeString(string name, char replace = '_')
		{
			_003C_003Ec__DisplayClass10_0 _003C_003Ec__DisplayClass10_ = new _003C_003Ec__DisplayClass10_0();
			_003C_003Ec__DisplayClass10_.replace = replace;
			_003C_003Ec__DisplayClass10_.invalids = Path.GetInvalidFileNameChars();
			return new string(name.Select(_003C_003Ec__DisplayClass10_._003CToPathSafeString_003Eb__0).ToArray());
		}
	}
}
