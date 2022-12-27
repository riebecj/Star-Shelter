using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.Serialization
{
	public sealed class DelegateFormatter<T> : BaseFormatter<T> where T : class
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Func<Type, string> _003C_003E9__6_0;

			public static Func<Type, string> _003C_003E9__6_1;

			internal string _003CDeserializeImplementation_003Eb__6_0(Type p)
			{
				return p.GetNiceFullName();
			}

			internal string _003CDeserializeImplementation_003Eb__6_1(Type p)
			{
				return p.GetNiceFullName();
			}
		}

		private static readonly Serializer<object> ObjectSerializer;

		private static readonly Serializer<string> StringSerializer;

		private static readonly Serializer<Type> TypeSerializer;

		private static readonly Serializer<Type[]> TypeArraySerializer;

		private static readonly Serializer<Delegate[]> DelegateArraySerializer;

		static DelegateFormatter()
		{
			ObjectSerializer = Serializer.Get<object>();
			StringSerializer = Serializer.Get<string>();
			TypeSerializer = Serializer.Get<Type>();
			TypeArraySerializer = Serializer.Get<Type[]>();
			DelegateArraySerializer = Serializer.Get<Delegate[]>();
			if (!typeof(Delegate).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(string.Concat("The type ", typeof(T), " is not a delegate."));
			}
		}

		protected override void DeserializeImplementation(ref T value, IDataReader reader)
		{
			Type type = typeof(T);
			Type type2 = null;
			object obj = null;
			string text = null;
			Type[] array = null;
			Type[] array2 = null;
			Delegate[] array3 = null;
			EntryType entryType;
			string name;
			while ((entryType = reader.PeekEntry(out name)) != EntryType.EndOfNode && entryType != EntryType.EndOfArray && entryType != EntryType.EndOfStream)
			{
				switch (name)
				{
				case "invocationList":
					array3 = DelegateArraySerializer.ReadValue(reader);
					break;
				case "target":
					obj = ObjectSerializer.ReadValue(reader);
					break;
				case "declaringType":
				{
					Type type4 = TypeSerializer.ReadValue(reader);
					if (type4 != null)
					{
						type2 = type4;
					}
					break;
				}
				case "methodName":
					text = StringSerializer.ReadValue(reader);
					break;
				case "delegateType":
				{
					Type type3 = TypeSerializer.ReadValue(reader);
					if (type3 != null)
					{
						type = type3;
					}
					break;
				}
				case "signature":
					array = TypeArraySerializer.ReadValue(reader);
					break;
				case "genericArguments":
					array2 = TypeArraySerializer.ReadValue(reader);
					break;
				default:
					reader.SkipEntry();
					break;
				}
			}
			if (array3 != null)
			{
				Delegate @delegate = null;
				try
				{
					@delegate = Delegate.Combine(array3);
				}
				catch (Exception ex)
				{
					reader.Context.Config.DebugContext.LogError("Recombining delegate invocation list upon deserialization failed with an exception of type " + ex.GetType().GetNiceFullName() + " with the message: " + ex.Message);
				}
				if ((object)@delegate != null)
				{
					try
					{
						value = (T)(object)@delegate;
						return;
					}
					catch (InvalidCastException)
					{
						reader.Context.Config.DebugContext.LogWarning("Could not cast recombined delegate of type " + @delegate.GetType().GetNiceFullName() + " to expected delegate type " + typeof(T).GetNiceFullName() + ".");
						return;
					}
				}
				return;
			}
			if (type2 == null)
			{
				reader.Context.Config.DebugContext.LogWarning("Missing declaring type of delegate on deserialize.");
				return;
			}
			if (text == null)
			{
				reader.Context.Config.DebugContext.LogError("Missing method name of delegate on deserialize.");
				return;
			}
			bool flag = false;
			bool flag2 = false;
			if (array != null)
			{
				flag = true;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == null)
					{
						flag = false;
						break;
					}
				}
			}
			MethodInfo methodInfo;
			if (flag)
			{
				try
				{
					methodInfo = type2.GetMethod(text, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, array, null);
				}
				catch (AmbiguousMatchException)
				{
					methodInfo = null;
					flag2 = true;
				}
			}
			else
			{
				try
				{
					methodInfo = type2.GetMethod(text, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				}
				catch (AmbiguousMatchException)
				{
					methodInfo = null;
					flag2 = true;
				}
			}
			if (methodInfo == null)
			{
				if (flag)
				{
					reader.Context.Config.DebugContext.LogWarning("Could not find method with signature " + name + "(" + string.Join(", ", array.Select(_003C_003Ec._003C_003E9__6_0 ?? (_003C_003Ec._003C_003E9__6_0 = _003C_003Ec._003C_003E9._003CDeserializeImplementation_003Eb__6_0)).ToArray()) + ") on type '" + type2.FullName + (flag2 ? "; resolution was ambiguous between multiple methods" : string.Empty) + ".");
				}
				else
				{
					reader.Context.Config.DebugContext.LogWarning("Could not find method with name " + name + " on type '" + type2.GetNiceFullName() + (flag2 ? "; resolution was ambiguous between multiple methods" : string.Empty) + ".");
				}
				return;
			}
			if (methodInfo.IsGenericMethodDefinition)
			{
				if (array2 == null)
				{
					reader.Context.Config.DebugContext.LogWarning("Method '" + type2.GetNiceFullName() + "." + methodInfo.GetNiceName() + "' of delegate to deserialize is a generic method definition, but no generic arguments were in the serialization data.");
					return;
				}
				int num = methodInfo.GetGenericArguments().Length;
				if (array2.Length != num)
				{
					reader.Context.Config.DebugContext.LogWarning("Method '" + type2.GetNiceFullName() + "." + methodInfo.GetNiceName() + "' of delegate to deserialize is a generic method definition, but there is the wrong number of generic arguments in the serialization data.");
					return;
				}
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j] == null)
					{
						reader.Context.Config.DebugContext.LogWarning("Method '" + type2.GetNiceFullName() + "." + methodInfo.GetNiceName() + "' of delegate to deserialize is a generic method definition, but one of the serialized generic argument types failed to bind on deserialization.");
						return;
					}
				}
				try
				{
					methodInfo = methodInfo.MakeGenericMethod(array2);
				}
				catch (Exception ex5)
				{
					reader.Context.Config.DebugContext.LogWarning("Method '" + type2.GetNiceFullName() + "." + methodInfo.GetNiceName() + "' of delegate to deserialize is a generic method definition, but failed to create generic method from definition, using generic arguments '" + string.Join(", ", array2.Select(_003C_003Ec._003C_003E9__6_1 ?? (_003C_003Ec._003C_003E9__6_1 = _003C_003Ec._003C_003E9._003CDeserializeImplementation_003Eb__6_1)).ToArray()) + "'. Method creation failed with an exception of type " + ex5.GetType().GetNiceFullName() + ", with the message: " + ex5.Message);
					return;
				}
			}
			if (methodInfo.IsStatic)
			{
				value = (T)(object)Delegate.CreateDelegate(type, methodInfo, false);
			}
			else
			{
				Type declaringType = methodInfo.DeclaringType;
				if (typeof(UnityEngine.Object).IsAssignableFrom(declaringType))
				{
					if (obj as UnityEngine.Object == null)
					{
						reader.Context.Config.DebugContext.LogWarning("Method '" + type2.GetNiceFullName() + "." + methodInfo.GetNiceName() + "' of delegate to deserialize is an instance method, but Unity object target of type '" + declaringType.GetNiceFullName() + "' was null on deserialization. Did something destroy it, or did you apply a delegate value targeting a scene-based UnityEngine.Object instance to a prefab?");
						return;
					}
				}
				else if (obj == null)
				{
					reader.Context.Config.DebugContext.LogWarning("Method '" + type2.GetNiceFullName() + "." + methodInfo.GetNiceName() + "' of delegate to deserialize is an instance method, but no valid instance target of type '" + declaringType.GetNiceFullName() + "' was in the serialization data. Has something been renamed since serialization?");
					return;
				}
				value = (T)(object)Delegate.CreateDelegate(type, obj, methodInfo, false);
			}
			if (value == null)
			{
				reader.Context.Config.DebugContext.LogWarning("Failed to create delegate of type " + type.GetNiceFullName() + " from method '" + type2.GetNiceFullName() + "." + methodInfo.GetNiceName() + "'.");
			}
			else
			{
				RegisterReferenceID(value, reader);
				InvokeOnDeserializingCallbacks(value, reader.Context);
			}
		}

		protected override void SerializeImplementation(ref T value, IDataWriter writer)
		{
			Delegate @delegate = (Delegate)(object)value;
			Delegate[] invocationList = @delegate.GetInvocationList();
			if (invocationList.Length > 1)
			{
				DelegateArraySerializer.WriteValue("invocationList", invocationList, writer);
				return;
			}
			MethodInfo method = @delegate.Method;
			if (method is DynamicMethod)
			{
				writer.Context.Config.DebugContext.LogError(string.Concat("Cannot serialize delegate made from dynamically emitted method ", method, "."));
				return;
			}
			if (method.IsGenericMethodDefinition)
			{
				writer.Context.Config.DebugContext.LogError(string.Concat("Cannot serialize delegate made from the unresolved generic method definition ", method, "; how did this even happen? It should not even be possible to have a delegate for a generic method definition that hasn't been turned into a generic method yet."));
				return;
			}
			if (@delegate.Target != null)
			{
				ObjectSerializer.WriteValue("target", @delegate.Target, writer);
			}
			TypeSerializer.WriteValue("declaringType", method.DeclaringType, writer);
			StringSerializer.WriteValue("methodName", method.Name, writer);
			TypeSerializer.WriteValue("delegateType", @delegate.GetType(), writer);
			ParameterInfo[] array = ((!method.IsGenericMethod) ? method.GetParameters() : method.GetGenericMethodDefinition().GetParameters());
			Type[] array2 = new Type[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array[i].ParameterType;
			}
			TypeArraySerializer.WriteValue("signature", array2, writer);
			if (method.IsGenericMethod)
			{
				Type[] genericArguments = method.GetGenericArguments();
				TypeArraySerializer.WriteValue("genericArguments", genericArguments, writer);
			}
		}

		protected override T GetUninitializedObject()
		{
			return null;
		}
	}
}
