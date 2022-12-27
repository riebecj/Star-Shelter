using System;

namespace Sirenix.OdinInspector
{
	[Obsolete("Use HideInPrefabInstance or HideInPrefabAsset instead.", false)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ShowForPrefabOnlyAttribute : Attribute
	{
	}
}
