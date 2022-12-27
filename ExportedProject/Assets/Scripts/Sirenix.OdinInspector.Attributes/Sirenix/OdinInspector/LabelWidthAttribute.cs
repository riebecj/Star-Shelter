using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[DontApplyToListElements]
	public class LabelWidthAttribute : Attribute
	{
		public float Width { get; private set; }

		public LabelWidthAttribute(float width)
		{
			Width = width;
		}
	}
}
