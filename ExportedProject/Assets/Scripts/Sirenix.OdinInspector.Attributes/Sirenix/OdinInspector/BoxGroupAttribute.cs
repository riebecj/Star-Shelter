using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class BoxGroupAttribute : PropertyGroupAttribute
	{
		public bool ShowLabel { get; private set; }

		public bool CenterLabel { get; private set; }

		public BoxGroupAttribute(string group, bool showLabel = true, bool centerLabel = false, int order = 0)
			: base(group, order)
		{
			ShowLabel = showLabel;
			CenterLabel = centerLabel;
		}

		public BoxGroupAttribute()
			: this("_DefaultBoxGroup", false)
		{
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			BoxGroupAttribute boxGroupAttribute = other as BoxGroupAttribute;
			if (!ShowLabel || !boxGroupAttribute.ShowLabel)
			{
				ShowLabel = false;
				boxGroupAttribute.ShowLabel = false;
			}
			CenterLabel |= boxGroupAttribute.CenterLabel;
		}
	}
}
