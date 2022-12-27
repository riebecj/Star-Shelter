using System;
using System.Collections;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class ArrayListFormatter : BaseFormatter<ArrayList>
	{
		private static readonly Serializer<object> Serializer = Sirenix.Serialization.Serializer.Get<object>();

		protected override ArrayList GetUninitializedObject()
		{
			return null;
		}

		protected override void DeserializeImplementation(ref ArrayList value, IDataReader reader)
		{
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			if (entryType == EntryType.StartOfArray)
			{
				try
				{
					long length;
					reader.EnterArray(out length);
					value = new ArrayList((int)length);
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

		protected override void SerializeImplementation(ref ArrayList value, IDataWriter writer)
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
