using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	[DontApplyToListElements]
	public sealed class CustomContextMenuAttribute : Attribute
	{
		public string MenuItem { get; private set; }

		public string MethodName { get; private set; }

		public CustomContextMenuAttribute(string menuItem, string methodName)
		{
			MenuItem = menuItem;
			MethodName = methodName;
		}
	}
}
