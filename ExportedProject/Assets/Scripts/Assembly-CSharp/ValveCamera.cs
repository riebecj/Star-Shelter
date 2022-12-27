using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VR;
using Valve.VRRenderingPackage;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ValveCamera : MonoBehaviour
{
	[NonSerialized]
	private const float DIRECTIONAL_LIGHT_PULLBACK_DISTANCE = 10000f;

	[NonSerialized]
	private const int MAX_LIGHTS = 18;

	[Header("Lights & Shadows")]
	[Range(1024f, 8192f)]
	public int m_valveShadowTextureWidth = 4096;

	[Range(1024f, 8192f)]
	public int m_valveShadowTextureHeight = 4096;

	[NonSerialized]
	private Camera m_shadowCamera;

	[NonSerialized]
	public RenderTexture m_shadowDepthTexture;

	[NonSerialized]
	public Shader m_shaderCastShadows;

	[Tooltip("Requires 'Directional Specular' General GI Mode")]
	public bool m_indirectLightmapsOnly;

	[Header("Adaptive Quality")]
	[Tooltip("Automatically adjusts render quality to maintain VR framerate")]
	public bool m_adaptiveQualityEnabled = true;

	[Tooltip("Shows Debug Overlay [Shift+F1] or launch with -vrdebug")]
	public bool m_adaptiveQualityDebugVis;

	[Range(0f, 8f)]
	public int m_MSAALevel = 4;

	public float m_minRenderTargetScale = 0.8f;

	public float m_maxRenderTargetScale = 1.4f;

	[NonSerialized]
	private int m_nFillRatePercentStep = 15;

	public int m_maxRenderTargetDimension = 4096;

	[NonSerialized]
	private static bool s_bUsingStaticSettings;

	[NonSerialized]
	private static bool s_bAdaptiveQualityVRDebug;

	[NonSerialized]
	private static bool s_bAdaptiveQualityVROverride;

	[NonSerialized]
	private static int s_nAdaptiveQualityVROverride;

	[NonSerialized]
	private static bool m_bAllowFlush = true;

	[NonSerialized]
	private GameObject m_adaptiveQualityDebugQuad;

	[Header("Helper")]
	public bool m_cullLightsInSceneEditor;

	public bool m_cullLightsFromEditorCamera;

	public bool m_hideAllValveMaterials;

	[NonSerialized]
	private int g_nNumFlushesThisFrame;

	[NonSerialized]
	private int m_nFlushCounterFrameCount;

	[NonSerialized]
	private int m_nAdaptiveQualityLevel;

	[NonSerialized]
	private int m_nAdaptiveQualityFrameCountLastChanged;

	[NonSerialized]
	private float[] m_adaptiveQualityRingBuffer = new float[12];

	[NonSerialized]
	private int m_nAdaptiveQualityRingBufferPos;

	[NonSerialized]
	private bool m_bInterleavedReprojectionEnabled;

	[NonSerialized]
	private List<float> m_adaptiveQualityRenderScaleArray = new List<float>();

	[NonSerialized]
	private int m_adaptiveQualityNumLevels;

	[NonSerialized]
	private int m_adaptiveQualityDefaultLevel;

	[NonSerialized]
	private int m_nLastQualityFrameCount = -1;

	[NonSerialized]
	private bool m_bFailedToPackLastTime;

	[NonSerialized]
	private int m_nLastRenderedFrameCount = -1;

	[NonSerialized]
	private int m_nWarnedTooManyLights;

	[NonSerialized]
	private Vector4[] g_vLightColor = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vLightPosition_flInvRadius = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vLightDirection = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vLightShadowIndex_vLightParams = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vLightFalloffParams = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vSpotLightInnerOuterConeCosines = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vShadowMinMaxUv = new Vector4[18];

	[NonSerialized]
	private Matrix4x4[] g_matWorldToShadow = new Matrix4x4[18];

	[NonSerialized]
	private Matrix4x4[] g_matWorldToLightCookie = new Matrix4x4[18];

	[NonSerialized]
	private bool m_isDisplayOnDesktopCached;

	[NonSerialized]
	private bool m_isDisplayOnDesktop = true;

	[CompilerGenerated]
	private static Comparison<ValveRealtimeLight> _003C_003Ef__am_0024cache0;

	public static bool HasCommandLineArg(string argumentName)
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].Equals(argumentName))
			{
				return true;
			}
		}
		return false;
	}

	public static int GetCommandLineArgValue(string argumentName, int nDefaultValue)
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].Equals(argumentName))
			{
				if (i == commandLineArgs.Length - 1)
				{
					return nDefaultValue;
				}
				return int.Parse(commandLineArgs[i + 1]);
			}
		}
		return nDefaultValue;
	}

	public static float GetCommandLineArgValue(string argumentName, float flDefaultValue)
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].Equals(argumentName))
			{
				if (i == commandLineArgs.Length - 1)
				{
					return flDefaultValue;
				}
				return (float)double.Parse(commandLineArgs[i + 1]);
			}
		}
		return flDefaultValue;
	}

	private int ClampMSAA(int nMSAA)
	{
		if (nMSAA < 2)
		{
			return 0;
		}
		if (nMSAA < 4)
		{
			return 2;
		}
		if (nMSAA < 8)
		{
			return 4;
		}
		return 8;
	}

	private void OnValidate()
	{
		if (m_valveShadowTextureWidth % 128 != 0)
		{
			m_valveShadowTextureWidth -= m_valveShadowTextureWidth % 128;
		}
		if (m_valveShadowTextureHeight % 128 != 0)
		{
			m_valveShadowTextureHeight -= m_valveShadowTextureHeight % 128;
		}
		m_MSAALevel = ClampMSAA(m_MSAALevel);
	}

	private void OnEnable()
	{
		if (HasCommandLineArg("-noflush"))
		{
			m_bAllowFlush = false;
		}
		if (HasCommandLineArg("-noaq"))
		{
			m_adaptiveQualityEnabled = false;
		}
		if (HasCommandLineArg("-aqoverride"))
		{
			s_bAdaptiveQualityVROverride = true;
			s_nAdaptiveQualityVROverride = GetCommandLineArgValue("-aqoverride", 0);
		}
		if (!s_bUsingStaticSettings)
		{
			s_bUsingStaticSettings = true;
			if (HasCommandLineArg("-vrdebug"))
			{
				s_bAdaptiveQualityVRDebug = true;
			}
			if (m_adaptiveQualityDebugVis)
			{
				s_bAdaptiveQualityVRDebug = true;
			}
		}
		if (Application.isPlaying)
		{
			int num2 = (QualitySettings.antiAliasing = ClampMSAA(GetCommandLineArgValue("-msaa", m_MSAALevel)));
			Debug.Log("[Valve Camera] Setting MSAA to " + num2 + "x\n");
		}
		InitializeAdaptiveQualityScale();
		CreateAssets();
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		if ((bool)m_shadowDepthTexture)
		{
			UnityEngine.Object.DestroyImmediate(m_shadowDepthTexture);
			m_shadowDepthTexture = null;
		}
	}

	private void CreateAssets()
	{
		if (!m_shadowCamera)
		{
			m_shadowCamera = CreateRenderCamera("Valve Shadow Camera");
			m_cullLightsInSceneEditor = false;
			m_cullLightsFromEditorCamera = false;
			m_hideAllValveMaterials = false;
		}
		if (!m_shadowDepthTexture || m_shadowDepthTexture.width != m_valveShadowTextureWidth || m_shadowDepthTexture.height != m_valveShadowTextureHeight)
		{
			if ((bool)m_shadowDepthTexture)
			{
				UnityEngine.Object.DestroyImmediate(m_shadowDepthTexture);
				m_shadowDepthTexture = null;
			}
			m_shadowDepthTexture = new RenderTexture(m_valveShadowTextureWidth, m_valveShadowTextureHeight, 24, RenderTextureFormat.Shadowmap, RenderTextureReadWrite.Linear);
			if ((bool)m_shadowDepthTexture)
			{
				m_shadowDepthTexture.name = "m_shadowDepthTexture";
				m_shadowDepthTexture.hideFlags = HideFlags.HideAndDontSave;
				m_shadowDepthTexture.useMipMap = false;
				m_shadowDepthTexture.filterMode = FilterMode.Bilinear;
				m_shadowDepthTexture.wrapMode = TextureWrapMode.Clamp;
				m_shadowDepthTexture.antiAliasing = 1;
				m_shadowDepthTexture.Create();
				m_shadowDepthTexture.SetGlobalShaderProperty("g_tShadowBuffer");
			}
			else
			{
				Debug.LogWarning("ERROR! Cannot create shadow depth texture!\n");
			}
		}
		if (!m_shaderCastShadows)
		{
			m_shaderCastShadows = Resources.Load("vr_cast_shadows") as Shader;
			if (!m_shaderCastShadows)
			{
				Debug.LogWarning("ERROR! Can't find Resources/vr_cast_shadows!\n");
			}
			else if (!m_shaderCastShadows.isSupported)
			{
				Debug.LogWarning("ERROR! Resources/vr_cast_shadows not supported!\n");
			}
		}
	}

	private static Camera CreateRenderCamera(string name)
	{
		GameObject gameObject = GameObject.Find(name);
		if (!gameObject)
		{
			gameObject = new GameObject(name);
		}
		Camera camera = gameObject.GetComponent<Camera>();
		if (!camera)
		{
			camera = gameObject.AddComponent<Camera>();
		}
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		camera.enabled = false;
		camera.renderingPath = RenderingPath.Forward;
		camera.nearClipPlane = 0.1f;
		camera.farClipPlane = 100f;
		camera.depthTextureMode = DepthTextureMode.None;
		camera.clearFlags = CameraClearFlags.Depth;
		camera.backgroundColor = Color.white;
		camera.orthographic = false;
		camera.hideFlags = HideFlags.HideAndDontSave;
		gameObject.SetActive(false);
		return camera;
	}

	private void ValveGLFlush()
	{
		if (m_bAllowFlush)
		{
			if (m_nFlushCounterFrameCount != Time.frameCount)
			{
				m_nFlushCounterFrameCount = Time.frameCount;
				g_nNumFlushesThisFrame = 0;
			}
			if (++g_nNumFlushesThisFrame <= 3)
			{
				GL.Flush();
			}
		}
	}

	private void ValveGLFlushIfNotReprojecting()
	{
		if (OpenVR.Compositor != null)
		{
			Compositor_FrameTiming pTiming = default(Compositor_FrameTiming);
			pTiming.m_nSize = (uint)Marshal.SizeOf(typeof(Compositor_FrameTiming));
			OpenVR.Compositor.GetFrameTiming(ref pTiming, 0u);
			if (pTiming.m_nNumFramePresents > 1)
			{
				return;
			}
		}
		ValveGLFlush();
	}

	private void MyEditorUpdate()
	{
	}

	private void LateUpdate()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.F1) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
		{
			s_bAdaptiveQualityVRDebug = ((!s_bAdaptiveQualityVRDebug) ? true : false);
		}
		if (Input.GetKeyDown(KeyCode.F2) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
		{
			s_bAdaptiveQualityVROverride = ((!s_bAdaptiveQualityVROverride) ? true : false);
			s_nAdaptiveQualityVROverride = m_nAdaptiveQualityLevel;
		}
		if (Input.GetKeyDown(KeyCode.F3) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
		{
			s_nAdaptiveQualityVROverride = Mathf.Max(s_nAdaptiveQualityVROverride - 1, 0);
		}
		if (Input.GetKeyDown(KeyCode.F4) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
		{
			s_nAdaptiveQualityVROverride++;
		}
		if (m_adaptiveQualityEnabled && s_bAdaptiveQualityVRDebug && !m_adaptiveQualityDebugQuad)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[4]
			{
				new Vector3(-0.5f, 0.9f, 1f),
				new Vector3(-0.5f, 1f, 1f),
				new Vector3(0.5f, 1f, 1f),
				new Vector3(0.5f, 0.9f, 1f)
			};
			mesh.uv = new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f)
			};
			mesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
			mesh.UploadMeshData(false);
			m_adaptiveQualityDebugQuad = new GameObject("AdaptiveQualityDebugQuad");
			m_adaptiveQualityDebugQuad.transform.parent = base.transform;
			m_adaptiveQualityDebugQuad.transform.localPosition = Vector3.forward * 1f;
			m_adaptiveQualityDebugQuad.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			m_adaptiveQualityDebugQuad.AddComponent<MeshFilter>().mesh = mesh;
			if ((bool)(Resources.Load("adaptive_quality_debug") as Material))
			{
				m_adaptiveQualityDebugQuad.AddComponent<MeshRenderer>().material = Resources.Load("adaptive_quality_debug") as Material;
				(Resources.Load("adaptive_quality_debug") as Material).renderQueue = 4000;
			}
		}
		else if (!s_bAdaptiveQualityVRDebug && (bool)m_adaptiveQualityDebugQuad)
		{
			UnityEngine.Object.DestroyImmediate(m_adaptiveQualityDebugQuad);
			m_adaptiveQualityDebugQuad = null;
		}
	}

	private void OnPreCull()
	{
		UpdateAdaptiveQuality();
		ValveShadowBufferRender();
	}

	private void OnPostRender()
	{
		ValveGLFlush();
	}

	private void ValveShadowBufferRender()
	{
		CullLightsAgainstCameraFrustum();
		RenderShadowBuffer();
		UpdateLightConstants();
	}

	private void InitializeAdaptiveQualityScale()
	{
		float commandLineArgValue = GetCommandLineArgValue("-aqminscale", m_minRenderTargetScale);
		float num = GetCommandLineArgValue("-aqmaxscale", m_maxRenderTargetScale);
		int commandLineArgValue2 = GetCommandLineArgValue("-aqfillratestep", m_nFillRatePercentStep);
		int num2 = m_maxRenderTargetDimension;
		if (HasCommandLineArg("-aqmaxres"))
		{
			num2 = GetCommandLineArgValue("-aqmaxres", num2);
			num = Mathf.Min((float)num2 / (float)VRSettings.eyeTextureWidth, (float)num2 / (float)VRSettings.eyeTextureHeight);
		}
		m_adaptiveQualityRenderScaleArray.Clear();
		m_adaptiveQualityRenderScaleArray.Add(commandLineArgValue);
		float num3 = commandLineArgValue;
		while (num3 <= num)
		{
			m_adaptiveQualityRenderScaleArray.Add(num3);
			num3 = Mathf.Sqrt((float)(commandLineArgValue2 + 100) / 100f * num3 * num3);
			if (num3 * (float)VRSettings.eyeTextureWidth > (float)num2 || num3 * (float)VRSettings.eyeTextureHeight > (float)num2)
			{
				break;
			}
		}
		m_adaptiveQualityDefaultLevel = 0;
		for (int i = 0; i < m_adaptiveQualityRenderScaleArray.Count; i++)
		{
			if (m_adaptiveQualityRenderScaleArray[i] >= 1f)
			{
				m_adaptiveQualityDefaultLevel = i;
				break;
			}
		}
		m_nAdaptiveQualityLevel = m_adaptiveQualityDefaultLevel;
		m_adaptiveQualityNumLevels = m_adaptiveQualityRenderScaleArray.Count;
		if (!Application.isPlaying)
		{
			return;
		}
		string text = "[Valve Camera] Adaptive Quality:\n";
		for (int j = 1; j < m_adaptiveQualityRenderScaleArray.Count; j++)
		{
			text = text + j + ". ";
			string text2 = text;
			text = text2 + " " + (int)((float)VRSettings.eyeTextureWidth * m_adaptiveQualityRenderScaleArray[j]) + "x" + (int)((float)VRSettings.eyeTextureHeight * m_adaptiveQualityRenderScaleArray[j]);
			text = text + " " + m_adaptiveQualityRenderScaleArray[j];
			if (j == m_adaptiveQualityDefaultLevel)
			{
				text += " (Default)";
			}
			if (j == 0)
			{
				text += " (Interleaved reprojection hint)";
			}
			text += "\n";
		}
		Debug.Log(text);
	}

	private void UpdateAdaptiveQuality()
	{
		if (!m_adaptiveQualityEnabled)
		{
			if (VRSettings.enabled)
			{
				if (VRSettings.renderScale != 1f)
				{
					VRSettings.renderScale = 1f;
				}
				if (VRSettings.renderViewportScale != 1f)
				{
					VRSettings.renderViewportScale = 1f;
				}
			}
		}
		else
		{
			if (m_nLastQualityFrameCount == Time.frameCount)
			{
				return;
			}
			m_nLastQualityFrameCount = Time.frameCount;
			float num = m_adaptiveQualityRenderScaleArray[m_adaptiveQualityNumLevels - 1];
			int length = m_adaptiveQualityRingBuffer.GetLength(0);
			m_nAdaptiveQualityRingBufferPos = (m_nAdaptiveQualityRingBufferPos + 1) % length;
			m_adaptiveQualityRingBuffer[m_nAdaptiveQualityRingBufferPos] = VRStats.gpuTimeLastFrame;
			int nAdaptiveQualityLevel = m_nAdaptiveQualityLevel;
			float num2 = ((!(VRDevice.refreshRate > 0f)) ? 11.111111f : (1000f / VRDevice.refreshRate));
			bool flag = false;
			if (OpenVR.Compositor != null)
			{
				flag = OpenVR.Compositor.ShouldAppRenderWithLowResources();
			}
			float num3 = ((!flag) ? 1f : 0.75f);
			float num4 = 0.7f * num2 * num3;
			float num5 = 0.85f * num2 * num3;
			float num6 = 0.9f * num2 * num3;
			float num7 = m_adaptiveQualityRingBuffer[(m_nAdaptiveQualityRingBufferPos + length) % length];
			float num8 = m_adaptiveQualityRingBuffer[(m_nAdaptiveQualityRingBufferPos - 1 + length) % length];
			float num9 = m_adaptiveQualityRingBuffer[(m_nAdaptiveQualityRingBufferPos - 2 + length) % length];
			int value = ((nAdaptiveQualityLevel == 2) ? 1 : (nAdaptiveQualityLevel - 2));
			if (Time.frameCount >= m_nAdaptiveQualityFrameCountLastChanged + 2 + 1 && num7 > num6)
			{
				int num10 = Mathf.Clamp(value, 0, m_adaptiveQualityNumLevels - 1);
				if (num10 != nAdaptiveQualityLevel)
				{
					m_nAdaptiveQualityLevel = num10;
					m_nAdaptiveQualityFrameCountLastChanged = Time.frameCount;
					return;
				}
			}
			if (Time.frameCount >= m_nAdaptiveQualityFrameCountLastChanged + 2 + 3 && num7 > num6 && num8 > num6 && num9 > num6)
			{
				int num11 = Mathf.Clamp(value, 0, m_adaptiveQualityNumLevels - 1);
				if (num11 != nAdaptiveQualityLevel)
				{
					m_nAdaptiveQualityLevel = num11;
					m_nAdaptiveQualityFrameCountLastChanged = Time.frameCount;
				}
			}
			if (Time.frameCount >= m_nAdaptiveQualityFrameCountLastChanged + 2 + 2 && num7 > num5)
			{
				float num12 = num7 - num8;
				if (Time.frameCount >= m_nAdaptiveQualityFrameCountLastChanged + 2 + 3)
				{
					float b = (num7 - num9) * 0.5f;
					num12 = Mathf.Max(num12, b);
				}
				if (num7 + num12 > num6)
				{
					int num13 = Mathf.Clamp(value, 0, m_adaptiveQualityNumLevels - 1);
					if (num13 != nAdaptiveQualityLevel)
					{
						m_nAdaptiveQualityLevel = num13;
						m_nAdaptiveQualityFrameCountLastChanged = Time.frameCount;
					}
				}
			}
			if (Time.frameCount >= m_nAdaptiveQualityFrameCountLastChanged + 2 + 3 && num7 < num4 && num8 < num4 && num9 < num4)
			{
				int num14 = Mathf.Clamp(nAdaptiveQualityLevel + 1, 0, m_adaptiveQualityNumLevels - 1);
				if (num14 != nAdaptiveQualityLevel)
				{
					m_nAdaptiveQualityLevel = num14;
					m_nAdaptiveQualityFrameCountLastChanged = Time.frameCount;
				}
			}
			int nAdaptiveQualityLevel2 = m_nAdaptiveQualityLevel;
			if (s_bAdaptiveQualityVROverride)
			{
				s_nAdaptiveQualityVROverride = Mathf.Clamp(s_nAdaptiveQualityVROverride, 0, m_adaptiveQualityNumLevels - 1);
				nAdaptiveQualityLevel2 = s_nAdaptiveQualityVROverride;
			}
			nAdaptiveQualityLevel2 = Mathf.Clamp(nAdaptiveQualityLevel2, 0, m_adaptiveQualityNumLevels - 1);
			float num15 = 1f;
			if (OpenVR.Compositor != null && OpenVR.System != null && !IsDisplayOnDesktop())
			{
				if (nAdaptiveQualityLevel2 == 0)
				{
					if (m_bInterleavedReprojectionEnabled)
					{
						if (num7 < num2 * 0.85f)
						{
							m_bInterleavedReprojectionEnabled = false;
						}
					}
					else if (num7 > num2 * 0.925f)
					{
						m_bInterleavedReprojectionEnabled = true;
					}
				}
				else
				{
					m_bInterleavedReprojectionEnabled = false;
				}
				OpenVR.Compositor.ForceInterleavedReprojectionOn(m_bInterleavedReprojectionEnabled);
			}
			else if (nAdaptiveQualityLevel2 == 0)
			{
				num15 = 0.8f;
			}
			if (VRSettings.enabled)
			{
				VRSettings.renderScale = num;
				VRSettings.renderViewportScale = m_adaptiveQualityRenderScaleArray[nAdaptiveQualityLevel2] / num * num15;
			}
			Shader.SetGlobalInt("g_nNumBins", m_adaptiveQualityNumLevels);
			Shader.SetGlobalInt("g_nDefaultBin", m_adaptiveQualityDefaultLevel);
			Shader.SetGlobalInt("g_nCurrentBin", nAdaptiveQualityLevel2);
			Shader.SetGlobalInt("g_nLastFrameInBudget", (!m_bInterleavedReprojectionEnabled && !(VRStats.gpuTimeLastFrame > num2)) ? 1 : 0);
		}
	}

	private bool AutoPackLightsIntoShadowTexture()
	{
		List<ValveRealtimeLight> s_allLights = ValveRealtimeLight.s_allLights;
		if (_003C_003Ef__am_0024cache0 == null)
		{
			_003C_003Ef__am_0024cache0 = _003CAutoPackLightsIntoShadowTexture_003Em__0;
		}
		s_allLights.Sort(_003C_003Ef__am_0024cache0);
		int num = 0;
		int num2 = 0;
		int num3 = -1;
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i < ValveRealtimeLight.s_allLights.Count; i++)
		{
			ValveRealtimeLight valveRealtimeLight = ValveRealtimeLight.s_allLights[i];
			valveRealtimeLight.m_bRenderShadowsThisFrame = false;
			if (!valveRealtimeLight.IsEnabled() || !valveRealtimeLight.CastsShadows())
			{
				continue;
			}
			num++;
			if (num > 18)
			{
				break;
			}
			if (num3 == -1 && (valveRealtimeLight.m_shadowResolution > m_shadowDepthTexture.width || valveRealtimeLight.m_shadowResolution > m_shadowDepthTexture.height))
			{
				Debug.LogError("ERROR! Valve shadow packer ran out of space in the " + m_shadowDepthTexture.width + "x" + m_shadowDepthTexture.height + " texture!\n\n");
				m_bFailedToPackLastTime = true;
				return false;
			}
			if (num3 == -1 || num2 + valveRealtimeLight.m_shadowResolution > m_shadowDepthTexture.width)
			{
				num2 = 0;
				num3 = num4;
				num4 += valveRealtimeLight.m_shadowResolution;
			}
			if (num3 + valveRealtimeLight.m_shadowResolution > m_shadowDepthTexture.height)
			{
				Debug.LogError("ERROR! Valve shadow packer ran out of space in the " + m_shadowDepthTexture.width + "x" + m_shadowDepthTexture.height + " texture!\n\n");
				m_bFailedToPackLastTime = true;
				return false;
			}
			valveRealtimeLight.m_shadowX[num5] = num2;
			valveRealtimeLight.m_shadowY[num5] = num3;
			valveRealtimeLight.m_bRenderShadowsThisFrame = true;
			num2 += valveRealtimeLight.m_shadowResolution;
			if (valveRealtimeLight.m_cachedLight.type == LightType.Point)
			{
				num5++;
				if (num5 < 6)
				{
					i--;
				}
				else
				{
					num5 = 0;
				}
			}
		}
		if (m_bFailedToPackLastTime)
		{
			m_bFailedToPackLastTime = false;
			Debug.Log("SUCCESS! Valve shadow packer can now fit all lights into the " + m_shadowDepthTexture.width + "x" + m_shadowDepthTexture.height + " texture!\n\n");
		}
		return (num != 0) ? true : false;
	}

	private void CullLightsAgainstCameraFrustum()
	{
		if (!m_shadowCamera)
		{
			Debug.LogWarning("ERROR! m_shadowCamera == null!\n");
			return;
		}
		Camera component = base.gameObject.GetComponent<Camera>();
		Plane[] array = GeometryUtility.CalculateFrustumPlanes(component);
		for (int i = 0; i < ValveRealtimeLight.s_allLights.Count; i++)
		{
			ValveRealtimeLight valveRealtimeLight = ValveRealtimeLight.s_allLights[i];
			Light cachedLight = valveRealtimeLight.m_cachedLight;
			valveRealtimeLight.m_bInCameraFrustum = true;
			if (!valveRealtimeLight.IsEnabled())
			{
				continue;
			}
			valveRealtimeLight.m_bInCameraFrustum = false;
			if (cachedLight.type == LightType.Directional)
			{
				valveRealtimeLight.m_bInCameraFrustum = true;
			}
			if (cachedLight.type == LightType.Point)
			{
				Bounds bounds = new Bounds(cachedLight.transform.position, new Vector3(cachedLight.range * 2f, cachedLight.range * 2f, cachedLight.range * 2f));
				if (GeometryUtility.TestPlanesAABB(array, bounds))
				{
					valveRealtimeLight.m_bInCameraFrustum = true;
					Vector3 position = cachedLight.transform.position;
					for (int j = 0; j < array.Length; j++)
					{
						float distanceToPoint = array[j].GetDistanceToPoint(position);
						if (distanceToPoint < 0f - cachedLight.range)
						{
							valveRealtimeLight.m_bInCameraFrustum = false;
							break;
						}
					}
				}
			}
			if (cachedLight.type != 0)
			{
				continue;
			}
			Transform transform = cachedLight.transform;
			Vector3 position2 = transform.position;
			Vector3 forward = transform.forward;
			float range = cachedLight.range;
			Vector3 center = position2 + forward * range * 0.5f;
			Bounds bounds2 = new Bounds(center, new Vector3(range, range, range));
			if (!GeometryUtility.TestPlanesAABB(array, bounds2))
			{
				continue;
			}
			Vector3 right = transform.right;
			Vector3 up = transform.up;
			float spotAngle = cachedLight.spotAngle;
			valveRealtimeLight.m_bInCameraFrustum = true;
			float num = Mathf.Tan(spotAngle * ((float)Math.PI / 180f) * 0.5f) * range;
			Vector3[] array2 = new Vector3[6];
			array2[0] = position2;
			array2[1] = position2 + forward * range;
			array2[2] = array2[1] + right * num;
			array2[3] = array2[1] - right * num;
			array2[4] = array2[1] + up * num;
			array2[5] = array2[1] - up * num;
			for (int k = 0; k < array.Length; k++)
			{
				bool flag = false;
				for (int l = 0; l < array2.Length; l++)
				{
					if (array[k].GetDistanceToPoint(array2[l]) > 0f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					valveRealtimeLight.m_bInCameraFrustum = false;
					break;
				}
			}
		}
	}

	private void RenderShadowBuffer()
	{
		if (m_nLastRenderedFrameCount == Time.frameCount)
		{
			return;
		}
		m_nLastRenderedFrameCount = Time.frameCount;
		if (!m_shadowCamera)
		{
			Debug.LogWarning("ERROR! m_shadowCamera == null!\n");
		}
		else if (!m_shadowDepthTexture)
		{
			Debug.LogWarning("ERROR! m_shadowDepthTexture == null!\n");
		}
		else
		{
			if (!AutoPackLightsIntoShadowTexture())
			{
				return;
			}
			m_shadowCamera.targetTexture = m_shadowDepthTexture;
			m_shadowCamera.pixelRect = new Rect(0f, 0f, m_shadowCamera.targetTexture.width, m_shadowCamera.targetTexture.height);
			m_shadowCamera.clearFlags = CameraClearFlags.Depth;
			m_shadowCamera.cullingMask = 0;
			m_shadowCamera.RenderWithShader(m_shaderCastShadows, "DO_NOT_RENDER");
			m_shadowCamera.clearFlags = CameraClearFlags.Nothing;
			m_shadowCamera.cullingMask = -1;
			int num = 0;
			for (int i = 0; i < ValveRealtimeLight.s_allLights.Count; i++)
			{
				ValveRealtimeLight valveRealtimeLight = ValveRealtimeLight.s_allLights[i];
				Light cachedLight = valveRealtimeLight.m_cachedLight;
				if (!valveRealtimeLight.IsEnabled() || !valveRealtimeLight.CastsShadows() || !valveRealtimeLight.m_bRenderShadowsThisFrame)
				{
					continue;
				}
				m_shadowCamera.pixelRect = new Rect(valveRealtimeLight.m_shadowX[num], valveRealtimeLight.m_shadowY[num], valveRealtimeLight.m_shadowResolution, valveRealtimeLight.m_shadowResolution);
				Quaternion rotation = cachedLight.transform.rotation;
				if (cachedLight.type == LightType.Point)
				{
					if (num < 4)
					{
						cachedLight.transform.Rotate(cachedLight.transform.up.normalized, 90f * (float)num, Space.World);
					}
					else
					{
						cachedLight.transform.Rotate(cachedLight.transform.right.normalized, 90f + 180f * (float)(num - 4), Space.World);
					}
				}
				m_shadowCamera.aspect = 1f;
				m_shadowCamera.orthographic = false;
				m_shadowCamera.nearClipPlane = valveRealtimeLight.m_shadowNearClipPlane;
				m_shadowCamera.farClipPlane = cachedLight.range;
				m_shadowCamera.fieldOfView = cachedLight.spotAngle;
				m_shadowCamera.transform.position = cachedLight.transform.position;
				m_shadowCamera.transform.rotation = cachedLight.transform.rotation;
				m_shadowCamera.cullingMask = valveRealtimeLight.m_shadowCastLayerMask;
				m_shadowCamera.useOcclusionCulling = valveRealtimeLight.m_useOcclusionCullingForShadows;
				if (cachedLight.type == LightType.Directional)
				{
					m_shadowCamera.nearClipPlane = 10000f;
					m_shadowCamera.farClipPlane = m_shadowCamera.nearClipPlane + valveRealtimeLight.m_directionalLightShadowRange;
					m_shadowCamera.fieldOfView = 57.29578f * Mathf.Tan(valveRealtimeLight.m_directionalLightShadowRadius / m_shadowCamera.nearClipPlane);
					m_shadowCamera.transform.position = new Vector3(cachedLight.transform.position.x - cachedLight.transform.forward.normalized.x * 10000f, cachedLight.transform.position.y - cachedLight.transform.forward.normalized.y * 10000f, cachedLight.transform.position.z - cachedLight.transform.forward.normalized.z * 10000f);
				}
				if (cachedLight.type == LightType.Point)
				{
					m_shadowCamera.fieldOfView = 90f;
				}
				Matrix4x4 identity = Matrix4x4.identity;
				identity.m00 = 0.5f;
				identity.m11 = 0.5f;
				identity.m22 = 0.5f;
				identity.m03 = 0.5f;
				identity.m13 = 0.5f;
				identity.m23 = 0.5f;
				Matrix4x4 identity2 = Matrix4x4.identity;
				identity2.m00 = (float)valveRealtimeLight.m_shadowResolution / (float)m_shadowDepthTexture.width;
				identity2.m11 = (float)valveRealtimeLight.m_shadowResolution / (float)m_shadowDepthTexture.height;
				identity2.m03 = (float)valveRealtimeLight.m_shadowX[num] / (float)m_shadowDepthTexture.width;
				identity2.m13 = (float)valveRealtimeLight.m_shadowY[num] / (float)m_shadowDepthTexture.height;
				valveRealtimeLight.m_shadowTransform[num] = identity2 * identity * m_shadowCamera.projectionMatrix * m_shadowCamera.worldToCameraMatrix;
				valveRealtimeLight.m_lightCookieTransform[num] = identity * m_shadowCamera.projectionMatrix * m_shadowCamera.worldToCameraMatrix;
				Shader.SetGlobalVector("g_vLightDirWs", new Vector4(cachedLight.transform.forward.normalized.x, cachedLight.transform.forward.normalized.y, cachedLight.transform.forward.normalized.z));
				m_shadowCamera.RenderWithShader(m_shaderCastShadows, "RenderType");
				if (cachedLight.type == LightType.Point)
				{
					cachedLight.transform.rotation = rotation;
					num++;
					if (num < 6)
					{
						i--;
					}
					else
					{
						num = 0;
					}
				}
			}
			ValveGLFlushIfNotReprojecting();
		}
	}

	private void UpdateLightConstants()
	{
		int num = 0;
		int num2 = 0;
		bool flag = false;
		int num3 = 0;
		for (int i = 0; i < ValveRealtimeLight.s_allLights.Count; i++)
		{
			ValveRealtimeLight valveRealtimeLight = ValveRealtimeLight.s_allLights[i];
			Light cachedLight = valveRealtimeLight.m_cachedLight;
			if (!valveRealtimeLight.IsEnabled())
			{
				continue;
			}
			if (cachedLight.type == LightType.Directional)
			{
				num2++;
				if (num2 > 18)
				{
					continue;
				}
				float num4 = ((!(cachedLight.intensity <= 1f)) ? (cachedLight.intensity * cachedLight.intensity) : cachedLight.intensity);
				g_vLightColor[num] = new Vector4(cachedLight.color.linear.r * num4, cachedLight.color.linear.g * num4, cachedLight.color.linear.b * num4);
				g_vLightPosition_flInvRadius[num] = new Vector4(cachedLight.transform.position.x - cachedLight.transform.forward.normalized.x * 10000f, cachedLight.transform.position.y - cachedLight.transform.forward.normalized.y * 10000f, cachedLight.transform.position.z - cachedLight.transform.forward.normalized.z * 10000f, -1f);
				g_vLightDirection[num] = new Vector4(cachedLight.transform.forward.normalized.x, cachedLight.transform.forward.normalized.y, cachedLight.transform.forward.normalized.z);
				g_vLightShadowIndex_vLightParams[num] = new Vector4(0f, 0f, 1f, 1f);
				g_vLightFalloffParams[num] = new Vector4(0f, 0f, float.MaxValue);
				g_vSpotLightInnerOuterConeCosines[num] = new Vector4(0f, -1f, 1f);
				if (cachedLight.shadows != 0 && valveRealtimeLight.m_bRenderShadowsThisFrame)
				{
					g_vLightShadowIndex_vLightParams[num].x = 1f;
					g_matWorldToShadow[num] = valveRealtimeLight.m_shadowTransform[num3].transpose;
					g_vShadowMinMaxUv[num] = new Vector4(0f, 0f, 1f, 1f);
				}
				num++;
			}
			if (cachedLight.type == LightType.Point)
			{
				num2++;
				if (num2 > 18)
				{
					continue;
				}
				float num5 = ((!(cachedLight.intensity <= 1f)) ? (cachedLight.intensity * cachedLight.intensity) : cachedLight.intensity);
				g_vLightColor[num] = new Vector4(cachedLight.color.linear.r * num5, cachedLight.color.linear.g * num5, cachedLight.color.linear.b * num5);
				g_vLightPosition_flInvRadius[num] = new Vector4(cachedLight.transform.position.x, cachedLight.transform.position.y, cachedLight.transform.position.z, 1f / cachedLight.range);
				g_vLightDirection[num] = new Vector4(0f, 0f, 0f);
				g_vLightShadowIndex_vLightParams[num] = new Vector4(0f, 0f, 1f, 1f);
				g_vLightFalloffParams[num] = new Vector4(1f, 0f, cachedLight.range * cachedLight.range);
				g_vSpotLightInnerOuterConeCosines[num] = new Vector4(0f, -1f, 1f);
				if (cachedLight.shadows != 0 && valveRealtimeLight.m_bRenderShadowsThisFrame)
				{
					flag = true;
					Quaternion rotation = cachedLight.transform.rotation;
					if (num3 < 4)
					{
						cachedLight.transform.Rotate(cachedLight.transform.up.normalized, 90f * (float)num3, Space.World);
					}
					else
					{
						cachedLight.transform.Rotate(cachedLight.transform.right.normalized, 90f + 180f * (float)(num3 - 4), Space.World);
					}
					g_vLightDirection[num] = new Vector4(cachedLight.transform.forward.normalized.x, cachedLight.transform.forward.normalized.y, cachedLight.transform.forward.normalized.z);
					g_vLightShadowIndex_vLightParams[num].x = 1f;
					g_vLightShadowIndex_vLightParams[num].y = 2f;
					g_matWorldToShadow[num] = valveRealtimeLight.m_shadowTransform[num3].transpose;
					g_matWorldToLightCookie[num] = valveRealtimeLight.m_lightCookieTransform[num3].transpose;
					g_vSpotLightInnerOuterConeCosines[num] = new Vector4(0f, 0.574f, 9999999f);
					g_vShadowMinMaxUv[num] = new Vector4((float)(valveRealtimeLight.m_shadowX[num3] + 1) / (float)m_shadowDepthTexture.width, (float)(valveRealtimeLight.m_shadowY[num3] + 1) / (float)m_shadowDepthTexture.height, (float)(valveRealtimeLight.m_shadowX[num3] + valveRealtimeLight.m_shadowResolution - 1) / (float)m_shadowDepthTexture.width, (float)(valveRealtimeLight.m_shadowY[num3] + valveRealtimeLight.m_shadowResolution - 1) / (float)m_shadowDepthTexture.height);
					cachedLight.transform.rotation = rotation;
					num3++;
					if (num3 < 6)
					{
						i--;
					}
					else
					{
						num3 = 0;
					}
				}
				num++;
			}
			if (cachedLight.type != 0)
			{
				continue;
			}
			num2++;
			if (num2 <= 18)
			{
				float num6 = ((!(cachedLight.intensity <= 1f)) ? (cachedLight.intensity * cachedLight.intensity) : cachedLight.intensity);
				g_vLightColor[num] = new Vector4(cachedLight.color.linear.r * num6, cachedLight.color.linear.g * num6, cachedLight.color.linear.b * num6);
				g_vLightPosition_flInvRadius[num] = new Vector4(cachedLight.transform.position.x, cachedLight.transform.position.y, cachedLight.transform.position.z, 1f / cachedLight.range);
				g_vLightDirection[num] = new Vector4(cachedLight.transform.forward.normalized.x, cachedLight.transform.forward.normalized.y, cachedLight.transform.forward.normalized.z);
				g_vLightShadowIndex_vLightParams[num] = new Vector4(0f, 0f, 1f, 1f);
				g_vLightFalloffParams[num] = new Vector4(1f, 0f, cachedLight.range * cachedLight.range);
				float num7 = Mathf.Clamp(valveRealtimeLight.m_innerSpotPercent, 0f, 100f) / 100f;
				float num8 = Mathf.Clamp(Mathf.Cos(cachedLight.spotAngle * 0.5f * ((float)Math.PI / 180f)), 0f, 1f);
				float num9 = Mathf.Clamp(Mathf.Cos(cachedLight.spotAngle * 0.5f * num7 * ((float)Math.PI / 180f)), 0f, 1f);
				g_vSpotLightInnerOuterConeCosines[num] = new Vector4(num9, num8, 1f / Mathf.Max(0.01f, num9 - num8));
				if (cachedLight.shadows != 0 && valveRealtimeLight.m_bRenderShadowsThisFrame)
				{
					g_vLightShadowIndex_vLightParams[num].x = 1f;
					g_matWorldToShadow[num] = valveRealtimeLight.m_shadowTransform[num3].transpose;
					g_vShadowMinMaxUv[num] = new Vector4(0f, 0f, 1f, 1f);
				}
				num++;
			}
		}
		if (num2 > 18)
		{
			if (num2 > m_nWarnedTooManyLights)
			{
				Debug.LogWarning("Warning! Found " + num2 + " runtime lights! Valve renderer supports up to " + 18 + " active runtime lights at a time!\nDisabling " + (num2 - 18) + " runtime light" + ((num2 - 18 <= 1) ? string.Empty : "s") + "!\n");
			}
			m_nWarnedTooManyLights = num2;
		}
		else if (m_nWarnedTooManyLights > 0)
		{
			m_nWarnedTooManyLights = 0;
			Debug.Log("SUCCESS! Found " + num2 + " runtime lights which is within the supported number of lights, " + 18 + ".\n\n");
		}
		Shader.SetGlobalInt("g_nNumLights", num);
		Shader.SetGlobalVectorArray("g_vLightPosition_flInvRadius", g_vLightPosition_flInvRadius);
		Shader.SetGlobalVectorArray("g_vLightColor", g_vLightColor);
		Shader.SetGlobalVectorArray("g_vLightDirection", g_vLightDirection);
		Shader.SetGlobalVectorArray("g_vLightShadowIndex_vLightParams", g_vLightShadowIndex_vLightParams);
		Shader.SetGlobalVectorArray("g_vLightFalloffParams", g_vLightFalloffParams);
		Shader.SetGlobalVectorArray("g_vSpotLightInnerOuterConeCosines", g_vSpotLightInnerOuterConeCosines);
		Shader.SetGlobalVectorArray("g_vShadowMinMaxUv", g_vShadowMinMaxUv);
		Shader.SetGlobalMatrixArray("g_matWorldToShadow", g_matWorldToShadow);
		Shader.SetGlobalMatrixArray("g_matWorldToLightCookie", g_matWorldToLightCookie);
		if (flag)
		{
			Shader.EnableKeyword("D_VALVE_SHADOWING_POINT_LIGHTS");
		}
		else
		{
			Shader.DisableKeyword("D_VALVE_SHADOWING_POINT_LIGHTS");
		}
		Shader.SetGlobalFloat("g_flTime", Time.timeSinceLevelLoad);
		if ((bool)m_shadowDepthTexture)
		{
			float num10 = 1f / (float)m_shadowDepthTexture.width;
			float num11 = 1f / (float)m_shadowDepthTexture.height;
			Vector4 value = new Vector4(0.074906364f, 0.123595506f, 0.2059925f, 0f);
			Vector4 value2 = new Vector4(num10, num11, 0f - num10, 0f - num11);
			Vector4 value3 = new Vector4(num10, num11, 0f, 0f);
			Vector4 value4 = new Vector4(0f - num10, 0f - num11, 0f, 0f);
			Shader.SetGlobalVector("g_vShadow3x3PCFTerms0", value);
			Shader.SetGlobalVector("g_vShadow3x3PCFTerms1", value2);
			Shader.SetGlobalVector("g_vShadow3x3PCFTerms2", value3);
			Shader.SetGlobalVector("g_vShadow3x3PCFTerms3", value4);
		}
		Shader.SetGlobalFloat("g_flValveGlobalVertexScale", (!m_hideAllValveMaterials) ? 1f : 0f);
		Shader.SetGlobalInt("g_bIndirectLightmaps", m_indirectLightmapsOnly ? 1 : 0);
	}

	private bool IsDisplayOnDesktop()
	{
		if (!m_isDisplayOnDesktopCached && OpenVR.System != null)
		{
			m_isDisplayOnDesktop = OpenVR.System.IsDisplayOnDesktop();
			m_isDisplayOnDesktopCached = true;
		}
		return m_isDisplayOnDesktop;
	}

	[CompilerGenerated]
	private static int _003CAutoPackLightsIntoShadowTexture_003Em__0(ValveRealtimeLight vl1, ValveRealtimeLight vl2)
	{
		int num = 0;
		bool flag = vl1.CastsShadows();
		bool flag2 = vl2.CastsShadows();
		num = ((!flag || !flag2) ? flag2.CompareTo(flag) : vl2.m_shadowResolution.CompareTo(vl1.m_shadowResolution));
		if (num == 0)
		{
			num = vl2.m_cachedLight.range.CompareTo(vl1.m_cachedLight.range);
		}
		if (num == 0)
		{
			num = vl2.GetInstanceID().CompareTo(vl1.GetInstanceID());
		}
		return num;
	}
}
