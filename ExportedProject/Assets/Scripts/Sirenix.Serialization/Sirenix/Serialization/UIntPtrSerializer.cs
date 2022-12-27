using System;

namespace Sirenix.Serialization
{
	public sealed class UIntPtrSerializer : Serializer<UIntPtr>
	{
		public override UIntPtr ReadValue(IDataReader reader)
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
				return new UIntPtr(value);
			}
			reader.Context.Config.DebugContext.LogWarning("Expected entry of type " + EntryType.Integer.ToString() + ", but got entry '" + name + "' of type " + entryType);
			reader.SkipEntry();
			return (UIntPtr)0u;
		}

		public override void WriteValue(string name, UIntPtr value, IDataWriter writer)
		{
			writer.WriteUInt64(name, (ulong)value);
		}
	}
}
