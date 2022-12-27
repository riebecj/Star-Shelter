using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public sealed class InlineButtonAttribute : Attribute
	{
		public string MemberMethod { get; private set; }

		public string Label { get; private set; }

		public InlineButtonAttribute(string memberMethod, string label = null)
		{
			MemberMethod = memberMethod;
			Label = label;
		}
	}
}
