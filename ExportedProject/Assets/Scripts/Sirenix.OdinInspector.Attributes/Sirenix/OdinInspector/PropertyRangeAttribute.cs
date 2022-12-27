using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class PropertyRangeAttribute : Attribute
	{
		public double Min { get; private set; }

		public double Max { get; private set; }

		public PropertyRangeAttribute(double min, double max)
		{
			Min = ((min < max) ? min : max);
			Max = ((max > min) ? max : min);
		}
	}
}
