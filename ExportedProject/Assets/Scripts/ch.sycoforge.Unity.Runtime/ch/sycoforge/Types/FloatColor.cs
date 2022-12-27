using System;
using UnityEngine;

namespace ch.sycoforge.Types
{
	[Serializable]
	public struct FloatColor
	{
		public float r;

		public float g;

		public float b;

		public float a;

		public static FloatColor black
		{
			get
			{
				return new FloatColor(0f, 0f, 0f, 1f);
			}
		}

		public static FloatColor blue
		{
			get
			{
				return new FloatColor(0f, 0f, 1f, 1f);
			}
		}

		public static FloatColor clear
		{
			get
			{
				return new FloatColor(0f, 0f, 0f, 0f);
			}
		}

		public static FloatColor cyan
		{
			get
			{
				return new FloatColor(0f, 1f, 1f, 1f);
			}
		}

		public static FloatColor gray
		{
			get
			{
				return new FloatColor(0.5f, 0.5f, 0.5f, 1f);
			}
		}

		public float grayscale
		{
			get
			{
				return 0.299f * r + 0.587f * g + 0.114f * b;
			}
		}

		public static FloatColor green
		{
			get
			{
				return new FloatColor(0f, 1f, 0f, 1f);
			}
		}

		public static FloatColor grey
		{
			get
			{
				return new FloatColor(0.5f, 0.5f, 0.5f, 1f);
			}
		}

