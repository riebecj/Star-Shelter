using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class HideMonoScriptAttribute : Attribute
	{
	}
}
