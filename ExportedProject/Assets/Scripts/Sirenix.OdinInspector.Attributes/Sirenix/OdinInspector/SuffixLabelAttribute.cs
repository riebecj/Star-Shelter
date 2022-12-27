using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
	public sealed class SuffixLabelAttribute : Attribute
	{
		public string Label { get; private set; }

		public bool Overlay { get; set; }

		public SuffixLabelAttribute(string label, bool overlay = false)
		{
			Label = label;
			Overlay = overlay;
		}
	}
}
