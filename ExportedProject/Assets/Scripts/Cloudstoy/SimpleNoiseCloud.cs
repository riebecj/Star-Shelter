using System;
using UnityEngine;

public class SimpleNoiseCloud
{
	private float[,] noise;

	private int NoiseHeight;

	private int NoiseWidth;

	public SimpleNoiseCloud(int width, int height)
	{
		NoiseWidth = width;
		NoiseHeight = height;
		noise = new float[width, height];
		GenerateNoise();
	}

	public void GenerateNoise()
	{
		System.Random random = new System.Random();
		for (int i = 0; i < NoiseWidth; i++)
		{
			for (int j = 0; j < NoiseHeight; j++)
			{
				noise[i, j] = random.Next(255);
			}
		}
	}

	public float bias(float a, float b)
	{
		if (a < 0.001f)
		{
			return 0f;
		}
		if (a > 0.999f)
		{
			return 1f;
		}
		if (b < 0.001f)
		{
			return 0f;
		}
		if (b > 0.999f)
		{
			return 1f;
		}
		return Mathf.Pow(a, Mathf.Log(b) / Mathf.Log(0.5f));
	}

	public float filter(float t)
	{
		t = bias(t, 0.67f);
		if (t < 0.5f)
		{
			t = gain(t, 0.86f);
		}
		t = bias(t, 0.35f);
		return t;
	}

	public float gain(float a, float b)
	{
		if (a < 0.001f)
		{
			return 0f;
		}
		if (a > 0.999f)
		{
			return 1f;
		}
		b = ((b < 0.001f) ? 0.0001f : ((b > 0.999f) ? 0.999f : b));
		float p = Mathf.Log(1f - b) / Mathf.Log(0.5f);
		if ((double)a < 0.5)
		{
			return Mathf.Pow(2f * a, p) / 2f;
		}
		return 1f - Mathf.Pow(2f * (1f - a), p) / 2f;
	}

	private float Interpolate(float x, float y, float a)
	{
		float num = 1f - a;
		float num2 = 3f * num * num - 2f * num * num * num;
		float num3 = 3f * a * a - 2f * a * a * a;
		return x * num2 + y * num3;
	}

	public float InvTurbulence(float x, float y, int octaves, float Lacunarity, float Gain)
	{
		float num = 0f;
		float num2 = 1f;
		float num3 = 1f;
		for (num2 = 1f; num2 < (float)octaves; num2 *= 2.01f - Lacunarity)
		{
			num += 1f / num2 * num3 * SmoothNoiseLerp(x / num2, y / num2) * num2;
			num3 *= Gain;
		}
		return num / (float)octaves;
	}

	public float MarbleNoise(float x, float y, int octaves, float Lacunarity, float Gain)
	{
		float num = Mathf.Sin(x + Turbulence(x, y, octaves, Lacunarity, Gain) + 1f) * 0.5f;
		float num2 = Mathf.Sin(y + Turbulence(x, y, octaves, Lacunarity, Gain) + 1f) * 0.5f;
		return Mathf.Sqrt((num + num2) / 2f + 1f);
	}

	public float Noise(float x, float y)
	{
		int num = (int)((x + (float)NoiseWidth) % (float)NoiseWidth);
		int num2 = (int)((y + (float)NoiseHeight) % (float)NoiseHeight);
		return noise[num, num2] / 256f;
	}

	public float SinInvTurbulence(float x, float y, int octaves, float Lacunarity, float Gain)
	{
		float num = 0f;
		float num2 = 1f;
		float num3 = 1f;
		for (num2 = 1f; num2 < (float)octaves; num2 *= 2.01f - Lacunarity)
		{
			num += 1f / Mathf.Sin(num3 * SmoothNoiseLerp(x / num2, y / num2) * num2);
			num3 *= Gain;
		}
		return num / (float)octaves;
	}

	public float SinTurbulence(float x, float y, int octaves, float Lacunarity, float Gain)
	{
		float num = 0f;
		float num2 = 1f;
		float num3 = 1f;
		for (num2 = 1f; num2 < (float)octaves; num2 *= 2.01f - Lacunarity)
		{
			num += num3 * Mathf.Sin(SmoothNoiseLerp(x / num2, y / num2) * num2);
			num3 *= Gain;
		}
		return num / (float)octaves;
	}

	private float SmoothNoise(int x, int y)
	{
		float num = (Noise(x - 1, y - 1) + Noise(x + 1, y - 1) + Noise(x - 1, y + 1) + Noise(x + 1, y + 1)) / 16f;
		float num2 = (Noise(x - 1, y) + Noise(x + 1, y) + Noise(x, y - 1) + Noise(x, y + 1)) / 8f;
		float num3 = Noise(x, y) / 4f;
		return num + num2 + num3;
	}

	public float SmoothNoiseLerp(float x, float y)
	{
		int num = (int)x;
		float a = x - (float)num;
		int num2 = (int)y;
		float a2 = y - (float)num2;
		float x2 = Noise(num, num2);
		float y2 = Noise(num + 1, num2);
		float x3 = Noise(num, num2 + 1);
		float y3 = Noise(num + 1, num2 + 1);
		float x4 = Interpolate(x2, y2, a);
		float y4 = Interpolate(x3, y3, a);
		return Interpolate(x4, y4, a2);
	}

	public float snoise(float x, float y)
	{
		return 2f * Noise(x, y) - 1f;
	}

	public float ssmoothnoise(float x, float y)
	{
		return 2f * SmoothNoiseLerp(x, y) - 1f;
	}

	public float Turbulence(float x, float y, int octaves, float Lacunarity, float Gain)
	{
		float num = 0f;
		float num2 = 1f;
		float num3 = 1f;
		for (num2 = 1f; num2 < (float)octaves; num2 *= 2.01f - Lacunarity)
		{
			num += num3 * SmoothNoiseLerp(x / num2, y / num2) * num2;
			num3 *= Gain;
		}
		return num / (float)octaves;
	}
}
