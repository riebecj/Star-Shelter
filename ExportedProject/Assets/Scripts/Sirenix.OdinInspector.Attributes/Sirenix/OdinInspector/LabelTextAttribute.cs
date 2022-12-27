using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[DontApplyToListElements]
	public class LabelTextAttribute : Attribute
	{
		public string Text { get; private set; }

		public LabelTextAttribute(string text)
		{
			Text = text;
		}
	}
}
