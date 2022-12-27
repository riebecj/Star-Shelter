using System;
using System.Runtime.InteropServices;

namespace ch.sycoforge.Types
{
	public static class FloatMath
	{
		public const float PI = (float)Math.PI;

		public const float Infinity = float.PositiveInfinity;

		public const float NegativeInfinity = float.NegativeInfinity;

		public const float Deg2Rad = (float)Math.PI / 180f;

		public const float Rad2Deg = 57.29578f;

		[DllImport("sycoforge_imaging", CallingConvention = CallingConvention.Cdecl)]
		private static extern float LinearToGamma(float value);

		[DllImport("sycoforge_imaging", CallingConvention = CallingConvention.Cdecl)]
		private static extern float GammaToLinear(float value);

		public static float InverseLerp(float from, float to, float value)
		{
			if (from < to)
			{
				if (value < from)
				{
					return 0f;
				}
				if (value > to)
				{
					return 1f;
				}
				value -= from;
				value /= to - from;
				return value;
			}
			if (from <= to)
			{
				return 0f;
			}
			if (value < to)
			{
				return 1f;
			}
			if (value > from)
			{
				return 0f;
			}
			return 1f - (value - to) / (from - to);
		}

		public static float Lerp(float from, float to, float t)
		{
			return from + (to - from) * Clamp01(t);
		}

		public static float CosineInterpolate(float from, float to, float t)
		{
			float num = (1f - Cos(t * (float)Math.PI)) / 2f;
			return from * (1f - t) + to * num;
		}

		public static float SmoothLerpT3(float from, float to, float t)
		{
			float num = t * t * (3f - 2f * t);
			return Lerp(from, to, t);
		}

		public static float SmoothLerpT5(float from, float to, float t)
		{
			float num = t * t * t * (t * (t * 6f - 15f) + 10f);
			return Lerp(from, to, t);
		}

		public static float GammaToLinearSpace(float value)
		{
			return GammaToLinear(value);
		}

		public static float LinearToGammaSpace(float value)
		{
			return LinearToGamma(value);
		}

		public static bool Approximately(float a, float b)
		{
			return Abs(b - a) < Max(1E-06f * Max(Abs(a), Abs(b)), 1.1E-44f);
		}

		public static float Frac(float f)
		{
			return f - Floor(f);
		}

		public static Float2 Frac(Float2 value)
		{
			value.x = Frac(value.x);
			value.y = Frac(value.y);
			return value;
		}

		public static Float3 Frac(Float3 value)
		{
			value.x = Frac(value.x);
			value.y = Frac(value.y);
			value.z = Frac(value.z);
			return value;
		}

		public static float Log(float f, float p)
		{
			return (float)Math.Log(f, p);
		}

		public static float Log(float f)
		{
			return (float)Math.Log(f);
		}

		public static float Log10(float f)
		{
			return (float)Math.Log10(f);
		}

		public static float Max(float a, float b)
		{
			return (a <= b) ? b : a;
		}

