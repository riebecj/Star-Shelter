using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class MaxValueAttribute : Attribute
	{
		public double MaxValue { get; private set; }

		public MaxValueAttribute(double maxValue)
		{
			MaxValue = maxValue;
		}
	}
}
