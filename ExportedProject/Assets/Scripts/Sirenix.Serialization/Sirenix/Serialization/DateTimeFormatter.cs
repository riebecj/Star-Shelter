using System;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public sealed class DateTimeFormatter : MinimalBaseFormatter<DateTime>
	{
		protected override void Read(ref DateTime value, IDataReader reader)
		{
			string name;
			if (reader.PeekEntry(out name) == EntryType.Integer)
			{
				long value2;
				reader.ReadInt64(out value2);
				value = DateTime.FromBinary(value2);
			}
		}

		protected override void Write(ref DateTime value, IDataWriter writer)
		{
			writer.WriteInt64(null, value.ToBinary());
		}
	}
}
