using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class ReadOnlyAttribute : Attribute
	{
	}
}
