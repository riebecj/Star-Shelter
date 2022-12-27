using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Internal;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class TabGroupAttribute : PropertyGroupAttribute, ISubGroupProviderAttribute
	{
		private class TabSubGroupAttribute : PropertyGroupAttribute
		{
			public TabSubGroupAttribute(string groupId, int order)
				: base(groupId, order)
			{
			}
		}

		public const string DEFAULT_NAME = "_DefaultTabGroup";

		public string TabName { get; private set; }

		public List<string> Tabs { get; private set; }

		public bool UseFixedHeight { get; private set; }

		public TabGroupAttribute(string tab, bool useFixedHeight = false, int order = 0)
			: this("_DefaultTabGroup", tab, useFixedHeight, order)
		{
		}

		public TabGroupAttribute(string group, string tab, bool useFixedHeight = false, int order = 0)
			: base(group, order)
		{
			TabName = tab;
			UseFixedHeight = useFixedHeight;
			Tabs = new List<string>();
			if (tab != null)
			{
				Tabs.Add(tab);
			}
			Tabs = new List<string>(Tabs);
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			base.CombineValuesWith(other);
			TabGroupAttribute tabGroupAttribute = other as TabGroupAttribute;
			if (tabGroupAttribute.TabName != null)
			{
				UseFixedHeight = UseFixedHeight || tabGroupAttribute.UseFixedHeight;
				if (!Tabs.Contains(tabGroupAttribute.TabName))
				{
					Tabs.Add(tabGroupAttribute.TabName);
				}
			}
		}

		IList<PropertyGroupAttribute> ISubGroupProviderAttribute.GetSubGroupAttributes()
		{
			int num = 0;
			List<PropertyGroupAttribute> list = new List<PropertyGroupAttribute>(Tabs.Count);
			foreach (string tab in Tabs)
			{
				list.Add(new TabSubGroupAttribute(base.GroupID + "/" + tab, num++));
			}
			return list;
		}

		string ISubGroupProviderAttribute.RepathMemberAttribute(PropertyGroupAttribute attr)
		{
			TabGroupAttribute tabGroupAttribute = (TabGroupAttribute)attr;
			return base.GroupID + "/" + tabGroupAttribute.TabName;
		}
	}
}
