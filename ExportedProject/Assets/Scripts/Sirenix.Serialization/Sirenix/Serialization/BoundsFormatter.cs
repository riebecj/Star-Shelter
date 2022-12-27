using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class BoundsFormatter : MinimalBaseFormatter<Bounds>
	{
		private static readonly Serializer<Vector3> Serializer = Sirenix.Serialization.Serializer.Get<Vector3>();

		protected override void Read(ref Bounds value, IDataReader reader)
		{
			value.center = Serializer.ReadValue(reader);
			value.size = Serializer.ReadValue(reader);
		}

		protected override void Write(ref Bounds value, IDataWriter writer)
		{
			Serializer.WriteValue(value.center, writer);
			Serializer.WriteValue(value.size, writer);
		}
	}
}
