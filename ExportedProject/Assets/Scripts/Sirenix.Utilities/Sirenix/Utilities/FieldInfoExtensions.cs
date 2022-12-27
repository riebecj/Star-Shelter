using System;
using System.Reflection;

namespace Sirenix.Utilities
{
	public static class FieldInfoExtensions
	{
		public static bool IsAliasField(this FieldInfo fieldInfo)
		{
			return fieldInfo is MemberAliasFieldInfo;
		}

		public static FieldInfo DeAliasField(this FieldInfo fieldInfo, bool throwOnNotAliased = false)
		{
			MemberAliasFieldInfo memberAliasFieldInfo = fieldInfo as MemberAliasFieldInfo;
			if (memberAliasFieldInfo != null)
			{
				return memberAliasFieldInfo.AliasedField;
			}
			if (throwOnNotAliased)
			{
				throw new ArgumentException("The field " + fieldInfo.GetNiceName() + " was not aliased.");
			}
			return fieldInfo;
		}
	}
}
