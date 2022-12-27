using System;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public sealed class EnableIfAttribute : Attribute
	{
		public string MemberName { get; private set; }

		public object Value { get; private set; }

		public EnableIfAttribute(string memberName)
		{
			MemberName = memberName;
		}

		public EnableIfAttribute(string memberName, object optionalValue)
		{
			MemberName = memberName;
			Value = optionalValue;
		}
	}
}
