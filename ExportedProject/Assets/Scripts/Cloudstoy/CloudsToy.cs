using System.Collections;
using System.IO;
using UnityEngine;

public class CloudsToy : MonoBehaviour
{
	public enum TypePreset
	{
		None = 0,
		Stormy = 1,
		Sunrise = 2,
		Fantasy = 3,
		Dark = 4,
		Cotton = 5,
		CottonLight = 6,
		OrangeOil = 7
	}

	public enum TypeRender
	{
		Bright = 0,
		Realistic = 1
	}

	public enum TypeDetail
	{
		Low = 0,
		Normal = 1,
		High = 2
	}

	public enum TypeCloud
	{
		Nimbus1 = 0,
		Nimbus2 = 1,
		Nimbus3 = 2,
		Nimbus4 = 3,
		Cirrus1 = 4,
		Cirrus2 = 5,
		MixNimbus = 6,
		MixCirrus = 7,
		MixAll = 8,
		PT1 = 9
	}

	public enum TypePaintDistr
	{
		Random = 0,
		Below = 1
	}

	public enum TypeShadow
	{
		All = 0,
		Most = 1,
		Half = 2,
		Some = 3,
		None = 4
	}

	public enum NoisePresetPT1
	{
		Cloud = 0,
		PerlinCloud = 1
	}

