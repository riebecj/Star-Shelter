using System;
using System.Globalization;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public sealed class DateTimeOffsetFormatter : MinimalBaseFormatter<DateTimeOffset>
	{
		protected override void Read(ref DateTimeOffset value, IDataReader reader)
		{
			string name;
			if (reader.PeekEntry(out name) == EntryType.String)
			{
				string value2;
				reader.ReadString(out value2);
				DateTimeOffset.TryParse(value2, out value);
			}
		}

		protected override void Write(ref DateTimeOffset value, IDataWriter writer)
		{
			writer.WriteString(null, value.ToString("O", CultureInfo.InvariantCulture));
		}
	}
}
