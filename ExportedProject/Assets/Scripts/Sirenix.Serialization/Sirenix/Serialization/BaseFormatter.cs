using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.Serialization
{
	public abstract class BaseFormatter<T> : IFormatter<T>, IFormatter
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass5_0
		{
			public Action<T> action;

			internal void _003C_002Ecctor_003Eb__1(T value, StreamingContext context)
			{
				action(value);
			}
		}

		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			internal Action<T, StreamingContext> _003C_002Ecctor_003Eb__5_0(MethodInfo info)
			{
				ParameterInfo[] parameters = info.GetParameters();
				if (parameters.Length == 0)
				{
					return new _003C_003Ec__DisplayClass5_0
					{
						action = EmitUtilities.CreateInstanceMethodCaller<T>(info)
					}._003C_002Ecctor_003Eb__1;
				}
				if (parameters.Length == 1 && parameters[0].ParameterType == typeof(StreamingContext) && !parameters[0].ParameterType.IsByRef)
				{
					return EmitUtilities.CreateInstanceMethodCaller<T, StreamingContext>(info);
				}
				DefaultLoggers.DefaultLogger.LogWarning("The method " + info.GetNiceName() + " has an invalid signature and will be ignored by the serialization system.");
				return null;
			}

			internal bool _003C_002Ecctor_003Eb__5_2(MethodInfo n)
			{
				return n.IsDefined(typeof(OnSerializingAttribute), true);
			}

			internal bool _003C_002Ecctor_003Eb__5_3(Action<T, StreamingContext> n)
			{
				return n != null;
			}

			internal bool _003C_002Ecctor_003Eb__5_4(MethodInfo n)
			{
				return n.IsDefined(typeof(OnSerializedAttribute), true);
			}

			internal bool _003C_002Ecctor_003Eb__5_5(Action<T, StreamingContext> n)
			{
				return n != null;
			}

			internal bool _003C_002Ecctor_003Eb__5_6(MethodInfo n)
			{
				return n.IsDefined(typeof(OnDeserializingAttribute), true);
			}

			internal bool _003C_002Ecctor_003Eb__5_7(Action<T, StreamingContext> n)
			{
				return n != null;
			}

			internal bool _003C_002Ecctor_003Eb__5_8(MethodInfo n)
			{
				return n.IsDefined(typeof(OnDeserializedAttribute), true);
			}

			internal bool _003C_002Ecctor_003Eb__5_9(Action<T, StreamingContext> n)
			{
				return n != null;
			}
		}

		protected static readonly Action<T, StreamingContext>[] OnSerializingCallbacks;

		protected static readonly Action<T, StreamingContext>[] OnSerializedCallbacks;

		protected static readonly Action<T, StreamingContext>[] OnDeserializingCallbacks;

		protected static readonly Action<T, StreamingContext>[] OnDeserializedCallbacks;

		protected static readonly bool IsValueType;

		public Type SerializedType
		{
			get
			{
				return typeof(T);
			}
		}

		static BaseFormatter()
		{
			IsValueType = typeof(T).IsValueType;
			if (typeof(T).ImplementsOrInherits(typeof(UnityEngine.Object)))
			{
				DefaultLoggers.DefaultLogger.LogWarning("A formatter has been created for the UnityEngine.Object type " + typeof(T).Name + " - this is *strongly* discouraged. Unity should be allowed to handle serialization and deserialization of its own weird objects. Remember to serialize with a UnityReferenceResolver as the external index reference resolver in the serialization context.");
			}
			MethodInfo[] methods = typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Func<MethodInfo, Action<T, StreamingContext>> selector = _003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__5_0;
			OnSerializingCallbacks = methods.Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__5_2).Select(selector).Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__5_3)
				.ToArray();
			OnSerializedCallbacks = methods.Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__5_4).Select(selector).Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__5_5)
				.ToArray();
			OnDeserializingCallbacks = methods.Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__5_6).Select(selector).Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__5_7)
				.ToArray();
			OnDeserializedCallbacks = methods.Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__5_8).Select(selector).Where(_003C_003Ec._003C_003E9._003C_002Ecctor_003Eb__5_9)
				.ToArray();
		}

		void IFormatter.Serialize(object value, IDataWriter writer)
		{
			Serialize((T)value, writer);
		}

		object IFormatter.Deserialize(IDataReader reader)
		{
			return Deserialize(reader);
		}

		public T Deserialize(IDataReader reader)
		{
			DeserializationContext context = reader.Context;
			T value = GetUninitializedObject();
			if (IsValueType)
			{
				InvokeOnDeserializingCallbacks(value, context);
			}
			else if (value != null)
			{
				RegisterReferenceID(value, reader);
				InvokeOnDeserializingCallbacks(value, context);
				if (typeof(T).ImplementsOrInherits(typeof(IObjectReference)))
				{
					try
					{
						value = (T)(value as IObjectReference).GetRealObject(context.StreamingContext);
						RegisterReferenceID(value, reader);
					}
					catch (Exception exception)
					{
						context.Config.DebugContext.LogException(exception);
					}
				}
			}
			try
			{
				DeserializeImplementation(ref value, reader);
			}
			catch (Exception exception2)
			{
				context.Config.DebugContext.LogException(exception2);
			}
			if (IsValueType || value != null)
			{
				for (int i = 0; i < OnDeserializedCallbacks.Length; i++)
				{
					try
					{
						OnDeserializedCallbacks[i](value, context.StreamingContext);
					}
					catch (Exception exception3)
					{
						context.Config.DebugContext.LogException(exception3);
					}
				}
				if (typeof(T).ImplementsOrInherits(typeof(ISerializationCallbackReceiver)))
				{
					try
					{
						(value as ISerializationCallbackReceiver).OnAfterDeserialize();
						return value;
					}
					catch (Exception exception4)
					{
						context.Config.DebugContext.LogException(exception4);
						return value;
					}
				}
			}
			return value;
		}

		public void Serialize(T value, IDataWriter writer)
		{
			SerializationContext context = writer.Context;
			for (int i = 0; i < OnSerializingCallbacks.Length; i++)
			{
				try
				{
					OnSerializingCallbacks[i](value, context.StreamingContext);
				}
				catch (Exception exception)
				{
					context.Config.DebugContext.LogException(exception);
				}
			}
			if (typeof(T).ImplementsOrInherits(typeof(ISerializationCallbackReceiver)))
			{
				try
				{
					(value as ISerializationCallbackReceiver).OnBeforeSerialize();
				}
				catch (Exception exception2)
				{
					context.Config.DebugContext.LogException(exception2);
				}
			}
			try
			{
				SerializeImplementation(ref value, writer);
			}
			catch (Exception exception3)
			{
				context.Config.DebugContext.LogException(exception3);
			}
			for (int j = 0; j < OnSerializedCallbacks.Length; j++)
			{
				try
				{
					OnSerializedCallbacks[j](value, context.StreamingContext);
				}
				catch (Exception exception4)
				{
					context.Config.DebugContext.LogException(exception4);
				}
			}
		}

		protected virtual T GetUninitializedObject()
		{
			if (typeof(T).IsValueType)
			{
				return default(T);
			}
			return (T)FormatterServices.GetUninitializedObject(typeof(T));
		}

		protected void RegisterReferenceID(T value, IDataReader reader)
		{
			if (!typeof(T).IsValueType)
			{
				int currentNodeId = reader.CurrentNodeId;
				if (currentNodeId < 0)
				{
					reader.Context.Config.DebugContext.LogError("Reference type node is missing id upon deserialization; some references will be broken.");
				}
				else
				{
					reader.Context.RegisterInternalReference(currentNodeId, value);
				}
			}
		}

		protected void InvokeOnDeserializingCallbacks(T value, DeserializationContext context)
		{
			for (int i = 0; i < OnDeserializingCallbacks.Length; i++)
			{
				try
				{
					OnDeserializingCallbacks[i](value, context.StreamingContext);
				}
				catch (Exception exception)
				{
					context.Config.DebugContext.LogException(exception);
				}
			}
			IDeserializationCallback deserializationCallback = value as IDeserializationCallback;
			if (deserializationCallback != null)
			{
				deserializationCallback.OnDeserialization(this);
			}
		}

		protected abstract void DeserializeImplementation(ref T value, IDataReader reader);

		protected abstract void SerializeImplementation(ref T value, IDataWriter writer);
	}
}
