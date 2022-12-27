using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class MultiLinePropertyAttribute : Attribute
	{
		public int Lines { get; private set; }

		public MultiLinePropertyAttribute(int lines = 3)
		{
			Lines = Math.Max(1, lines);
		}
	}
}
