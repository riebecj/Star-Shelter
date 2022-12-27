using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class FilePathAttribute : Attribute
	{
		public bool AbsolutePath { get; set; }

		public string Extensions { get; set; }

		public string ParentFolder { get; set; }

		public bool RequireValidPath { get; set; }

		public bool UseBackslashes { get; set; }

		public bool ReadOnly { get; set; }
	}
}
