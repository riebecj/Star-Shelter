using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class FoldoutGroupAttribute : PropertyGroupAttribute
	{
		[Obsolete("Use [FoldoutGroup(\"$MemberName\")] instead.")]
		public string TitleStringMemberName { get; private set; }

		public bool Expanded { get; private set; }

		public bool HasDefinedExpanded { get; private set; }

		public FoldoutGroupAttribute(string groupName, int order = 0)
			: base(groupName, order)
		{
		}

		public FoldoutGroupAttribute(string groupName, bool expanded, int order = 0)
			: base(groupName, order)
		{
			Expanded = expanded;
			HasDefinedExpanded = true;
		}

		[Obsolete("Use [FoldoutGroup(\"$MemberName\")] instead.")]
		public FoldoutGroupAttribute(string groupName, string titleStringMemberName, int order = 0)
			: base(groupName, order)
		{
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			FoldoutGroupAttribute foldoutGroupAttribute = other as FoldoutGroupAttribute;
			if (foldoutGroupAttribute.HasDefinedExpanded)
			{
				HasDefinedExpanded = true;
				Expanded = foldoutGroupAttribute.Expanded;
			}
			if (HasDefinedExpanded)
			{
				foldoutGroupAttribute.HasDefinedExpanded = true;
				foldoutGroupAttribute.Expanded = Expanded;
			}
		}
	}
}
