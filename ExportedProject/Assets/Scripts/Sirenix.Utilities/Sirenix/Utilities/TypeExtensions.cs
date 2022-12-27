using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Sirenix.Utilities
{
	public static class TypeExtensions
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<Type, string> _003C_003E9__6_0;

			internal string _003Cget_GetNiceNameProp_003Eb__6_0(Type type)
			{
				if (type.IsArray)
				{
					int arrayRank = type.GetArrayRank();
					return type.GetElementType().GetNiceName() + ((arrayRank == 1) ? "[]" : "[,]");
				}
				if (type.InheritsFrom(typeof(Nullable<>)))
				{
					return type.GetGenericArguments()[0].GetNiceName() + "?";
				}
				if (type.IsByRef)
				{
					return "ref " + type.GetElementType().GetNiceName();
				}
				if (type.IsGenericParameter || !type.IsGenericType)
				{
					return type.TypeNameGauntlet();
				}
				StringBuilder stringBuilder = new StringBuilder();
				string name = type.Name;
				int num = name.IndexOf("`");
				if (num != -1)
				{
					stringBuilder.Append(name.Substring(0, num));
				}
				else
				{
					stringBuilder.Append(name);
				}
				stringBuilder.Append('<');
				Type[] genericArguments = type.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					Type type2 = genericArguments[i];
					if (i != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(type2.GetNiceName());
				}
				stringBuilder.Append('>');
				return stringBuilder.ToString();
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass15_0
		{
			public MethodInfo method;

			internal object _003CGetCastMethodDelegate_003Eb__0(object obj)
			{
				return method.Invoke(null, new object[1] { obj });
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec__18<T>
		{
			public static readonly _003C_003Ec__18<T> _003C_003E9 = new _003C_003Ec__18<T>();

			public static Func<MethodInfo, bool> _003C_003E9__18_0;

			public static Func<Quaternion, Quaternion, bool> _003C_003E9__18_1;

			public static Func<T, T, bool> _003C_003E9__18_2;

			public static Func<T, T, bool> _003C_003E9__18_3;

			internal bool _003CGetEqualityComparerDelegate_003Eb__18_0(MethodInfo x)
			{
				ParameterInfo[] parameters = x.GetParameters();
				if (parameters.Length != 2)
				{
					return false;
				}
				if (x.ReturnType != typeof(bool))
				{
					return false;
				}
				if (parameters[0].ParameterType != typeof(T))
				{
					return false;
				}
				if (parameters[1].ParameterType != typeof(T))
				{
					return false;
				}
				return true;
			}

			internal bool _003CGetEqualityComparerDelegate_003Eb__18_1(Quaternion a, Quaternion b)
			{
				if (a.x == b.x && a.y == b.y && a.z == b.z)
				{
					return a.w == b.w;
				}
				return false;
			}

			internal bool _003CGetEqualityComparerDelegate_003Eb__18_2(T a, T b)
			{
				return ((IEquatable<T>)(object)a).Equals(b);
			}

			internal bool _003CGetEqualityComparerDelegate_003Eb__18_3(T a, T b)
			{
				if ((object)a == (object)b)
				{
					return true;
				}
				if (a == null)
				{
					return false;
				}
				return ((IEquatable<T>)(object)a).Equals(b);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass27_0
		{
			public Type openGenericInterfaceType;

			internal bool _003CImplementsOpenGenericInterface_003Eb__0(Type i)
			{
				if (i.IsGenericType)
				{
					return i.ImplementsOpenGenericInterface(openGenericInterfaceType);
				}
				return false;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass32_0
		{
			public string methodName;

			internal bool _003CGetOperatorMethods_003Eb__0(MethodInfo x)
			{
				return x.Name == methodName;
			}
		}

		private static readonly DoubleLookupDictionary<Type, Type, Func<object, object>> WeaklyTypedTypeCastDelegates = new DoubleLookupDictionary<Type, Type, Func<object, object>>();

		private static readonly DoubleLookupDictionary<Type, Type, Delegate> StronglyTypedTypeCastDelegates = new DoubleLookupDictionary<Type, Type, Delegate>();

		private static HashSet<string> ReservedCSharpKeywords = new HashSet<string>
		{
			"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
			"class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum",
			"event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto",
			"if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
			"new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
			"readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
			"struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
			"unsafe", "ushort", "using", "static", "void", "volatile", "while"
		};

		public static readonly Dictionary<string, string> TypeNameAlternatives = new Dictionary<string, string>
		{
			{ "Single", "float" },
			{ "Double", "double" },
			{ "SByte", "sbyte" },
			{ "Int16", "short" },
			{ "Int32", "int" },
			{ "Int64", "long" },
			{ "Byte", "byte" },
			{ "UInt16", "ushort" },
			{ "UInt32", "uint" },
			{ "UInt64", "ulong" },
			{ "Decimal", "decimal" },
			{ "String", "string" },
			{ "Char", "char" },
			{ "Boolean", "bool" },
			{ "Single[]", "float[]" },
			{ "Double[]", "double[]" },
			{ "SByte[]", "sbyte[]" },
			{ "Int16[]", "short[]" },
			{ "Int32[]", "int[]" },
			{ "Int64[]", "long[]" },
			{ "Byte[]", "byte[]" },
			{ "UInt16[]", "ushort[]" },
			{ "UInt32[]", "uint[]" },
			{ "UInt64[]", "ulong[]" },
			{ "Decimal[]", "decimal[]" },
			{ "String[]", "string[]" },
			{ "Char[]", "char[]" },
			{ "Boolean[]", "bool[]" }
		};

		private static Func<Type, string> getNiceName;

		private static readonly Type VoidPointerType = typeof(void).MakePointerType();

		private static readonly Dictionary<Type, HashSet<Type>> PrimitiveImplicitCasts = new Dictionary<Type, HashSet<Type>>
		{
			{
				typeof(long),
				new HashSet<Type>
				{
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				typeof(int),
				new HashSet<Type>
				{
					typeof(long),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				typeof(short),
				new HashSet<Type>
				{
					typeof(int),
					typeof(long),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				typeof(sbyte),
				new HashSet<Type>
				{
					typeof(short),
					typeof(int),
					typeof(long),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				typeof(ulong),
				new HashSet<Type>
				{
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				typeof(uint),
				new HashSet<Type>
				{
					typeof(long),
					typeof(ulong),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				typeof(ushort),
				new HashSet<Type>
				{
					typeof(int),
					typeof(uint),
					typeof(long),
					typeof(ulong),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				typeof(byte),
				new HashSet<Type>
				{
					typeof(short),
					typeof(ushort),
					typeof(int),
					typeof(uint),
					typeof(long),
					typeof(ulong),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				typeof(char),
				new HashSet<Type>
				{
					typeof(ushort),
					typeof(int),
					typeof(uint),
					typeof(long),
					typeof(ulong),
					typeof(float),
					typeof(double),
					typeof(decimal)
				}
			},
			{
				typeof(bool),
				new HashSet<Type>()
			},
			{
				typeof(decimal),
				new HashSet<Type>()
			},
			{
				typeof(float),
				new HashSet<Type> { typeof(double) }
			},
			{
				typeof(double),
				new HashSet<Type>()
			},
			{
				typeof(IntPtr),
				new HashSet<Type>()
			},
			{
				typeof(UIntPtr),
				new HashSet<Type>()
			},
			{
				VoidPointerType,
				new HashSet<Type>()
			}
		};

		private static readonly HashSet<Type> ExplicitCastIntegrals = new HashSet<Type>
		{
			typeof(long),
			typeof(int),
			typeof(short),
			typeof(sbyte),
			typeof(ulong),
			typeof(uint),
			typeof(ushort),
			typeof(byte),
			typeof(char),
			typeof(decimal),
			typeof(float),
			typeof(double),
			typeof(IntPtr),
			typeof(UIntPtr)
		};

		private static Func<Type, string> GetNiceNameProp
		{
			get
			{
				return getNiceName ?? (getNiceName = (_003C_003Ec._003C_003E9__6_0 ?? (_003C_003Ec._003C_003E9__6_0 = _003C_003Ec._003C_003E9._003Cget_GetNiceNameProp_003Eb__6_0)).Memoize());
			}
		}

		private static bool HasCastDefined(this Type from, Type to, bool requireImplicitCast)
		{
			if (from.IsEnum)
			{
				return Enum.GetUnderlyingType(from).IsCastableTo(to);
			}
			if (to.IsEnum)
			{
				return Enum.GetUnderlyingType(to).IsCastableTo(from);
			}
			if ((from.IsPrimitive || from == VoidPointerType) && (to.IsPrimitive || to == VoidPointerType))
			{
				if (requireImplicitCast)
				{
					return PrimitiveImplicitCasts[from].Contains(to);
				}
				if (from == typeof(IntPtr))
				{
					if (to == typeof(UIntPtr))
					{
						return false;
					}
					if (to == VoidPointerType)
					{
						return true;
					}
				}
				else if (from == typeof(UIntPtr))
				{
					if (to == typeof(IntPtr))
					{
						return false;
					}
					if (to == VoidPointerType)
					{
						return true;
					}
				}
				if (ExplicitCastIntegrals.Contains(from))
				{
					return ExplicitCastIntegrals.Contains(to);
				}
				return false;
			}
			return from.GetCastMethod(to, requireImplicitCast) != null;
		}

		public static bool IsValidIdentifier(string identifier)
		{
			if (identifier == null || identifier.Length == 0)
			{
				return false;
			}
			int num = identifier.IndexOf('.');
			if (num >= 0)
			{
				string[] array = identifier.Split('.');
				for (int i = 0; i < array.Length; i++)
				{
					if (!IsValidIdentifier(array[i]))
					{
						return false;
					}
				}
				return true;
			}
			if (ReservedCSharpKeywords.Contains(identifier))
			{
				return false;
			}
			if (!IsValidIdentifierStartCharacter(identifier[0]))
			{
				return false;
			}
			for (int j = 1; j < identifier.Length; j++)
			{
				if (!IsValidIdentifierPartCharacter(identifier[j]))
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsValidIdentifierStartCharacter(char c)
		{
			if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '_' && c != '@')
			{
				return char.IsLetter(c);
			}
			return true;
		}

		private static bool IsValidIdentifierPartCharacter(char c)
		{
			if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
			{
				switch (c)
				{
				default:
					return char.IsLetter(c);
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '_':
					break;
				}
			}
			return true;
		}

		public static bool IsCastableTo(this Type from, Type to, bool requireImplicitCast = false)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from");
			}
			if (to == null)
			{
				throw new ArgumentNullException("to");
			}
			if (from == to)
			{
				return true;
			}
			if (!to.IsAssignableFrom(from))
			{
				return from.HasCastDefined(to, requireImplicitCast);
			}
			return true;
		}

		public static Func<object, object> GetCastMethodDelegate(this Type from, Type to, bool requireImplicitCast = false)
		{
			Func<object, object> value;
			if (!WeaklyTypedTypeCastDelegates.TryGetInnerValue(from, to, out value))
			{
				_003C_003Ec__DisplayClass15_0 _003C_003Ec__DisplayClass15_ = new _003C_003Ec__DisplayClass15_0();
				_003C_003Ec__DisplayClass15_.method = from.GetCastMethod(to, requireImplicitCast);
				if (_003C_003Ec__DisplayClass15_.method != null)
				{
					value = _003C_003Ec__DisplayClass15_._003CGetCastMethodDelegate_003Eb__0;
				}
				WeaklyTypedTypeCastDelegates.AddInner(from, to, value);
			}
			return value;
		}

		public static Func<TFrom, TTo> GetCastMethodDelegate<TFrom, TTo>(bool requireImplicitCast = false)
		{
			Delegate value;
			if (!StronglyTypedTypeCastDelegates.TryGetInnerValue(typeof(TFrom), typeof(TTo), out value))
			{
				MethodInfo castMethod = typeof(TFrom).GetCastMethod(typeof(TTo), requireImplicitCast);
				if (castMethod != null)
				{
					value = Delegate.CreateDelegate(typeof(Func<TFrom, TTo>), castMethod);
				}
				StronglyTypedTypeCastDelegates.AddInner(typeof(TFrom), typeof(TTo), value);
			}
			return (Func<TFrom, TTo>)value;
		}

		public static MethodInfo GetCastMethod(this Type from, Type to, bool requireImplicitCast = false)
		{
			MethodInfo[] methods = from.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
			foreach (MethodInfo methodInfo in methods)
			{
				if ((methodInfo.Name == "op_Implicit" || (!requireImplicitCast && methodInfo.Name == "op_Explicit")) && to.IsAssignableFrom(methodInfo.ReturnType))
				{
					return methodInfo;
				}
			}
			MethodInfo[] methods2 = to.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			foreach (MethodInfo methodInfo2 in methods2)
			{
				if ((methodInfo2.Name == "op_Implicit" || (!requireImplicitCast && methodInfo2.Name == "op_Explicit")) && methodInfo2.GetParameters()[0].ParameterType.IsAssignableFrom(from))
				{
					return methodInfo2;
				}
			}
			return null;
		}

		public static Func<T, T, bool> GetEqualityComparerDelegate<T>()
		{
			MethodInfo methodInfo = typeof(T).GetOperatorMethods(Operator.Equality).FirstOrDefault(_003C_003Ec__18<T>._003C_003E9__18_0 ?? (_003C_003Ec__18<T>._003C_003E9__18_0 = _003C_003Ec__18<T>._003C_003E9._003CGetEqualityComparerDelegate_003Eb__18_0));
			if (methodInfo != null)
			{
				if (typeof(T) == typeof(Quaternion))
				{
					Func<Quaternion, Quaternion, bool> func = (Func<Quaternion, Quaternion, bool>)Delegate.CreateDelegate(typeof(Func<Quaternion, Quaternion, bool>), methodInfo, true);
					return (Func<T, T, bool>)(object)(_003C_003Ec__18<T>._003C_003E9__18_1 ?? (_003C_003Ec__18<T>._003C_003E9__18_1 = _003C_003Ec__18<T>._003C_003E9._003CGetEqualityComparerDelegate_003Eb__18_1));
				}
				return (Func<T, T, bool>)Delegate.CreateDelegate(typeof(Func<T, T, bool>), methodInfo, true);
			}
			if (typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
			{
				if (typeof(T).IsValueType)
				{
					return _003C_003Ec__18<T>._003C_003E9__18_2 ?? (_003C_003Ec__18<T>._003C_003E9__18_2 = _003C_003Ec__18<T>._003C_003E9._003CGetEqualityComparerDelegate_003Eb__18_2);
				}
				return _003C_003Ec__18<T>._003C_003E9__18_3 ?? (_003C_003Ec__18<T>._003C_003E9__18_3 = _003C_003Ec__18<T>._003C_003E9._003CGetEqualityComparerDelegate_003Eb__18_3);
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			return @default.Equals;
		}

		public static bool HasCustomAttribute<T>(this MemberInfo memberInfo) where T : Attribute
		{
			return memberInfo.HasCustomAttribute(typeof(T), false);
		}

		public static bool HasCustomAttribute<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
		{
			return memberInfo.HasCustomAttribute(typeof(T), inherit);
		}

		public static bool HasCustomAttribute<T>(this MemberInfo memberInfo, out T attribute) where T : Attribute
		{
			return memberInfo.HasCustomAttribute<T>(false, out attribute);
		}

		public static bool HasCustomAttribute<T>(this MemberInfo memberInfo, bool inherit, out T attribute) where T : Attribute
		{
			Attribute attribute2;
			bool flag = memberInfo.HasCustomAttribute(typeof(T), inherit, out attribute2);
			attribute = (flag ? ((T)attribute2) : null);
			return flag;
		}

		public static bool HasCustomAttribute(this MemberInfo memberInfo, Type customAttributeType, bool inherit)
		{
			if (memberInfo == null)
			{
				throw new ArgumentNullException("memberInfo");
			}
			if (customAttributeType == null)
			{
				throw new ArgumentNullException("customAttributeType");
			}
			if (!typeof(Attribute).IsAssignableFrom(customAttributeType))
			{
				throw new ArgumentException("Type " + customAttributeType.Name + " is not an attribute.");
			}
			return memberInfo.GetCustomAttributes(customAttributeType, inherit).Length != 0;
		}

		public static bool HasCustomAttribute(this MemberInfo memberInfo, Type customAttributeType, bool inherit, out Attribute attribute)
		{
			if (memberInfo == null || customAttributeType == null)
			{
				throw new ArgumentNullException();
			}
			if (!typeof(Attribute).IsAssignableFrom(customAttributeType))
			{
				throw new ArgumentException("Type " + customAttributeType.Name + " is not an attribute.");
			}
			object[] customAttributes = memberInfo.GetCustomAttributes(customAttributeType, inherit);
			if (customAttributes.Length != 0)
			{
				attribute = (Attribute)customAttributes[0];
				return true;
			}
			attribute = null;
			return false;
		}

		public static T GetAttribute<T>(this Type type, bool inherit) where T : Attribute
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(T), inherit);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			return (T)customAttributes[0];
		}

		public static bool ImplementsOrInherits(this Type type, Type to)
		{
			return to.IsAssignableFrom(type);
		}

		public static bool ImplementsOpenGenericInterface(this Type candidateType, Type openGenericInterfaceType)
		{
			_003C_003Ec__DisplayClass27_0 _003C_003Ec__DisplayClass27_ = new _003C_003Ec__DisplayClass27_0();
			_003C_003Ec__DisplayClass27_.openGenericInterfaceType = openGenericInterfaceType;
			if (candidateType == null || _003C_003Ec__DisplayClass27_.openGenericInterfaceType == null)
			{
				throw new ArgumentNullException();
			}
			if (!_003C_003Ec__DisplayClass27_.openGenericInterfaceType.IsGenericTypeDefinition || !_003C_003Ec__DisplayClass27_.openGenericInterfaceType.IsInterface)
			{
				throw new ArgumentException("Type " + _003C_003Ec__DisplayClass27_.openGenericInterfaceType.Name + " is not a generic type definition and an interface.");
			}
			if (!candidateType.Equals(_003C_003Ec__DisplayClass27_.openGenericInterfaceType) && (!candidateType.IsGenericType || !candidateType.GetGenericTypeDefinition().Equals(_003C_003Ec__DisplayClass27_.openGenericInterfaceType)))
			{
				return candidateType.GetInterfaces().Any(_003C_003Ec__DisplayClass27_._003CImplementsOpenGenericInterface_003Eb__0);
			}
			return true;
		}

		public static bool ImplementsOpenGenericClass(this Type candidateType, Type openGenericType)
		{
			if (candidateType == null || openGenericType == null)
			{
				throw new ArgumentNullException();
			}
			if (!openGenericType.IsGenericTypeDefinition || !openGenericType.IsClass)
			{
				throw new ArgumentException("Type " + openGenericType.Name + " is not a generic type definition and a class.");
			}
			if (!candidateType.IsGenericType || candidateType.GetGenericTypeDefinition() != openGenericType)
			{
				if (candidateType.BaseType != null)
				{
					return candidateType.BaseType.ImplementsOpenGenericClass(openGenericType);
				}
				return false;
			}
			return true;
		}

		public static Type[] GetArgumentsOfInheritedOpenGenericClass(this Type candidateType, Type openGenericType)
		{
			if (candidateType == null || openGenericType == null)
			{
				throw new ArgumentNullException();
			}
			if (!openGenericType.IsGenericTypeDefinition || !openGenericType.IsClass)
			{
				throw new ArgumentException("Type " + openGenericType.Name + " is not a generic type definition and a class.");
			}
			if (candidateType.IsGenericType && candidateType.GetGenericTypeDefinition() == openGenericType)
			{
				return candidateType.GetGenericArguments();
			}
			if (candidateType.BaseType != null && candidateType.BaseType.ImplementsOpenGenericClass(openGenericType))
			{
				return candidateType.BaseType.GetArgumentsOfInheritedOpenGenericClass(openGenericType);
			}
			return new Type[0];
		}

		public static Type[] GetArgumentsOfInheritedOpenGenericInterface(this Type candidateType, Type openGenericInterfaceType)
		{
			if (candidateType == null || openGenericInterfaceType == null)
			{
				throw new ArgumentNullException();
			}
			if (!openGenericInterfaceType.IsGenericTypeDefinition || !openGenericInterfaceType.IsInterface)
			{
				throw new ArgumentException("Type " + openGenericInterfaceType.Name + " is not a generic type definition and an interface.");
			}
			if (candidateType.Equals(openGenericInterfaceType) || (candidateType.IsGenericType && candidateType.GetGenericTypeDefinition().Equals(openGenericInterfaceType)))
			{
				return candidateType.GetGenericArguments();
			}
			Type[] interfaces = candidateType.GetInterfaces();
			foreach (Type type in interfaces)
			{
				Type[] argumentsOfInheritedOpenGenericInterface;
				if (type.IsGenericType && (argumentsOfInheritedOpenGenericInterface = type.GetArgumentsOfInheritedOpenGenericInterface(openGenericInterfaceType)) != null)
				{
					return argumentsOfInheritedOpenGenericInterface;
				}
			}
			return null;
		}

		public static MethodInfo GetOperatorMethod(this Type type, Operator op)
		{
			string name;
			switch (op)
			{
			case Operator.Equality:
				name = "op_Equality";
				break;
			case Operator.Inequality:
				name = "op_Inequality";
				break;
			case Operator.Addition:
				name = "op_Addition";
				break;
			case Operator.Subtraction:
				name = "op_Subtraction";
				break;
			case Operator.LessThan:
				name = "op_LessThan";
				break;
			case Operator.GreaterThan:
				name = "op_GreaterThan";
				break;
			case Operator.LessThanOrEqual:
				name = "op_LessThanOrEqual";
				break;
			case Operator.GreaterThanOrEqual:
				name = "op_GreaterThanOrEqual";
				break;
			default:
				throw new NotImplementedException();
			}
			return type.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static MethodInfo[] GetOperatorMethods(this Type type, Operator op)
		{
			_003C_003Ec__DisplayClass32_0 _003C_003Ec__DisplayClass32_ = new _003C_003Ec__DisplayClass32_0();
			switch (op)
			{
			case Operator.Equality:
				_003C_003Ec__DisplayClass32_.methodName = "op_Equality";
				break;
			case Operator.Inequality:
				_003C_003Ec__DisplayClass32_.methodName = "op_Inequality";
				break;
			case Operator.Addition:
				_003C_003Ec__DisplayClass32_.methodName = "op_Addition";
				break;
			case Operator.Subtraction:
				_003C_003Ec__DisplayClass32_.methodName = "op_Subtraction";
				break;
			case Operator.LessThan:
				_003C_003Ec__DisplayClass32_.methodName = "op_LessThan";
				break;
			case Operator.GreaterThan:
				_003C_003Ec__DisplayClass32_.methodName = "op_GreaterThan";
				break;
			case Operator.LessThanOrEqual:
				_003C_003Ec__DisplayClass32_.methodName = "op_LessThanOrEqual";
				break;
			case Operator.GreaterThanOrEqual:
				_003C_003Ec__DisplayClass32_.methodName = "op_GreaterThanOrEqual";
				break;
			default:
				throw new NotImplementedException();
			}
			return type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(_003C_003Ec__DisplayClass32_._003CGetOperatorMethods_003Eb__0).ToArray();
		}

		public static IEnumerable<MemberInfo> GetAllMembers(this Type type, BindingFlags flags = BindingFlags.Default)
		{
			Type currentType = type;
			flags |= BindingFlags.DeclaredOnly;
			do
			{
				MemberInfo[] members = currentType.GetMembers(flags);
				for (int i = 0; i < members.Length; i++)
				{
					yield return members[i];
				}
				currentType = currentType.BaseType;
			}
			while (currentType != typeof(object));
		}

		public static IEnumerable<MemberInfo> GetAllMembers(this Type type, string name, BindingFlags flags = BindingFlags.Default)
		{
			Type currentType = type;
			flags |= BindingFlags.DeclaredOnly;
			do
			{
				MemberInfo[] member = currentType.GetMember(name, flags);
				for (int i = 0; i < member.Length; i++)
				{
					yield return member[i];
				}
				currentType = currentType.BaseType;
			}
			while (currentType != typeof(object));
		}

		public static IEnumerable<T> GetAllMembers<T>(this Type type, BindingFlags flags = BindingFlags.Default) where T : MemberInfo
		{
			Type currentType = type;
			flags |= BindingFlags.DeclaredOnly;
			do
			{
				MemberInfo[] members = currentType.GetMembers(flags);
				foreach (MemberInfo memberInfo in members)
				{
					T val = memberInfo as T;
					if (val != null)
					{
						yield return val;
					}
				}
				currentType = currentType.BaseType;
			}
			while (currentType != typeof(object));
		}

		public static Type GetGenericBaseType(this Type type, Type baseType)
		{
			int depthCount;
			return type.GetGenericBaseType(baseType, out depthCount);
		}

		public static Type GetGenericBaseType(this Type type, Type baseType, out int depthCount)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (baseType == null)
			{
				throw new ArgumentNullException("baseType");
			}
			if (!baseType.IsGenericType)
			{
				throw new ArgumentException("Type " + baseType.Name + " is not a generic type.");
			}
			if (!type.InheritsFrom(baseType))
			{
				throw new ArgumentException("Type " + type.Name + " does not inherit from " + baseType.Name + ".");
			}
			Type type2 = type;
			depthCount = 0;
			while (type2 != null && (!type2.IsGenericType || type2.GetGenericTypeDefinition() != baseType))
			{
				depthCount++;
				type2 = type2.BaseType;
			}
			if (type2 == null)
			{
				throw new ArgumentException(type.Name + " is assignable from " + baseType.Name + ", but base type was not found?");
			}
			return type2;
		}

		public static IEnumerable<Type> GetBaseTypes(this Type type, bool includeSelf = false)
		{
			IEnumerable<Type> enumerable = type.GetBaseClasses(includeSelf).Concat(type.GetInterfaces());
			if (includeSelf && type.IsInterface)
			{
				enumerable.Concat(new Type[1] { type });
			}
			return enumerable;
		}

		public static IEnumerable<Type> GetBaseClasses(this Type type, bool includeSelf = false)
		{
			if (type != null && type.BaseType != null)
			{
				if (includeSelf)
				{
					yield return type;
				}
				for (Type current = type.BaseType; current != null; current = current.BaseType)
				{
					yield return current;
				}
			}
		}

		private static string TypeNameGauntlet(this Type type)
		{
			string text = type.Name;
			string value = string.Empty;
			if (TypeNameAlternatives.TryGetValue(text, out value))
			{
				text = value;
			}
			return text;
		}

		public static string GetNiceName(this Type type)
		{
			if (type.IsNested && !type.IsGenericParameter)
			{
				return type.DeclaringType.GetNiceName() + "." + GetNiceNameProp(type);
			}
			return GetNiceNameProp(type);
		}

		public static string GetNiceFullName(this Type type)
		{
			if (type.IsNested && !type.IsGenericParameter)
			{
				return type.DeclaringType.GetNiceFullName() + "." + GetNiceNameProp(type);
			}
			string text = GetNiceNameProp(type);
			if (type.Namespace != null)
			{
				text = type.Namespace + "." + text;
			}
			return text;
		}

		public static string GetCompilableNiceName(this Type type)
		{
			return type.GetNiceName().Replace('<', '_').Replace('>', '_')
				.TrimEnd('_');
		}

		public static string GetCompilableNiceFullName(this Type type)
		{
			return type.GetNiceFullName().Replace('<', '_').Replace('>', '_')
				.TrimEnd('_');
		}

		public static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
		{
			T[] array = type.GetCustomAttributes<T>(inherit).ToArray();
			if (!array.IsNullOrEmpty())
			{
				return array[0];
			}
			return null;
		}

		public static T GetCustomAttribute<T>(this Type type) where T : Attribute
		{
			return type.GetCustomAttribute<T>(false);
		}

		public static IEnumerable<T> GetCustomAttributes<T>(this Type type) where T : Attribute
		{
			return type.GetCustomAttributes<T>(false);
		}

		public static IEnumerable<T> GetCustomAttributes<T>(this Type type, bool inherit) where T : Attribute
		{
			return type.GetCustomAttributes(typeof(T), inherit).Cast<T>();
		}

		public static bool IsDefined<T>(this Type type) where T : Attribute
		{
			return type.IsDefined(typeof(T), false);
		}

		public static bool IsDefined<T>(this Type type, bool inherit) where T : Attribute
		{
			return type.IsDefined(typeof(T), inherit);
		}

		public static bool InheritsFrom<TBase>(this Type type)
		{
			return type.InheritsFrom(typeof(TBase));
		}

		public static bool InheritsFrom(this Type type, Type baseType)
		{
			if (baseType.IsAssignableFrom(type))
			{
				return true;
			}
			if (type.IsInterface && !baseType.IsInterface)
			{
				return false;
			}
			if (baseType.IsInterface)
			{
				return type.GetInterfaces().Contains(baseType);
			}
			for (Type type2 = type; type2 != null; type2 = type2.BaseType)
			{
				if (type2 == baseType)
				{
					return true;
				}
				if (baseType.IsGenericTypeDefinition && type2.IsGenericType && type2.GetGenericTypeDefinition() == baseType)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetInheritanceDistance(this Type type, Type baseType)
		{
			Type type2;
			Type type3;
			if (type.IsAssignableFrom(baseType))
			{
				type2 = baseType;
				type3 = type;
			}
			else
			{
				if (!baseType.IsAssignableFrom(type))
				{
					throw new ArgumentException("Cannot assign types '" + type.GetNiceName() + "' and '" + baseType.GetNiceName() + "' to each other.");
				}
				type2 = type;
				type3 = baseType;
			}
			Type type4 = type3;
			int num = 0;
			if (type2.IsInterface)
			{
				while (type4 != null && type4 != typeof(object))
				{
					num++;
					type4 = type4.BaseType;
					Type[] interfaces = type4.GetInterfaces();
					for (int i = 0; i < interfaces.Length; i++)
					{
						if (interfaces[i] == type2)
						{
							type4 = null;
							break;
						}
					}
				}
			}
			else
			{
				while (type4 != type2 && type4 != null && type4 != typeof(object))
				{
					num++;
					type4 = type4.BaseType;
				}
			}
			return num;
		}

		public static bool HasParamaters(this MethodInfo methodInfo, IList<Type> paramTypes, bool inherit = true)
		{
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length == paramTypes.Count)
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					if (inherit && !paramTypes[i].InheritsFrom(parameters[i].ParameterType))
					{
						return false;
					}
					if (parameters[i].ParameterType != paramTypes[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public static Type GetReturnType(this MemberInfo memberInfo)
		{
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			if (fieldInfo != null)
			{
				return fieldInfo.FieldType;
			}
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
			{
				return propertyInfo.PropertyType;
			}
			MethodInfo methodInfo = memberInfo as MethodInfo;
			if (methodInfo != null)
			{
				return methodInfo.ReturnType;
			}
			EventInfo eventInfo = memberInfo as EventInfo;
			if (eventInfo != null)
			{
				return eventInfo.EventHandlerType;
			}
			return null;
		}

		public static object GetMemberValue(this MemberInfo member, object obj)
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

		public static void SetMemberValue(this MemberInfo member, object obj, object value)
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

		public static bool TryInferGenericParameters(this Type genericTypeDefinition, out Type[] inferredParams, params Type[] knownParameters)
		{
			if (genericTypeDefinition == null)
			{
				throw new ArgumentNullException("genericTypeDefinition");
			}
			if (knownParameters == null)
			{
				throw new ArgumentNullException("knownParameters");
			}
			if (!genericTypeDefinition.IsGenericType)
			{
				throw new ArgumentException("The genericTypeDefinition parameter must be a generic type.");
			}
			Dictionary<Type, Type> dictionary = new Dictionary<Type, Type>();
			Type[] genericArguments = genericTypeDefinition.GetGenericArguments();
			if (!genericTypeDefinition.IsGenericTypeDefinition)
			{
				Type[] array = genericArguments;
				genericTypeDefinition = genericTypeDefinition.GetGenericTypeDefinition();
				genericArguments = genericTypeDefinition.GetGenericArguments();
				int num = 0;
				for (int i = 0; i < array.Length; i++)
				{
					if (!array[i].IsGenericParameter)
					{
						dictionary[genericArguments[i]] = array[i];
					}
					else
					{
						num++;
					}
				}
				if (num == knownParameters.Length)
				{
					int num2 = 0;
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].IsGenericParameter)
						{
							array[j] = knownParameters[num2++];
						}
					}
					if (genericTypeDefinition.AreGenericConstraintsSatisfiedBy(array))
					{
						inferredParams = array;
						return true;
					}
				}
			}
			if (genericArguments.Length == knownParameters.Length && genericTypeDefinition.AreGenericConstraintsSatisfiedBy(knownParameters))
			{
				inferredParams = knownParameters;
				return true;
			}
			Type[] array2 = genericArguments;
			foreach (Type type in array2)
			{
				if (dictionary.ContainsKey(type))
				{
					continue;
				}
				Type[] genericParameterConstraints = type.GetGenericParameterConstraints();
				Type[] array3 = genericParameterConstraints;
				foreach (Type type2 in array3)
				{
					foreach (Type type3 in knownParameters)
					{
						if (!type2.IsGenericType)
						{
							continue;
						}
						Type genericTypeDefinition2 = type2.GetGenericTypeDefinition();
						Type[] genericArguments2 = type2.GetGenericArguments();
						Type[] array4;
						if (type3.IsGenericType && genericTypeDefinition2 == type3.GetGenericTypeDefinition())
						{
							array4 = type3.GetGenericArguments();
						}
						else if (genericTypeDefinition2.IsInterface && type3.ImplementsOpenGenericInterface(genericTypeDefinition2))
						{
							array4 = type3.GetArgumentsOfInheritedOpenGenericInterface(genericTypeDefinition2);
						}
						else
						{
							if (!genericTypeDefinition2.IsClass || !type3.ImplementsOpenGenericClass(genericTypeDefinition2))
							{
								continue;
							}
							array4 = type3.GetArgumentsOfInheritedOpenGenericClass(genericTypeDefinition2);
						}
						dictionary[type] = type3;
						for (int n = 0; n < genericArguments2.Length; n++)
						{
							if (genericArguments2[n].IsGenericParameter)
							{
								dictionary[genericArguments2[n]] = array4[n];
							}
						}
					}
				}
			}
			if (dictionary.Count == genericArguments.Length)
			{
				inferredParams = new Type[dictionary.Count];
				for (int num3 = 0; num3 < genericArguments.Length; num3++)
				{
					inferredParams[num3] = dictionary[genericArguments[num3]];
				}
				if (genericTypeDefinition.AreGenericConstraintsSatisfiedBy(inferredParams))
				{
					return true;
				}
			}
			inferredParams = null;
			return false;
		}

		public static bool AreGenericConstraintsSatisfiedBy(this Type genericTypeDefinition, params Type[] parameters)
		{
			if (genericTypeDefinition == null)
			{
				throw new ArgumentNullException("genericTypeDefinition");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("types");
			}
			if (!genericTypeDefinition.IsGenericTypeDefinition)
			{
				throw new ArgumentException("The genericTypeDefinition parameter must be a generic type definition.");
			}
			Type[] genericArguments = genericTypeDefinition.GetGenericArguments();
			if (genericArguments.Length != parameters.Length)
			{
				return false;
			}
			Dictionary<Type, Type> resolvedMap = new Dictionary<Type, Type>();
			for (int i = 0; i < genericArguments.Length; i++)
			{
				Type genericParameterDefinition = genericArguments[i];
				Type parameterType = parameters[i];
				if (!genericParameterDefinition.TypeFulfillsGenericParameter(parameterType, resolvedMap))
				{
					return false;
				}
			}
			return true;
		}

		private static bool TypeFulfillsGenericParameter(this Type genericParameterDefinition, Type parameterType, Dictionary<Type, Type> resolvedMap)
		{
			if (genericParameterDefinition == null)
			{
				throw new ArgumentNullException("genericParameterDefinition");
			}
			if (parameterType == null)
			{
				throw new ArgumentNullException("parameterType");
			}
			if (resolvedMap == null)
			{
				throw new ArgumentNullException("resolvedMap");
			}
			if (!genericParameterDefinition.IsGenericParameter && genericParameterDefinition.IsAssignableFrom(parameterType))
			{
				return true;
			}
			GenericParameterAttributes genericParameterAttributes = genericParameterDefinition.GenericParameterAttributes;
			if (genericParameterAttributes != 0)
			{
				if ((genericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint)
				{
					if (!parameterType.IsValueType || (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>)))
					{
						return false;
					}
				}
				else if ((genericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint && parameterType.IsValueType)
				{
					return false;
				}
				if ((genericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint && (parameterType.IsAbstract || (!parameterType.IsValueType && parameterType.GetConstructor(Type.EmptyTypes) == null)))
				{
					return false;
				}
			}
			if (resolvedMap.ContainsKey(genericParameterDefinition) && !parameterType.IsAssignableFrom(resolvedMap[genericParameterDefinition]))
			{
				return false;
			}
			Type[] genericParameterConstraints = genericParameterDefinition.GetGenericParameterConstraints();
			for (int i = 0; i < genericParameterConstraints.Length; i++)
			{
				Type type = genericParameterConstraints[i];
				if (type.IsGenericParameter && resolvedMap.ContainsKey(type))
				{
					type = resolvedMap[type];
				}
				if (type.IsGenericParameter)
				{
					if (!type.TypeFulfillsGenericParameter(parameterType, resolvedMap))
					{
						return false;
					}
					continue;
				}
				if (type.IsClass || type.IsInterface || type.IsValueType)
				{
					if (type.IsGenericType)
					{
						Type genericTypeDefinition = type.GetGenericTypeDefinition();
						Type[] genericArguments = type.GetGenericArguments();
						Type[] array;
						if (parameterType.IsGenericType && genericTypeDefinition == parameterType.GetGenericTypeDefinition())
						{
							array = parameterType.GetGenericArguments();
						}
						else if (genericTypeDefinition.IsClass)
						{
							if (!parameterType.ImplementsOpenGenericClass(genericTypeDefinition))
							{
								return false;
							}
							array = parameterType.GetArgumentsOfInheritedOpenGenericClass(genericTypeDefinition);
						}
						else
						{
							if (!parameterType.ImplementsOpenGenericInterface(genericTypeDefinition))
							{
								return false;
							}
							array = parameterType.GetArgumentsOfInheritedOpenGenericInterface(genericTypeDefinition);
						}
						for (int j = 0; j < genericArguments.Length; j++)
						{
							Type type2 = genericArguments[j];
							Type type3 = array[j];
							if (type2.IsGenericParameter && resolvedMap.ContainsKey(type2))
							{
								type2 = resolvedMap[type2];
							}
							if (type2.IsGenericParameter)
							{
								if (!type2.TypeFulfillsGenericParameter(type3, resolvedMap))
								{
									return false;
								}
							}
							else if (type2 != type3 && !type2.IsAssignableFrom(type3))
							{
								return false;
							}
						}
					}
					else if (!type.IsAssignableFrom(parameterType))
					{
						return false;
					}
					continue;
				}
				throw new Exception("Unknown parameter constraint type! " + type.GetNiceName());
			}
			resolvedMap[genericParameterDefinition] = parameterType;
			return true;
		}

		public static string GetGenericConstraintsString(this Type type, bool useFullTypeNames = false)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!type.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Type '" + type.GetNiceName() + "' is not a generic type definition!");
			}
			Type[] genericArguments = type.GetGenericArguments();
			string[] array = new string[genericArguments.Length];
			for (int i = 0; i < genericArguments.Length; i++)
			{
				array[i] = genericArguments[i].GetGenericParameterConstraintsString(useFullTypeNames);
			}
			return string.Join(" ", array);
		}

		public static string GetGenericParameterConstraintsString(this Type type, bool useFullTypeNames = false)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!type.IsGenericParameter)
			{
				throw new ArgumentException("Type '" + type.GetNiceName() + "' is not a generic parameter!");
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			GenericParameterAttributes genericParameterAttributes = type.GenericParameterAttributes;
			if ((genericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint)
			{
				stringBuilder.Append("where ").Append(type.Name).Append(" : struct");
				flag = true;
			}
			else if ((genericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint)
			{
				stringBuilder.Append("where ").Append(type.Name).Append(" : class");
				flag = true;
			}
			if ((genericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint)
			{
				if (flag)
				{
					stringBuilder.Append(", new()");
				}
				else
				{
					stringBuilder.Append("where ").Append(type.Name).Append(" : new()");
					flag = true;
				}
			}
			Type[] genericParameterConstraints = type.GetGenericParameterConstraints();
			if (genericParameterConstraints.Length != 0)
			{
				foreach (Type type2 in genericParameterConstraints)
				{
					if (flag)
					{
						stringBuilder.Append(", ");
						if (useFullTypeNames)
						{
							stringBuilder.Append(type2.GetNiceFullName());
						}
						else
						{
							stringBuilder.Append(type2.GetNiceName());
						}
						continue;
					}
					stringBuilder.Append("where ").Append(type.Name).Append(" : ");
					if (useFullTypeNames)
					{
						stringBuilder.Append(type2.GetNiceFullName());
					}
					else
					{
						stringBuilder.Append(type2.GetNiceName());
					}
					flag = true;
				}
			}
			return stringBuilder.ToString();
		}

		public static bool GenericArgumentsContainsTypes(this Type type, params Type[] types)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!type.IsGenericType)
			{
				return false;
			}
			bool[] array = new bool[types.Length];
			Type[] genericArguments = type.GetGenericArguments();
			Stack<Type> stack = new Stack<Type>(genericArguments);
			while (stack.Count > 0)
			{
				Type type2 = stack.Pop();
				for (int i = 0; i < types.Length; i++)
				{
					Type type3 = types[i];
					if (type3 == type2)
					{
						array[i] = true;
					}
					else if (type3.IsGenericTypeDefinition && type2.IsGenericType && !type2.IsGenericTypeDefinition && type2.GetGenericTypeDefinition() == type3)
					{
						array[i] = true;
					}
				}
				bool flag = true;
				for (int j = 0; j < array.Length; j++)
				{
					if (!array[j])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return true;
				}
				if (type2.IsGenericType)
				{
					Type[] genericArguments2 = type2.GetGenericArguments();
					foreach (Type t in genericArguments2)
					{
						stack.Push(t);
					}
				}
			}
			return false;
		}

		public static bool IsFullyConstructedGenericType(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!type.IsGenericType || type.IsGenericTypeDefinition)
			{
				return false;
			}
			Type[] genericArguments = type.GetGenericArguments();
			foreach (Type type2 in genericArguments)
			{
				if (type2.IsGenericParameter)
				{
					return false;
				}
				if (type2.IsGenericType && !type2.IsFullyConstructedGenericType())
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsNullableType(this Type type)
		{
			if (!type.IsPrimitive && !type.IsValueType)
			{
				return !type.IsEnum;
			}
			return false;
		}

		public static ulong GetEnumBitmask(object value, Type enumType)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("enumType");
			}
			try
			{
				return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
			}
			catch (OverflowException)
			{
				return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);
			}
		}
	}
}
