namespace Sirenix.Utilities
{
	public class SirenixEditorConfigAttribute : SirenixGlobalConfigAttribute
	{
		public SirenixEditorConfigAttribute(string menuItem)
			: base(SirenixAssetPaths.OdinEditorConfigsPath, menuItem)
		{
		}

		public SirenixEditorConfigAttribute(string path, string menuItem)
			: base(path, menuItem)
		{
		}
	}
}
