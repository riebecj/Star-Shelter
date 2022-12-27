using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ch.sycoforge.Types
{
	[Serializable]
	[ComVisible(true)]
	public struct ColorGradient
	{
		public GradientKey[] ColorKeys;

		public GradientKey[] AlphaKeys;

		public int colorKeyCount;

		public int alphaKeyCount;

		[CompilerGenerated]
		private static Comparison<GradientKey> CS_0024_003C_003E9__CachedAnonymousMethodDelegate1;

		[CompilerGenerated]
		private static Comparison<GradientKey> CS_0024_003C_003E9__CachedAnonymousMethodDelegate4;

		[CompilerGenerated]
		private static Comparison<GradientKey> CS_0024_003C_003E9__CachedAnonymousMethodDelegate5;

		public ColorGradient(int colorKeyCount, int alphaKeyCount)
		{
			ColorKeys = new GradientKey[colorKeyCount];
			this.colorKeyCount = colorKeyCount;
			AlphaKeys = new GradientKey[alphaKeyCount];
			this.alphaKeyCount = alphaKeyCount;
		}

		public Float4 SampleColor(float position)
		{
			Float4 result = Float4.zero;
			if (ColorKeys.Length >= 2)
			{
				if (position < ColorKeys[0].Position)
				{
					result = ColorKeys[0].Color;
				}
				else if (position > ColorKeys[ColorKeys.Length - 1].Position)
				{
					result = ColorKeys[ColorKeys.Length - 1].Color;
				}
				else
				{
					result = Sample(position, ColorKeys);
					result.w = Sample(position, AlphaKeys).w;
				}
			}
			return result;
		}

		private Float4 Sample(float position, GradientKey[] keys)
		{
			Float4 result = Float4.zero;
			if (keys != null)
			{
				if (CS_0024_003C_003E9__CachedAnonymousMethodDelegate1 == null)
				{
					CS_0024_003C_003E9__CachedAnonymousMethodDelegate1 = _003CSample_003Eb__0;
				}
				Array.Sort(keys, CS_0024_003C_003E9__CachedAnonymousMethodDelegate1);
				GradientKey gradientKey = default(GradientKey);
				GradientKey gradientKey2 = default(GradientKey);
				for (int i = 0; i < keys.Length - 1; i++)
				{
					gradientKey = keys[i];
					gradientKey2 = keys[i + 1];
					if (position > gradientKey.Position && position < gradientKey2.Position)
					{
						break;
					}
				}
				float position2 = gradientKey.Position;
				float num = gradientKey2.Position - position2;
				float num2 = 1f / num;
				float num3 = (position - position2) * num2;
				result = gradientKey2.Color * num3 + gradientKey.Color * (1f - num3);
			}
			return result;
		}

		public void Sort()
		{
			GradientKey[] colorKeys = ColorKeys;
			if (CS_0024_003C_003E9__CachedAnonymousMethodDelegate4 == null)
			{
				CS_0024_003C_003E9__CachedAnonymousMethodDelegate4 = _003CSort_003Eb__2;
			}
			Array.Sort(colorKeys, CS_0024_003C_003E9__CachedAnonymousMethodDelegate4);
			GradientKey[] alphaKeys = AlphaKeys;
			if (CS_0024_003C_003E9__CachedAnonymousMethodDelegate5 == null)
			{
				CS_0024_003C_003E9__CachedAnonymousMethodDelegate5 = _003CSort_003Eb__3;
			}
			Array.Sort(alphaKeys, CS_0024_003C_003E9__CachedAnonymousMethodDelegate5);
			colorKeyCount = ColorKeys.Length;
			alphaKeyCount = AlphaKeys.Length;
		}

		public static ColorGradient GetDefault()
		{
			ColorGradient result = default(ColorGradient);
			result.colorKeyCount = 2;
			result.alphaKeyCount = 2;
			result.ColorKeys = new GradientKey[2];
			result.AlphaKeys = new GradientKey[2];
			result.ColorKeys[0] = new GradientKey(new Float4(1f, 0.5f, 0f), 0f);
			result.ColorKeys[1] = new GradientKey(new Float4(1f, 1f, 1f), 0.51f);
			result.AlphaKeys[0] = new GradientKey(new Float4(1f, 1f, 1f, 1f), 0f);
			result.AlphaKeys[1] = new GradientKey(new Float4(1f, 1f, 1f, 1f), 1f);
			return result;
		}

		[CompilerGenerated]
		private static int _003CSample_003Eb__0(GradientKey x, GradientKey y)
		{
			return x.Position.CompareTo(y.Position);
		}

		[CompilerGenerated]
		private static int _003CSort_003Eb__2(GradientKey x, GradientKey y)
		{
			return x.Position.CompareTo(y.Position);
		}

		[CompilerGenerated]
		private static int _003CSort_003Eb__3(GradientKey x, GradientKey y)
		{
			return x.Position.CompareTo(y.Position);
		}
	}
}
