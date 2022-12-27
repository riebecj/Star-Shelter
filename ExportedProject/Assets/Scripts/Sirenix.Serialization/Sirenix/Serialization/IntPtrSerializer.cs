using System;

namespace Sirenix.Serialization
{
	public sealed class IntPtrSerializer : Serializer<IntPtr>
	{
		public override IntPtr ReadValue(IDataReader reader)
		{
			string name;
			EntryType entryType = reader.PeekEntry(out name);
			if (entryType == EntryType.Integer)
			{
				long value;
				if (!reader.ReadInt64(out value))
				{
					reader.Context.Config.DebugContext.LogWarning("Failed to read entry '" + name + "' of type " + entryType);
				}
				return new IntPtr(value);
			}
			reader.Context.Config.DebugContext.LogWarning("Expected entry of type " + EntryType.Integer.ToString() + ", but got entry '" + name + "' of type " + entryType);
			reader.SkipEntry();
			return (IntPtr)0;
		}

		public override void WriteValue(string name, IntPtr value, IDataWriter writer)
		{
			writer.WriteInt64(name, (long)value);
		}
	}
}
