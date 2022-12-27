using System;

namespace Sirenix.OdinInspector
{
	[Obsolete("Use DisableInPrefabInstance or DisableInPrefabAsset instead.", false)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class EnableForPrefabOnlyAttribute : Attribute
	{
	}
}
