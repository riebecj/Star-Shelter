using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
	public class InlinePropertyAttribute : Attribute
	{
		public int LabelWidth { get; set; }
	}
}
