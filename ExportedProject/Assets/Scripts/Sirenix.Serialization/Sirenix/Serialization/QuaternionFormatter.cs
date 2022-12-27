using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class QuaternionFormatter : MinimalBaseFormatter<Quaternion>
	{
		private static readonly Serializer<float> Serializer = Sirenix.Serialization.Serializer.Get<float>();

		protected override void Read(ref Quaternion value, IDataReader reader)
		{
			value.x = Serializer.ReadValue(reader);
			value.y = Serializer.ReadValue(reader);
			value.z = Serializer.ReadValue(reader);
			value.w = Serializer.ReadValue(reader);
		}

		protected override void Write(ref Quaternion value, IDataWriter writer)
		{
			Serializer.WriteValue(value.x, writer);
			Serializer.WriteValue(value.y, writer);
			Serializer.WriteValue(value.z, writer);
			Serializer.WriteValue(value.w, writer);
		}
	}
}
