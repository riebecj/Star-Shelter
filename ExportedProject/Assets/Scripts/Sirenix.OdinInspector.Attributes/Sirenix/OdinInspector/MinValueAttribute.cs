using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class MinValueAttribute : Attribute
	{
		public double MinValue { get; private set; }

		public MinValueAttribute(double minValue)
		{
			MinValue = minValue;
		}
	}
}
