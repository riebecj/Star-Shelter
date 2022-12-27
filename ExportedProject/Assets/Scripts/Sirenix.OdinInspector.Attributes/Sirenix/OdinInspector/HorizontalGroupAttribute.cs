using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class HorizontalGroupAttribute : PropertyGroupAttribute
	{
		public float Width { get; set; }

		public float MarginLeft { get; set; }

		public float MarginRight { get; set; }

		public float PaddingLeft { get; set; }

		public float PaddingRight { get; set; }

		public float MinWidth { get; set; }

		public float MaxWidth { get; set; }

		public string Title { get; set; }

		public float LabelWidth { get; set; }

		public HorizontalGroupAttribute(string group, float width = 0f, int marginLeft = 0, int marginRight = 0, int order = 0)
			: base(group, order)
		{
			Width = width;
			MarginLeft = marginLeft;
			MarginRight = marginRight;
		}

		public HorizontalGroupAttribute(float width = 0f, int marginLeft = 0, int marginRight = 0, int order = 0)
			: this("_DefaultHorizontalGroup", width, marginLeft, marginRight, order)
		{
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			Title = Title ?? (other as HorizontalGroupAttribute).Title;
			LabelWidth = Math.Max(LabelWidth, (other as HorizontalGroupAttribute).LabelWidth);
		}
	}
}
