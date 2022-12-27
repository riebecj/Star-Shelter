using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class TableListAttribute : Attribute
	{
		public int NumberOfItemsPerPage { get; set; }
	}
}
