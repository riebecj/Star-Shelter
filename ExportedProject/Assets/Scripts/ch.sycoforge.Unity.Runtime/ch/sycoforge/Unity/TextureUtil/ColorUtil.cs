using System;
using System.Globalization;
using UnityEngine;
using ch.sycoforge.Types;

namespace ch.sycoforge.Unity.TextureUtil
{
	public static class ColorUtil
	{
		public static Color Parse(string hexstring)
		{
			if (hexstring.StartsWith("#"))
			{
				hexstring = hexstring.Substring(1);
			}
			if (hexstring.StartsWith("0x"))
			{
				hexstring = hexstring.Substring(2);
			}
			if (hexstring.Length != 6)
			{
				throw new Exception(string.Format("{0} is not a valid color string.", hexstring));
			}
			byte r = byte.Parse(hexstring.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hexstring.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hexstring.Substring(4, 2), NumberStyles.HexNumber);
			return new Color32(r, g, b, 1);
		}

		public static Color NormalToColor(Vector3 normal)
		{
			float r = (normal.x + 1f) * 0.5f;
			float g = (normal.y + 1f) * 0.5f;
			float b = (normal.z + 1f) * 0.5f;
			return new Color(r, g, b);
		}

		public static Color NormalToColorUnity(Vector3 normal)
		{
			float num = (normal.y + 1f) * 0.5f;
			float a = (normal.x + 1f) * 0.5f;
			return new Color(num, num, num, a);
		}

		public static Float3 ColorToVector3(FloatColor color)
		{
			float r = color.r;
			float g = color.g;
			float b = color.b;
			return new Float3(r, g, b);
		}

		public static Float3 ColorToNormal(FloatColor color)
		{
			float x = color.r * 2f - 1f;
			float y = color.g * 2f - 1f;
			float z = color.b * 2f - 1f;
			return new Float3(x, y, z).normalized;
		}

		public static FloatColor NormalToColor(Float3 normal)
		{
			float r = (normal.x + 1f) * 0.5f;
			float g = (normal.y + 1f) * 0.5f;
			float b = (normal.z + 1f) * 0.5f;
			return new FloatColor(r, g, b);
		}

		public static FloatColor NormalToColorUnity(Float3 normal)
		{
			float num = (normal.y + 1f) * 0.5f;
			float a = (normal.x + 1f) * 0.5f;
			return new FloatColor(num, num, num, a);
		}

		public static Color ToDXT5(Color color)
		{
			float g = color.g;
			float r = color.r;
			return new Color(g, g, g, r);
		}

		public static Vector3 ColorToNormal(Color color)
		{
			float x = color.r * 2f - 1f;
			float y = color.g * 2f - 1f;
			float z = color.b * 2f - 1f;
			return new Vector3(x, y, z).normalized;
		}

		public static Vector3 ColorToVector3(Color color)
		{
			float r = color.r;
			float g = color.g;
			float b = color.b;
			return new Vector3(r, g, b);
		}

		public static int ToARGBInteger(Color color)
		{
			return ToARGBInteger((Color32)color);
		}

		public static int ToARGBInteger(Color32 color)
		{
			return (color.a << 24) | (color.r << 16) | (color.g << 8) | color.b;
		}

		public static Color FromARGBInteger(int color)
		{
			byte b = (byte)(color >> 24);
			byte b2 = (byte)((uint)(color >> 16) & 0xFu);
			byte b3 = (byte)((uint)(color >> 8) & 0xFu);
			byte b4 = (byte)((uint)color & 0xFu);
			return new Color((int)b2, (int)b3, (int)b4, (int)b);
		}

		public static int NextPowerOfTwo(int number)
		{
			int num = 2;
			while ((float)num < Mathf.Pow(2f, 14f))
			{
				num *= 2;
				if (num >= number)
				{
					return num;
				}
			}
			return num;
		}
	}
}
