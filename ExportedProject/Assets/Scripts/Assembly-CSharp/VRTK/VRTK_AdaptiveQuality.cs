using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.VR;

namespace VRTK
{
	public sealed class VRTK_AdaptiveQuality : MonoBehaviour
	{
		private sealed class AdaptiveSetting<T>
		{
			public readonly int increaseFrameCost;

			public readonly int decreaseFrameCost;

			private T _currentValue;

			public T currentValue
			{
				get
				{
					return _currentValue;
				}
				set
				{
					if (!EqualityComparer<T>.Default.Equals(value, _currentValue))
					{
						lastChangeFrameCount = Time.frameCount;
					}
					previousValue = _currentValue;
					_currentValue = value;
				}
			}

			public T previousValue { get; private set; }

			public int lastChangeFrameCount { get; private set; }

			public AdaptiveSetting(T currentValue, int increaseFrameCost, int decreaseFrameCost)
			{
				previousValue = currentValue;
				this.currentValue = currentValue;
				this.increaseFrameCost = increaseFrameCost;
				this.decreaseFrameCost = decreaseFrameCost;
			}
		}

		private static class CommandLineArguments
		{
			public const string Disable = "-noaq";

			public const string MinimumRenderScale = "-aqminscale";

			public const string MaximumRenderScale = "-aqmaxscale";

			public const string MaximumRenderTargetDimension = "-aqmaxres";

			public const string RenderScaleFillRateStepSizeInPercent = "-aqfillratestep";

			public const string OverrideRenderScaleLevel = "-aqoverride";

			public const string DrawDebugVisualization = "-vrdebug";

			public const string MSAALevel = "-msaa";
		}

		private static class KeyboardShortcuts
		{
			public static readonly KeyCode[] Modifiers = new KeyCode[2]
			{
				KeyCode.LeftShift,
				KeyCode.RightShift
			};

			public const KeyCode ToggleDrawDebugVisualization = KeyCode.F1;

			public const KeyCode ToggleOverrideRenderScale = KeyCode.F2;

			public const KeyCode DecreaseOverrideRenderScaleLevel = KeyCode.F3;

			public const KeyCode IncreaseOverrideRenderScaleLevel = KeyCode.F4;
		}

		private static class ShaderPropertyIDs
		{
			public static readonly int RenderScaleLevelsCount = Shader.PropertyToID("_RenderScaleLevelsCount");

			public static readonly int DefaultRenderViewportScaleLevel = Shader.PropertyToID("_DefaultRenderViewportScaleLevel");

			public static readonly int CurrentRenderViewportScaleLevel = Shader.PropertyToID("_CurrentRenderViewportScaleLevel");

			public static readonly int CurrentRenderScaleLevel = Shader.PropertyToID("_CurrentRenderScaleLevel");

			public static readonly int LastFrameIsInBudget = Shader.PropertyToID("_LastFrameIsInBudget");
		}

		private sealed class Timing
		{
			private readonly float[] buffer = new float[12];

			private int bufferIndex;

			public void SaveCurrentFrameTiming()
			{
				bufferIndex = (bufferIndex + 1) % buffer.Length;
				buffer[bufferIndex] = VRStats.gpuTimeLastFrame;
			}

			public float GetFrameTiming(int framesAgo)
			{
				return buffer[(bufferIndex - framesAgo + buffer.Length) % buffer.Length];
			}

			public bool WasFrameTimingBad(int framesAgo, float thresholdInMilliseconds, int lastChangeFrameCount, int changeFrameCost)
			{
				if (!AreFramesAvailable(framesAgo, lastChangeFrameCount, changeFrameCost))
				{
					return false;
				}
				for (int i = 0; i < framesAgo; i++)
				{
					if (GetFrameTiming(i) <= thresholdInMilliseconds)
					{
						return false;
					}
				}
				return true;
			}

			public bool WasFrameTimingGood(int framesAgo, float thresholdInMilliseconds, int lastChangeFrameCount, int changeFrameCost)
			{
				if (!AreFramesAvailable(framesAgo, lastChangeFrameCount, changeFrameCost))
				{
					return false;
				}
				for (int i = 0; i < framesAgo; i++)
				{
					if (GetFrameTiming(i) > thresholdInMilliseconds)
					{
						return false;
					}
				}
				return true;
			}

