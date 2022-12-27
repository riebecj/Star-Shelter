namespace Sirenix.Serialization
{
	public sealed class UInt64Serializer : Serializer<ulong>
	{
		public override ulong ReadValue(IDataReader reader)
		{
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			if (entryType == EntryType.Integer)
			{
				ulong value;
				if (!reader.ReadUInt64(out value))
				{
					reader.Context.Config.DebugContext.LogWarning("Failed to read entry '" + name + "' of type " + entryType);
				}
				return value;
			}
			reader.Context.Config.DebugContext.LogWarning("Expected entry of type " + EntryType.Integer.ToString() + ", but got entry '" + name + "' of type " + entryType);
			reader.SkipEntry();
			return 0uL;
		}

		public override void WriteValue(string name, ulong value, IDataWriter writer)
		{
			writer.WriteUInt64(name, value);
		}
	}
}
