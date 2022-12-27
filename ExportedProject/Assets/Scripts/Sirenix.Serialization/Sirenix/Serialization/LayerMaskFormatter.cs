using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class LayerMaskFormatter : MinimalBaseFormatter<LayerMask>
	{
		private static readonly Serializer<int> Serializer = Sirenix.Serialization.Serializer.Get<int>();

		protected override void Read(ref LayerMask value, IDataReader reader)
		{
			value.value = Serializer.ReadValue(reader);
		}

		protected override void Write(ref LayerMask value, IDataWriter writer)
		{
			Serializer.WriteValue(value.value, writer);
		}
	}
}
