using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class PropertyOrderAttribute : Attribute
	{
		public int Order { get; private set; }

		public PropertyOrderAttribute(int order)
		{
			Order = order;
		}
	}
}
