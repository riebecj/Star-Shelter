using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class MinMaxSliderAttribute : Attribute
	{
		public float MinValue { get; private set; }

		public float MaxValue { get; private set; }

		public bool ShowFields { get; private set; }

		public MinMaxSliderAttribute(float minValue, float maxValue, bool showFields = false)
		{
			MinValue = minValue;
			MaxValue = maxValue;
			ShowFields = showFields;
		}
	}
}
