namespace Sirenix.Serialization
{
	public sealed class ByteSerializer : Serializer<byte>
	{
		public override byte ReadValue(IDataReader reader)
		{
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			if (entryType == EntryType.Integer)
			{
				byte value;
				if (!reader.ReadByte(out value))
				{
					reader.Context.Config.DebugContext.LogWarning("Failed to read entry '" + name + "' of type " + entryType);
				}
				return value;
			}
			reader.Context.Config.DebugContext.LogWarning("Expected entry of type " + EntryType.Integer.ToString() + ", but got entry '" + name + "' of type " + entryType);
			reader.SkipEntry();
			return 0;
		}

		public override void WriteValue(string name, byte value, IDataWriter writer)
		{
			writer.WriteByte(name, value);
		}
	}
}
