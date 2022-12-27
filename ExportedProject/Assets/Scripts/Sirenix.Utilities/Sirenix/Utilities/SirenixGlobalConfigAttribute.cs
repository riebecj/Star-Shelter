namespace Sirenix.Utilities
{
	public class SirenixGlobalConfigAttribute : EditorGlobalConfigAttribute
	{
		protected override string EditorWindowTitle
		{
			get
			{
				return "Sirenix Preferences";
			}
		}

		public SirenixGlobalConfigAttribute(string menuItem)
			: base(SirenixAssetPaths.OdinResourcesConfigsPath, menuItem)
		{
		}

		public SirenixGlobalConfigAttribute(string path, string menuItem)
			: base(path, menuItem)
		{
		}
	}
}
