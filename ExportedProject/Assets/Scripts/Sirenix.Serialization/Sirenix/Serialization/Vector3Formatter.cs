using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class Vector3Formatter : MinimalBaseFormatter<Vector3>
	{
		private static readonly Serializer<float> Serializer = Sirenix.Serialization.Serializer.Get<float>();

		protected override void Read(ref Vector3 value, IDataReader reader)
		{
			value.x = Serializer.ReadValue(reader);
			value.y = Serializer.ReadValue(reader);
			value.z = Serializer.ReadValue(reader);
		}

		protected override void Write(ref Vector3 value, IDataWriter writer)
		{
			Serializer.WriteValue(value.x, writer);
			Serializer.WriteValue(value.y, writer);
			Serializer.WriteValue(value.z, writer);
		}
	}
}