			public bool WillFrameTimingBeBad(float extrapolationThresholdInMilliseconds, float thresholdInMilliseconds, int lastChangeFrameCount, int changeFrameCost)
			{
				if (!AreFramesAvailable(2, lastChangeFrameCount, changeFrameCost))
				{
					return false;
				}
				float frameTiming = GetFrameTiming(0);
				if (frameTiming <= extrapolationThresholdInMilliseconds)
				{
					return false;
				}
				float num = frameTiming - GetFrameTiming(1);
				if (!AreFramesAvailable(3, lastChangeFrameCount, changeFrameCost))
				{
					num = Mathf.Max(num, (frameTiming - GetFrameTiming(2)) / 2f);
				}
				return frameTiming + num > thresholdInMilliseconds;
			}

			private static bool AreFramesAvailable(int framesAgo, int lastChangeFrameCount, int changeFrameCost)
			{
				return Time.frameCount >= framesAgo + lastChangeFrameCount + changeFrameCost;
			}
		}

		[Tooltip("Toggles whether to show the debug overlay.\n\nEach square represents a different level on the quality scale. Levels increase from left to right, the first green box that is lit above represents the recommended render target resolution provided by the current `VRDevice`, the box that is lit below in cyan represents the current resolution and the filled box represents the current viewport scale. The yellow boxes represent resolutions below the recommended render target resolution.\nThe currently lit box becomes red whenever the user is likely seeing reprojection in the HMD since the application isn't maintaining VR framerate. If lit, the box all the way on the left is almost always lit red because it represents the lowest render scale with reprojection on.")]
		public bool drawDebugVisualization;

		[Tooltip("Toggles whether to allow keyboard shortcuts to control this script.\n\n* The supported shortcuts are:\n * `Shift+F1`: Toggle debug visualization on/off\n * `Shift+F2`: Toggle usage of override render scale on/off\n * `Shift+F3`: Decrease override render scale level\n * `Shift+F4`: Increase override render scale level")]
		public bool allowKeyboardShortcuts = true;

		[Tooltip("Toggles whether to allow command line arguments to control this script at startup of the standalone build.\n\n* The supported command line arguments all begin with '-' and are:\n * `-noaq`: Disable adaptive quality\n * `-aqminscale X`: Set minimum render scale to X\n * `-aqmaxscale X`: Set maximum render scale to X\n * `-aqmaxres X`: Set maximum render target dimension to X\n * `-aqfillratestep X`: Set render scale fill rate step size in percent to X (X from 1 to 100)\n * `-aqoverride X`: Set override render scale level to X\n * `-vrdebug`: Enable debug visualization\n * `-msaa X`: Set MSAA level to X")]
		public bool allowCommandLineArguments = true;

		[Tooltip("The MSAA level to use.")]
		[Header("Quality")]
		[Range(0f, 8f)]
		public int msaaLevel = 4;

		[Tooltip("Toggles whether the render viewport scale is dynamically adjusted to maintain VR framerate.\n\nIf unchecked, the renderer will render at the recommended resolution provided by the current `VRDevice`.")]
		public bool scaleRenderViewport = true;

		[Tooltip("The minimum allowed render scale.")]
		[Range(0.01f, 5f)]
		public float minimumRenderScale = 0.8f;

		[Tooltip("The maximum allowed render scale.")]
		public float maximumRenderScale = 1.4f;

		[Tooltip("The maximum allowed render target dimension.\n\nThis puts an upper limit on the size of the render target regardless of the maximum render scale.")]
		public int maximumRenderTargetDimension = 4096;

		[Tooltip("The fill rate step size in percent by which the render scale levels will be calculated.")]
		[Range(1f, 100f)]
		public int renderScaleFillRateStepSizeInPercent = 15;

		[Tooltip("Toggles whether the render target resolution is dynamically adjusted to maintain VR framerate.\n\nIf unchecked, the renderer will use the maximum target resolution specified by `maximumRenderScale`.")]
		public bool scaleRenderTargetResolution;

		[NonSerialized]
		[Tooltip("Toggles whether to override the used render viewport scale level.")]
		[Header("Override")]
		public bool overrideRenderViewportScale;

		[NonSerialized]
		[Tooltip("The render viewport scale level to override the current one with.")]
		public int overrideRenderViewportScaleLevel;

		public readonly ReadOnlyCollection<float> renderScales;

		private const float DefaultFrameDurationInMilliseconds = 11.111111f;

