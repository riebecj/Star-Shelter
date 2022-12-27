using System;
using System.Collections.Generic;

namespace Sirenix.Serialization
{
	[CustomGenericFormatter(typeof(List<>), 0)]
	public class ListFormatter<T> : BaseFormatter<List<T>>
	{
		private static readonly Serializer<T> Serializer;

		static ListFormatter()
		{
			Serializer = Sirenix.Serialization.Serializer.Get<T>();
			new ListFormatter<int>();
		}

		protected override List<T> GetUninitializedObject()
		{
			return null;
		}

		protected override void DeserializeImplementation(ref List<T> value, IDataReader reader)
		{
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			if (entryType == EntryType.StartOfArray)
			{
				try
				{
					long length;
					reader.EnterArray(out length);
					value = new List<T>((int)length);
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

		protected override void SerializeImplementation(ref List<T> value, IDataWriter writer)
		{
			try
			{
				writer.BeginArrayNode(value.Count);
				for (int i = 0; i < value.Count; i++)
				{
					try
					{
						Serializer.WriteValue(value[i], writer);
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
