using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
	[DontApplyToListElements]
	public class HideInEditorModeAttribute : Attribute
	{
	}
}
