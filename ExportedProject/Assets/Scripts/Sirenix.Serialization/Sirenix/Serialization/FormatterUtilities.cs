using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sirenix.Serialization
{
	public static class FormatterUtilities
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<MemberInfo, string> _003C_003E9__15_0;

			public static Func<MemberInfo, MemberInfo> _003C_003E9__15_1;

			public static Func<MemberInfo, bool> _003C_003E9__16_0;

			internal string _003CFindSerializableMembersMap_003Eb__15_0(MemberInfo n)
			{
				return n.Name;
			}

			internal MemberInfo _003CFindSerializableMembersMap_003Eb__15_1(MemberInfo n)
			{
				return n;
			}

			internal bool _003CFindSerializableMembers_003Eb__16_0(MemberInfo n)
			{
				if (!(n is FieldInfo))
				{
					return n is PropertyInfo;
				}
				return true;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass16_0
		{
			public MemberInfo member;

			internal bool _003CFindSerializableMembers_003Eb__1(MemberInfo n)
			{
				return n.Name == member.Name;
			}
		}

		private static readonly DoubleLookupDictionary<ISerializationPolicy, Type, MemberInfo[]> MemberArrayCache;

		private static readonly DoubleLookupDictionary<ISerializationPolicy, Type, Dictionary<string, MemberInfo>> MemberMapCache;

		private static readonly object LOCK;

		private static readonly HashSet<Type> PrimitiveArrayTypes;

		private static readonly FieldInfo UnityObjectRuntimeErrorStringField;

		private const string UnityObjectRuntimeErrorString = "The variable nullValue of {0} has not been assigned.\r\nYou probably need to assign the nullValue variable of the {0} script in the inspector.";

		static FormatterUtilities()
		{
			MemberArrayCache = new DoubleLookupDictionary<ISerializationPolicy, Type, MemberInfo[]>();
			MemberMapCache = new DoubleLookupDictionary<ISerializationPolicy, Type, Dictionary<string, MemberInfo>>();
			LOCK = new object();
			PrimitiveArrayTypes = new HashSet<Type>
			{
				typeof(char),
				typeof(sbyte),
				typeof(short),
				typeof(int),
				typeof(long),
				typeof(byte),
				typeof(ushort),
				typeof(uint),
				typeof(ulong),
				typeof(decimal),
				typeof(bool),
				typeof(float),
				typeof(double),
				typeof(Guid)
			};
		}

		public static Dictionary<string, MemberInfo> GetSerializableMembersMap(Type type, ISerializationPolicy policy)
		{
			if (policy == null)
			{
				policy = SerializationPolicies.Strict;
			}
			lock (LOCK)
			{
				Dictionary<string, MemberInfo> value;
				if (!MemberMapCache.TryGetInnerValue(policy, type, out value))
				{
					value = FindSerializableMembersMap(type, policy);
					MemberMapCache.AddInner(policy, type, value);
					return value;
				}
				return value;
			}
		}

		public static MemberInfo[] GetSerializableMembers(Type type, ISerializationPolicy policy)
		{
			if (policy == null)
			{
				policy = SerializationPolicies.Strict;
			}
			lock (LOCK)
			{
				MemberInfo[] value;
				if (!MemberArrayCache.TryGetInnerValue(policy, type, out value))
				{
					List<MemberInfo> list = new List<MemberInfo>();
					FindSerializableMembers(type, list, policy);
					value = list.ToArray();
					MemberArrayCache.AddInner(policy, type, value);
					return value;
				}
				return value;
			}
		}

		public static UnityEngine.Object CreateUnityNull(Type nullType, Type owningType)
		{
			if (nullType == null || owningType == null)
			{
				throw new ArgumentNullException();
			}
			if (!nullType.ImplementsOrInherits(typeof(UnityEngine.Object)))
			{
				throw new ArgumentException("Type " + nullType.Name + " is not a Unity object.");
			}
			if (!owningType.ImplementsOrInherits(typeof(UnityEngine.Object)))
			{
				throw new ArgumentException("Type " + owningType.Name + " is not a Unity object.");
			}
			UnityEngine.Object @object = (UnityEngine.Object)FormatterServices.GetUninitializedObject(nullType);
			if (UnityObjectRuntimeErrorStringField != null)
			{
				UnityObjectRuntimeErrorStringField.SetValue(@object, string.Format(CultureInfo.InvariantCulture, "The variable nullValue of {0} has not been assigned.\r\nYou probably need to assign the nullValue variable of the {0} script in the inspector.", owningType.Name));
			}
			return @object;
		}

		public static bool IsPrimitiveType(Type type)
		{
			if (!type.IsPrimitive && !type.IsEnum && type != typeof(decimal) && type != typeof(string))
			{
				return type == typeof(Guid);
			}
			return true;
		}

		public static bool IsPrimitiveArrayType(Type type)
		{
			return PrimitiveArrayTypes.Contains(type);
		}

		public static Type GetContainedType(MemberInfo member)
		{
			if (member is FieldInfo)
			{
				return (member as FieldInfo).FieldType;
			}
			if (member is PropertyInfo)
			{
				return (member as PropertyInfo).PropertyType;
			}
			throw new ArgumentException("Can't get the contained type of a " + member.GetType().Name);
		}

		public static object GetMemberValue(MemberInfo member, object obj)
		{
			if (member is FieldInfo)
			{
				return (member as FieldInfo).GetValue(obj);
			}
			if (member is PropertyInfo)
			{
				return (member as PropertyInfo).GetGetMethod(true).Invoke(obj, null);
			}
			throw new ArgumentException("Can't get the value of a " + member.GetType().Name);
		}

		public static void SetMemberValue(MemberInfo member, object obj, object value)
		{
			if (member is FieldInfo)
			{
				(member as FieldInfo).SetValue(obj, value);
				return;
			}
			if (member is PropertyInfo)
			{
				MethodInfo setMethod = (member as PropertyInfo).GetSetMethod(true);
				if (setMethod != null)
				{
					setMethod.Invoke(obj, new object[1] { value });
					return;
				}
				throw new ArgumentException("Property " + member.Name + " has no setter");
			}
			throw new ArgumentException("Can't set the value of a " + member.GetType().Name);
		}

		private static Dictionary<string, MemberInfo> FindSerializableMembersMap(Type type, ISerializationPolicy policy)
		{
			Dictionary<string, MemberInfo> dictionary = GetSerializableMembers(type, policy).ToDictionary(_003C_003Ec._003C_003E9__15_0 ?? (_003C_003Ec._003C_003E9__15_0 = _003C_003Ec._003C_003E9._003CFindSerializableMembersMap_003Eb__15_0), _003C_003Ec._003C_003E9__15_1 ?? (_003C_003Ec._003C_003E9__15_1 = _003C_003Ec._003C_003E9._003CFindSerializableMembersMap_003Eb__15_1));
			foreach (MemberInfo item in dictionary.Values.ToList())
			{
				IEnumerable<FormerlySerializedAsAttribute> attributes = item.GetAttributes<FormerlySerializedAsAttribute>();
				foreach (FormerlySerializedAsAttribute item2 in attributes)
				{
					if (!dictionary.ContainsKey(item2.oldName))
					{
						dictionary.Add(item2.oldName, item);
					}
				}
			}
			return dictionary;
		}

		private static void FindSerializableMembers(Type type, List<MemberInfo> members, ISerializationPolicy policy)
		{
			if (type.BaseType != typeof(object) && type.BaseType != null)
			{
				FindSerializableMembers(type.BaseType, members, policy);
			}
			using (IEnumerator<MemberInfo> enumerator = type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(_003C_003Ec._003C_003E9__16_0 ?? (_003C_003Ec._003C_003E9__16_0 = _003C_003Ec._003C_003E9._003CFindSerializableMembers_003Eb__16_0)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					_003C_003Ec__DisplayClass16_0 _003C_003Ec__DisplayClass16_ = new _003C_003Ec__DisplayClass16_0();
					_003C_003Ec__DisplayClass16_.member = enumerator.Current;
					if (_003C_003Ec__DisplayClass16_.member is PropertyInfo)
					{
						PropertyInfo propInfo = _003C_003Ec__DisplayClass16_.member as PropertyInfo;
						if (!propInfo.IsAutoProperty())
						{
							continue;
						}
					}
					if (policy.ShouldSerializeMember(_003C_003Ec__DisplayClass16_.member))
					{
						bool flag = members.Any(_003C_003Ec__DisplayClass16_._003CFindSerializableMembers_003Eb__1);
						if (MemberIsPrivate(_003C_003Ec__DisplayClass16_.member) && flag)
						{
							members.Add(GetPrivateMemberAlias(_003C_003Ec__DisplayClass16_.member));
						}
						else if (flag)
						{
							members.Add(GetPrivateMemberAlias(_003C_003Ec__DisplayClass16_.member));
						}
						else
						{
							members.Add(_003C_003Ec__DisplayClass16_.member);
						}
					}
				}
			}
		}

		public static MemberInfo GetPrivateMemberAlias(MemberInfo member, string prefixString = null, string separatorString = null)
		{
			if (member is FieldInfo)
			{
				if (separatorString != null)
				{
					return new MemberAliasFieldInfo(member as FieldInfo, prefixString ?? member.DeclaringType.Name, separatorString);
				}
				return new MemberAliasFieldInfo(member as FieldInfo, prefixString ?? member.DeclaringType.Name);
			}
			if (member is PropertyInfo)
			{
				if (separatorString != null)
				{
					return new MemberAliasPropertyInfo(member as PropertyInfo, prefixString ?? member.DeclaringType.Name, separatorString);
				}
				return new MemberAliasPropertyInfo(member as PropertyInfo, prefixString ?? member.DeclaringType.Name);
			}
			if (member is MethodInfo)
			{
				if (separatorString != null)
				{
					return new MemberAliasMethodInfo(member as MethodInfo, prefixString ?? member.DeclaringType.Name, separatorString);
				}
				return new MemberAliasMethodInfo(member as MethodInfo, prefixString ?? member.DeclaringType.Name);
			}
			throw new NotImplementedException();
		}

		private static bool MemberIsPrivate(MemberInfo member)
		{
			if (member is FieldInfo)
			{
				return (member as FieldInfo).IsPrivate;
			}
			if (member is PropertyInfo)
			{
				PropertyInfo propertyInfo = member as PropertyInfo;
				MethodInfo getMethod = propertyInfo.GetGetMethod();
				MethodInfo setMethod = propertyInfo.GetSetMethod();
				if (getMethod != null && setMethod != null && getMethod.IsPrivate)
				{
					return setMethod.IsPrivate;
				}
				return false;
			}
			if (member is MethodInfo)
			{
				return (member as MethodInfo).IsPrivate;
			}
			throw new NotImplementedException();
		}
	}
}
