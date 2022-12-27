using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace Wilberforce.VAO
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[HelpURL("https://projectwilberforce.github.io/vaomanual/")]
	[AddComponentMenu("Image Effects/Rendering/Volumetric Ambient Occlusion")]
	public class VAOEffect : MonoBehaviour
	{
		public enum EffectMode
		{
			Simple = 1,
			ColorTint = 2,
			ColorBleed = 3
		}

		public enum LuminanceModeType
		{
			Luma = 1,
			HSVValue = 2
		}

		public enum GiBlurAmmount
		{
			Auto = 1,
			Less = 2,
			More = 3
		}

		public enum CullingPrepassModeType
		{
			Off = 0,
			Greedy = 1,
			Careful = 2
		}

		public enum AdaptiveSamplingType
		{
			Disabled = 0,
			EnabledAutomatic = 1,
			EnabledManual = 2
		}

		public enum BlurModeType
		{
			Disabled = 0,
			Basic = 1,
			Enhanced = 2
		}

		public enum ColorBleedSelfOcclusionFixLevelType
		{
			Off = 0,
			Soft = 1,
			Hard = 2
		}

		public enum ScreenTextureFormat
		{
			Auto = 0,
			ARGB32 = 1,
			ARGBHalf = 2,
			ARGBFloat = 3,
			Default = 4,
			DefaultHDR = 5
		}

		public enum FarPlaneSourceType
		{
			ProjectionParams = 0,
			Camera = 1
		}

		public enum DistanceFalloffModeType
		{
			Off = 0,
			Absolute = 1,
			Relative = 2
		}

		public enum VAOCameraEventType
		{
			AfterLighting = 0,
			BeforeReflections = 1,
			BeforeImageEffectsOpaque = 2,
			BeforeLighting = 3,
			AfterForwardAlpha = 4
		}

		private enum ShaderPass
		{
			BasicNoise = 0,
			BasicNoiseColorBleed = 1,
			BasicNoiseGBuffer = 2,
			BasicNoiseColorBleedGBuffer = 3,
			BasicNoisecullingPrepassType = 4,
			BasicNoiseColorBleedcullingPrepassType = 5,
			BasicNoiseGBuffercullingPrepassType = 6,
			BasicNoiseColorBleedGBuffercullingPrepassType = 7,
			StandardBlurUniform = 8,
			EnhancedBlurFirst = 9,
			EnhancedBlurSecond = 10,
			StandardBlurUniformColorbleed = 11,
			EnhancedBlurFirstColorbleed = 12,
			EnhancedBlurSecondColorbleed = 13,
			StandardBlurUniformGBuffer = 14,
			EnhancedBlurFirstGBuffer = 15,
			EnhancedBlurSecondGBuffer = 16,
			StandardBlurUniformColorbleedGBuffer = 17,
			EnhancedBlurFirstColorbleedGBuffer = 18,
			EnhancedBlurSecondColorbleedGBuffer = 19,
			MixingBasic = 20,
			MixingColorbleed = 21,
			MixingAOOnly = 22,
			YAxisCorrectCopy = 23
		}

		public Shader vaoShader;

		public float Radius = 0.5f;

		public float Power = 0.7f;

		public float Presence = 0.1f;

		public float ColorBleedPresence = 1f;

		public bool ColorbleedHueSuppresionEnabled;

		public float ColorBleedHueSuppresionWidth = 2f;

		public float ColorBleedHueSuppresionThreshold = 7f;

		public float ColorBleedHueSuppresionSaturationThreshold = 0.5f;

		public float ColorBleedHueSuppresionSaturationWidth = 0.2f;

		public float ColorBleedHueSuppresionBrightness;

		public bool MaxRadiusEnabled = true;

		public float MaxRadius = 1f;

		public DistanceFalloffModeType DistanceFalloffMode;

		public float DistanceFalloffStartAbsolute = 100f;

		public float DistanceFalloffStartRelative = 0.1f;

		public float DistanceFalloffSpeedAbsolute = 30f;

		public float DistanceFalloffSpeedRelative = 0.1f;

		public int Quality = 16;

		public bool OutputAOOnly;

		public BlurModeType BlurMode = BlurModeType.Enhanced;

		public int EnhancedBlurSize = 5;

		public float EnhancedBlurDeviation = 1f;

		public ColorBleedSelfOcclusionFixLevelType ColorBleedSelfOcclusionFixLevel = ColorBleedSelfOcclusionFixLevelType.Hard;

		public AdaptiveSamplingType AdaptiveType = AdaptiveSamplingType.EnabledAutomatic;

		private float AdaptiveQuality = 0.2f;

		public float AdaptiveQualityCoefficient = 1f;

		private float AdaptiveMin;

		private float AdaptiveMax = -10f;

		public CullingPrepassModeType CullingPrepassMode = CullingPrepassModeType.Careful;

		private int DoomFactor = 8;

		public bool IsLumaSensitive;

		public float LumaThreshold = 0.7f;

		public float LumaKneeLinearity = 3f;

		public float LumaKneeWidth = 0.3f;

		public EffectMode Mode = EffectMode.Simple;

		public LuminanceModeType LuminanceMode = LuminanceModeType.Luma;

		public Color ColorTint = Color.black;

		public FarPlaneSourceType FarPlaneSource = FarPlaneSourceType.Camera;

		public float ColorBleedPower = 5f;

		public int ColorBleedQuality = 2;

		public bool GiBackfaces;

		public int Downsampling = 1;

		private const int noiseSize = 3;

		private bool isSupported;

		private Material VAOMaterial;

		public bool CommandBufferEnabled;

		public ScreenTextureFormat IntermediateScreenTextureFormat;

		public bool UseGBuffer;

		public bool ForcedSwitchPerformed;

		public VAOCameraEventType VaoCameraEvent = VAOCameraEventType.BeforeImageEffectsOpaque;

		private Dictionary<CameraEvent, CommandBuffer> cameraEventsRegistered = new Dictionary<CameraEvent, CommandBuffer>();

		private bool isCommandBufferAlive;

		private Vector4[] gaussian;

		private float gaussianWeight;

		private float lastDeviation = 0.5f;

		private Camera myCamera;

		private Vector4[] samplesLarge = new Vector4[70];

		private int lastSamplesLength;

		private int lastDownsampling;

		private CullingPrepassModeType lastcullingPrepassType;

		private int lastDoomFactor;

		private BlurModeType lastBlurMode;

		private bool lastEnhancedBlur;

		private EffectMode lastMode;

		private bool lastUseGBuffer;

		private AdaptiveSamplingType lastAdaptiveType;

		private bool lastOutputAOOnly;

		private LuminanceModeType lastLuminanceMode;

		private bool lastIsLumaSensitive;

		private CameraEvent lastCameraEvent;

		private bool lastIsHDR;

		private bool isHDR;

		private ScreenTextureFormat lastIntermediateScreenTextureFormat;

		private Vector4[] adaptiveSamples;

		private Vector4[] carefulCache;

		private static Vector4[] samp2 = new Vector4[2]
		{
			new Vector4(0.4392292f, 0.0127914f, 0.898284f),
			new Vector4(-0.894406f, -0.162116f, 0.41684f)
		};

		private static Vector4[] samp4 = new Vector4[4]
		{
			new Vector4(-0.07984404f, -0.2016976f, 0.976188f),
			new Vector4(0.4685118f, -0.8404996f, 0.272135f),
			new Vector4(-0.793633f, 0.293059f, 0.533164f),
			new Vector4(0.2998218f, 0.4641494f, 0.83347f)
		};

		private static Vector4[] samp8 = new Vector4[8]
		{
			new Vector4(-0.4999112f, -0.571184f, 0.651028f),
			new Vector4(0.2267525f, -0.668142f, 0.708639f),
			new Vector4(0.0657284f, -0.123769f, 0.990132f),
			new Vector4(0.9259827f, -0.2030669f, 0.318307f),
			new Vector4(-0.9850165f, 0.1247843f, 0.119042f),
			new Vector4(-0.2988613f, 0.2567392f, 0.919112f),
			new Vector4(0.4734727f, 0.2830991f, 0.834073f),
			new Vector4(0.1319883f, 0.9544416f, 0.267621f)
		};

		private static Vector4[] samp16 = new Vector4[16]
		{
			new Vector4(-0.6870962f, -0.7179669f, 0.111458f),
			new Vector4(-0.2574025f, -0.6144419f, 0.745791f),
			new Vector4(-0.408366f, -0.162244f, 0.898284f),
			new Vector4(-0.07098053f, 0.02052395f, 0.997267f),
			new Vector4(0.2019972f, -0.760972f, 0.616538f),
			new Vector4(0.706282f, -0.6368136f, 0.309248f),
			new Vector4(0.169605f, -0.2892981f, 0.942094f),
			new Vector4(0.7644456f, -0.05826119f, 0.64205f),
			new Vector4(-0.745912f, 0.0501786f, 0.664152f),
			new Vector4(-0.7588732f, 0.4313389f, 0.487911f),
			new Vector4(-0.3806622f, 0.3446409f, 0.85809f),
			new Vector4(-0.1296651f, 0.8794711f, 0.45795f),
			new Vector4(0.1557318f, 0.137468f, 0.978187f),
			new Vector4(0.5990864f, 0.2485375f, 0.761133f),
			new Vector4(0.1727637f, 0.5753375f, 0.799462f),
			new Vector4(0.5883294f, 0.7348878f, 0.337355f)
		};

		private static Vector4[] samp32 = new Vector4[32]
		{
			new Vector4(-0.626056f, -0.7776781f, 0.0571977f),
			new Vector4(-0.1335098f, -0.9164876f, 0.377127f),
			new Vector4(-0.2668636f, -0.5663173f, 0.779787f),
			new Vector4(-0.5712572f, -0.4639561f, 0.67706f),
			new Vector4(-0.6571807f, -0.2969118f, 0.692789f),
			new Vector4(-0.8896923f, -0.1314662f, 0.437223f),
			new Vector4(-0.5037534f, -0.03057539f, 0.863306f),
			new Vector4(-0.1773856f, -0.2664998f, 0.947371f),
			new Vector4(-0.02786797f, -0.02453661f, 0.99931f),
			new Vector4(0.173095f, -0.964425f, 0.199805f),
			new Vector4(0.280491f, -0.716259f, 0.638982f),
			new Vector4(0.7610048f, -0.4987299f, 0.414898f),
			new Vector4(0.135136f, -0.388973f, 0.911284f),
			new Vector4(0.4836829f, -0.4782286f, 0.73304f),
			new Vector4(0.1905736f, -0.1039435f, 0.976154f),
			new Vector4(0.4855643f, 0.01388972f, 0.87409f),
			new Vector4(0.5684234f, -0.2864941f, 0.771243f),
			new Vector4(0.8165832f, 0.01384446f, 0.577062f),
			new Vector4(-0.9814694f, 0.18555f, 0.0478435f),
			new Vector4(-0.5357604f, 0.3316899f, 0.776494f),
			new Vector4(-0.1238877f, 0.03315933f, 0.991742f),
			new Vector4(-0.1610546f, 0.3801286f, 0.910804f),
			new Vector4(-0.5923722f, 0.628729f, 0.503781f),
			new Vector4(-0.05504921f, 0.5483891f, 0.834409f),
			new Vector4(-0.3805041f, 0.8377199f, 0.391717f),
			new Vector4(-0.101651f, 0.9530866f, 0.285119f),
			new Vector4(0.1613653f, 0.2561041f, 0.953085f),
			new Vector4(0.4533991f, 0.2896196f, 0.842941f),
			new Vector4(0.6665574f, 0.4639243f, 0.583503f),
			new Vector4(0.8873722f, 0.4278904f, 0.1717f),
			new Vector4(0.2869751f, 0.732805f, 0.616962f),
			new Vector4(0.4188429f, 0.7185978f, 0.555147f)
		};

		private static Vector4[] samp64 = new Vector4[64]
		{
			new Vector4(-0.6700248f, -0.6370129f, 0.381157f),
			new Vector4(-0.7385408f, -0.6073685f, 0.292679f),
			new Vector4(-0.4108568f, -0.8852778f, 0.2179f),
			new Vector4(-0.3058583f, -0.8047022f, 0.508828f),
			new Vector4(0.01087609f, -0.7610992f, 0.648545f),
			new Vector4(-0.3629634f, -0.5480431f, 0.753595f),
			new Vector4(-0.1480379f, -0.6927805f, 0.70579f),
			new Vector4(-0.9533184f, -0.276674f, 0.12098f),
			new Vector4(-0.6387863f, -0.3999016f, 0.65729f),
			new Vector4(-0.891588f, -0.115146f, 0.437964f),
			new Vector4(-0.775663f, 0.0194654f, 0.630848f),
			new Vector4(-0.5360528f, -0.1828935f, 0.824134f),
			new Vector4(-0.513927f, -0.000130296f, 0.857834f),
			new Vector4(-0.4368436f, -0.2831443f, 0.853813f),
			new Vector4(-0.1794069f, -0.4226944f, 0.888337f),
			new Vector4(-0.00183062f, -0.4371257f, 0.899398f),
			new Vector4(-0.2598701f, -0.1719497f, 0.950211f),
			new Vector4(-0.08650014f, -0.004176182f, 0.996243f),
			new Vector4(0.006921067f, -0.001478712f, 0.999975f),
			new Vector4(0.05654667f, -0.9351676f, 0.349662f),
			new Vector4(0.1168661f, -0.754741f, 0.64553f),
			new Vector4(0.3534952f, -0.7472929f, 0.562667f),
			new Vector4(0.1635596f, -0.5863093f, 0.793404f),
			new Vector4(0.5910167f, -0.786864f, 0.177609f),
			new Vector4(0.5820105f, -0.5659724f, 0.5839f),
			new Vector4(0.7254612f, -0.5323696f, 0.436221f),
			new Vector4(0.4016336f, -0.4329237f, 0.807012f),
			new Vector4(0.5287027f, -0.4064075f, 0.745188f),
			new Vector4(0.314015f, -0.2375291f, 0.919225f),
			new Vector4(0.02922117f, -0.2097672f, 0.977315f),
			new Vector4(0.4201531f, -0.1445212f, 0.895871f),
			new Vector4(0.2821195f, -0.01079273f, 0.959319f),
			new Vector4(0.7152653f, -0.1972963f, 0.670425f),
			new Vector4(0.8167331f, -0.1217311f, 0.564029f),
			new Vector4(0.8517836f, 0.01290532f, 0.523735f),
			new Vector4(-0.657816f, 0.134013f, 0.74116f),
			new Vector4(-0.851676f, 0.321285f, 0.414033f),
			new Vector4(-0.603183f, 0.361627f, 0.710912f),
			new Vector4(-0.6607267f, 0.5282444f, 0.533289f),
			new Vector4(-0.323619f, 0.182656f, 0.92839f),
			new Vector4(-0.2080927f, 0.1494067f, 0.966631f),
			new Vector4(-0.4205947f, 0.4184987f, 0.804959f),
			new Vector4(-0.06831062f, 0.3712724f, 0.926008f),
			new Vector4(-0.165943f, 0.5029928f, 0.84821f),
			new Vector4(-0.6137413f, 0.7001954f, 0.364758f),
			new Vector4(-0.3009551f, 0.6550035f, 0.693107f),
			new Vector4(-0.1356791f, 0.6460465f, 0.751143f),
			new Vector4(-0.3677429f, 0.7920387f, 0.487278f),
			new Vector4(-0.08688695f, 0.9677781f, 0.236338f),
			new Vector4(0.07250954f, 0.1327261f, 0.988497f),
			new Vector4(0.5244588f, 0.05565827f, 0.849615f),
			new Vector4(0.2498424f, 0.3364912f, 0.907938f),
			new Vector4(0.2608168f, 0.5340923f, 0.804189f),
			new Vector4(0.3888291f, 0.3207975f, 0.863655f),
			new Vector4(0.6413552f, 0.1619097f, 0.749966f),
			new Vector4(0.8523082f, 0.2647078f, 0.451111f),
			new Vector4(0.5591328f, 0.3038472f, 0.771393f),
			new Vector4(0.9147445f, 0.3917669f, 0.0987938f),
			new Vector4(0.08110893f, 0.7317293f, 0.676752f),
			new Vector4(0.3154335f, 0.7388063f, 0.59554f),
			new Vector4(0.1677455f, 0.9625717f, 0.212877f),
			new Vector4(0.3015989f, 0.9509261f, 0.069128f),
			new Vector4(0.5600207f, 0.5649592f, 0.605969f),
			new Vector4(0.6455291f, 0.7387806f, 0.193637f)
		};

		private ShaderPass GetBlurPass(EffectMode mode, BlurModeType blurMode, bool useGBuffer, bool isFirstPass)
		{
			if (blurMode == BlurModeType.Enhanced)
			{
				if (isFirstPass)
				{
					if (mode == EffectMode.ColorBleed)
					{
						return (!useGBuffer) ? ShaderPass.EnhancedBlurFirstColorbleed : ShaderPass.EnhancedBlurFirstColorbleedGBuffer;
					}
					return (!useGBuffer) ? ShaderPass.EnhancedBlurFirst : ShaderPass.EnhancedBlurFirstGBuffer;
				}
				if (mode == EffectMode.ColorBleed)
				{
					return (!useGBuffer) ? ShaderPass.EnhancedBlurSecondColorbleed : ShaderPass.EnhancedBlurSecondColorbleedGBuffer;
				}
				return (!useGBuffer) ? ShaderPass.EnhancedBlurSecond : ShaderPass.EnhancedBlurSecondGBuffer;
			}
			if (mode == EffectMode.ColorBleed)
			{
				return (!useGBuffer) ? ShaderPass.StandardBlurUniformColorbleedGBuffer : ShaderPass.StandardBlurUniformColorbleed;
			}
			return (!useGBuffer) ? ShaderPass.StandardBlurUniformGBuffer : ShaderPass.StandardBlurUniform;
		}

		private ShaderPass GetNoisePass(EffectMode mode, bool isAdaptive, bool useGBuffer, bool iscullingPrepassType)
		{
			if (iscullingPrepassType)
			{
				if (mode == EffectMode.ColorBleed)
				{
					return (!useGBuffer) ? ShaderPass.BasicNoiseColorBleedcullingPrepassType : ShaderPass.BasicNoiseColorBleedGBuffercullingPrepassType;
				}
				return (!useGBuffer) ? ShaderPass.BasicNoisecullingPrepassType : ShaderPass.BasicNoiseGBuffercullingPrepassType;
			}
			if (mode == EffectMode.ColorBleed)
			{
				return (!useGBuffer) ? ShaderPass.BasicNoiseColorBleed : ShaderPass.BasicNoiseColorBleedGBuffer;
			}
			return useGBuffer ? ShaderPass.BasicNoiseGBuffer : ShaderPass.BasicNoise;
		}

		private ShaderPass GetMixingPass(EffectMode mode, bool aoOnly, LuminanceModeType lumaMode, bool isLuma)
		{
			if (aoOnly)
			{
				return ShaderPass.MixingAOOnly;
			}
			if (mode == EffectMode.ColorBleed)
			{
				return ShaderPass.MixingColorbleed;
			}
			return ShaderPass.MixingBasic;
		}

		private void ReportError(string error)
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogError("VAO Effect Error: " + error);
			}
		}

		private void ReportWarning(string error)
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("VAO Effect Warning: " + error);
			}
		}

		private void OnValidate()
		{
			Radius = Mathf.Clamp(Radius, 0.001f, float.MaxValue);
			Power = Mathf.Clamp(Power, 0f, float.MaxValue);
		}

		private void EnsureCommandBuffer(bool settingsDirty = false)
		{
			if ((!settingsDirty && isCommandBufferAlive) || !CommandBufferEnabled)
			{
				return;
			}
			try
			{
				CreateCommandBuffer();
				lastCameraEvent = GetCameraEvent(VaoCameraEvent);
				isCommandBufferAlive = true;
			}
			catch (Exception ex)
			{
				ReportError("There was an error while trying to create command buffer. " + ex.Message);
			}
		}

		private CameraEvent GetCameraEvent(VAOCameraEventType vaoCameraEvent)
		{
			switch (vaoCameraEvent)
			{
			case VAOCameraEventType.AfterForwardAlpha:
				return CameraEvent.AfterForwardAlpha;
			case VAOCameraEventType.AfterLighting:
				return CameraEvent.AfterLighting;
			case VAOCameraEventType.BeforeImageEffectsOpaque:
				return CameraEvent.BeforeImageEffectsOpaque;
			case VAOCameraEventType.BeforeLighting:
				return CameraEvent.BeforeLighting;
			case VAOCameraEventType.BeforeReflections:
				return CameraEvent.BeforeReflections;
			default:
				return CameraEvent.BeforeImageEffectsOpaque;
			}
		}

		private void TeardownCommandBuffer()
		{
			if (!isCommandBufferAlive)
			{
				return;
			}
			try
			{
				isCommandBufferAlive = false;
				if (myCamera != null)
				{
					foreach (KeyValuePair<CameraEvent, CommandBuffer> item in cameraEventsRegistered)
					{
						myCamera.RemoveCommandBuffer(item.Key, item.Value);
					}
				}
				cameraEventsRegistered.Clear();
				VAOMaterial = null;
				EnsureMaterials();
			}
			catch (Exception ex)
			{
				ReportError("There was an error while trying to destroy command buffer. " + ex.Message);
			}
		}

		private void CreateCommandBuffer()
		{
			VAOMaterial = null;
			EnsureMaterials();
			CameraEvent cameraEvent = GetCameraEvent(VaoCameraEvent);
			CommandBuffer value;
			if (cameraEventsRegistered.TryGetValue(cameraEvent, out value))
			{
				value.Clear();
			}
			else
			{
				value = new CommandBuffer();
				myCamera.AddCommandBuffer(cameraEvent, value);
				value.name = "Volumetric Ambient Occlusion";
				cameraEventsRegistered[cameraEvent] = value;
			}
			int num = myCamera.pixelWidth / Downsampling;
			int num2 = myCamera.pixelHeight / Downsampling;
			RenderTextureFormat renderTextureFormat = GetRenderTextureFormat(IntermediateScreenTextureFormat, isHDR);
			int num3 = Shader.PropertyToID("screenTextureRT");
			value.GetTemporaryRT(num3, myCamera.pixelWidth, myCamera.pixelHeight, 0, FilterMode.Bilinear, renderTextureFormat, RenderTextureReadWrite.Linear);
			int num4 = Shader.PropertyToID("vaoTextureRT");
			value.GetTemporaryRT(num4, num, num2, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			int? num5 = null;
			value.Blit(BuiltinRenderTextureType.CurrentActive, num3);
			if (CullingPrepassMode != 0)
			{
				num5 = Shader.PropertyToID("cullingPrepassTextureRT");
				value.GetTemporaryRT(num5.Value, num / DoomFactor, num2 / DoomFactor, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				value.Blit(num3, num5.Value, VAOMaterial, (int)GetNoisePass(Mode, false, UseGBuffer, true));
				value.SetGlobalTexture("cullingPrepassTexture", num5.Value);
			}
			value.Blit(num3, num4, VAOMaterial, (int)GetNoisePass(Mode, AdaptiveType != AdaptiveSamplingType.Disabled, UseGBuffer, false));
			if (BlurMode != 0)
			{
				int pixelWidth = myCamera.pixelWidth;
				int pixelHeight = myCamera.pixelHeight;
				int num6 = Shader.PropertyToID("blurredVAOTextureRT");
				value.GetTemporaryRT(num6, pixelWidth, pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				if (BlurMode == BlurModeType.Enhanced)
				{
					int num7 = Shader.PropertyToID("tempTextureRT");
					value.GetTemporaryRT(num7, pixelWidth, pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					value.Blit(num4, num7, VAOMaterial, (int)GetBlurPass(Mode, BlurMode, UseGBuffer, true));
					value.Blit(num7, num6, VAOMaterial, (int)GetBlurPass(Mode, BlurMode, UseGBuffer, false));
					value.ReleaseTemporaryRT(num7);
				}
				else
				{
					value.Blit(num4, num6, VAOMaterial, (int)GetBlurPass(Mode, BlurMode, UseGBuffer, false));
				}
				value.SetGlobalTexture("textureAO", num6);
				value.Blit(num3, BuiltinRenderTextureType.CameraTarget, VAOMaterial, (int)GetMixingPass(Mode, OutputAOOnly, LuminanceMode, IsLumaSensitive));
				value.ReleaseTemporaryRT(num6);
				value.ReleaseTemporaryRT(num4);
			}
			else
			{
				value.SetGlobalTexture("textureAO", num4);
				value.Blit(num3, BuiltinRenderTextureType.CameraTarget, VAOMaterial, (int)GetMixingPass(Mode, OutputAOOnly, LuminanceMode, IsLumaSensitive));
				value.ReleaseTemporaryRT(num4);
			}
			if (num5.HasValue)
			{
				value.ReleaseTemporaryRT(num5.Value);
			}
			value.ReleaseTemporaryRT(num3);
		}

		private void Start()
		{
			if (vaoShader == null)
			{
				vaoShader = Shader.Find("Hidden/Wilberforce/VAOShader");
			}
			if (vaoShader == null)
			{
				ReportError("Could not locate VAO Shader. Make sure there is 'VAOShader.shader' file added to the project.");
				isSupported = false;
				base.enabled = false;
				return;
			}
			if (!SystemInfo.supportsImageEffects || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth) || SystemInfo.graphicsShaderLevel < 30)
			{
				if (!SystemInfo.supportsImageEffects)
				{
					ReportError("System does not support image effects.");
				}
				if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
				{
					ReportError("System does not support depth texture.");
				}
				if (SystemInfo.graphicsShaderLevel < 30)
				{
					ReportError("This effect needs at least Shader Model 3.0.");
				}
				isSupported = false;
				base.enabled = false;
				return;
			}
			EnsureMaterials();
			if (!VAOMaterial || VAOMaterial.passCount != 23)
			{
				ReportError("Could not create shader.");
				isSupported = false;
				base.enabled = false;
				return;
			}
			if (adaptiveSamples == null)
			{
				adaptiveSamples = GenerateAdaptiveSamples();
			}
			isSupported = true;
		}

		private void OnEnable()
		{
			myCamera = GetComponent<Camera>();
			TeardownCommandBuffer();
			if (!(myCamera != null) || (CommandBufferEnabled && UseGBuffer))
			{
				return;
			}
			try
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				Type type = executingAssembly.GetType("UnityEngine.PostProcessing.PostProcessingBehaviour");
				Component component = GetComponent(type);
				if (component != null)
				{
					if (ForcedSwitchPerformed)
					{
						ReportWarning("Post Processing Stack Detected! We recommend switching to command buffer pipeline and GBuffer inputs if you encounter compatibility problems.");
						return;
					}
					ReportWarning("Post Processing Stack Detected! Switching to command buffer pipeline and GBuffer inputs!");
					CommandBufferEnabled = true;
					UseGBuffer = true;
					ForcedSwitchPerformed = true;
				}
			}
			catch (Exception)
			{
			}
		}

		private void OnDisable()
		{
			TeardownCommandBuffer();
		}

		private void OnPreRender()
		{
			DepthTextureMode depthTextureMode = myCamera.depthTextureMode;
			if (myCamera.actualRenderingPath == RenderingPath.DeferredShading && UseGBuffer)
			{
				if ((depthTextureMode & DepthTextureMode.Depth) != DepthTextureMode.Depth)
				{
					myCamera.depthTextureMode |= DepthTextureMode.Depth;
				}
			}
			else if ((depthTextureMode & DepthTextureMode.DepthNormals) != DepthTextureMode.DepthNormals)
			{
				myCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
			}
			if (myCamera.actualRenderingPath != RenderingPath.DeferredShading && UseGBuffer)
			{
				ReportWarning("Could not set G-Buffer inputs and reverting to default! Please set camera rendering path to DEFERRED before switching to G-Buffer inputs.");
				UseGBuffer = false;
			}
			EnsureCommandBuffer(CheckSettingsChanges());
			TrySetUniforms();
		}

		private bool CheckSettingsChanges()
		{
			bool result = false;
			if (GetCameraEvent(VaoCameraEvent) != lastCameraEvent)
			{
				TeardownCommandBuffer();
			}
			if (Downsampling != lastDownsampling)
			{
				lastDownsampling = Downsampling;
				result = true;
			}
			if (CullingPrepassMode != lastcullingPrepassType)
			{
				lastcullingPrepassType = CullingPrepassMode;
				result = true;
			}
			if (DoomFactor != lastDoomFactor)
			{
				lastDoomFactor = DoomFactor;
				result = true;
			}
			if (BlurMode != lastBlurMode)
			{
				lastBlurMode = BlurMode;
				result = true;
			}
			if (Mode != lastMode)
			{
				lastMode = Mode;
				result = true;
			}
			if (UseGBuffer != lastUseGBuffer)
			{
				lastUseGBuffer = UseGBuffer;
				result = true;
			}
			if (OutputAOOnly != lastOutputAOOnly)
			{
				lastOutputAOOnly = OutputAOOnly;
				result = true;
			}
			isHDR = isCameraHDR(myCamera);
			if (isHDR != lastIsHDR)
			{
				lastIsHDR = isHDR;
				result = true;
			}
			if (lastIntermediateScreenTextureFormat != IntermediateScreenTextureFormat)
			{
				lastIntermediateScreenTextureFormat = IntermediateScreenTextureFormat;
				result = true;
			}
			return result;
		}

		private void TrySetUniforms()
		{
			if (VAOMaterial == null)
			{
				return;
			}
			int num = myCamera.pixelWidth / Downsampling;
			int num2 = myCamera.pixelHeight / Downsampling;
			Vector4[] array = null;
			switch (Quality)
			{
			case 2:
				array = samp2;
				break;
			case 4:
				array = samp4;
				break;
			case 8:
				array = samp8;
				break;
			case 16:
				array = samp16;
				break;
			case 32:
				array = samp32;
				break;
			case 64:
				array = samp64;
				break;
			default:
				ReportError("Unsupported quality setting " + Quality + " encountered. Reverting to low setting");
				Quality = 16;
				array = samp16;
				break;
			}
			if (AdaptiveType != 0)
			{
				switch (Quality)
				{
				case 64:
					AdaptiveQuality = 0.025f;
					break;
				case 32:
					AdaptiveQuality = 0.025f;
					break;
				case 16:
					AdaptiveQuality = 0.05f;
					break;
				case 8:
					AdaptiveQuality = 0.1f;
					break;
				case 4:
					AdaptiveQuality = 0.2f;
					break;
				case 2:
					AdaptiveQuality = 0.4f;
					break;
				}
				if (AdaptiveType == AdaptiveSamplingType.EnabledManual)
				{
					AdaptiveQuality *= AdaptiveQualityCoefficient;
				}
				else
				{
					AdaptiveQualityCoefficient = 1f;
				}
			}
			AdaptiveMax = GetDepthForScreenSize(myCamera, AdaptiveQuality);
			Vector2 vector = new Vector2(1f / (float)num, 1f / (float)num2);
			float depthForScreenSize = GetDepthForScreenSize(myCamera, Mathf.Max(vector.x, vector.y));
			VAOMaterial.SetMatrix("projMatrix", myCamera.projectionMatrix);
			VAOMaterial.SetMatrix("invProjMatrix", myCamera.projectionMatrix.inverse);
			VAOMaterial.SetFloat("halfRadiusSquared", Radius * 0.5f * (Radius * 0.5f));
			VAOMaterial.SetFloat("halfRadius", Radius * 0.5f);
			VAOMaterial.SetFloat("radius", Radius);
			VAOMaterial.SetInt("sampleCount", Quality);
			VAOMaterial.SetInt("sampleCountOpenGLBug", Quality);
			VAOMaterial.SetFloat("aoPower", Power);
			VAOMaterial.SetFloat("aoPresence", Presence);
			VAOMaterial.SetFloat("giPresence", 1f - ColorBleedPresence);
			VAOMaterial.SetFloat("LumaThreshold", LumaThreshold);
			VAOMaterial.SetFloat("LumaKneeWidth", LumaKneeWidth);
			VAOMaterial.SetFloat("LumaTwiceKneeWidthRcp", 1f / (LumaKneeWidth * 2f));
			VAOMaterial.SetFloat("LumaKneeLinearity", LumaKneeLinearity);
			VAOMaterial.SetInt("giBackfaces", (!GiBackfaces) ? 1 : 0);
			VAOMaterial.SetFloat("adaptiveMin", AdaptiveMin);
			VAOMaterial.SetFloat("adaptiveMax", AdaptiveMax);
			VAOMaterial.SetVector("texelSize", vector);
			VAOMaterial.SetFloat("blurDepthThreshold", Radius);
			VAOMaterial.SetInt("cullingPrepassMode", (int)CullingPrepassMode);
			VAOMaterial.SetVector("cullingPrepassTexelSize", new Vector2(0.5f / (float)(myCamera.pixelWidth / DoomFactor), 0.5f / (float)(myCamera.pixelHeight / DoomFactor)));
			VAOMaterial.SetInt("forceFlip", MustForceFlip(myCamera) ? 1 : 0);
			VAOMaterial.SetInt("enhancedBlurSize", EnhancedBlurSize / 2);
			VAOMaterial.SetInt("giSelfOcclusionFix", (int)ColorBleedSelfOcclusionFixLevel);
			VAOMaterial.SetInt("adaptiveMode", (int)AdaptiveType);
			VAOMaterial.SetInt("IsLumaSensitive", IsLumaSensitive ? 1 : 0);
			VAOMaterial.SetInt("LumaMode", (int)LuminanceMode);
			VAOMaterial.SetInt("giEnabled", (Mode == EffectMode.ColorBleed) ? 1 : 0);
			VAOMaterial.SetFloat("cameraFarPlane", myCamera.farClipPlane);
			VAOMaterial.SetInt("UseCameraFarPlane", (FarPlaneSource == FarPlaneSourceType.Camera) ? 1 : 0);
			VAOMaterial.SetFloat("maxRadiusEnabled", MaxRadiusEnabled ? 1 : 0);
			VAOMaterial.SetFloat("maxRadiusCutoffDepth", GetDepthForScreenSize(myCamera, MaxRadius));
			VAOMaterial.SetFloat("projMatrix11", myCamera.projectionMatrix.m11);
			VAOMaterial.SetFloat("maxRadiusOnScreen", MaxRadius);
			VAOMaterial.SetInt("minRadiusEnabled", (int)DistanceFalloffMode);
			VAOMaterial.SetFloat("minRadiusCutoffDepth", (DistanceFalloffMode != DistanceFalloffModeType.Relative) ? (0f - DistanceFalloffStartAbsolute) : (Mathf.Abs(depthForScreenSize) * (0f - DistanceFalloffStartRelative * DistanceFalloffStartRelative)));
			VAOMaterial.SetFloat("minRadiusSoftness", (DistanceFalloffMode != DistanceFalloffModeType.Relative) ? DistanceFalloffSpeedAbsolute : (Mathf.Abs(depthForScreenSize) * (DistanceFalloffSpeedRelative * DistanceFalloffSpeedRelative)));
			VAOMaterial.SetInt("giSameHueAttenuationEnabled", ColorbleedHueSuppresionEnabled ? 1 : 0);
			VAOMaterial.SetFloat("giSameHueAttenuationThreshold", ColorBleedHueSuppresionThreshold);
			VAOMaterial.SetFloat("giSameHueAttenuationWidth", ColorBleedHueSuppresionWidth);
			VAOMaterial.SetFloat("giSameHueAttenuationSaturationThreshold", ColorBleedHueSuppresionSaturationThreshold);
			VAOMaterial.SetFloat("giSameHueAttenuationSaturationWidth", ColorBleedHueSuppresionSaturationWidth);
			VAOMaterial.SetFloat("giSameHueAttenuationBrightness", ColorBleedHueSuppresionBrightness);
			VAOMaterial.SetFloat("subpixelRadiusCutoffDepth", Mathf.Min(0.99f, depthForScreenSize / (0f - myCamera.farClipPlane)));
			if (Quality == 4 || (Quality == 8 && (ColorBleedQuality == 2 || ColorBleedQuality == 4)))
			{
				VAOMaterial.SetInt("giBlur", 3);
			}
			else
			{
				VAOMaterial.SetInt("giBlur", 2);
			}
			if (Mode == EffectMode.ColorBleed)
			{
				VAOMaterial.SetFloat("giPower", ColorBleedPower);
				if (Quality == 2 && ColorBleedQuality == 4)
				{
					VAOMaterial.SetInt("giQuality", 2);
				}
				else
				{
					VAOMaterial.SetInt("giQuality", ColorBleedQuality);
				}
			}
			if (CullingPrepassMode != 0)
			{
				SetSampleSetNoBuffer("eightSamples", VAOMaterial, samp8);
			}
			if (AdaptiveType != 0)
			{
				SetSampleSet("samples", VAOMaterial, GetAdaptiveSamples());
			}
			else if (CullingPrepassMode == CullingPrepassModeType.Careful)
			{
				SetSampleSet("samples", VAOMaterial, GetCarefulDoomSamples(array, samp4));
			}
			else
			{
				SetSampleSet("samples", VAOMaterial, array);
			}
			if (Mode == EffectMode.Simple)
			{
				VAOMaterial.SetColor("colorTint", Color.black);
			}
			else
			{
				VAOMaterial.SetColor("colorTint", ColorTint);
			}
			if (BlurMode == BlurModeType.Enhanced)
			{
				if (gaussian == null || gaussian.Length != EnhancedBlurSize || EnhancedBlurDeviation != lastDeviation)
				{
					gaussian = GenerateGaussian(EnhancedBlurSize, EnhancedBlurDeviation, out gaussianWeight);
					lastDeviation = EnhancedBlurDeviation;
				}
				VAOMaterial.SetFloat("gaussWeight", gaussianWeight);
				VAOMaterial.SetVectorArray("gauss", gaussian);
			}
		}

		private static Material CreateMaterial(Shader shader)
		{
			if (!shader)
			{
				return null;
			}
			Material material = new Material(shader);
			material.hideFlags = HideFlags.HideAndDontSave;
			return material;
		}

		private static void DestroyMaterial(Material mat)
		{
			if ((bool)mat)
			{
				UnityEngine.Object.DestroyImmediate(mat);
				mat = null;
			}
		}

		private void EnsureMaterials()
		{
			if (!VAOMaterial && vaoShader.isSupported)
			{
				VAOMaterial = CreateMaterial(vaoShader);
			}
			if (!vaoShader.isSupported)
			{
				ReportError("Could not create shader (Shader not supported).");
			}
		}

		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!isSupported || !vaoShader.isSupported)
			{
				base.enabled = false;
				return;
			}
			EnsureMaterials();
			if (CommandBufferEnabled)
			{
				Graphics.Blit(source, destination);
				return;
			}
			TeardownCommandBuffer();
			int width = source.width / Downsampling;
			int height = source.height / Downsampling;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
			RenderTexture renderTexture = null;
			if (CullingPrepassMode != 0)
			{
				renderTexture = RenderTexture.GetTemporary(source.width / DoomFactor, source.height / DoomFactor, 0);
				Graphics.Blit(source, renderTexture, VAOMaterial, (int)GetNoisePass(Mode, false, UseGBuffer, true));
			}
			if (renderTexture != null)
			{
				VAOMaterial.SetTexture("cullingPrepassTexture", renderTexture);
			}
			Graphics.Blit(source, temporary, VAOMaterial, (int)GetNoisePass(Mode, AdaptiveType != AdaptiveSamplingType.Disabled, UseGBuffer, false));
			if (BlurMode != 0)
			{
				int width2 = source.width;
				int height2 = source.height;
				RenderTexture temporary2 = RenderTexture.GetTemporary(width2, height2, 0);
				if (BlurMode == BlurModeType.Enhanced)
				{
					RenderTexture temporary3 = RenderTexture.GetTemporary(width2, height2, 0);
					Graphics.Blit(temporary, temporary3, VAOMaterial, (int)GetBlurPass(Mode, BlurMode, UseGBuffer, true));
					Graphics.Blit(temporary3, temporary2, VAOMaterial, (int)GetBlurPass(Mode, BlurMode, UseGBuffer, false));
					RenderTexture.ReleaseTemporary(temporary3);
				}
				else
				{
					Graphics.Blit(temporary, temporary2, VAOMaterial, (int)GetBlurPass(Mode, BlurMode, UseGBuffer, true));
				}
				VAOMaterial.SetTexture("textureAO", temporary2);
				Graphics.Blit(source, destination, VAOMaterial, (int)GetMixingPass(Mode, OutputAOOnly, LuminanceMode, IsLumaSensitive));
				RenderTexture.ReleaseTemporary(temporary2);
				RenderTexture.ReleaseTemporary(temporary);
			}
			else
			{
				VAOMaterial.SetTexture("textureAO", temporary);
				Graphics.Blit(source, destination, VAOMaterial, (int)GetMixingPass(Mode, OutputAOOnly, LuminanceMode, IsLumaSensitive));
				RenderTexture.ReleaseTemporary(temporary);
			}
			if (renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
		}

		private Vector4[] GetAdaptiveSamples()
		{
			if (adaptiveSamples == null)
			{
				adaptiveSamples = GenerateAdaptiveSamples();
			}
			return adaptiveSamples;
		}

		private Vector4[] GetCarefulDoomSamples(Vector4[] samples, Vector4[] carefulSamples)
		{
			if (carefulCache != null && carefulCache.Length == samples.Length + carefulSamples.Length)
			{
				return carefulCache;
			}
			carefulCache = new Vector4[samples.Length + carefulSamples.Length];
			Array.Copy(samples, 0, carefulCache, 0, samples.Length);
			Array.Copy(carefulSamples, 0, carefulCache, samples.Length, carefulSamples.Length);
			return carefulCache;
		}

		private Vector4[] GenerateAdaptiveSamples()
		{
			Vector4[] array = new Vector4[62];
			Array.Copy(samp32, 0, array, 0, 32);
			Array.Copy(samp16, 0, array, 32, 16);
			Array.Copy(samp8, 0, array, 48, 8);
			Array.Copy(samp4, 0, array, 56, 4);
			Array.Copy(samp2, 0, array, 60, 2);
			return array;
		}

		private float GetDepthForScreenSize(Camera camera, float sizeOnScreen)
		{
			return (0f - Radius * camera.projectionMatrix.m11) / sizeOnScreen;
		}

		private bool isCameraHDR(Camera camera)
		{
			if (camera != null)
			{
				return camera.allowHDR;
			}
			return false;
		}

		private bool MustForceFlip(Camera camera)
		{
			return false;
		}

		private RenderTextureFormat GetRenderTextureFormat(ScreenTextureFormat format, bool isHDR)
		{
			switch (format)
			{
			case ScreenTextureFormat.Default:
				return RenderTextureFormat.Default;
			case ScreenTextureFormat.DefaultHDR:
				return RenderTextureFormat.DefaultHDR;
			case ScreenTextureFormat.ARGB32:
				return RenderTextureFormat.ARGB32;
			case ScreenTextureFormat.ARGBFloat:
				return RenderTextureFormat.ARGBFloat;
			case ScreenTextureFormat.ARGBHalf:
				return RenderTextureFormat.ARGBHalf;
			default:
				return (!isHDR) ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
			}
		}

		private Vector4[] GenerateGaussian(int size, float d, out float gaussianWeight)
		{
			Vector4[] array = new Vector4[50];
			float num = 0f;
			for (int i = 0; i <= size / 2; i++)
			{
				float num2 = (float)(Math.Pow(Math.E, -3f * (float)i / ((float)size * d)) / Math.Sqrt(2.0 * (double)d * (double)d * Math.PI));
				array[size / 2 + i].x = num2;
				array[size / 2 - i].x = num2;
				num += ((i <= 0) ? num2 : (num2 * 2f));
			}
			gaussianWeight = 0f;
			for (int j = 0; j < size; j++)
			{
				gaussianWeight += array[j].x;
			}
			return array;
		}

		private void SetSampleSetNoBuffer(string name, Material VAOMaterial, Vector4[] samples)
		{
			VAOMaterial.SetVectorArray(name, samples);
		}

		private void SetSampleSet(string name, Material VAOMaterial, Vector4[] samples)
		{
			if (lastSamplesLength != samples.Length)
			{
				Array.Copy(samples, samplesLarge, samples.Length);
				lastSamplesLength = samples.Length;
			}
			VAOMaterial.SetVectorArray(name, samplesLarge);
		}
	}
}
