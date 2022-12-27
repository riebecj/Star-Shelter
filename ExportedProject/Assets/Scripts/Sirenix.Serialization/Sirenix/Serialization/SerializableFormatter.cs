using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Sirenix.Serialization
{
	public sealed class SerializableFormatter<T> : BaseFormatter<T> where T : ISerializable
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass2_0
		{
			public ConstructorInfo constructor;

			internal T _003C_002Ecctor_003Eb__0(SerializationInfo info, StreamingContext context)
			{
				return (T)constructor.Invoke(new object[2] { info, context });
			}
		}

		private static readonly Func<SerializationInfo, StreamingContext, T> ISerializableConstructor;

		private static readonly ReflectionFormatter<T> ReflectionFormatter;

		static SerializableFormatter()
		{
			_003C_003Ec__DisplayClass2_0 _003C_003Ec__DisplayClass2_ = new _003C_003Ec__DisplayClass2_0();
			_003C_003Ec__DisplayClass2_.constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
			{
				typeof(SerializationInfo),
				typeof(StreamingContext)
			}, null);
			if (_003C_003Ec__DisplayClass2_.constructor != null)
			{
				ISerializableConstructor = _003C_003Ec__DisplayClass2_._003C_002Ecctor_003Eb__0;
				return;
			}
			DefaultLoggers.DefaultLogger.LogWarning("Type " + typeof(T).Name + " implements the interface ISerializable but does not implement the required constructor with signature " + typeof(T).Name + "(SerializationInfo info, StreamingContext context). The interface declaration will be ignored, and the formatter fallbacks to reflection.");
			ReflectionFormatter = new ReflectionFormatter<T>();
		}

		protected override T GetUninitializedObject()
		{
			return default(T);
		}

		protected override void DeserializeImplementation(ref T value, IDataReader reader)
		{
			if (ISerializableConstructor != null)
			{
				SerializationInfo serializationInfo = ReadSerializationInfo(reader);
				if (serializationInfo == null)
				{
					return;
				}
				try
				{
					value = ISerializableConstructor(serializationInfo, reader.Context.StreamingContext);
					InvokeOnDeserializingCallbacks(value, reader.Context);
					if (!BaseFormatter<T>.IsValueType)
					{
						RegisterReferenceID(value, reader);
					}
					return;
				}
				catch (Exception exception)
				{
					reader.Context.Config.DebugContext.LogException(exception);
					return;
				}
			}
			value = ReflectionFormatter.Deserialize(reader);
			InvokeOnDeserializingCallbacks(value, reader.Context);
			if (!BaseFormatter<T>.IsValueType)
			{
				RegisterReferenceID(value, reader);
			}
		}

		protected override void SerializeImplementation(ref T value, IDataWriter writer)
		{
			if (ISerializableConstructor != null)
			{
				ISerializable serializable = value;
				SerializationInfo info = new SerializationInfo(value.GetType(), writer.Context.FormatterConverter);
				try
				{
					serializable.GetObjectData(info, writer.Context.StreamingContext);
				}
				catch (Exception exception)
				{
					writer.Context.Config.DebugContext.LogException(exception);
				}
				WriteSerializationInfo(info, writer);
			}
			else
			{
				ReflectionFormatter.Serialize(value, writer);
			}
		}

		private SerializationInfo ReadSerializationInfo(IDataReader reader)
		{
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			if (entryType == EntryType.StartOfArray)
			{
				try
				{
					long length;
					reader.EnterArray(out length);
					SerializationInfo serializationInfo = new SerializationInfo(typeof(T), reader.Context.FormatterConverter);
					for (int i = 0; i < length; i++)
					{
						Type type = null;
						entryType = reader.PeekEntry(out name);
						if (entryType == EntryType.String && name == "type")
						{
							string value;
							reader.ReadString(out value);
							type = reader.Binder.BindToType(value, reader.Context.Config.DebugContext);
						}
						if (type == null)
						{
							reader.SkipEntry();
						}
						else
						{
							entryType = reader.PeekEntry(out name);
							Serializer serializer = Serializer.Get(type);
							object value2 = serializer.ReadValueWeak(reader);
							serializationInfo.AddValue(name, value2);
						}
					}
					return serializationInfo;
				}
				finally
				{
					reader.ExitArray();
				}
			}
			return null;
		}

		private void WriteSerializationInfo(SerializationInfo info, IDataWriter writer)
		{
			try
			{
				writer.BeginArrayNode(info.MemberCount);
				SerializationInfoEnumerator enumerator = info.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SerializationEntry current = enumerator.Current;
					try
					{
						writer.WriteString("type", writer.Binder.BindToName(current.ObjectType, writer.Context.Config.DebugContext));
						Serializer serializer = Serializer.Get(current.ObjectType);
						serializer.WriteValueWeak(current.Name, current.Value, writer);
					}
					catch (Exception exception)
					{
						writer.Context.Config.DebugContext.LogException(exception);
					}
				}
			}
			finally
			{
				writer.EndArrayNode();
			}
		}
	}
}
