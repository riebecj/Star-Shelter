using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Sirenix.Utilities
{
	public static class MemberInfoExtensions
	{
		public static bool IsDefined<T>(this MemberInfo member, bool inherit) where T : Attribute
		{
			return member.IsDefined(typeof(T), inherit);
		}

		public static bool IsDefined<T>(this MemberInfo member) where T : Attribute
		{
			return member.IsDefined<T>(false);
		}

		public static T GetAttribute<T>(this MemberInfo member, bool inherit) where T : Attribute
		{
			T[] array = member.GetAttributes<T>(inherit).ToArray();
			if (!array.IsNullOrEmpty())
			{
				return array[0];
			}
			return null;
		}

		public static T GetAttribute<T>(this MemberInfo member) where T : Attribute
		{
			return member.GetAttribute<T>(false);
		}

		public static IEnumerable<T> GetAttributes<T>(this MemberInfo member) where T : Attribute
		{
			return member.GetAttributes<T>(false);
		}

		public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit) where T : Attribute
		{
			return member.GetCustomAttributes(typeof(T), inherit).Cast<T>();
		}

		public static Attribute[] GetAttributes(this MemberInfo member)
		{
			return member.GetAttributes<Attribute>().ToArray();
		}

		public static Attribute[] GetAttributes(this MemberInfo member, bool inherit)
		{
			return member.GetAttributes<Attribute>(inherit).ToArray();
		}

		public static string GetNiceName(this MemberInfo member)
		{
			MethodBase methodBase = member as MethodBase;
			string input = ((methodBase == null) ? member.Name : methodBase.GetFullName());
			return input.ToTitleCase();
		}

		public static bool IsStatic(this MemberInfo member)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo != null)
			{
				return fieldInfo.IsStatic;
			}
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo != null)
			{
				if (!propertyInfo.CanRead)
				{
					return propertyInfo.GetSetMethod(true).IsStatic;
				}
				return propertyInfo.GetGetMethod(true).IsStatic;
			}
			MethodInfo methodInfo = member as MethodInfo;
			if (methodInfo != null)
			{
				return methodInfo.IsStatic;
			}
			string message = string.Format(CultureInfo.InvariantCulture, "Unable to determine IsStatic for member {0}.{1}MemberType was {2} but only fields, properties and methods are supported.", member.Name, member.MemberType, Environment.NewLine);
			throw new NotSupportedException(message);
		}

		public static bool IsAlias(this MemberInfo memberInfo)
		{
			if (!(memberInfo is MemberAliasFieldInfo) && !(memberInfo is MemberAliasPropertyInfo))
			{
				return memberInfo is MemberAliasMethodInfo;
			}
			return true;
		}

		public static MemberInfo DeAlias(this MemberInfo memberInfo, bool throwOnNotAliased = false)
		{
			MemberAliasFieldInfo memberAliasFieldInfo = memberInfo as MemberAliasFieldInfo;
			if (memberAliasFieldInfo != null)
			{
				return memberAliasFieldInfo.AliasedField;
			}
			MemberAliasPropertyInfo memberAliasPropertyInfo = memberInfo as MemberAliasPropertyInfo;
			if (memberAliasPropertyInfo != null)
			{
				return memberAliasPropertyInfo.AliasedProperty;
			}
			MemberAliasMethodInfo memberAliasMethodInfo = memberInfo as MemberAliasMethodInfo;
			if (memberAliasMethodInfo != null)
			{
				return memberAliasMethodInfo.AliasedMethod;
			}
			if (throwOnNotAliased)
			{
				throw new ArgumentException("The member " + memberInfo.GetNiceName() + " was not aliased.");
			}
			return memberInfo;
		}
	}
}
