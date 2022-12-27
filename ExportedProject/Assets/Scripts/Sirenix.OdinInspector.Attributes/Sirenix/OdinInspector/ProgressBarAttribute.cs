using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class ProgressBarAttribute : Attribute
	{
		public double Min { get; private set; }

		public double Max { get; private set; }

		public float R { get; private set; }

		public float G { get; private set; }

		public float B { get; private set; }

		public int Height { get; set; }

		public string ColorMember { get; set; }

		public string BackgroundColorMember { get; set; }

		public ProgressBarAttribute(double min, double max, float r = 0.15f, float g = 0.47f, float b = 0.74f)
		{
			Min = min;
			Max = max;
			R = r;
			G = g;
			B = b;
			Height = 12;
		}
	}
}
