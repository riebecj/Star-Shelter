using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class Vector2Formatter : MinimalBaseFormatter<Vector2>
	{
		private static readonly Serializer<float> Serializer = Sirenix.Serialization.Serializer.Get<float>();

		protected override void Read(ref Vector2 value, IDataReader reader)
		{
			value.x = Serializer.ReadValue(reader);
			value.y = Serializer.ReadValue(reader);
		}

		protected override void Write(ref Vector2 value, IDataWriter writer)
		{
			Serializer.WriteValue(value.x, writer);
			Serializer.WriteValue(value.y, writer);
		}
	}
}
