using UnityEngine;

public class ProceduralCloudTexture : MonoBehaviour
{
	public enum NoisePreset
	{
		Cloud = 0,
		PerlinCloud = 1
	}

	public NoisePreset TypeNoise;

	public float AlphaIndex = 0.5f;

	public Color BackgroundColor = new Color(0f, 0f, 0f, 1f);

	public float ContrastMult;

	public Color FinalColor = new Color(1f, 1f, 1f, 1f);

	private FractalNoiseCloud fractal;

	public float FractalIncrement = 0.5f;

	public float HaloEffect = 0.1f;

	public float HaloInsideRadius = 0.8f;

	public bool HasChanged = true;

	private bool Inicialized;

	public bool InvertColors;

	public bool IsHalo = true;

	public float Lacunarity = 3f;

	public Texture2D MyAlphaDrawTexture;

	public Texture2D MyAlphaTexture;

	public Texture2D MyTexture;

	private float noiseF;

	public float Octaves = 7f;

	public float Offset = 1f;

	private PerlinCloud perlin;

	private float RandomX;

	private float RandomY;

	public float ScaleFactor = 1f;

	public float ScaleHeight = 1f;

	public float ScaleWidth = 1f;

	public int Seed = 132;

	private SimpleNoiseCloud simplenoise;

	public int TextureHeight = 64;

	private bool TextureUpdated;

	public int TextureWidth = 64;

	public float TurbGain = 0.5f;

	public float TurbLacun = 0.01f;

	public float turbPower = 5f;

	public int TurbSize = 16;

	public bool UseAlphaTexture;

	public float xyPeriod = 12f;

	private void Start()
	{
		MyTexture = new Texture2D(TextureWidth, TextureHeight, TextureFormat.RGB24, false);
		MyAlphaTexture = new Texture2D(TextureWidth, TextureHeight, TextureFormat.ARGB32, false);
		MyAlphaDrawTexture = new Texture2D(TextureWidth, TextureHeight, TextureFormat.ARGB32, false);
	}

	private void Update()
	{
		if (HasChanged)
		{
			Calculate();
			if (!Inicialized)
			{
				Inicialized = true;
			}
		}
		HasChanged = false;
	}

	private void Calculate()
	{
		TextureUpdated = true;
		if (perlin == null)
		{
			perlin = new PerlinCloud();
		}
		fractal = new FractalNoiseCloud(FractalIncrement, Lacunarity, Octaves, perlin);
		if (simplenoise == null)
		{
			simplenoise = new SimpleNoiseCloud(TextureWidth, TextureHeight);
		}
		BackgroundColor.a = 1f;
		FinalColor.a = 1f;
		Vector2 center = new Vector2((float)TextureWidth * 0.5f, (float)TextureHeight * 0.5f);
		float radius = (float)TextureWidth * 0.5f * HaloEffect;
		for (int i = 0; i < TextureHeight; i++)
		{
			for (int j = 0; j < TextureWidth; j++)
			{
				switch (TypeNoise)
				{
				case NoisePreset.Cloud:
				{
					Vector2 vector = new Vector2(TextureWidth / 2, TextureHeight / 2);
					float f = (float)(TextureWidth / 2) / ScaleWidth * xyPeriod;
					float num = Mathf.Pow((float)j - vector.x, 2f) + Mathf.Pow((float)i - vector.y, 2f);
					float num2 = Mathf.Pow(f, 2f);
					float num3 = Mathf.Abs((num - num2) / 10000f);
					if (num3 > xyPeriod)
					{
						num3 = xyPeriod;
					}
					noiseF = (xyPeriod - num3) * turbPower * simplenoise.Turbulence((float)j * ScaleWidth * ScaleFactor / (float)TextureWidth + RandomX, (float)i * ScaleHeight * ScaleFactor / (float)TextureHeight + RandomY, TurbSize, TurbLacun, TurbGain);
					break;
				}
				case NoisePreset.PerlinCloud:
					noiseF = fractal.HybridMultifractal((float)j * ScaleWidth * ScaleFactor / (float)TextureWidth + RandomX, (float)i * ScaleHeight * ScaleFactor / (float)TextureHeight + RandomY, Offset);
					break;
				}
				Color myColor = ((!InvertColors) ? Color.Lerp(BackgroundColor, FinalColor, noiseF) : Color.Lerp(FinalColor, BackgroundColor, noiseF));
				myColor = ((!IsHalo) ? PerformHaloEffect(myColor, center, (float)(TextureWidth / 2) * 1.404f, j, i) : PerformHaloEffect(myColor, center, radius, j, i));
				if (myColor.r > 0f && myColor.g > 0f && myColor.b > 0f)
				{
					myColor += new Color(ContrastMult, ContrastMult, ContrastMult, 1f);
				}
				MyTexture.SetPixel(j, i, myColor);
				float r = myColor.r;
				Color color = myColor;
				color.a = r;
				if (AlphaIndex > 0f)
				{
					color.a -= AlphaIndex;
				}
				color.a = Mathf.Clamp01(color.a);
				MyAlphaTexture.SetPixel(j, i, color);
				if (UseAlphaTexture)
				{
					if (AlphaIndex > 0f)
					{
						color = new Color(0f, r - AlphaIndex, 0f, r - AlphaIndex);
					}
					color.a = Mathf.Clamp01(color.a);
					color.g = Mathf.Clamp01(color.g);
					MyAlphaDrawTexture.SetPixel(j, i, color);
				}
			}
		}
		MyTexture.Apply(false);
		MyAlphaTexture.Apply(false);
		if (UseAlphaTexture)
		{
			MyAlphaDrawTexture.Apply(false);
		}
	}

	public void CreateNewTexture()
	{
		MyTexture = new Texture2D(TextureWidth, TextureHeight, TextureFormat.RGB24, false);
		MyAlphaTexture = new Texture2D(TextureWidth, TextureHeight, TextureFormat.ARGB32, false);
		MyAlphaDrawTexture = new Texture2D(TextureWidth, TextureHeight, TextureFormat.ARGB32, false);
		MyTexture.Apply(false);
		if (UseAlphaTexture)
		{
			MyAlphaTexture.Apply(false);
			MyAlphaDrawTexture.Apply(false);
		}
		HasChanged = true;
	}

	public bool IsInicialized()
	{
		return Inicialized;
	}

	public bool IsTextureUpdated()
	{
		bool textureUpdated = TextureUpdated;
		TextureUpdated = false;
		return textureUpdated;
	}

	public void NewRandomSeed()
	{
		Random.InitState(Seed);
		RandomX = Random.Range(0f, ScaleWidth * 10f);
		RandomY = Random.Range(0f, ScaleHeight * 10f);
		HasChanged = true;
	}

	private Color PerformHaloEffect(Color MyColor, Vector2 center, float radius, float x, float y)
	{
		float num = Mathf.Pow(x - center.x, 2f) + Mathf.Pow(y - center.y, 2f);
		float num2 = Mathf.Pow(radius, 2f);
		float num3 = Mathf.Abs((num - num2) / 10000f);
		if (num3 < HaloInsideRadius)
		{
			float num4 = Mathf.Min(HaloInsideRadius - num3, 1f);
			num4 = Mathf.Pow(1f - Mathf.Clamp01(1f - num4), 3f);
			MyColor -= new Color(num4, num4, num4, 1f);
		}
		MyColor.r = Mathf.Clamp01(MyColor.r);
		MyColor.g = Mathf.Clamp01(MyColor.g);
		MyColor.b = Mathf.Clamp01(MyColor.b);
		if (num >= num2)
		{
			MyColor = Color.black;
		}
		return MyColor;
	}
}
