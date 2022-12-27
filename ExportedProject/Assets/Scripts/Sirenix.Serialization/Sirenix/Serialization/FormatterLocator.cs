using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.Serialization
{
	public static class FormatterLocator
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<_003C_003Ef__AnonymousType0<Type, CustomFormatterAttribute, Type>, int> _003C_003E9__4_6;

			public static Func<_003C_003Ef__AnonymousType0<Type, CustomGenericFormatterAttribute, Type>, int> _003C_003E9__4_14;

			public static Func<Type, string> _003C_003E9__13_0;

			internal IEnumerable<Type> _003C_002Ecctor_003Eb__4_0(Assembly ass)
			{
				try
				{
					if (ass.GetName().Name == "Sirenix.Serialization.AOTGenerated" && EmitUtilities.CanEmit)
					{
						return Enumerable.Empty<Type>();
					}
					return ass.GetTypes();
				}
				catch (TypeLoadException)
				{
					if (ass.GetName().Name == "Sirenix.Serialization")
					{
						Debug.LogError("A TypeLoadException occurred when FormatterLocator tried to load types from assembly '" + ass.FullName + "'. No serialization formatters in this assembly will be found. Serialization will be utterly broken.");
					}
					return Enumerable.Empty<Type>();
				}
				catch (ReflectionTypeLoadException)
				{
					if (ass.GetName().Name == "Sirenix.Serialization")
					{
						Debug.LogError("A ReflectionTypeLoadException occurred when FormatterLocator tried to load types from assembly '" + ass.FullName + "'. No serialization formatters in this assembly will be found. Serialization will be utterly broken.");
					}
					return Enumerable.Empty<Type>();
				}
			}

			internal bool _003C_002Ecctor_003Eb__4_1(Type t)
			{
				if (t.IsClass && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null && t.IsDefined(typeof(CustomFormatterAttribute), true))
				{
					return t.ImplementsOpenGenericInterface(typeof(IFormatter<>));
				}
				return false;
			}

			internal bool _003C_002Ecctor_003Eb__4_2(Type t)
			{
				if (!t.IsGenericType)
				{
					return !t.IsGenericTypeDefinition;
				}
				return false;
			}

			internal _003C_003Ef__AnonymousType0<Type, CustomFormatterAttribute, Type> _003C_002Ecctor_003Eb__4_3(Type t)
			{
				return new _003C_003Ef__AnonymousType0<Type, CustomFormatterAttribute, Type>(t, t.GetAttribute<CustomFormatterAttribute>(true), t.GetArgumentsOfInheritedOpenGenericInterface(typeof(IFormatter<>))[0]);
			}

			internal Type _003C_002Ecctor_003Eb__4_4(_003C_003Ef__AnonymousType0<Type, CustomFormatterAttribute, Type> n)
			{
				return n.SerializedType;
			}

			internal _003C_003Ef__AnonymousType0<Type, CustomFormatterAttribute, Type> _003C_002Ecctor_003Eb__4_5(IGrouping<Type, _003C_003Ef__AnonymousType0<Type, CustomFormatterAttribute, Type>> n)
			{
				return n.OrderByDescending(_003C_003E9__4_6 ?? (_003C_003E9__4_6 = _003C_003E9._003C_002Ecctor_003Eb__4_6)).First();
			}

			internal int _003C_002Ecctor_003Eb__4_6(_003C_003Ef__AnonymousType0<Type, CustomFormatterAttribute, Type> m)
			{
				return m.Attr.Priority;
			}

			internal Type _003C_002Ecctor_003Eb__4_7(_003C_003Ef__AnonymousType0<Type, CustomFormatterAttribute, Type> n)
			{
				return n.SerializedType;
			}

			internal Type _003C_002Ecctor_003Eb__4_8(_003C_003Ef__AnonymousType0<Type, CustomFormatterAttribute, Type> n)
			{
				return n.Type;
			}

			internal bool _003C_002Ecctor_003Eb__4_9(Type t)
			{
				if (t.IsGenericTypeDefinition)
				{
					return t.IsDefined(typeof(CustomGenericFormatterAttribute), true);
				}
				return false;
			}

			internal _003C_003Ef__AnonymousType0<Type, CustomGenericFormatterAttribute, Type> _003C_002Ecctor_003Eb__4_10(Type t)
			{
				return new _003C_003Ef__AnonymousType0<Type, CustomGenericFormatterAttribute, Type>(t, t.GetAttribute<CustomGenericFormatterAttribute>(true), t.GetArgumentsOfInheritedOpenGenericInterface(typeof(IFormatter<>))[0]);
			}

			internal bool _003C_002Ecctor_003Eb__4_11(_003C_003Ef__AnonymousType0<Type, CustomGenericFormatterAttribute, Type> n)
			{
				if (n.SerializedType.IsGenericType)
				{
					return n.SerializedType.GetGenericTypeDefinition() == n.Attr.SerializedGenericTypeDefinition;
				}
				return false;
			}

			internal Type _003C_002Ecctor_003Eb__4_12(_003C_003Ef__AnonymousType0<Type, CustomGenericFormatterAttribute, Type> n)
			{
				return n.Attr.SerializedGenericTypeDefinition;
			}

			internal _003C_003Ef__AnonymousType0<Type, CustomGenericFormatterAttribute, Type> _003C_002Ecctor_003Eb__4_13(IGrouping<Type, _003C_003Ef__AnonymousType0<Type, CustomGenericFormatterAttribute, Type>> n)
			{
				return n.OrderByDescending(_003C_003E9__4_14 ?? (_003C_003E9__4_14 = _003C_003E9._003C_002Ecctor_003Eb__4_14)).First();
			}

			internal int _003C_002Ecctor_003Eb__4_14(_003C_003Ef__AnonymousType0<Type, CustomGenericFormatterAttribute, Type> m)
			{
				return m.Attr.Priority;
			}

			internal Type _003C_002Ecctor_003Eb__4_15(_003C_003Ef__AnonymousType0<Type, CustomGenericFormatterAttribute, Type> n)
			{
				return n.Attr.SerializedGenericTypeDefinition;
			}

			internal Type _003C_002Ecctor_003Eb__4_16(_003C_003Ef__AnonymousType0<Type, CustomGenericFormatterAttribute, Type> n)
			{
				return n.Type;
			}

			internal string _003CLogAOTError_003Eb__13_0(Type t)
			{
				return t.GetNiceFullName();
			}
		}

		private static readonly object LOCK;

		private static readonly Dictionary<Type, Type> CustomGenericFormatterTypes;

		private static readonly Dictionary<Type, Type> CustomFormatterTypes;

		private static readonly Dictionary<Type, IFormatter> Formatters;

		public static event Func<Type, IFormatter> FormatterResolve
		{
			add
			{
				lock (LOCK)
				{
					FormatterResolvePrivate += value;
				}
			}
			remove
			{
				lock (LOCK)
				{
					FormatterResolvePrivate -= value;
				}
			}
		}

		private static event Func<Type, IFormatter> FormatterResolvePrivate;

		static FormatterLocator()
		{
			LOCK = new object();
			Formatters = new Dictionary<Type, IFormatter>();
			Type[] source = AppDomain.CurrentDomain.GetAssemblies().SelectMany(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_0).Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_1)
				.ToArray();
			CustomFormatterTypes = source.Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_2).Select(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_3).GroupBy(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_4)
				.Select(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_5)
				.ToDictionary(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_7, _003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_8);
			CustomGenericFormatterTypes = source.Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_9).Select(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_10).Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_11)
				.GroupBy(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_12)
				.Select(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_13)
				.ToDictionary(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_15, _003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__4_16);
		}

		public static IFormatter<T> GetFormatter<T>(ISerializationPolicy policy)
		{
			return (IFormatter<T>)GetFormatter(typeof(T), policy);
		}

		public static IFormatter GetFormatter(Type type, ISerializationPolicy policy)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (policy == null)
			{
				policy = SerializationPolicies.Strict;
			}
			lock (LOCK)
			{
				IFormatter value;
				if (!Formatters.TryGetValue(type, out value))
				{
					try
					{
						value = CreateFormatter(type, policy);
					}
					catch (TargetInvocationException ex)
					{
						if (!(ex.GetBaseException() is ExecutionEngineException))
						{
							throw ex;
						}
						LogAOTError(type, ex.GetBaseException() as ExecutionEngineException);
					}
					catch (TypeInitializationException ex2)
					{
						if (!(ex2.GetBaseException() is ExecutionEngineException))
						{
							throw ex2;
						}
						LogAOTError(type, ex2.GetBaseException() as ExecutionEngineException);
					}
					catch (ExecutionEngineException ex3)
					{
						LogAOTError(type, ex3);
					}
					Formatters.Add(type, value);
					return value;
				}
				return value;
			}
		}

		private static void LogAOTError(Type type, ExecutionEngineException ex)
		{
			string[] value = GetAllPossibleMissingAOTTypes(type).Select(_003C_003Ec._003C_003E9__13_0 ?? (_003C_003Ec._003C_003E9__13_0 = _003C_003Ec._003C_003E9._003CLogAOTError_003Eb__13_0)).ToArray();
			Debug.LogError("Creating a serialization formatter for the type '" + type.GetNiceFullName() + "' failed. \n\n Please use Odin's AOT generation feature to generate an AOT dll before building, and ensure that all of the following types are in the supported types list after a scan (if they are not, please report an issue with the details and add them manually): \n\n" + string.Join("\n", value) + "\n\nThe exception contained the following message: \n" + ex.Message);
			throw new SerializationAbortException("AOT formatter support was missing for type '" + type.GetNiceFullName() + "'.");
		}

		private static IEnumerable<Type> GetAllPossibleMissingAOTTypes(Type type)
		{
			yield return type;
			if (!type.IsGenericType)
			{
				yield break;
			}
			Type[] genericArguments = type.GetGenericArguments();
			foreach (Type arg in genericArguments)
			{
				yield return arg;
				if (!arg.IsGenericType)
				{
					continue;
				}
				foreach (Type allPossibleMissingAOTType in GetAllPossibleMissingAOTTypes(arg))
				{
					yield return allPossibleMissingAOTType;
				}
			}
		}

		private static IFormatter CreateFormatter(Type type, ISerializationPolicy policy)
		{
			if (FormatterUtilities.IsPrimitiveType(type))
			{
				throw new ArgumentException("Cannot create formatters for a primitive type like " + type.Name);
			}
			bool flag = type.ImplementsOrInherits(typeof(ISelfFormatter));
			if (flag && type.IsDefined<AlwaysFormatsSelfAttribute>())
			{
				return (IFormatter)Activator.CreateInstance(typeof(SelfFormatterFormatter<>).MakeGenericType(type));
			}
			if (FormatterLocator.FormatterResolvePrivate != null)
			{
				Type to = typeof(IFormatter<>).MakeGenericType(type);
				Delegate[] invocationList = FormatterLocator.FormatterResolvePrivate.GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					IFormatter formatter = @delegate.Method.Invoke(@delegate.Target, new object[1] { type }) as IFormatter;
					if (formatter != null && formatter.GetType().ImplementsOrInherits(to))
					{
						return formatter;
					}
				}
			}
			Type value;
			if (CustomFormatterTypes.TryGetValue(type, out value))
			{
				return (IFormatter)Activator.CreateInstance(value);
			}
			if (type.IsGenericType)
			{
				Type genericTypeDefinition = type.GetGenericTypeDefinition();
				Type value2;
				if (CustomGenericFormatterTypes.TryGetValue(genericTypeDefinition, out value2))
				{
					Type type2 = value2.MakeGenericType(type.GetGenericArguments());
					return (IFormatter)Activator.CreateInstance(type2);
				}
			}
			if (type.ImplementsOpenGenericClass(typeof(Dictionary<, >)) && type.GetConstructor(Type.EmptyTypes) != null)
			{
				Type[] argumentsOfInheritedOpenGenericClass = type.GetArgumentsOfInheritedOpenGenericClass(typeof(Dictionary<, >));
				Type type3 = typeof(DerivedDictionaryFormatter<, , >).MakeGenericType(type, argumentsOfInheritedOpenGenericClass[0], argumentsOfInheritedOpenGenericClass[1]);
				return (IFormatter)Activator.CreateInstance(type3);
			}
			if (flag)
			{
				return (IFormatter)Activator.CreateInstance(typeof(SelfFormatterFormatter<>).MakeGenericType(type));
			}
			if (typeof(Delegate).IsAssignableFrom(type))
			{
				return (IFormatter)Activator.CreateInstance(typeof(DelegateFormatter<>).MakeGenericType(type));
			}
			if (typeof(Type).IsAssignableFrom(type))
			{
				return new TypeFormatter();
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() == 1)
				{
					if (FormatterUtilities.IsPrimitiveArrayType(type.GetElementType()))
					{
						return (IFormatter)Activator.CreateInstance(typeof(PrimitiveArrayFormatter<>).MakeGenericType(type.GetElementType()));
					}
					return (IFormatter)Activator.CreateInstance(typeof(ArrayFormatter<>).MakeGenericType(type.GetElementType()));
				}
				return (IFormatter)Activator.CreateInstance(typeof(MultiDimensionalArrayFormatter<, >).MakeGenericType(type, type.GetElementType()));
			}
			if (type.ImplementsOrInherits(typeof(ISerializable)))
			{
				return (IFormatter)Activator.CreateInstance(typeof(SerializableFormatter<>).MakeGenericType(type));
			}
			Type elementType;
			if (GenericCollectionFormatter.CanFormat(type, out elementType))
			{
				return (IFormatter)Activator.CreateInstance(typeof(GenericCollectionFormatter<, >).MakeGenericType(type, elementType));
			}
			if (EmitUtilities.CanEmit)
			{
				return FormatterEmitter.GetEmittedFormatter(type, policy);
			}
			if (EmitUtilities.CanEmit)
			{
				Debug.LogWarning("Fallback to reflection for type " + type.Name + " when emit is possible on this platform.");
			}
			return (IFormatter)Activator.CreateInstance(typeof(ReflectionFormatter<>).MakeGenericType(type));
		}
	}
}
