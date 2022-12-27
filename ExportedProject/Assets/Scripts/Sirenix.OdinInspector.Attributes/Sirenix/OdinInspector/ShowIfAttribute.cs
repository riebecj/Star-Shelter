using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	[DontApplyToListElements]
	public sealed class ShowIfAttribute : Attribute
	{
		public string MemberName { get; private set; }

		public bool Animate { get; private set; }

		public object Value { get; private set; }

		public ShowIfAttribute(string memberName, bool animate = true)
		{
			MemberName = memberName;
			Animate = animate;
		}

		public ShowIfAttribute(string memberName, object optionalValue, bool animate = true)
		{
			MemberName = memberName;
			Value = optionalValue;
			Animate = animate;
		}
	}
}
