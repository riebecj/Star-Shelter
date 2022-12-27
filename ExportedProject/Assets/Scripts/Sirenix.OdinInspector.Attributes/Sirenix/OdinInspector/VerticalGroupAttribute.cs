using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class VerticalGroupAttribute : PropertyGroupAttribute
	{
		public float PaddingTop { get; set; }

		public float PaddingBottom { get; set; }

		public VerticalGroupAttribute(string groupId, int order = 0)
			: base(groupId, order)
		{
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			VerticalGroupAttribute verticalGroupAttribute = other as VerticalGroupAttribute;
			if (verticalGroupAttribute != null)
			{
				if (verticalGroupAttribute.PaddingTop != 0f)
				{
					PaddingTop = verticalGroupAttribute.PaddingTop;
				}
				if (verticalGroupAttribute.PaddingBottom != 0f)
				{
					PaddingBottom = verticalGroupAttribute.PaddingBottom;
				}
			}
		}
	}
}