		private readonly AdaptiveSetting<int> renderViewportScaleSetting = new AdaptiveSetting<int>(0, 30, 10);

		private readonly AdaptiveSetting<int> renderScaleSetting = new AdaptiveSetting<int>(0, 180, 90);

		private readonly List<float> allRenderScales = new List<float>();

		private int defaultRenderViewportScaleLevel;

		private float previousMinimumRenderScale;

		private float previousMaximumRenderScale;

		private float previousRenderScaleFillRateStepSizeInPercent;

		private readonly Timing timing = new Timing();

		private int lastRenderViewportScaleLevelBelowRenderScaleLevelFrameCount;

		private bool interleavedReprojectionEnabled;

		private bool hmdDisplayIsOnDesktop;

		private float singleFrameDurationInMilliseconds;

		private GameObject debugVisualizationQuad;

		private Material debugVisualizationQuadMaterial;

		[CompilerGenerated]
		private static Func<KeyCode, bool> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Predicate<float> _003C_003Ef__am_0024cache0;

		public static float CurrentRenderScale
		{
			get
			{
				return VRSettings.renderScale * VRSettings.renderViewportScale;
			}
		}

		public Vector2 defaultRenderTargetResolution
		{
			get
			{
				return RenderTargetResolutionForRenderScale(allRenderScales[defaultRenderViewportScaleLevel]);
			}
		}

		public Vector2 currentRenderTargetResolution
		{
			get
			{
				return RenderTargetResolutionForRenderScale(CurrentRenderScale);
			}
		}

		public VRTK_AdaptiveQuality()
		{
			renderScales = allRenderScales.AsReadOnly();
		}

		public static Vector2 RenderTargetResolutionForRenderScale(float renderScale)
		{
			return new Vector2((int)((float)VRSettings.eyeTextureWidth / VRSettings.renderScale * renderScale), (int)((float)VRSettings.eyeTextureHeight / VRSettings.renderScale * renderScale));
		}

		public float BiggestAllowedMaximumRenderScale()
		{
			if (VRSettings.eyeTextureWidth == 0 || VRSettings.eyeTextureHeight == 0)
			{
				return maximumRenderScale;
			}
			float a = (float)maximumRenderTargetDimension * VRSettings.renderScale / (float)VRSettings.eyeTextureWidth;
			float b = (float)maximumRenderTargetDimension * VRSettings.renderScale / (float)VRSettings.eyeTextureHeight;
			return Mathf.Min(a, b);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("Adaptive Quality\n");
			stringBuilder.AppendLine("Render Scale:");
			stringBuilder.AppendLine("Level - Resolution - Multiplier");
			for (int i = 0; i < allRenderScales.Count; i++)
			{
				float num = allRenderScales[i];
				Vector2 vector = RenderTargetResolutionForRenderScale(num);
				stringBuilder.AppendFormat("{0, 3} {1, 5}x{2, -5} {3, -8}", i, (int)vector.x, (int)vector.y, num);
				if (i == 0)
				{
					stringBuilder.Append(" (Interleaved reprojection hint)");
				}
				else if (i == defaultRenderViewportScaleLevel)
				{
					stringBuilder.Append(" (Default)");
				}
				if (i == renderViewportScaleSetting.currentValue)
				{
					stringBuilder.Append(" (Current Viewport)");
				}
				if (i == renderScaleSetting.currentValue)
				{
					stringBuilder.Append(" (Current Target Resolution)");
				}
				if (i != allRenderScales.Count - 1)
				{
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		private void OnEnable()
		{
			Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(OnCameraPreCull));
			hmdDisplayIsOnDesktop = VRTK_SDK_Bridge.IsDisplayOnDesktop();
			singleFrameDurationInMilliseconds = ((!(VRDevice.refreshRate > 0f)) ? 11.111111f : (1000f / VRDevice.refreshRate));
			HandleCommandLineArguments();
			if (!Application.isEditor)
			{
				OnValidate();
			}
		}

		private void OnDisable()
		{
			Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(OnCameraPreCull));
			SetRenderScale(1f, 1f);
		}

		private void OnValidate()
		{
			minimumRenderScale = Mathf.Max(0.01f, minimumRenderScale);
			maximumRenderScale = Mathf.Max(minimumRenderScale, maximumRenderScale);
			maximumRenderTargetDimension = Mathf.Max(2, maximumRenderTargetDimension);
			renderScaleFillRateStepSizeInPercent = Mathf.Max(1, renderScaleFillRateStepSizeInPercent);
			msaaLevel = ((msaaLevel != 1) ? Mathf.Clamp(Mathf.ClosestPowerOfTwo(msaaLevel), 0, 8) : 0);
		}

		private void Update()
		{
			HandleKeyPresses();
			UpdateRenderScaleLevels();
			CreateOrDestroyDebugVisualization();
			UpdateDebugVisualization();
			timing.SaveCurrentFrameTiming();
		}

		private void LateUpdate()
		{
			UpdateRenderScale();
		}

		private void OnCameraPreCull(Camera camera)
		{
			if (!(camera.transform != VRTK_SDK_Bridge.GetHeadsetCamera()))
			{
				UpdateMSAALevel();
			}
		}

		private void HandleCommandLineArguments()
		{
			if (!allowCommandLineArguments)
			{
				return;
			}
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				string text = commandLineArgs[i];
				string s = ((i + 1 >= commandLineArgs.Length) ? string.Empty : commandLineArgs[i + 1]);
				switch (text)
				{
				case "-noaq":
					scaleRenderViewport = false;
					break;
				case "-aqminscale":
					minimumRenderScale = float.Parse(s);
					break;
				case "-aqmaxscale":
					maximumRenderScale = float.Parse(s);
					break;
				case "-aqmaxres":
					maximumRenderTargetDimension = int.Parse(s);
					break;
				case "-aqfillratestep":
					renderScaleFillRateStepSizeInPercent = int.Parse(s);
					break;
				case "-aqoverride":
					overrideRenderViewportScale = true;
					overrideRenderViewportScaleLevel = int.Parse(s);
					break;
				case "-vrdebug":
					drawDebugVisualization = true;
					break;
				case "-msaa":
					msaaLevel = int.Parse(s);
					break;
				}
			}
		}

