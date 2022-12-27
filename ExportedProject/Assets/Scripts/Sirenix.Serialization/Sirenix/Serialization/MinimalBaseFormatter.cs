using System;
using System.Runtime.Serialization;

namespace Sirenix.Serialization
{
	public abstract class MinimalBaseFormatter<T> : IFormatter<T>, IFormatter
	{
		public Type SerializedType
		{
			get
			{
				return typeof(T);
			}
		}

		public T Deserialize(IDataReader reader)
		{
			T value = GetUninitializedObject();
			if (!typeof(T).IsValueType && value != null)
			{
				RegisterReferenceID(value, reader);
			}
			Read(ref value, reader);
			return value;
		}

		public void Serialize(T value, IDataWriter writer)
		{
			Write(ref value, writer);
		}

		void IFormatter.Serialize(object value, IDataWriter writer)
		{
			if (value is T)
			{
				Serialize((T)value, writer);
			}
		}

		object IFormatter.Deserialize(IDataReader reader)
		{
			return Deserialize(reader);
		}

		protected virtual T GetUninitializedObject()
		{
			if (typeof(T).IsValueType)
			{
				return default(T);
			}
			return (T)FormatterServices.GetUninitializedObject(typeof(T));
		}

		protected abstract void Read(ref T value, IDataReader reader);

		protected abstract void Write(ref T value, IDataWriter writer);

		protected void RegisterReferenceID(T value, IDataReader reader)
		{
			if (!typeof(T).IsValueType)
			{
				int currentNodeId = reader.CurrentNodeId;
				if (currentNodeId < 0)
				{
					reader.Context.Config.DebugContext.LogError("Reference type node is missing id upon deserialization. Some references will be broken.");
				}
				else
				{
					reader.Context.RegisterInternalReference(currentNodeId, value);
				}
			}
		}
	}
}
