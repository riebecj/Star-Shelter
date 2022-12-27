using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class DisplayAsStringAttribute : Attribute
	{
		public bool Overflow { get; set; }

		public DisplayAsStringAttribute()
		{
			Overflow = true;
		}

		public DisplayAsStringAttribute(bool overflow)
		{
			Overflow = overflow;
		}
	}
}
