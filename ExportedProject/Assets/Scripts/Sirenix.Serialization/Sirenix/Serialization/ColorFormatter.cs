using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class ColorFormatter : MinimalBaseFormatter<Color>
	{
		private static readonly Serializer<float> Serializer = Sirenix.Serialization.Serializer.Get<float>();

		protected override void Read(ref Color value, IDataReader reader)
		{
			value.r = Serializer.ReadValue(reader);
			value.g = Serializer.ReadValue(reader);
			value.b = Serializer.ReadValue(reader);
			value.a = Serializer.ReadValue(reader);
		}

		protected override void Write(ref Color value, IDataWriter writer)
		{
			Serializer.WriteValue(value.r, writer);
			Serializer.WriteValue(value.g, writer);
			Serializer.WriteValue(value.b, writer);
			Serializer.WriteValue(value.a, writer);
		}
	}
}
