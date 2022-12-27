using System;

namespace Sirenix.Utilities
{
	[Flags]
	public enum AssemblyTypeFlags
	{
		None = 0,
		UserTypes = 1,
		PluginTypes = 2,
		UnityTypes = 4,
		UserEditorTypes = 8,
		PluginEditorTypes = 0x10,
		UnityEditorTypes = 0x20,
		OtherTypes = 0x40,
		CustomTypes = 0x1B,
		GameTypes = 0x47,
		EditorTypes = 0x38,
		All = 0x7F
	}
}
