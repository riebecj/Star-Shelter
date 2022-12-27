using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	[DontApplyToListElements]
	public sealed class OnValueChangedAttribute : Attribute
	{
		public string MethodName { get; private set; }

		public bool IncludeChildren { get; private set; }

		public OnValueChangedAttribute(string methodName, bool includeChildren = false)
		{
			MethodName = methodName;
			IncludeChildren = includeChildren;
		}
	}
}
