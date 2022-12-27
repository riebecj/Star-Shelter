using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[DontApplyToListElements]
	public sealed class DisableContextMenuAttribute : Attribute
	{
		public bool DisableForMember { get; private set; }

		public bool DisableForCollectionElements { get; private set; }

		public DisableContextMenuAttribute(bool disableForMember = true, bool disableCollectionElements = false)
		{
			DisableForMember = disableForMember;
			DisableForCollectionElements = disableCollectionElements;
		}
	}
}
