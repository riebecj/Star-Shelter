using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.Serialization
{
	public static class SerializationPolicies
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<MemberInfo, bool> _003C_003E9__6_0;

			public static Func<MemberInfo, bool> _003C_003E9__10_0;

			internal bool _003Cget_Everything_003Eb__6_0(MemberInfo member)
			{
				if (!(member is FieldInfo))
				{
					return false;
				}
				if (member.IsDefined<OdinSerializeAttribute>(true))
				{
					return true;
				}
				return !member.IsDefined<NonSerializedAttribute>(true);
			}

			internal bool _003Cget_Strict_003Eb__10_0(MemberInfo member)
			{
				if (member.IsDefined<NonSerializedAttribute>())
				{
					return false;
				}
				if (member is FieldInfo && member.DeclaringType.IsNestedPrivate && member.DeclaringType.IsDefined<CompilerGeneratedAttribute>())
				{
					return true;
				}
				if (!member.IsDefined<SerializeField>(true))
				{
					return member.IsDefined<OdinSerializeAttribute>(true);
				}
				return true;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass8_0
		{
			public Type tupleInterface;

			internal bool _003Cget_Unity_003Eb__0(MemberInfo member)
			{
				if (member.IsDefined<NonSerializedAttribute>(true) && !member.IsDefined<OdinSerializeAttribute>())
				{
					return false;
				}
				if (member is FieldInfo && ((member as FieldInfo).IsPublic || (member.DeclaringType.IsNestedPrivate && member.DeclaringType.IsDefined<CompilerGeneratedAttribute>()) || (tupleInterface != null && tupleInterface.IsAssignableFrom(member.DeclaringType))))
				{
					return true;
				}
				if (!member.IsDefined<SerializeField>(true))
				{
					return member.IsDefined<OdinSerializeAttribute>(true);
				}
				return true;
			}
		}

		private static readonly object LOCK = new object();

		private static volatile ISerializationPolicy everythingPolicy;

		private static volatile ISerializationPolicy unityPolicy;

		private static volatile ISerializationPolicy strictPolicy;

		public static ISerializationPolicy Everything
		{
			get
			{
				if (everythingPolicy == null)
				{
					lock (LOCK)
					{
						if (everythingPolicy == null)
						{
							everythingPolicy = new CustomSerializationPolicy("Sirenix.SerializationPolicies.Everything", true, _003C_003Ec._003C_003E9__6_0 ?? (_003C_003Ec._003C_003E9__6_0 = _003C_003Ec._003C_003E9._003Cget_Everything_003Eb__6_0));
						}
					}
				}
				return everythingPolicy;
			}
		}

		public static ISerializationPolicy Unity
		{
			get
			{
				if (unityPolicy == null)
				{
					lock (LOCK)
					{
						if (unityPolicy == null)
						{
							_003C_003Ec__DisplayClass8_0 _003C_003Ec__DisplayClass8_ = new _003C_003Ec__DisplayClass8_0();
							_003C_003Ec__DisplayClass8_.tupleInterface = typeof(string).Assembly.GetType("System.ITuple");
							unityPolicy = new CustomSerializationPolicy("Sirenix.SerializationPolicies.Unity", true, _003C_003Ec__DisplayClass8_._003Cget_Unity_003Eb__0);
						}
					}
				}
				return unityPolicy;
			}
		}

		public static ISerializationPolicy Strict
		{
			get
			{
				if (strictPolicy == null)
				{
					lock (LOCK)
					{
						if (strictPolicy == null)
						{
							strictPolicy = new CustomSerializationPolicy("Sirenix.SerializationPolicies.Strict", true, _003C_003Ec._003C_003E9__10_0 ?? (_003C_003Ec._003C_003E9__10_0 = _003C_003Ec._003C_003E9._003Cget_Strict_003Eb__10_0));
						}
					}
				}
				return strictPolicy;
			}
		}

		public static bool TryGetByID(string name, out ISerializationPolicy policy)
		{
			switch (name)
			{
			case "Sirenix.SerializationPolicies.Everything":
				policy = Everything;
				break;
			case "Sirenix.SerializationPolicies.Unity":
				policy = Unity;
				break;
			case "Sirenix.SerializationPolicies.Strict":
				policy = Strict;
				break;
			default:
				policy = null;
				break;
			}
			return policy != null;
		}
	}
}
