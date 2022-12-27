using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class RectFormatter : MinimalBaseFormatter<Rect>
	{
		private static readonly Serializer<float> Serializer = Sirenix.Serialization.Serializer.Get<float>();

		protected override void Read(ref Rect value, IDataReader reader)
		{
			value.x = Serializer.ReadValue(reader);
			value.y = Serializer.ReadValue(reader);
			value.width = Serializer.ReadValue(reader);
			value.height = Serializer.ReadValue(reader);
		}

		protected override void Write(ref Rect value, IDataWriter writer)
		{
			Serializer.WriteValue(value.x, writer);
			Serializer.WriteValue(value.y, writer);
			Serializer.WriteValue(value.width, writer);
			Serializer.WriteValue(value.height, writer);
		}
	}
}
