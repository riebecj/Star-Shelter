using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public sealed class ToggleGroupAttribute : PropertyGroupAttribute
	{
		public string ToggleMemberName
		{
			get
			{
				return base.GroupName;
			}
		}

		public string ToggleGroupTitle { get; private set; }

		[Obsolete("Add a $ infront of group title instead, i.e: \"$MyStringMember\".")]
		public string TitleStringMemberName { get; private set; }

		public bool CollapseOthersOnExpand { get; set; }

		public ToggleGroupAttribute(string toggleMemberName, int order = 0, string groupTitle = null)
			: base(toggleMemberName, order)
		{
			ToggleGroupTitle = groupTitle;
			CollapseOthersOnExpand = true;
		}

		public ToggleGroupAttribute(string toggleMemberName, string groupTitle)
			: this(toggleMemberName, 0, groupTitle)
		{
		}

		[Obsolete("Use [ToggleGroup(\"toggleMemberName\", groupTitle: \"$titleStringMemberName\")] instead")]
		public ToggleGroupAttribute(string toggleMemberName, int order, string groupTitle, string titleStringMemberName)
			: base(toggleMemberName, order)
		{
			ToggleGroupTitle = groupTitle;
			CollapseOthersOnExpand = true;
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			ToggleGroupAttribute toggleGroupAttribute = other as ToggleGroupAttribute;
			if (ToggleGroupTitle == null)
			{
				ToggleGroupTitle = toggleGroupAttribute.ToggleGroupTitle;
			}
			else if (toggleGroupAttribute.ToggleGroupTitle == null)
			{
				toggleGroupAttribute.ToggleGroupTitle = ToggleGroupTitle;
			}
			CollapseOthersOnExpand = CollapseOthersOnExpand || toggleGroupAttribute.CollapseOthersOnExpand;
			toggleGroupAttribute.CollapseOthersOnExpand = CollapseOthersOnExpand;
		}
	}
}
