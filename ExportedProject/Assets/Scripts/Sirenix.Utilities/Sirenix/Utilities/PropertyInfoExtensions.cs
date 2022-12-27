using System;
using System.Reflection;

namespace Sirenix.Utilities
{
	public static class PropertyInfoExtensions
	{
		public static bool IsAutoProperty(this PropertyInfo propInfo)
		{
			if (!propInfo.CanWrite || !propInfo.CanRead)
			{
				return false;
			}
			MethodInfo getMethod = propInfo.GetGetMethod(true);
			MethodInfo setMethod = propInfo.GetSetMethod(true);
			if ((getMethod != null && (getMethod.IsAbstract || getMethod.IsVirtual)) || (setMethod != null && (setMethod.IsAbstract || setMethod.IsVirtual)))
			{
				return false;
			}
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
			string value = "<" + propInfo.Name + ">";
			FieldInfo[] fields = propInfo.DeclaringType.GetFields(bindingAttr);
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].Name.Contains(value))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsAliasProperty(this PropertyInfo propertyInfo)
		{
			return propertyInfo is MemberAliasPropertyInfo;
		}

		public static PropertyInfo DeAliasProperty(this PropertyInfo propertyInfo, bool throwOnNotAliased = false)
		{
			MemberAliasPropertyInfo memberAliasPropertyInfo = propertyInfo as MemberAliasPropertyInfo;
			if (memberAliasPropertyInfo != null)
			{
				return memberAliasPropertyInfo.AliasedProperty;
			}
			if (throwOnNotAliased)
			{
				throw new ArgumentException("The property " + propertyInfo.GetNiceName() + " was not aliased.");
			}
			return propertyInfo;
		}
	}
}
