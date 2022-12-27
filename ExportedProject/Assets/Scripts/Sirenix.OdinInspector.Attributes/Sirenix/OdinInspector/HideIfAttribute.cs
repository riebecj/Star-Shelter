using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	[DontApplyToListElements]
	public sealed class HideIfAttribute : Attribute
	{
		public string MemberName { get; private set; }

		public object Value { get; private set; }

		public bool Animate { get; private set; }

		public HideIfAttribute(string memberName, bool animate = true)
		{
			MemberName = memberName;
			Animate = animate;
		}

		public HideIfAttribute(string memberName, object optionalValue, bool animate = true)
		{
			MemberName = memberName;
			Value = optionalValue;
			Animate = animate;
		}
	}
}
