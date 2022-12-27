using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class TableColumnWidthAttribute : Attribute
	{
		public int Width;

		public TableColumnWidthAttribute(int width)
		{
			Width = width;
		}
	}
}
