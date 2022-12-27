using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class ButtonGroupAttribute : PropertyGroupAttribute
	{
		public ButtonGroupAttribute(string group = "_DefaultGroup", int order = 0)
			: base(group, order)
		{
		}
	}
}
