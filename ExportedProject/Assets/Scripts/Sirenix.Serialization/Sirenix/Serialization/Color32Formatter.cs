using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class Color32Formatter : MinimalBaseFormatter<Color32>
	{
		private static readonly Serializer<byte> Serializer = Sirenix.Serialization.Serializer.Get<byte>();

		protected override void Read(ref Color32 value, IDataReader reader)
		{
			value.r = Serializer.ReadValue(reader);
			value.g = Serializer.ReadValue(reader);
			value.b = Serializer.ReadValue(reader);
			value.a = Serializer.ReadValue(reader);
		}

		protected override void Write(ref Color32 value, IDataWriter writer)
		{
			Serializer.WriteValue(value.r, writer);
			Serializer.WriteValue(value.g, writer);
			Serializer.WriteValue(value.b, writer);
			Serializer.WriteValue(value.a, writer);
		}
	}
}