		private void HandleKeyPresses()
		{
			if (!allowKeyboardShortcuts)
			{
				return;
			}
			KeyCode[] modifiers = KeyboardShortcuts.Modifiers;
			if (_003C_003Ef__mg_0024cache0 == null)
			{
				_003C_003Ef__mg_0024cache0 = Input.GetKey;
			}
			if (modifiers.Any(_003C_003Ef__mg_0024cache0))
			{
				if (Input.GetKeyDown(KeyCode.F1))
				{
					drawDebugVisualization = !drawDebugVisualization;
				}
				else if (Input.GetKeyDown(KeyCode.F2))
				{
					overrideRenderViewportScale = !overrideRenderViewportScale;
				}
				else if (Input.GetKeyDown(KeyCode.F3))
				{
					overrideRenderViewportScaleLevel = ClampRenderScaleLevel(overrideRenderViewportScaleLevel - 1);
				}
				else if (Input.GetKeyDown(KeyCode.F4))
				{
					overrideRenderViewportScaleLevel = ClampRenderScaleLevel(overrideRenderViewportScaleLevel + 1);
				}
			}
		}

		private void UpdateMSAALevel()
		{
			if (QualitySettings.antiAliasing != msaaLevel)
			{
				QualitySettings.antiAliasing = msaaLevel;
			}
		}

		private void UpdateRenderScaleLevels()
		{
			if (Mathf.Abs(previousMinimumRenderScale - minimumRenderScale) <= float.Epsilon && Mathf.Abs(previousMaximumRenderScale - maximumRenderScale) <= float.Epsilon && Mathf.Abs(previousRenderScaleFillRateStepSizeInPercent - (float)renderScaleFillRateStepSizeInPercent) <= float.Epsilon)
			{
				return;
			}
			previousMinimumRenderScale = minimumRenderScale;
			previousMaximumRenderScale = maximumRenderScale;
			previousRenderScaleFillRateStepSizeInPercent = renderScaleFillRateStepSizeInPercent;
			allRenderScales.Clear();
			float num = BiggestAllowedMaximumRenderScale();
			float num2 = Mathf.Min(minimumRenderScale, num);
			allRenderScales.Add(num2);
			while (num2 <= maximumRenderScale)
			{
				allRenderScales.Add(num2);
				num2 = Mathf.Sqrt((float)(renderScaleFillRateStepSizeInPercent + 100) / 100f * num2 * num2);
				if (num2 > num)
				{
					break;
				}
			}
			List<float> list = allRenderScales;
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CUpdateRenderScaleLevels_003Em__0;
			}
			defaultRenderViewportScaleLevel = Mathf.Clamp(list.FindIndex(_003C_003Ef__am_0024cache0), 1, allRenderScales.Count - 1);
			renderViewportScaleSetting.currentValue = defaultRenderViewportScaleLevel;
			renderScaleSetting.currentValue = defaultRenderViewportScaleLevel;
			overrideRenderViewportScaleLevel = ClampRenderScaleLevel(overrideRenderViewportScaleLevel);
		}

