using System;
using System.Reflection;
using UnityEngine;

namespace Sirenix.Serialization
{
	[CustomFormatter]
	public class GradientFormatter : MinimalBaseFormatter<Gradient>
	{
		private static readonly Serializer<GradientAlphaKey[]> AlphaKeysSerializer = Serializer.Get<GradientAlphaKey[]>();

		private static readonly Serializer<GradientColorKey[]> ColorKeysSerializer = Serializer.Get<GradientColorKey[]>();

		private static readonly PropertyInfo ModeProperty = typeof(Gradient).GetProperty("mode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		private static readonly Serializer<Enum> EnumSerializer = ((ModeProperty != null) ? Serializer.Get<Enum>() : null);

		protected override Gradient GetUninitializedObject()
		{
			return new Gradient();
		}

		protected override void Read(ref Gradient value, IDataReader reader)
		{
			value.alphaKeys = AlphaKeysSerializer.ReadValue(reader);
			value.colorKeys = ColorKeysSerializer.ReadValue(reader);
			if (ModeProperty != null)
			{
				ModeProperty.SetValue(value, EnumSerializer.ReadValue(reader), null);
			}
		}

		protected override void Write(ref Gradient value, IDataWriter writer)
		{
			AlphaKeysSerializer.WriteValue(value.alphaKeys, writer);
			ColorKeysSerializer.WriteValue(value.colorKeys, writer);
			if (ModeProperty != null)
			{
				EnumSerializer.WriteValue((Enum)ModeProperty.GetValue(value, null), writer);
			}
		}
	}
}
