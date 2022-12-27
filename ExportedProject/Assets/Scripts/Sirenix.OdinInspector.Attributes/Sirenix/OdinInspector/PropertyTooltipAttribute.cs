using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[DontApplyToListElements]
	public sealed class PropertyTooltipAttribute : Attribute
	{
		public string Tooltip { get; private set; }

		public PropertyTooltipAttribute(string tooltip)
		{
			Tooltip = tooltip;
		}
	}
}
