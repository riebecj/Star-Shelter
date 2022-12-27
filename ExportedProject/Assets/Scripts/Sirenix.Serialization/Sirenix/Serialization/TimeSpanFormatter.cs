using System;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public sealed class TimeSpanFormatter : MinimalBaseFormatter<TimeSpan>
	{
		protected override void Read(ref TimeSpan value, IDataReader reader)
		{
			string name;
			if (reader.PeekEntry(out name) == EntryType.Integer)
			{
				long value2;
				reader.ReadInt64(out value2);
				value = new TimeSpan(value2);
			}
		}

		protected override void Write(ref TimeSpan value, IDataWriter writer)
		{
			writer.WriteInt64(null, value.Ticks);
		}
	}
}
