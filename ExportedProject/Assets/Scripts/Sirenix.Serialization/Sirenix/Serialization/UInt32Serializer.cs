namespace Sirenix.Serialization
{
	public sealed class UInt32Serializer : Serializer<uint>
	{
		public override uint ReadValue(IDataReader reader)
		{
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			if (entryType == EntryType.Integer)
			{
				uint value;
				if (!reader.ReadUInt32(out value))
				{
					reader.Context.Config.DebugContext.LogWarning("Failed to read entry '" + name + "' of type " + entryType);
				}
				return value;
			}
			reader.Context.Config.DebugContext.LogWarning("Expected entry of type " + EntryType.Integer.ToString() + ", but got entry '" + name + "' of type " + entryType);
			reader.SkipEntry();
			return 0u;
		}

		public override void WriteValue(string name, uint value, IDataWriter writer)
		{
			writer.WriteUInt32(name, value);
		}
	}
}
