using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public sealed class TitleGroupAttribute : PropertyGroupAttribute
	{
		public string Subtitle { get; private set; }

		public TitleAlignments Alignment { get; private set; }

		public bool HorizontalLine { get; private set; }

		public bool BoldTitle { get; private set; }

		public bool Indent { get; private set; }

		public TitleGroupAttribute(string title, string subtitle = null, TitleAlignments alignment = TitleAlignments.Left, bool horizontalLine = true, bool boldTitle = true, bool indent = false, int order = 0)
			: base(title, order)
		{
			Subtitle = subtitle;
			Alignment = alignment;
			HorizontalLine = horizontalLine;
			BoldTitle = boldTitle;
			Indent = indent;
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			TitleGroupAttribute titleGroupAttribute = other as TitleGroupAttribute;
			if (Subtitle != null)
			{
				titleGroupAttribute.Subtitle = Subtitle;
			}
			else
			{
				Subtitle = titleGroupAttribute.Subtitle;
			}
			if (Alignment != 0)
			{
				titleGroupAttribute.Alignment = Alignment;
			}
			else
			{
				Alignment = titleGroupAttribute.Alignment;
			}
			if (!HorizontalLine)
			{
				titleGroupAttribute.HorizontalLine = HorizontalLine;
			}
			else
			{
				HorizontalLine = titleGroupAttribute.HorizontalLine;
			}
			if (!BoldTitle)
			{
				titleGroupAttribute.BoldTitle = BoldTitle;
			}
			else
			{
				BoldTitle = titleGroupAttribute.BoldTitle;
			}
			if (Indent)
			{
				titleGroupAttribute.Indent = Indent;
			}
			else
			{
				Indent = titleGroupAttribute.Indent;
			}
		}
	}
}
