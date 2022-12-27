using System;

namespace Sirenix.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class ExcludeDataFromInspectorAttribute : Attribute
	{
	}
}