		private void UpdateRenderScale()
		{
			if (!scaleRenderViewport)
			{
				renderViewportScaleSetting.currentValue = defaultRenderViewportScaleLevel;
				renderScaleSetting.currentValue = defaultRenderViewportScaleLevel;
				SetRenderScale(1f, 1f);
				return;
			}
			float num = ((!VRTK_SDK_Bridge.ShouldAppRenderWithLowResources()) ? singleFrameDurationInMilliseconds : (singleFrameDurationInMilliseconds * 0.75f));
			float thresholdInMilliseconds = 0.7f * num;
			float extrapolationThresholdInMilliseconds = 0.85f * num;
			float thresholdInMilliseconds2 = 0.9f * num;
			int num2 = renderViewportScaleSetting.currentValue;
			if (timing.WasFrameTimingBad(1, thresholdInMilliseconds2, renderViewportScaleSetting.lastChangeFrameCount, renderViewportScaleSetting.decreaseFrameCost) || timing.WasFrameTimingBad(3, thresholdInMilliseconds2, renderViewportScaleSetting.lastChangeFrameCount, renderViewportScaleSetting.decreaseFrameCost) || timing.WillFrameTimingBeBad(extrapolationThresholdInMilliseconds, thresholdInMilliseconds2, renderViewportScaleSetting.lastChangeFrameCount, renderViewportScaleSetting.decreaseFrameCost))
			{
				num2 = ClampRenderScaleLevel((renderViewportScaleSetting.currentValue == 2) ? 1 : (renderViewportScaleSetting.currentValue - 2));
			}
			else if (timing.WasFrameTimingGood(12, thresholdInMilliseconds, renderViewportScaleSetting.lastChangeFrameCount - renderViewportScaleSetting.increaseFrameCost, renderViewportScaleSetting.increaseFrameCost))
			{
				num2 = ClampRenderScaleLevel(renderViewportScaleSetting.currentValue + 2);
			}
			else if (timing.WasFrameTimingGood(6, thresholdInMilliseconds, renderViewportScaleSetting.lastChangeFrameCount, renderViewportScaleSetting.increaseFrameCost))
			{
				num2 = ClampRenderScaleLevel(renderViewportScaleSetting.currentValue + 1);
			}
			if (num2 != renderViewportScaleSetting.currentValue)
			{
				if (renderViewportScaleSetting.currentValue >= renderScaleSetting.currentValue && num2 < renderScaleSetting.currentValue)
				{
					lastRenderViewportScaleLevelBelowRenderScaleLevelFrameCount = Time.frameCount;
				}
				renderViewportScaleSetting.currentValue = num2;
			}
			if (overrideRenderViewportScale)
			{
				renderViewportScaleSetting.currentValue = overrideRenderViewportScaleLevel;
			}
			float num3 = 1f;
			if (!hmdDisplayIsOnDesktop)
			{
				if (renderViewportScaleSetting.currentValue == 0)
				{
					if (interleavedReprojectionEnabled && timing.GetFrameTiming(0) < singleFrameDurationInMilliseconds * 0.85f)
					{
						interleavedReprojectionEnabled = false;
					}
					else if (timing.GetFrameTiming(0) > singleFrameDurationInMilliseconds * 0.925f)
					{
						interleavedReprojectionEnabled = true;
					}
				}
				else
				{
					interleavedReprojectionEnabled = false;
				}
				VRTK_SDK_Bridge.ForceInterleavedReprojectionOn(interleavedReprojectionEnabled);
			}
			else if (renderViewportScaleSetting.currentValue == 0)
			{
				num3 = 0.8f;
			}
			int currentValue = renderScaleSetting.currentValue;
			int b = (renderViewportScaleSetting.currentValue - renderScaleSetting.currentValue) / 2;
			if (renderScaleSetting.currentValue < renderViewportScaleSetting.currentValue && Time.frameCount >= renderScaleSetting.lastChangeFrameCount + renderScaleSetting.increaseFrameCost)
			{
				currentValue = ClampRenderScaleLevel(renderScaleSetting.currentValue + Mathf.Max(1, b));
			}
			else if (renderScaleSetting.currentValue > renderViewportScaleSetting.currentValue && Time.frameCount >= renderScaleSetting.lastChangeFrameCount + renderScaleSetting.decreaseFrameCost && Time.frameCount >= lastRenderViewportScaleLevelBelowRenderScaleLevelFrameCount + renderViewportScaleSetting.increaseFrameCost)
			{
				currentValue = ((!timing.WasFrameTimingGood(6, thresholdInMilliseconds, 0, 0)) ? renderViewportScaleSetting.currentValue : ClampRenderScaleLevel(renderScaleSetting.currentValue + Mathf.Min(-1, b)));
			}
			renderScaleSetting.currentValue = currentValue;
			if (!scaleRenderTargetResolution)
			{
				renderScaleSetting.currentValue = allRenderScales.Count - 1;
			}
			float num4 = allRenderScales[renderScaleSetting.currentValue];
			float renderViewportScale = allRenderScales[Mathf.Min(renderViewportScaleSetting.currentValue, renderScaleSetting.currentValue)] / num4 * num3;
			SetRenderScale(num4, renderViewportScale);
		}