		public float this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return r;
				case 1:
					return g;
				case 2:
					return b;
				case 3:
					return a;
				default:
					throw new IndexOutOfRangeException("Invalid Vector3 index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					r = value;
					break;
				case 1:
					g = value;
					break;
				case 2:
					b = value;
					break;
				case 3:
					a = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Vector3 index!");
				}
			}
		}

		public FloatColor linear
		{
			get
			{
				return new FloatColor(FloatMath.GammaToLinearSpace(r), FloatMath.GammaToLinearSpace(g), FloatMath.GammaToLinearSpace(b), a);
			}
		}

		public FloatColor gamma
		{
			get
			{
				return new FloatColor(FloatMath.LinearToGammaSpace(r), FloatMath.LinearToGammaSpace(g), FloatMath.LinearToGammaSpace(b), a);
			}
		}

		public float PerceivedBrightness
		{
			get
			{
				return GetPerceivedBrightness(this);
			}
		}

		public float Contrast
		{
			get
			{
				return GetContrast(this);
			}
		}

		public float RelativeLuminance
		{
			get
			{
				return GetRelativeLuminance(this);
			}
		}

		public float FastLuminance
		{
			get
			{
				return GetFastLuminance(this);
			}
		}

		public static FloatColor magenta
		{
			get
			{
				return new FloatColor(1f, 0f, 1f, 1f);
			}
		}

		public float maxColorComponent
		{
			get
			{
				return FloatMath.Max(FloatMath.Max(r, g), b);
			}
		}

		public static FloatColor red
		{
			get
			{
				return new FloatColor(1f, 0f, 0f, 1f);
			}
		}

		public static FloatColor white
		{
			get
			{
				return new FloatColor(1f, 1f, 1f, 1f);
			}
		}

		public static FloatColor yellow
		{
			get
			{
				return new FloatColor(1f, 47f / 51f, 0.015686275f, 1f);
			}
		}

		public FloatColor(float r, float g, float b, float a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public FloatColor(float r, float g, float b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			a = 1f;
		}

		internal FloatColor AlphaMultiplied(float multiplier)
		{
			return new FloatColor(r, g, b, a * multiplier);
		}

		public override bool Equals(object other)
		{
			if (!(other is FloatColor))
			{
				return false;
			}
			FloatColor floatColor = (FloatColor)other;
			return r.Equals(floatColor.r) && g.Equals(floatColor.g) && b.Equals(floatColor.b) && a.Equals(floatColor.a);
		}

		public override int GetHashCode()
		{
			return GetHashCode();
		}

		public static float GetRelativeLuminance(FloatColor pixel)
		{
			return 0.2126f * pixel.r + 0.7152f * pixel.g + 0.0722f * pixel.b;
		}

		public static float GetContrast(FloatColor pixel)
		{
			return 0.299f * pixel.r + 0.587f * pixel.g + 0.114f * pixel.b;
		}

		public static float GetPerceivedBrightness(FloatColor pixel)
		{
			return FloatMath.Sqrt(pixel.r * pixel.r * 0.241f + pixel.g * pixel.g * 0.691f + pixel.b * pixel.b * 0.068f);
		}

		public static float GetFastLuminance(FloatColor pixel)
		{
			return (pixel.r + pixel.r + pixel.g + pixel.g + pixel.g + pixel.r * pixel.r) / 6f;
		}

		public static int GetError(FloatColor c1, FloatColor c2)
		{
			if (c1.a == 0f || c2.a == 0f)
			{
				return 0;
			}
			int num = 1000;
			int num2 = (int)(c1.r * (float)num) - (int)(c2.r * (float)num);
			int num3 = (int)(c1.g * (float)num) - (int)(c1.g * (float)num);
			int num4 = (int)(c1.b * (float)num) - (int)(c1.b * (float)num);
			int num5 = num2 * num2;
			int num6 = num3 * num3;
			int num7 = num4 * num4;
			return num5 + num6 + num7;
		}

		public static FloatColor BlendColors(FloatColor c1, FloatColor c2)
		{
			return new FloatColor((c1.r + c2.r) * 0.5f, (c1.g + c2.g) * 0.5f, (c1.b + c2.b) * 0.5f);
		}

		public static FloatColor Lerp(FloatColor a, FloatColor b, float t)
		{
			t = FloatMath.Clamp01(t);
			return new FloatColor(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
		}

		public static FloatColor operator +(FloatColor a, FloatColor b)
		{
			return new FloatColor(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
		}

		public static FloatColor operator /(FloatColor a, float b)
		{
			return new FloatColor(a.r / b, a.g / b, a.b / b, a.a / b);
		}

		public static bool operator ==(FloatColor lhs, FloatColor rhs)
		{
			return lhs == rhs;
		}

		public static implicit operator Float4(FloatColor c)
		{
			return new Float4(c.r, c.g, c.b, c.a);
		}

		public static implicit operator FloatColor(Float4 v)
		{
			return new FloatColor(v.x, v.y, v.z, v.w);
		}

		public static implicit operator Color(FloatColor c)
		{
			return new Color(c.r, c.g, c.b, c.a);
		}

		public static implicit operator FloatColor(Color c)
		{
			return new FloatColor(c.r, c.g, c.b, c.a);
		}

		public static bool operator !=(FloatColor lhs, FloatColor rhs)
		{
			return !(lhs == rhs);
		}

		public static FloatColor operator *(FloatColor a, FloatColor b)
		{
			return new FloatColor(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
		}

		public static FloatColor operator *(FloatColor a, float b)
		{
			return new FloatColor(a.r * b, a.g * b, a.b * b, a.a * b);
		}

		public static FloatColor operator *(float b, FloatColor a)
		{
			return new FloatColor(a.r * b, a.g * b, a.b * b, a.a * b);
		}

		public static FloatColor operator -(FloatColor a, FloatColor b)
		{
			return new FloatColor(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
		}

		internal FloatColor RGBMultiplied(float multiplier)
		{
			return new FloatColor(r * multiplier, g * multiplier, b * multiplier, a);
		}

		internal FloatColor RGBMultiplied(FloatColor multiplier)
		{
			return new FloatColor(r * multiplier.r, g * multiplier.g, b * multiplier.b, a);
		}

		public float Hue()
		{
			float num = FloatMath.Min(FloatMath.Min(r, g), b);
			float num2 = FloatMath.Max(FloatMath.Max(r, g), b);
			float num3 = 0f;
			num3 = (FloatMath.Approximately(num2, r) ? ((g - b) / (num2 - num)) : ((!FloatMath.Approximately(num2, g)) ? (4f + (r - g) / (num2 - num)) : (2f + (b - r) / (num2 - num))));
			num3 *= 60f;
			if (num3 < 0f)
			{
				num3 += 360f;
			}
			return num3;
		}

		private static double ColorCalc(double c, double t1, double t2)
		{
			if (c < 0.0)
			{
				c += 1.0;
			}
			if (c > 1.0)
			{
				c -= 1.0;
			}
			if (6.0 * c < 1.0)
			{
				return t1 + (t2 - t1) * 6.0 * c;
			}
			if (2.0 * c < 1.0)
			{
				return t2;
			}
			if (3.0 * c < 2.0)
			{
				return t1 + (t2 - t1) * (2.0 / 3.0 - c) * 6.0;
			}
			return t1;
		}

		public override string ToString()
		{
			return string.Format("RGBA({0:F3}, {1:F3}, {2:F3}, {3:F3})", r, g, b, a);
		}

		public static int Compare(FloatColor c1, FloatColor c2)
		{
			return c1.grayscale.CompareTo(c2.grayscale);
		}

		public void Clamp()
		{
			r = FloatMath.Clamp01(r);
			g = FloatMath.Clamp01(g);
			b = FloatMath.Clamp01(b);
			a = FloatMath.Clamp01(a);
		}

		public static float EuclidianDistance(FloatColor c1, FloatColor c2)
		{
			Float3 @float = new Float3(c1.r, c1.g, c1.b);
			Float3 float2 = new Float3(c2.r, c2.g, c2.b);
			return Float3.Distance(@float, float2);
		}

		public static float HueDistance(FloatColor c1, FloatColor c2)
		{
			return FloatMath.Abs(c1.Hue() - c2.Hue());
		}
	}
}
