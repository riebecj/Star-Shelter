using System;
using System.Collections.Generic;
using Sirenix.Utilities;

namespace Sirenix.Serialization
{
	[CustomGenericFormatter(typeof(IndexedDictionary<, >), 0)]
	public sealed class IndexedDictionaryFormatter<TKey, TValue> : BaseFormatter<IndexedDictionary<TKey, TValue>>
	{
		private static readonly Serializer<TKey> KeyReaderWriter;

		private static readonly Serializer<TValue> ValueReaderWriter;

		static IndexedDictionaryFormatter()
		{
			KeyReaderWriter = Serializer.Get<TKey>();
			ValueReaderWriter = Serializer.Get<TValue>();
			new DictionaryFormatter<int, string>();
		}

		protected override IndexedDictionary<TKey, TValue> GetUninitializedObject()
		{
			return null;
		}

		protected override void DeserializeImplementation(ref IndexedDictionary<TKey, TValue> value, IDataReader reader)
		{
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			if (entryType == EntryType.StartOfArray)
			{
				try
				{
					long length;
					reader.EnterArray(out length);
					value = new IndexedDictionary<TKey, TValue>((int)length);
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
							TKey key = KeyReaderWriter.ReadValue(reader);
							TValue value2 = ValueReaderWriter.ReadValue(reader);
							value.Add(key, value2);
						}
						catch (SerializationAbortException ex)
						{
							flag = false;
							throw ex;
						}
						catch (Exception exception)
						{
							reader.Context.Config.DebugContext.LogException(exception);
						}
						finally
						{
							if (flag)
							{
								reader.ExitNode();
							}
						}
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

		protected override void SerializeImplementation(ref IndexedDictionary<TKey, TValue> value, IDataWriter writer)
		{
			try
			{
				writer.BeginArrayNode(value.Count);
				bool flag = true;
				for (int i = 0; i < value.Count; i++)
				{
					KeyValuePair<TKey, TValue> keyValuePair = value.Get(i);
					try
					{
						writer.BeginStructNode(null, null);
						KeyReaderWriter.WriteValue(keyValuePair.Key, writer);
						ValueReaderWriter.WriteValue(keyValuePair.Value, writer);
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
