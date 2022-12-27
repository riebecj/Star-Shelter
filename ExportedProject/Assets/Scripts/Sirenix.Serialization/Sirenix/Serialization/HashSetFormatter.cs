using System;
using System.Collections.Generic;

namespace Sirenix.Serialization
{
	[CustomGenericFormatter(typeof(HashSet<>), 0)]
	public class HashSetFormatter<T> : BaseFormatter<HashSet<T>>
	{
		private static readonly Serializer<T> Serializer;

		static HashSetFormatter()
		{
			Serializer = Sirenix.Serialization.Serializer.Get<T>();
			new HashSetFormatter<int>();
		}

		protected override HashSet<T> GetUninitializedObject()
		{
			return null;
		}

		protected override void DeserializeImplementation(ref HashSet<T> value, IDataReader reader)
		{
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			if (entryType == EntryType.StartOfArray)
			{
				try
				{
					long length;
					reader.EnterArray(out length);
					value = new HashSet<T>();
					RegisterReferenceID(value, reader);
					for (int i = 0; i < length; i++)
					{
						if (reader.PeekEntry(out name) == EntryType.EndOfArray)
						{
							reader.Context.Config.DebugContext.LogError("Reached end of array after " + i + " elements, when " + length + " elements were expected.");
							break;
						}
						value.Add(Serializer.ReadValue(reader));
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

		protected override void SerializeImplementation(ref HashSet<T> value, IDataWriter writer)
		{
			try
			{
				writer.BeginArrayNode(value.Count);
				foreach (T item in value)
				{
					try
					{
						Serializer.WriteValue(item, writer);
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