		private static void SetRenderScale(float renderScale, float renderViewportScale)
		{
			if (Mathf.Abs(VRSettings.renderScale - renderScale) > float.Epsilon)
			{
				VRSettings.renderScale = renderScale;
			}
			if (Mathf.Abs(VRSettings.renderViewportScale - renderViewportScale) > float.Epsilon)
			{
				VRSettings.renderViewportScale = renderViewportScale;
			}
		}

		private int ClampRenderScaleLevel(int renderScaleLevel)
		{
			return Mathf.Clamp(renderScaleLevel, 0, allRenderScales.Count - 1);
		}

		private void CreateOrDestroyDebugVisualization()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (base.enabled && drawDebugVisualization && debugVisualizationQuad == null)
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
				Mesh mesh2 = mesh;
				mesh2.UploadMeshData(true);
				debugVisualizationQuad = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, "AdaptiveQualityDebugVisualizationQuad"));
				if ((bool)VRTK_SDKManager.instance)
				{
					debugVisualizationQuad.transform.SetParent(VRTK_DeviceFinder.HeadsetTransform());
				}
				debugVisualizationQuad.transform.localPosition = Vector3.forward;
				debugVisualizationQuad.transform.localRotation = Quaternion.identity;
				debugVisualizationQuad.AddComponent<MeshFilter>().mesh = mesh2;
				debugVisualizationQuadMaterial = Resources.Load<Material>("AdaptiveQualityDebugVisualization");
				debugVisualizationQuad.AddComponent<MeshRenderer>().material = debugVisualizationQuadMaterial;
			}
			else if ((!base.enabled || !drawDebugVisualization) && debugVisualizationQuad != null)
			{
				UnityEngine.Object.Destroy(debugVisualizationQuad);
				debugVisualizationQuad = null;
				debugVisualizationQuadMaterial = null;
			}
		}

		private void UpdateDebugVisualization()
		{
			if (drawDebugVisualization && !(debugVisualizationQuadMaterial == null))
			{
				int value = ((!interleavedReprojectionEnabled && !(VRStats.gpuTimeLastFrame > singleFrameDurationInMilliseconds)) ? 1 : 0);
				debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.RenderScaleLevelsCount, allRenderScales.Count);
				debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.DefaultRenderViewportScaleLevel, defaultRenderViewportScaleLevel);
				debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.CurrentRenderViewportScaleLevel, renderViewportScaleSetting.currentValue);
				debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.CurrentRenderScaleLevel, renderScaleSetting.currentValue);
				debugVisualizationQuadMaterial.SetInt(ShaderPropertyIDs.LastFrameIsInBudget, value);
			}
		}

		[CompilerGenerated]
		private static bool _003CUpdateRenderScaleLevels_003Em__0(float renderScale)
		{
			return renderScale >= 1f;
		}
	}
}
