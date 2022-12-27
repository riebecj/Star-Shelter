using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class TableMatrixAttribute : Attribute
	{
		public bool IsReadOnly;

		public bool ResizableColumns = true;

		public string VerticalTitle;

		public string HorizontalTitle;

		public string DrawElementMethod;

		public int RowHeight;

		public bool SquareCells;

		public bool HideColumnIndices;

		public bool HideRowIndices;
	}
}
