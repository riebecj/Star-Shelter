using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class KeyframeFormatter : MinimalBaseFormatter<Keyframe>
	{
		private static readonly Serializer<float> FloatSerializer = Serializer.Get<float>();

		private static readonly Serializer<int> IntSerializer = Serializer.Get<int>();

		protected override void Read(ref Keyframe value, IDataReader reader)
		{
			value.inTangent = FloatSerializer.ReadValue(reader);
			value.outTangent = FloatSerializer.ReadValue(reader);
			value.time = FloatSerializer.ReadValue(reader);
			value.value = FloatSerializer.ReadValue(reader);
			value.tangentMode = IntSerializer.ReadValue(reader);
		}

		protected override void Write(ref Keyframe value, IDataWriter writer)
		{
			FloatSerializer.WriteValue(value.inTangent, writer);
			FloatSerializer.WriteValue(value.outTangent, writer);
			FloatSerializer.WriteValue(value.time, writer);
			FloatSerializer.WriteValue(value.value, writer);
			IntSerializer.WriteValue(value.tangentMode, writer);
		}
	}
}