		public static float Max(params float[] values)
		{
			int num = values.Length;
			if (num == 0)
			{
				return 0f;
			}
			float num2 = values[0];
			for (int i = 1; i < num; i++)
			{
				if (values[i] > num2)
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		public static int Max(int a, int b)
		{
			return (a <= b) ? b : a;
		}

		public static int Step(int a, int x)
		{
			return (x >= a) ? 1 : 0;
		}

		public static float Step(float a, float x)
		{
			return (x >= a) ? 1f : 0f;
		}

		public static float Gamma(float value, float absmax, float gamma)
		{
			bool flag = false;
			if (value < 0f)
			{
				flag = true;
			}
			float num = Abs(value);
			if (num > absmax)
			{
				return (!flag) ? num : (0f - num);
			}
			float num2 = Pow(num / absmax, gamma) * absmax;
			return (!flag) ? num2 : (0f - num2);
		}

		public static int Max(params int[] values)
		{
			int num = values.Length;
			if (num == 0)
			{
				return 0;
			}
			int num2 = values[0];
			for (int i = 1; i < num; i++)
			{
				if (values[i] > num2)
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		public static float Min(float a, float b)
		{
			return (a >= b) ? b : a;
		}

		public static float Min(params float[] values)
		{
			int num = values.Length;
			if (num == 0)
			{
				return 0f;
			}
			float num2 = values[0];
			for (int i = 1; i < num; i++)
			{
				if (values[i] < num2)
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		public static int Min(int a, int b)
		{
			return (a >= b) ? b : a;
		}

		public static int Min(params int[] values)
		{
			int num = values.Length;
			if (num == 0)
			{
				return 0;
			}
			int num2 = values[0];
			for (int i = 1; i < num; i++)
			{
				if (values[i] < num2)
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		public static int Clamp(int value, int min, int max)
		{
			return (value < min) ? min : ((value > max) ? max : value);
		}

		public static float Clamp(float value, float min, float max)
		{
			return (value < min) ? min : ((value > max) ? max : value);
		}

		public static Float2 Clamp(Float2 value, int min, int max)
		{
			value.x = Clamp(value.x, min, max);
			value.y = Clamp(value.y, min, max);
			return value;
		}

		public static Float3 Clamp(Float3 value, int min, int max)
		{
			value.x = Clamp(value.x, min, max);
			value.y = Clamp(value.y, min, max);
			value.z = Clamp(value.z, min, max);
			return value;
		}

		public static Float2 Clamp(Float2 value, float min, float max)
		{
			value.x = Clamp(value.x, min, max);
			value.y = Clamp(value.y, min, max);
			return value;
		}

		public static Float3 Clamp(Float3 value, float min, float max)
		{
			value.x = Clamp(value.x, min, max);
			value.y = Clamp(value.y, min, max);
			value.z = Clamp(value.z, min, max);
			return value;
		}

		public static float Clamp01(float value)
		{
			if (value < 0f)
			{
				return 0f;
			}
			if (value > 1f)
			{
				return 1f;
			}
			return value;
		}

		public static Float2 Clamp01(Float2 value)
		{
			value.x = Clamp01(value.x);
			value.y = Clamp01(value.y);
			return value;
		}

		public static Float3 Clamp01(Float3 value)
		{
			value.x = Clamp01(value.x);
			value.y = Clamp01(value.y);
			value.z = Clamp01(value.z);
			return value;
		}

		public static float Floor(float value)
		{
			return (value >= 0f) ? ((float)(int)value) : ((float)(int)value - 1f);
		}

		public static Float2 Floor(Float2 value)
		{
			value.x = Floor(value.x);
			value.y = Floor(value.y);
			return value;
		}

		public static Float3 Floor(Float3 value)
		{
			value.x = Floor(value.x);
			value.y = Floor(value.y);
			value.z = Floor(value.z);
			return value;
		}

		public static int FloorToInt(float value)
		{
			return (int)value;
		}

		public static float Ceil(float value)
		{
			return (int)value + 1;
		}

		public static int CeilToInt(float value)
		{
			return (int)value + 1;
		}

		public static int RoundToInt(float value)
		{
			return FloorToInt(value + 0.5f);
		}

		public static float Pow(float f, float p)
		{
			return (float)Math.Pow(f, p);
		}

		public static float Repeat(float t, float length)
		{
			return t - Floor(t / length) * length;
		}

		public static float Round(float f)
		{
			return (float)Math.Round(f);
		}

		public static int Abs(int value)
		{
			return Math.Abs(value);
		}

		public static float Abs(float value)
		{
			return Math.Abs(value);
		}

		public static float Sin(float x)
		{
			return (float)Math.Sin(x);
		}

		public static float Cos(float x)
		{
			return (float)Math.Cos(x);
		}

		public static float Acos(float x)
		{
			return (float)Math.Acos(x);
		}

		public static float SinApprox(float x)
		{
			float num = 4f / (float)Math.PI * x + -0.4052847f * x * Abs(x);
			return 0.225f * (num * Abs(num) - num) + num;
		}

		public static float CosApprox(float x)
		{
			return SinApprox(x + (float)Math.PI / 2f);
		}

		public static float AcosApprox(float x)
		{
			float num = 1.43f + 0.59f * x;
			num = (num + (2f + 2f * x) / num) / 2f;
			float num2 = 1.65f - 1.41f * x;
			num2 = (num2 + (2f - 2f * x) / num2) / 2f;
			float num3 = 0.88f - 0.77f * x;
			num3 = (num3 + (2f - num) / num3) / 2f;
			return 2f * num3 - num2 / 3f;
		}

		public static float SqrtApprox(int value)
		{
			float num = value;
			float num2 = 1f;
			for (int i = 0; i < value; i++)
			{
				num2 = 0.5f * (num2 + num / num2);
			}
			return num2;
		}

		public static float SqrtApprox(float value)
		{
			if (value < 0f)
			{
				value = -1f * value;
			}
			float num = 0f;
			float num2 = value;
			float num3 = num2;
			float num4 = num;
			float num5 = 0f;
			float num6 = 0f;
			while (num6 != value)
			{
				num5 = (num2 + num) / 2f;
				num6 = num5 * num5;
				if (num6 > value)
				{
					num2 = num5;
				}
				else if (num6 < value)
				{
					num = num5;
				}
				if (num3 == num && num4 == num2)
				{
					break;
				}
				num3 = num;
				num4 = num2;
			}
			return num5;
		}

		public static float Sign(float f)
		{
			return (f < 0f) ? (-1f) : 1f;
		}

		public static float Sqrt(float f)
		{
			return (float)Math.Sqrt(f);
		}

		public static float Tan(float f)
		{
			return (float)Math.Tan(f);
		}
	}
}