	private enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2,
		XNeg = 3,
		YNeg = 4,
		ZNeg = 5
	}

	public int MaximunClouds = 300;

	public TypePreset CloudPreset;

	public TypeRender CloudRender;

	public TypeDetail CloudDetail;

	public TypeCloud TypeClouds;

	public Shader realisticShader;

	public Shader brightShader;

	public Material projectorMaterial;

	public float positionCheckerTime = 0.1f;

	public float SizeFactorPart = 1f;

	public float EmissionMult = 1f;

	public bool _showDebug = true;

	public bool SoftClouds;

	public Vector3 SpreadDir = new Vector3(-1f, 0f, 0f);

	public float LengthSpread = 1f;

	public int NumberClouds = 100;

	public Vector3 Side = new Vector3(1000f, 500f, 1000f);

	public Vector3 MaximunVelocity = new Vector3(-10f, 0f, 0f);

	public float VelocityMultipier = 1f;

	public float DisappearMultiplier = 1.5f;

	public TypePaintDistr PaintType = TypePaintDistr.Below;

	public Color CloudColor = new Color(1f, 1f, 1f, 1f);

	public Color MainColor = new Color(1f, 1f, 1f, 1f);

	public Color SecondColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	public int TintStrength = 50;

	public float offset = 0.5f;

	public int MaxWidthCloud = 100;

	public int MaxHeightCloud = 40;

	public int MaxDepthCloud = 100;

	public bool FixedSize = true;

	public TypeShadow NumberOfShadows = TypeShadow.Some;

	public Texture2D[] CloudsTextAdd = new Texture2D[6];

	public Texture2D[] CloudsTextBlended = new Texture2D[6];

	public bool IsAnimate = true;

	public float AnimationVelocity;

	public bool isFadeInOut;

	public float fadeRatio = 1f;

	public float alphaRange = 0.3f;

	public ProceduralCloudTexture ProceduralTexture;

	public int PT1TextureWidth = 128;

	public int PT1TextureHeight = 128;

	public NoisePresetPT1 PT1TypeNoise;

	public int PT1Seed = 132;

	public float PT1ScaleWidth = 1f;

	public float PT1ScaleHeight = 1f;

	public float PT1ScaleFactor = 1f;

	public float PT1Lacunarity = 3f;

	public float PT1FractalIncrement = 0.5f;

	public float PT1Octaves = 7f;

	public float PT1Offset = 1f;

	public int PT1TurbSize = 16;

	public float PT1TurbLacun = 0.01f;

	public float PT1TurbGain = 0.5f;

	public float PT1turbPower = 5f;

	public float PT1xyPeriod = 0.8f;

	public float PT1HaloEffect = 1.7f;

	public float PT1HaloInsideRadius = 0.1f;

	public bool PT1InvertColors;

	public float PT1ContrastMult;

	public bool PT1UseAlphaTexture = true;

	public float PT1AlphaIndex = 0.1f;

	private Material[] CloudsMatAdditive = new Material[6];

	private Material[] CloudsMatBlended = new Material[6];

	private Material CloudsPTMatAdditive;

	private Material CloudsPTMatBlended;

	private Axis CloudsGenerateAxis;

	private Transform MyTransform;

	private Vector3 MyPosition;

	private ArrayList MyCloudsParticles = new ArrayList();

	private bool bAssignProcTexture;

	private float internalLateUpdateTimer;

	private TypeRender CloudRenderPrev;

	private TypeDetail CloudDetailPrev;

	private TypeCloud TypeCloudsPrev;

	private float EmissionMultPrev = 1f;

	private float SizeFactorPartPrev = 1f;

	private bool SoftCloudsPrev;

	private Vector3 SpreadDirPrev = new Vector3(-1f, 0f, 0f);

	private float LengthSpreadPrev = 1f;

	private int NumberCloudsPrev = 10;

	private Vector3 MaximunVelocityPrev;

	private float VelocityMultipierPrev;

	private TypePaintDistr PaintTypePrev = TypePaintDistr.Below;

	private Color CloudColorPrev = new Color(1f, 1f, 1f, 1f);

	private Color MainColorPrev = new Color(1f, 1f, 1f, 1f);

	private Color SecondColorPrev = new Color(0.5f, 0.5f, 0.5f, 1f);

	private int TintStrengthPrev = 5;

	private float offsetPrev;

	private TypeShadow NumberOfShadowsPrev;

	private int MaxWidthCloudPrev = 200;

	private int MaxHeightCloudPrev = 50;

	private int MaxDepthCloudPrev = 200;

	private bool IsAnimatePrev = true;

	private float AnimationVelocityPrev;

	private bool isFadeInOutPrev = true;

	public float fadeRatioPrev;

	private float alphaRangePrev;

	public int maximunCloudsOrig;

	public void CTDebug(string text)
	{
		if (_showDebug)
		{
			Debug.Log(text);
		}
	}

	public void SetPresetNone()
	{
		CloudPreset = TypePreset.None;
	}

	public void SetPresetStormy()
	{
		CloudPreset = TypePreset.Stormy;
		CloudRender = TypeRender.Realistic;
		CloudDetail = TypeDetail.Normal;
		SetCloudDetailParams();
		TypeClouds = TypeCloud.Nimbus2;
		SoftClouds = false;
		SpreadDir = new Vector3(-1f, 0f, 0f);
		LengthSpread = 1f;
		NumberClouds = 100;
		DisappearMultiplier = 2f;
		MaximunVelocity = new Vector3(-10f, 0f, 0f);
		VelocityMultipier = 0.85f;
		PaintType = TypePaintDistr.Below;
		CloudColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);
		MainColor = new Color(0.62f, 0.62f, 0.62f, 0.3f);
		SecondColor = new Color(0.31f, 0.31f, 0.31f, 1f);
		TintStrength = 80;
		offset = 0.8f;
		MaxWidthCloud = 200;
		MaxHeightCloud = 50;
		MaxDepthCloud = 200;
		FixedSize = false;
		IsAnimate = false;
		isFadeInOut = false;
		NumberOfShadows = TypeShadow.Some;
	}

	public void SetPresetSunrise()
	{
		CloudPreset = TypePreset.Sunrise;
		CloudRender = TypeRender.Bright;
		CloudDetail = TypeDetail.Low;
		SetCloudDetailParams();
		EmissionMult = 1.6f;
		SizeFactorPart = 1.5f;
		TypeClouds = TypeCloud.Cirrus1;
		SoftClouds = true;
		SpreadDir = new Vector3(-1f, 0f, 0f);
		LengthSpread = 4f;
		NumberClouds = 135;
		DisappearMultiplier = 2f;
		MaximunVelocity = new Vector3(-10f, 0f, 0f);
		VelocityMultipier = 6.2f;
		PaintType = TypePaintDistr.Below;
		CloudColor = new Color(0.2f, 0.2f, 0.2f, 0.33f);
		MainColor = new Color(1f, 1f, 0.66f, 0.5f);
		SecondColor = new Color(1f, 0.74f, 0f, 1f);
		TintStrength = 100;
		offset = 1f;
		MaxWidthCloud = 500;
		MaxHeightCloud = 20;
		MaxDepthCloud = 500;
		FixedSize = true;
		IsAnimate = false;
		isFadeInOut = false;
		NumberOfShadows = TypeShadow.None;
	}

	public void SetPresetFantasy()
	{
		CloudPreset = TypePreset.Fantasy;
		CloudRender = TypeRender.Bright;
		CloudDetail = TypeDetail.Low;
		EmissionMult = 0.3f;
		SetCloudDetailParams();
		TypeClouds = TypeCloud.Nimbus4;
		SoftClouds = false;
		SpreadDir = new Vector3(-1f, 0f, 0f);
		LengthSpread = 1f;
		NumberClouds = 200;
		DisappearMultiplier = 2f;
		MaximunVelocity = new Vector3(-10f, 0f, 0f);
		VelocityMultipier = 0.5f;
		PaintType = TypePaintDistr.Random;
		CloudColor = new Color(0.15f, 0.15f, 0.15f, 0.5f);
		MainColor = new Color(1f, 0.62f, 0f, 1f);
		SecondColor = new Color(0.5f, 0.5f, 0.5f, 1f);
		TintStrength = 50;
		offset = 0.2f;
		MaxWidthCloud = 200;
		MaxHeightCloud = 50;
		MaxDepthCloud = 200;
		FixedSize = true;
		IsAnimate = false;
		isFadeInOut = false;
		NumberOfShadows = TypeShadow.Some;
	}

	public void SetPresetDark()
	{
		CloudPreset = TypePreset.Dark;
		CloudRender = TypeRender.Realistic;
		CloudDetail = TypeDetail.Normal;
		SetCloudDetailParams();
		TypeClouds = TypeCloud.Nimbus2;
		SoftClouds = false;
		SpreadDir = new Vector3(-1f, 0f, 0f);
		LengthSpread = 1f;
		NumberClouds = 100;
		DisappearMultiplier = 2f;
		MaximunVelocity = new Vector3(-10f, 0f, 0f);
		VelocityMultipier = 0.85f;
		PaintType = TypePaintDistr.Below;
		CloudColor = new Color(0.11f, 0.102f, 0.102f, 0.502f);
		MainColor = new Color(0.612f, 0.482f, 0.314f, 0.145f);
		SecondColor = new Color(0.09f, 0.102f, 0.063f, 1f);
		TintStrength = 80;
		offset = 0.8f;
		MaxWidthCloud = 200;
		MaxHeightCloud = 50;
		MaxDepthCloud = 200;
		FixedSize = false;
		IsAnimate = false;
		isFadeInOut = false;
		NumberOfShadows = TypeShadow.None;
	}

	public void SetPresetCotton()
	{
		CloudPreset = TypePreset.Cotton;
		CloudRender = TypeRender.Realistic;
		CloudDetail = TypeDetail.Normal;
		SetCloudDetailParams();
		TypeClouds = TypeCloud.Nimbus2;
		SoftClouds = false;
		SpreadDir = new Vector3(-1f, 0f, 0f);
		LengthSpread = 1f;
		NumberClouds = 32;
		DisappearMultiplier = 2f;
		MaximunVelocity = new Vector3(-10f, 0f, 0f);
		VelocityMultipier = 0.85f;
		PaintType = TypePaintDistr.Below;
		CloudColor = new Color(0.902f, 0.902f, 0.902f, 0.502f);
		MainColor = new Color(0.796f, 0.796f, 0.796f, 0.302f);
		SecondColor = new Color(0.514f, 0.514f, 0.514f, 1f);
		TintStrength = 80;
		offset = 0.8f;
		MaxWidthCloud = 283;
		MaxHeightCloud = 97;
		MaxDepthCloud = 278;
		FixedSize = false;
		IsAnimate = true;
		AnimationVelocity = 0.1f;
		isFadeInOut = true;
		fadeRatio = 20f;
		alphaRange = 0.3f;
		NumberOfShadows = TypeShadow.None;
	}

	public void SetPresetCottonLight()
	{
		CloudPreset = TypePreset.CottonLight;
		CloudRender = TypeRender.Bright;
		CloudDetail = TypeDetail.Low;
		SetCloudDetailParams();
		TypeClouds = TypeCloud.Cirrus1;
		SoftClouds = false;
		SpreadDir = new Vector3(-1f, 0f, 0f);
		LengthSpread = 1f;
		NumberClouds = 185;
		DisappearMultiplier = 2f;
		MaximunVelocity = new Vector3(-10f, 0f, 0f);
		VelocityMultipier = 0.5f;
		PaintType = TypePaintDistr.Random;
		CloudColor = new Color(0.149f, 0.149f, 0.149f, 0.671f);
		MainColor = new Color(1f, 0.62f, 0f, 0.627f);
		SecondColor = new Color(0.502f, 0.502f, 0.502f, 0.839f);
		TintStrength = 50;
		offset = 0.8f;
		MaxWidthCloud = 206;
		MaxHeightCloud = 63;
		MaxDepthCloud = 225;
		FixedSize = false;
		IsAnimate = true;
		AnimationVelocity = 0.1f;
		isFadeInOut = true;
		fadeRatio = 30f;
		alphaRange = 0.2f;
		NumberOfShadows = TypeShadow.None;
	}

	public void SetPresetOrangeOil()
	{
		CloudPreset = TypePreset.OrangeOil;
		CloudRender = TypeRender.Realistic;
		CloudDetail = TypeDetail.Normal;
		SetCloudDetailParams();
		TypeClouds = TypeCloud.Cirrus2;
		SoftClouds = false;
		SpreadDir = new Vector3(-1f, 0f, 0f);
		LengthSpread = 1f;
		NumberClouds = 100;
		DisappearMultiplier = 2f;
		MaximunVelocity = new Vector3(-10f, 0f, 0f);
		VelocityMultipier = 0.85f;
		PaintType = TypePaintDistr.Below;
		CloudColor = new Color(0.973f, 0.855f, 0.855f, 0.502f);
		MainColor = new Color(0.945f, 0.553f, 0.271f, 0.302f);
		SecondColor = new Color(0.31f, 0.31f, 0.31f, 1f);
		TintStrength = 80;
		offset = 0.8f;
		MaxWidthCloud = 200;
		MaxHeightCloud = 50;
		MaxDepthCloud = 200;
		FixedSize = false;
		IsAnimate = false;
		isFadeInOut = false;
		NumberOfShadows = TypeShadow.None;
	}

	private Color ConvertColor(int r, int g, int b, int a)
	{
		Color color = new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, (float)a / 255f);
		Debug.Log("nResult" + color);
		return color;
	}

	public void SetCloudDetailParams()
	{
		if (CloudDetailPrev != CloudDetail)
		{
			if (CloudDetail == TypeDetail.Low)
			{
				EmissionMult = 1f;
				SizeFactorPart = 1f;
			}
			else if (CloudDetail == TypeDetail.Normal)
			{
				EmissionMult = 1.5f;
				SizeFactorPart = 1.2f;
			}
			else if (CloudDetail == TypeDetail.High)
			{
				EmissionMult = 2f;
				SizeFactorPart = 1.3f;
			}
			CloudDetailPrev = CloudDetail;
		}
	}

	public void EditorRepaintClouds()
	{
		if (MyCloudsParticles.Count == 0)
		{
			return;
		}
		for (int i = 0; i < MaximunClouds; i++)
		{
			CloudParticle cloudParticle = (CloudParticle)MyCloudsParticles[i];
			if (cloudParticle.IsActive())
			{
				AssignCloudTypeProperty(i, cloudParticle);
				cloudParticle.UpdateCloudsPosition();
				cloudParticle.SetMainColor(CloudColor);
				PaintTheParticlesShadows(cloudParticle);
				cloudParticle.SetCloudAlphaAnimation(fadeRatio, alphaRange, isFadeInOut);
			}
		}
	}

	private void CloudDetailChanged()
	{
		SetCloudDetailParams();
		for (int i = 0; i < NumberClouds; i++)
		{
			CloudParticle cloudParticle = (CloudParticle)MyCloudsParticles[i];
			cloudParticle.SetCloudEmitter(i, SpreadDir, SoftClouds, SizeFactorPart, EmissionMult, MaximunVelocity, VelocityMultipier);
			CloudActivateAndPositionUpdater(cloudParticle);
			cloudParticle.SetMainColor(CloudColor);
			PaintTheParticlesShadows(cloudParticle);
		}
	}

	private void CloudNumberChanged()
	{
		for (int i = 0; i < MaximunClouds; i++)
		{
			CloudParticle cloudParticle = (CloudParticle)MyCloudsParticles[i];
			if (i < NumberClouds && !cloudParticle.IsActive())
			{
				CloudActivateAndPositionUpdater(cloudParticle);
				if (SoftClouds)
				{
					SoftCloudsPrev = !SoftClouds;
				}
			}
			else if (i >= NumberClouds && cloudParticle.IsActive())
			{
				cloudParticle.DesactivateRecursively();
			}
		}
		NumberCloudsPrev = NumberClouds;
	}

	private void CloudActivateAndPositionUpdater(CloudParticle _cloudParticle)
	{
		_cloudParticle.SetActive(true);
		_cloudParticle.UpdateCloudsPosition();
	}

	private void UpdateAllCloudsMainColor()
	{
		for (int i = 0; i < NumberClouds; i++)
		{
			((CloudParticle)MyCloudsParticles[i]).SetMainColor(CloudColor);
			((CloudParticle)MyCloudsParticles[i]).SetNewCloudColor(CloudColor);
		}
	}

	private void UpdateAllCloudsFadeAnimators()
	{
		for (int i = 0; i < NumberClouds; i++)
		{
			((CloudParticle)MyCloudsParticles[i]).SetCloudAlphaAnimation(fadeRatio, alphaRange, isFadeInOut);
		}
	}

	private void RepaintAllClouds()
	{
		for (int i = 0; i < NumberClouds; i++)
		{
			PaintTheParticlesShadows((CloudParticle)MyCloudsParticles[i]);
		}
	}

	private void PaintTheParticlesShadows(CloudParticle MyCloudParticle)
	{
		if (PaintType == TypePaintDistr.Random)
		{
			MyCloudParticle.PaintParticlesBelow(MainColor, SecondColor, TintStrength, offset, 0);
		}
		else if (PaintType == TypePaintDistr.Below)
		{
			MyCloudParticle.PaintParticlesBelow(MainColor, SecondColor, TintStrength, offset, 1);
		}
	}

	private void ChangeCloudsVelocity()
	{
		if (MaximunVelocity.x > 0f)
		{
			CloudsGenerateAxis = Axis.X;
		}
		else if (MaximunVelocity.x < 0f)
		{
			CloudsGenerateAxis = Axis.XNeg;
		}
		else if (MaximunVelocity.y > 0f)
		{
			CloudsGenerateAxis = Axis.Y;
		}
		else if (MaximunVelocity.y < 0f)
		{
			CloudsGenerateAxis = Axis.YNeg;
		}
		else if (MaximunVelocity.z > 0f)
		{
			CloudsGenerateAxis = Axis.Z;
		}
		else if (MaximunVelocity.z < 0f)
		{
			CloudsGenerateAxis = Axis.ZNeg;
		}
	}

	private void SetAllCloudsVelocity()
	{
		for (int i = 0; i < MaximunClouds; i++)
		{
			((CloudParticle)MyCloudsParticles[i]).SetCloudVelocity(MaximunVelocity, VelocityMultipier);
		}
	}

	private void AssignCloudTypeProperty(int num, CloudParticle MyCloudParticle)
	{
		if (TypeClouds == TypeCloud.Nimbus1 || TypeClouds == TypeCloud.Nimbus2 || TypeClouds == TypeCloud.Nimbus3 || TypeClouds == TypeCloud.Nimbus4 || TypeClouds == TypeCloud.MixNimbus || TypeClouds == TypeCloud.MixAll || TypeClouds == TypeCloud.PT1)
		{
			MyCloudParticle.DefineCloudProperties(num, MaxWidthCloud, MaxHeightCloud, MaxDepthCloud, 0, FixedSize, true, true);
		}
		else if (TypeClouds == TypeCloud.Cirrus1 || TypeClouds == TypeCloud.Cirrus2 || TypeClouds == TypeCloud.MixCirrus)
		{
			MyCloudParticle.DefineCloudProperties(num, MaxWidthCloud, MaxHeightCloud, MaxDepthCloud, 1, FixedSize, true, true);
		}
	}

	private void ChangeAllCloudsSize()
	{
		for (int i = 0; i < NumberClouds; i++)
		{
			CloudParticle cloudParticle = (CloudParticle)MyCloudsParticles[i];
			AssignCloudTypeProperty(i, cloudParticle);
			cloudParticle.SetCloudEmitter(i, SpreadDir, SoftClouds, SizeFactorPart, EmissionMult, MaximunVelocity, VelocityMultipier);
			CloudActivateAndPositionUpdater(cloudParticle);
			cloudParticle.SetMainColor(CloudColor);
			PaintTheParticlesShadows(cloudParticle);
		}
	}

	private void SetSoftCloudsProperties()
	{
		for (int i = 0; i < NumberClouds; i++)
		{
			CloudParticle cloudParticle = (CloudParticle)MyCloudsParticles[i];
			if (LengthSpreadPrev != LengthSpread)
			{
				cloudParticle.SetLengthScale(LengthSpread);
			}
			else if (SpreadDirPrev != SpreadDir)
			{
				cloudParticle.SetWorldVelocity(SpreadDir);
				CloudActivateAndPositionUpdater(cloudParticle);
			}
			cloudParticle.SetCloudVelocity(MaximunVelocity, VelocityMultipier);
		}
	}

	private void SetSoftClouds(bool areSoftClouds)
	{
		for (int i = 0; i < NumberClouds; i++)
		{
			CloudParticle cloudParticle = (CloudParticle)MyCloudsParticles[i];
			cloudParticle.SetCloudEmitter(i, SpreadDir, areSoftClouds, SizeFactorPart, EmissionMult, MaximunVelocity, VelocityMultipier);
			cloudParticle.SoftCloud(areSoftClouds);
			CloudActivateAndPositionUpdater(cloudParticle);
			cloudParticle.SetWorldVelocity(MaximunVelocity);
		}
	}

	private void SetCloudsEmission()
	{
		for (int i = 0; i < NumberClouds; i++)
		{
			CloudParticle cloudParticle = (CloudParticle)MyCloudsParticles[i];
			cloudParticle.SetCloudEmitter(i, SpreadDir, SoftClouds, SizeFactorPart, EmissionMult, MaximunVelocity, VelocityMultipier);
			CloudActivateAndPositionUpdater(cloudParticle);
		}
	}

	private void Start()
	{
		MyTransform = base.transform;
		MyPosition = MyTransform.position;
		CloudRenderPrev = CloudRender;
		CloudDetailPrev = CloudDetail;
		TypeCloudsPrev = TypeClouds;
		EmissionMultPrev = EmissionMult;
		SizeFactorPartPrev = SizeFactorPart;
		SoftCloudsPrev = SoftClouds;
		SpreadDirPrev = SpreadDir;
		LengthSpreadPrev = LengthSpread;
		NumberCloudsPrev = NumberClouds;
		MaximunVelocityPrev = MaximunVelocity;
		VelocityMultipierPrev = VelocityMultipier;
		PaintTypePrev = PaintType;
		CloudColorPrev = CloudColor;
		MainColorPrev = MainColor;
		SecondColorPrev = SecondColor;
		TintStrengthPrev = TintStrength;
		offsetPrev = offset;
		NumberOfShadowsPrev = NumberOfShadows;
		MaxWidthCloudPrev = MaxWidthCloud;
		MaxHeightCloudPrev = MaxHeightCloud;
		MaxDepthCloudPrev = MaxDepthCloud;
		IsAnimatePrev = IsAnimate;
		AnimationVelocityPrev = AnimationVelocity;
		isFadeInOutPrev = isFadeInOut;
		fadeRatioPrev = fadeRatio;
		alphaRangePrev = alphaRange;
		ChangeCloudsVelocity();
		if (TypeClouds == TypeCloud.PT1)
		{
			CreateProceduralTextureContainerObject();
		}
		for (int i = 0; i < 6; i++)
		{
			CloudsMatAdditive[i] = new Material(brightShader);
			CloudsMatAdditive[i].mainTexture = CloudsTextAdd[i];
		}
		for (int i = 0; i < 6; i++)
		{
			CloudsMatBlended[i] = new Material(realisticShader);
			CloudsMatBlended[i].SetColor("_TintColor", CloudColor);
			CloudsMatBlended[i].mainTexture = CloudsTextBlended[i];
		}
		if ((bool)ProceduralTexture && ProceduralTexture.IsInicialized() && !bAssignProcTexture)
		{
			AssignProceduralTextureMaterials();
			bAssignProcTexture = true;
		}
		Vector3 vector = Side * 0.5f;
		for (int i = 0; i < MaximunClouds; i++)
		{
			Vector3 myPosition = MyPosition;
			myPosition.x = Random.Range(myPosition.x - vector.x, myPosition.x + vector.x);
			myPosition.y = Random.Range(myPosition.y - vector.y, myPosition.y + vector.y);
			myPosition.z = Random.Range(myPosition.z - vector.z, myPosition.z + vector.z);
			CloudParticle cloudParticle = new CloudParticle(myPosition, Quaternion.identity);
			cloudParticle.SetCloudParent(MyTransform);
			cloudParticle.SetCloudShadowMaterial(projectorMaterial);
			MyCloudsParticles.Add(cloudParticle);
			AssignCloudTypeProperty(i, cloudParticle);
			AssignCloudMaterial(cloudParticle, CloudRender, TypeClouds);
			cloudParticle.SetCloudEmitter(i, SpreadDir, SoftClouds, SizeFactorPart, EmissionMult, MaximunVelocity, VelocityMultipier);
			cloudParticle.SetCloudVelocity(MaximunVelocity, VelocityMultipier);
			cloudParticle.SoftCloud(SoftClouds);
			cloudParticle.SetLengthScale(LengthSpread);
			cloudParticle.AnimateCloud(IsAnimate, AnimationVelocity);
			ManageCloudShadow(cloudParticle);
			if (i < NumberClouds)
			{
				if (TypeClouds != TypeCloud.PT1)
				{
					CloudActivateAndPositionUpdater(cloudParticle);
					cloudParticle.SetMainColor(CloudColor);
					cloudParticle.SetCloudAlphaAnimation(fadeRatio, alphaRange, isFadeInOut);
					PaintTheParticlesShadows(cloudParticle);
					cloudParticle.SetWorldVelocity(SpreadDir);
				}
			}
			else
			{
				cloudParticle.DesactivateRecursively();
			}
		}
		maximunCloudsOrig = MaximunClouds;
	}

	private void Update()
	{
		if (maximunCloudsOrig != MaximunClouds)
		{
			Debug.LogWarning("WARNING! Changing 'MaximunClouds' value in Runtime will generate fatal errors!!!!");
			maximunCloudsOrig = MaximunClouds;
		}
		if (TypeClouds == TypeCloud.PT1 && !ProceduralTexture)
		{
			CreateProceduralTextureContainerObject();
			AssignProceduralTextureMaterials();
		}
		if ((bool)ProceduralTexture && ProceduralTexture.IsInicialized() && !bAssignProcTexture)
		{
			AssignProceduralTexturesToClouds();
			bAssignProcTexture = true;
		}
		if (NumberCloudsPrev != NumberClouds)
		{
			NumberClouds = Mathf.Clamp(NumberClouds, 0, MaximunClouds);
			CloudNumberChanged();
		}
		if (CloudDetailPrev != CloudDetail)
		{
			CloudDetailChanged();
		}
		if (SizeFactorPartPrev != SizeFactorPart || EmissionMultPrev != EmissionMult)
		{
			SetCloudsEmission();
			SizeFactorPartPrev = SizeFactorPart;
			EmissionMultPrev = EmissionMult;
		}
		if (SoftCloudsPrev != SoftClouds)
		{
			SetSoftClouds(SoftClouds);
			SoftCloudsPrev = SoftClouds;
		}
		if (SpreadDirPrev != SpreadDir || LengthSpreadPrev != LengthSpread)
		{
			SetSoftCloudsProperties();
			SpreadDirPrev = SpreadDir;
			LengthSpreadPrev = LengthSpread;
		}
		if (MaxWidthCloud != MaxWidthCloudPrev || MaxHeightCloud != MaxHeightCloudPrev || MaxDepthCloud != MaxDepthCloudPrev)
		{
			ChangeAllCloudsSize();
			MaxWidthCloudPrev = MaxWidthCloud;
			MaxHeightCloudPrev = MaxHeightCloud;
			MaxDepthCloudPrev = MaxDepthCloud;
		}
		if (TypeCloudsPrev != TypeClouds || CloudRenderPrev != CloudRender)
		{
			AssignAllCloudsMaterial();
			TypeCloudsPrev = TypeClouds;
			CloudRenderPrev = CloudRender;
		}
		if (MaximunVelocityPrev != MaximunVelocity || VelocityMultipierPrev != VelocityMultipier)
		{
			ChangeCloudsVelocity();
			SetAllCloudsVelocity();
			MaximunVelocityPrev = MaximunVelocity;
			VelocityMultipierPrev = VelocityMultipier;
		}
		if (CloudColorPrev != CloudColor)
		{
			UpdateAllCloudsMainColor();
			UpdateAllCloudsFadeAnimators();
			CloudColorPrev = CloudColor;
		}
		if (MainColorPrev != MainColor)
		{
			RepaintAllClouds();
			MainColorPrev = MainColor;
		}
		if (SecondColorPrev != SecondColor || TintStrengthPrev != TintStrength)
		{
			RepaintAllClouds();
			SecondColorPrev = SecondColor;
			TintStrengthPrev = TintStrength;
		}
		if (offsetPrev != offset)
		{
			RepaintAllClouds();
			offsetPrev = offset;
		}
		if (PaintTypePrev != PaintType)
		{
			RepaintAllClouds();
			PaintTypePrev = PaintType;
		}
		if (NumberOfShadowsPrev != NumberOfShadows)
		{
			ManageAllCloudsShadows();
		}
		if (IsAnimatePrev != IsAnimate || AnimationVelocityPrev != AnimationVelocity)
		{
			UpdateAllCloudsAnimation();
			IsAnimatePrev = IsAnimate;
			AnimationVelocityPrev = AnimationVelocity;
		}
		if (isFadeInOutPrev != isFadeInOut || fadeRatioPrev != fadeRatio || alphaRangePrev != alphaRange)
		{
			UpdateAllCloudsFadeAnimators();
			isFadeInOutPrev = isFadeInOut;
			fadeRatioPrev = fadeRatio;
			alphaRangePrev = alphaRange;
		}
	}

	private void LateUpdate()
	{
		if (MyPosition != MyTransform.position)
		{
			MyPosition = MyTransform.position;
		}
		if (MaximunVelocity == Vector3.zero)
		{
			return;
		}
		if (positionCheckerTime > 0f)
		{
			internalLateUpdateTimer += Time.deltaTime;
			if (internalLateUpdateTimer <= positionCheckerTime)
			{
				return;
			}
			internalLateUpdateTimer = 0f;
		}
		bool flag = false;
		Vector3 vector = Side * DisappearMultiplier * 0.5f;
		for (int i = 0; i < MyCloudsParticles.Count; i++)
		{
			CloudParticle cloudParticle = (CloudParticle)MyCloudsParticles[i];
			Vector3 cloudPosition = cloudParticle.GetCloudPosition();
			if (cloudPosition.x < MyPosition.x - vector.x || cloudPosition.x > MyPosition.x + vector.x)
			{
				flag = true;
			}
			else if (cloudPosition.y < MyPosition.y - vector.y || cloudPosition.y > MyPosition.y + vector.y)
			{
				flag = true;
			}
			else if (cloudPosition.z < MyPosition.z - vector.z || cloudPosition.z > MyPosition.z + vector.z)
			{
				flag = true;
			}
			if (flag)
			{
				flag = false;
				cloudPosition = MyPosition;
				cloudPosition.x = Random.Range(cloudPosition.x - Side.x * 0.5f, cloudPosition.x + Side.x * 0.5f);
				cloudPosition.y = Random.Range(cloudPosition.y - Side.y * 0.5f, cloudPosition.y + Side.y * 0.5f);
				cloudPosition.z = Random.Range(cloudPosition.z - Side.z * 0.5f, cloudPosition.z + Side.z * 0.5f);
				switch (CloudsGenerateAxis)
				{
				case Axis.X:
					cloudPosition.x = MyPosition.x - vector.x;
					break;
				case Axis.XNeg:
					cloudPosition.x = MyPosition.x + vector.x;
					break;
				case Axis.Y:
					cloudPosition.y = MyPosition.y - vector.y;
					break;
				case Axis.YNeg:
					cloudPosition.y = MyPosition.y + vector.y;
					break;
				case Axis.Z:
					cloudPosition.z = MyPosition.z - vector.z;
					break;
				case Axis.ZNeg:
					cloudPosition.z = MyPosition.z + vector.z;
					break;
				}
				cloudParticle.SetCloudPosition(cloudPosition);
			}
		}
	}

	private void UpdateAllCloudsAnimation()
	{
		for (int i = 0; i < NumberClouds; i++)
		{
			((CloudParticle)MyCloudsParticles[i]).AnimateCloud(IsAnimate, AnimationVelocity);
		}
	}

	private void AssignAllCloudsMaterial()
	{
		for (int i = 0; i < MaximunClouds; i++)
		{
			AssignCloudMaterial((CloudParticle)MyCloudsParticles[i], CloudRender, TypeClouds);
		}
	}

	private void AssignCloudMaterial(CloudParticle MyCloudParticle, TypeRender CloudRender, TypeCloud TypeClouds)
	{
		ModifyPTMaterials();
		if (CloudRender == TypeRender.Bright)
		{
			switch (TypeClouds)
			{
			case TypeCloud.Nimbus1:
				MyCloudParticle.SetMaterial(CloudsMatAdditive[0]);
				break;
			case TypeCloud.Nimbus2:
				MyCloudParticle.SetMaterial(CloudsMatAdditive[1]);
				break;
			case TypeCloud.Nimbus3:
				MyCloudParticle.SetMaterial(CloudsMatAdditive[2]);
				break;
			case TypeCloud.Nimbus4:
				MyCloudParticle.SetMaterial(CloudsMatAdditive[3]);
				break;
			case TypeCloud.Cirrus1:
				MyCloudParticle.SetMaterial(CloudsMatAdditive[4]);
				break;
			case TypeCloud.Cirrus2:
				MyCloudParticle.SetMaterial(CloudsMatAdditive[5]);
				break;
			case TypeCloud.MixNimbus:
				MyCloudParticle.SetMaterial(CloudsMatAdditive[Random.Range(0, 4)]);
				break;
			case TypeCloud.MixCirrus:
				MyCloudParticle.SetMaterial(CloudsMatAdditive[Random.Range(4, 6)]);
				break;
			case TypeCloud.MixAll:
				MyCloudParticle.SetMaterial(CloudsMatAdditive[Random.Range(0, 6)]);
				break;
			case TypeCloud.PT1:
				MyCloudParticle.SetMaterial(CloudsPTMatAdditive);
				break;
			}
		}
		else
		{
			switch (TypeClouds)
			{
			case TypeCloud.Nimbus1:
				MyCloudParticle.SetMaterial(CloudsMatBlended[0]);
				break;
			case TypeCloud.Nimbus2:
				MyCloudParticle.SetMaterial(CloudsMatBlended[1]);
				break;
			case TypeCloud.Nimbus3:
				MyCloudParticle.SetMaterial(CloudsMatBlended[2]);
				break;
			case TypeCloud.Nimbus4:
				MyCloudParticle.SetMaterial(CloudsMatBlended[3]);
				break;
			case TypeCloud.Cirrus1:
				MyCloudParticle.SetMaterial(CloudsMatBlended[4]);
				break;
			case TypeCloud.Cirrus2:
				MyCloudParticle.SetMaterial(CloudsMatBlended[5]);
				break;
			case TypeCloud.MixNimbus:
				MyCloudParticle.SetMaterial(CloudsMatBlended[Random.Range(0, 4)]);
				break;
			case TypeCloud.MixCirrus:
				MyCloudParticle.SetMaterial(CloudsMatBlended[Random.Range(4, 6)]);
				break;
			case TypeCloud.MixAll:
				MyCloudParticle.SetMaterial(CloudsMatBlended[Random.Range(0, 6)]);
				break;
			case TypeCloud.PT1:
				MyCloudParticle.SetMaterial(CloudsPTMatBlended);
				break;
			}
		}
	}

	private void ManageAllCloudsShadows()
	{
		for (int i = 0; i < NumberClouds; i++)
		{
			ManageCloudShadow((CloudParticle)MyCloudsParticles[i]);
		}
		NumberOfShadowsPrev = NumberOfShadows;
	}

	private void ManageCloudShadow(CloudParticle MyCloudParticle)
	{
		int num = Random.Range(0, 10);
		bool flag = true;
		if (NumberOfShadows != 0)
		{
			if (NumberOfShadows == TypeShadow.Most && num > 7)
			{
				flag = false;
			}
			else if (NumberOfShadows == TypeShadow.Half && num > 5)
			{
				flag = false;
			}
			else if (NumberOfShadows == TypeShadow.Some && num <= 7)
			{
				flag = false;
			}
			else if (NumberOfShadows == TypeShadow.None)
			{
				flag = false;
			}
			if (!flag && MyCloudParticle.IsShadowActive())
			{
				MyCloudParticle.SetShadowActive(false);
			}
			else if (flag && !MyCloudParticle.IsShadowActive())
			{
				MyCloudParticle.SetShadowActive(true);
			}
		}
		else if (!MyCloudParticle.IsShadowActive())
		{
			MyCloudParticle.SetShadowActive(true);
		}
	}

	private void OnDestroy()
	{
		MyCloudsParticles.Clear();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(base.transform.position, Side);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(base.transform.position, Side * DisappearMultiplier);
	}

	private void CreateProceduralTextureContainerObject()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "CloudsToyPT1";
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.transform.parent = MyTransform;
		ProceduralTexture = gameObject.AddComponent<ProceduralCloudTexture>();
		PT1CopyInitialParameters();
	}

	private void AssignProceduralTextureMaterials()
	{
		CloudsPTMatAdditive = new Material(brightShader);
		CloudsPTMatAdditive.SetColor("_TintColor", CloudColor);
		if (ProceduralTexture.IsInicialized())
		{
			CloudsPTMatAdditive.mainTexture = ProceduralTexture.MyTexture;
		}
		CloudsPTMatBlended = new Material(realisticShader);
		CloudsPTMatBlended.SetColor("_TintColor", CloudColor);
		if (ProceduralTexture.IsInicialized())
		{
			CloudsPTMatBlended.mainTexture = ProceduralTexture.MyAlphaTexture;
		}
	}

	private void AssignProceduralTexturesToClouds()
	{
		CloudsPTMatAdditive.mainTexture = ProceduralTexture.MyTexture;
		CloudsPTMatBlended.SetColor("_TintColor", CloudColor);
		CloudsPTMatBlended.mainTexture = ProceduralTexture.MyAlphaTexture;
		if (TypeClouds != TypeCloud.PT1)
		{
			return;
		}
		for (int i = 0; i < MaximunClouds; i++)
		{
			if (i < NumberClouds)
			{
				CloudParticle cloudParticle = (CloudParticle)MyCloudsParticles[i];
				cloudParticle.SetActive(true);
				cloudParticle.UpdateCloudsPosition();
				cloudParticle.SetMainColor(CloudColor);
				PaintTheParticlesShadows(cloudParticle);
			}
		}
	}

	public void PT1CopyInitialParameters()
	{
		ProceduralTexture.TextureWidth = PT1TextureWidth;
		ProceduralTexture.TextureHeight = PT1TextureHeight;
		ProceduralTexture.ScaleWidth = PT1ScaleWidth;
		ProceduralTexture.ScaleHeight = PT1ScaleHeight;
		ProceduralTexture.ScaleFactor = PT1ScaleFactor;
		ProceduralTexture.Seed = PT1Seed;
		ProceduralTexture.Lacunarity = PT1Lacunarity;
		ProceduralTexture.FractalIncrement = PT1FractalIncrement;
		ProceduralTexture.Octaves = PT1Octaves;
		ProceduralTexture.Offset = PT1Offset;
		ProceduralTexture.InvertColors = PT1InvertColors;
		ProceduralTexture.ContrastMult = PT1ContrastMult;
		ProceduralTexture.TypeNoise = (ProceduralCloudTexture.NoisePreset)PT1TypeNoise;
		ProceduralTexture.TurbSize = PT1TurbSize;
		ProceduralTexture.TurbLacun = PT1TurbLacun;
		ProceduralTexture.TurbGain = PT1TurbGain;
		ProceduralTexture.turbPower = PT1turbPower;
		ProceduralTexture.xyPeriod = PT1xyPeriod;
		ProceduralTexture.HaloEffect = PT1HaloEffect;
		ProceduralTexture.HaloInsideRadius = PT1HaloInsideRadius;
		ProceduralTexture.UseAlphaTexture = PT1UseAlphaTexture;
		ProceduralTexture.AlphaIndex = PT1AlphaIndex;
		ProceduralTexture.HasChanged = true;
	}

	public void ResetCloudParameters()
	{
		if (PT1TypeNoise == NoisePresetPT1.PerlinCloud)
		{
			PT1ScaleWidth = 1f;
			PT1ScaleHeight = 1f;
			PT1HaloInsideRadius = 1.7f;
		}
		else
		{
			PT1ScaleWidth = 50f;
			PT1ScaleHeight = 50f;
			PT1HaloInsideRadius = 0.1f;
		}
		PT1ScaleFactor = 1f;
		PT1Seed = 132;
		PT1Lacunarity = 3f;
		PT1FractalIncrement = 0.5f;
		PT1Octaves = 7f;
		PT1Offset = 1f;
		PT1InvertColors = false;
		PT1ContrastMult = 0f;
		PT1TurbSize = 16;
		PT1TurbLacun = 0.01f;
		PT1TurbGain = 0.5f;
		PT1turbPower = 5f;
		PT1xyPeriod = 0.6f;
		PT1HaloEffect = 1.7f;
		PT1UseAlphaTexture = true;
		PT1AlphaIndex = 0.1f;
	}

	public void PT1CreateNewTexture()
	{
		ProceduralTexture.CreateNewTexture();
	}

	public void PT1NewRandomSeed()
	{
		ProceduralTexture.Seed = PT1Seed;
		ProceduralTexture.NewRandomSeed();
	}

	public void PT1CopyParameters()
	{
		ProceduralTexture.ScaleWidth = PT1ScaleWidth;
		ProceduralTexture.ScaleHeight = PT1ScaleHeight;
		ProceduralTexture.ScaleFactor = PT1ScaleFactor;
		ProceduralTexture.Seed = PT1Seed;
		ProceduralTexture.Lacunarity = PT1Lacunarity;
		ProceduralTexture.FractalIncrement = PT1FractalIncrement;
		ProceduralTexture.Octaves = PT1Octaves;
		ProceduralTexture.Offset = PT1Offset;
		ProceduralTexture.InvertColors = PT1InvertColors;
		ProceduralTexture.ContrastMult = PT1ContrastMult;
		ProceduralTexture.TypeNoise = (ProceduralCloudTexture.NoisePreset)PT1TypeNoise;
		ProceduralTexture.TurbSize = PT1TurbSize;
		ProceduralTexture.TurbLacun = PT1TurbLacun;
		ProceduralTexture.TurbGain = PT1TurbGain;
		ProceduralTexture.turbPower = PT1turbPower;
		ProceduralTexture.xyPeriod = PT1xyPeriod;
		ProceduralTexture.HaloEffect = PT1HaloEffect;
		ProceduralTexture.HaloInsideRadius = PT1HaloInsideRadius;
		ProceduralTexture.UseAlphaTexture = PT1UseAlphaTexture;
		ProceduralTexture.AlphaIndex = PT1AlphaIndex;
		ProceduralTexture.HasChanged = true;
	}

	public void ModifyPTMaterials()
	{
		if ((bool)ProceduralTexture && ProceduralTexture.IsInicialized())
		{
			CloudsPTMatAdditive.mainTexture = ProceduralTexture.MyTexture;
			CloudsPTMatBlended.SetColor("_TintColor", CloudColor);
			CloudsPTMatBlended.mainTexture = ProceduralTexture.MyAlphaTexture;
		}
	}

	public void PrintPT1Paramaters()
	{
		CTDebug("PT1TextureWidth : " + PT1TextureWidth);
		CTDebug("PT1TextureHeight : " + PT1TextureHeight);
		CTDebug("PT1Seed : " + PT1Seed);
		CTDebug("PT1ScaleWidth : " + PT1ScaleWidth);
		CTDebug("PT1ScaleHeight : " + PT1ScaleHeight);
		CTDebug("PT1ScaleFactor : " + PT1ScaleFactor);
		CTDebug("PT1Lacunarity : " + PT1Lacunarity);
		CTDebug("PT1FractalIncrement : " + PT1FractalIncrement);
		CTDebug("PT1Octaves : " + PT1Octaves);
		CTDebug("PT1Offset : " + PT1Offset);
		CTDebug("PT1HaloEffect : " + PT1HaloEffect);
		CTDebug("PT1HaloInsideRadius : " + PT1HaloInsideRadius);
		CTDebug("PT1InvertColors : " + PT1InvertColors);
		CTDebug("PT1ContrastMult : " + PT1ContrastMult);
		CTDebug("PT1UseAlphaTexture : " + PT1UseAlphaTexture);
		CTDebug("PT1AlphaIndex : " + PT1AlphaIndex);
	}

	public void SaveProceduralTexture()
	{
		int num = 0;
		string path;
		do
		{
			num++;
			path = "Assets/Volumetric Clouds/Textures/Procedural/PTAdd" + num + ".png";
			string text = "Assets/Volumetric Clouds/Textures/Procedural/PTBlended" + num + ".png";
		}
		while (File.Exists(path));
	}
}
