using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	[DontApplyToListElements]
	public sealed class IndentAttribute : Attribute
	{
		public int IndentLevel { get; private set; }

		public IndentAttribute(int indentLevel = 1)
		{
			IndentLevel = indentLevel;
		}
	}
}
