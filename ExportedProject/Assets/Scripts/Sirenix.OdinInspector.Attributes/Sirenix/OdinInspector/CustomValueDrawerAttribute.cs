using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class CustomValueDrawerAttribute : Attribute
	{
		public string MethodName { get; set; }

		public CustomValueDrawerAttribute(string methodName)
		{
			MethodName = methodName;
		}
	}
}
