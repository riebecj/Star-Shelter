using System;
using System.Collections.Generic;

namespace Sirenix.Serialization
{
	[CustomGenericFormatter(typeof(Dictionary<, >), 0)]
	public sealed class DictionaryFormatter<TKey, TValue> : BaseFormatter<Dictionary<TKey, TValue>>
	{
		private static readonly bool KeyIsValueType;

		private static readonly Serializer<IEqualityComparer<TKey>> EqualityComparerSerializer;

		private static readonly Serializer<TKey> KeyReaderWriter;

		private static readonly Serializer<TValue> ValueReaderWriter;

		static DictionaryFormatter()
		{
			KeyIsValueType = typeof(TKey).IsValueType;
			EqualityComparerSerializer = Serializer.Get<IEqualityComparer<TKey>>();
			KeyReaderWriter = Serializer.Get<TKey>();
			ValueReaderWriter = Serializer.Get<TValue>();
			new DictionaryFormatter<int, string>();
		}

		protected override Dictionary<TKey, TValue> GetUninitializedObject()
		{
			return null;
		}

		protected override void DeserializeImplementation(ref Dictionary<TKey, TValue> value, IDataReader reader)
		{
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			IEqualityComparer<TKey> equalityComparer = null;
			if (entryType == EntryType.StartOfNode)
			{
				equalityComparer = EqualityComparerSerializer.ReadValue(reader);
				entryType = reader.PeekEntry(out name);
			}
			if (entryType == EntryType.StartOfArray)
			{
				try
				{
					long length;
					reader.EnterArray(out length);
					value = ((equalityComparer == null) ? new Dictionary<TKey, TValue>((int)length) : new Dictionary<TKey, TValue>((int)length, equalityComparer));
					RegisterReferenceID(value, reader);
					for (int i = 0; i < length; i++)
					{
						if (reader.PeekEntry(out name) == EntryType.EndOfArray)
						{
							reader.Context.Config.DebugContext.LogError("Reached end of array after " + i + " elements, when " + length + " elements were expected.");
							break;
						}
						bool flag = true;
						try
						{
							Type type;
							reader.EnterNode(out type);
							TKey val = KeyReaderWriter.ReadValue(reader);
							TValue value2 = ValueReaderWriter.ReadValue(reader);
							if (KeyIsValueType || val != null)
							{
								value[val] = value2;
								goto IL_015f;
							}
							reader.Context.Config.DebugContext.LogWarning("Dictionary key of type '" + typeof(TKey).FullName + "' was null upon deserialization. A key has gone missing.");
						}
						catch (SerializationAbortException ex)
						{
							flag = false;
							throw ex;
						}
						catch (Exception exception)
						{
							reader.Context.Config.DebugContext.LogException(exception);
							goto IL_015f;
						}
						finally
						{
							if (flag)
							{
								reader.ExitNode();
							}
						}
						continue;
						IL_015f:
						if (!reader.IsInArrayNode)
						{
							reader.Context.Config.DebugContext.LogError("Reading array went wrong at position " + reader.Stream.Position + ".");
							break;
						}
					}
					return;
				}
				finally
				{
					reader.ExitArray();
				}
			}
			reader.SkipEntry();
		}

		protected override void SerializeImplementation(ref Dictionary<TKey, TValue> value, IDataWriter writer)
		{
			try
			{
				if (value.Comparer != null)
				{
					EqualityComparerSerializer.WriteValue(value.Comparer, writer);
				}
				writer.BeginArrayNode(value.Count);
				foreach (KeyValuePair<TKey, TValue> item in value)
				{
					bool flag = true;
					try
					{
						writer.BeginStructNode(null, null);
						KeyReaderWriter.WriteValue(item.Key, writer);
						ValueReaderWriter.WriteValue(item.Value, writer);
					}
					catch (SerializationAbortException ex)
					{
						flag = false;
						throw ex;
					}
					catch (Exception exception)
					{
						writer.Context.Config.DebugContext.LogException(exception);
					}
					finally
					{
						if (flag)
						{
							writer.EndNode(null);
						}
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
