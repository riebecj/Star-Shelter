using System;

namespace Sirenix.Serialization
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class AlwaysFormatsSelfAttribute : Attribute
	{
	}
}
