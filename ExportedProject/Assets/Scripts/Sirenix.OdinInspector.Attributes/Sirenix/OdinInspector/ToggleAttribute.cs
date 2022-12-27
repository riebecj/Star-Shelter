using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class ToggleAttribute : Attribute
	{
		public string ToggleMemberName { get; private set; }

		public bool CollapseOthersOnExpand { get; set; }

		public ToggleAttribute(string toggleMemberName)
		{
			ToggleMemberName = toggleMemberName;
			CollapseOthersOnExpand = true;
		}
	}
}
