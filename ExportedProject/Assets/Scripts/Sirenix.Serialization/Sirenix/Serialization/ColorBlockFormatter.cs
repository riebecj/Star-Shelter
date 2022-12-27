using UnityEngine;
using UnityEngine.UI;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class ColorBlockFormatter : MinimalBaseFormatter<ColorBlock>
	{
		private static readonly Serializer<float> FloatSerializer = Serializer.Get<float>();

		private static readonly Serializer<Color> ColorSerializer = Serializer.Get<Color>();

		protected override void Read(ref ColorBlock value, IDataReader reader)
		{
			value.normalColor = ColorSerializer.ReadValue(reader);
			value.highlightedColor = ColorSerializer.ReadValue(reader);
			value.pressedColor = ColorSerializer.ReadValue(reader);
			value.disabledColor = ColorSerializer.ReadValue(reader);
			value.colorMultiplier = FloatSerializer.ReadValue(reader);
			value.fadeDuration = FloatSerializer.ReadValue(reader);
		}

		protected override void Write(ref ColorBlock value, IDataWriter writer)
		{
			ColorSerializer.WriteValue(value.normalColor, writer);
			ColorSerializer.WriteValue(value.highlightedColor, writer);
			ColorSerializer.WriteValue(value.pressedColor, writer);
			ColorSerializer.WriteValue(value.disabledColor, writer);
			FloatSerializer.WriteValue(value.colorMultiplier, writer);
			FloatSerializer.WriteValue(value.fadeDuration, writer);
		}
	}
}
