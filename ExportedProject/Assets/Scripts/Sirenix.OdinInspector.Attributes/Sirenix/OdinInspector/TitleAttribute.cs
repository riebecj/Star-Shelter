using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[DontApplyToListElements]
	public class TitleAttribute : Attribute
	{
		public string Title { get; private set; }

		public string Subtitle { get; private set; }

		public bool Bold { get; private set; }

		public bool HorizontalLine { get; private set; }

		public TitleAlignments TitleAlignment { get; private set; }

		public TitleAttribute(string title, string subtitle = null, TitleAlignments titleAlignment = TitleAlignments.Left, bool horizontalLine = true, bool bold = true)
		{
			Title = title ?? "null";
			Subtitle = subtitle;
			Bold = bold;
			TitleAlignment = titleAlignment;
			HorizontalLine = horizontalLine;
		}
	}
}
