using System;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class DisableIfAttribute : Attribute
	{
		public string MemberName { get; private set; }

		public object Value { get; private set; }

		public DisableIfAttribute(string memberName)
		{
			MemberName = memberName;
		}

		public DisableIfAttribute(string memberName, object optionalValue)
		{
			MemberName = memberName;
			Value = optionalValue;
		}
	}
}
