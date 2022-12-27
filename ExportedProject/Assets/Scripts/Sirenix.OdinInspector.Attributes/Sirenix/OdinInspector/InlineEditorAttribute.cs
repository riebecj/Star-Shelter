using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class InlineEditorAttribute : Attribute
	{
		public bool Expanded;

		public bool DrawHeader;

		public bool DrawGUI;

		public bool DrawPreview;

		public float MaxHeight;

		public float PreviewWidth = 100f;

		public float PreviewHeight = 35f;

		public bool IncrementInlineEditorDrawerDepth = true;

		public InlineEditorAttribute(InlineEditorModes inlineEditorMode = InlineEditorModes.GUIOnly)
		{
			switch (inlineEditorMode)
			{
			case InlineEditorModes.GUIOnly:
				DrawGUI = true;
				break;
			case InlineEditorModes.GUIAndHeader:
				DrawGUI = true;
				DrawHeader = true;
				break;
			case InlineEditorModes.GUIAndPreview:
				DrawGUI = true;
				DrawPreview = true;
				break;
			case InlineEditorModes.SmallPreview:
				Expanded = true;
				DrawPreview = true;
				break;
			case InlineEditorModes.LargePreview:
				Expanded = true;
				DrawPreview = true;
				PreviewHeight = 170f;
				break;
			case InlineEditorModes.FullEditor:
				DrawGUI = true;
				DrawHeader = true;
				DrawPreview = true;
				break;
			default:
				throw new NotImplementedException();
			}
		}
	}
}
