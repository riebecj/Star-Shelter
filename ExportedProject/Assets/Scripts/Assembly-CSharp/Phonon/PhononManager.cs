using System;
using UnityEngine;

namespace Phonon
{
	public class PhononManager : MonoBehaviour
	{
		public PhononMaterialPreset materialPreset;

		public PhononMaterialValue materialValue;

		public SimulationSettingsPreset simulationPreset;

		public SimulationSettingsValue simulationValue;

		public AudioEngine audioEngine;

		public bool updateComponents = true;

		public bool showLoadTimeOptions;

		public bool showMassBakingOptions;

		public UnityEngine.Object currentlyBakingObject;

		private PhononManagerContainer phononContainer = new PhononManagerContainer();

		private AudioListener audioListener;

		private PhononListener phononListener;

		private PhononStaticListener phononStaticListener;

		private CustomSpeakerLayout customSpeakerLayout;

		private CustomPhononSettings customPhononSettings;

		private bool isSetPhononStaticListener;

		private bool isSetCustomSpeakerLayout;

		private bool isSetCustomPhononSettings;

		private bool isInitialized;

		private void Awake()
		{
			bool initializeRenderer = true;
			Initialize(initializeRenderer);
			phononContainer.Initialize(initializeRenderer, this);
		}

		private void OnDestroy()
		{
			Destroy();
			phononContainer.Destroy();
		}

		private void Update()
		{
			bool setValue = updateComponents;
			AudioListener(setValue);
			PhononListener(setValue);
		}

		public void Initialize(bool initializeRenderer)
		{
			if (!isInitialized)
			{
				isInitialized = true;
				bool setValue = true;
				AudioListener(setValue);
				PhononListener(setValue);
				PhononStaticListener();
				CustomPhononSettings();
				CustomSpeakerLayout();
			}
		}

		public void Destroy()
		{
			audioListener = null;
			phononListener = null;
			phononStaticListener = null;
			customSpeakerLayout = null;
			customPhononSettings = null;
			isSetPhononStaticListener = false;
			isSetCustomSpeakerLayout = false;
			isSetCustomPhononSettings = false;
			isInitialized = false;
		}

		public PhononManagerContainer PhononManagerContainer()
		{
			return phononContainer;
		}

		public SimulationSettings SimulationSettings()
		{
			SceneType sceneType = RayTracerOption();
			float duration = simulationValue.Duration;
			int ambisonicsOrder = simulationValue.AmbisonicsOrder;
			int maxSources = simulationValue.MaxSources;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (false)
			{
				num = simulationValue.BakeRays;
				num2 = simulationValue.BakeSecondaryRays;
				num3 = simulationValue.BakeBounces;
			}
			else
			{
				num = simulationValue.RealtimeRays;
				num2 = simulationValue.RealtimeSecondaryRays;
				num3 = simulationValue.RealtimeBounces;
			}
			SimulationSettings result = default(SimulationSettings);
			result.sceneType = sceneType;
			result.rays = num;
			result.secondaryRays = num2;
			result.bounces = num3;
			result.irDuration = duration;
			result.ambisonicsOrder = ambisonicsOrder;
			result.maxConvolutionSources = maxSources;
			return result;
		}

		public GlobalContext GlobalContext()
		{
			GlobalContext result = default(GlobalContext);
			result.logCallback = IntPtr.Zero;
			result.allocateCallback = IntPtr.Zero;
			result.freeCallback = IntPtr.Zero;
			return result;
		}

		public RenderingSettings RenderingSettings()
		{
			int bufferLength;
			int numBuffers;
			AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
			RenderingSettings result = default(RenderingSettings);
			result.samplingRate = AudioSettings.outputSampleRate;
			result.frameSize = bufferLength;
			result.convolutionOption = ConvolutionOption();
			return result;
		}

		public AudioFormat AudioFormat()
		{
			AudioSpeakerMode speakerMode = AudioSettings.GetConfiguration().speakerMode;
			AudioSpeakerMode driverCapabilities = AudioSettings.driverCapabilities;
			AudioSpeakerMode audioSpeakerMode = ((speakerMode != AudioSpeakerMode.Prologic || driverCapabilities != AudioSpeakerMode.Prologic) ? ((speakerMode >= driverCapabilities) ? driverCapabilities : speakerMode) : AudioSpeakerMode.Stereo);
			AudioFormat result = default(AudioFormat);
			result.channelLayoutType = ChannelLayoutType.Speakers;
			result.speakerDirections = null;
			result.ambisonicsOrder = -1;
			result.ambisonicsOrdering = AmbisonicsOrdering.ACN;
			result.ambisonicsNormalization = AmbisonicsNormalization.N3D;
			result.channelOrder = ChannelOrder.Interleaved;
			switch (audioSpeakerMode)
			{
			case AudioSpeakerMode.Mono:
				result.channelLayout = ChannelLayout.Mono;
				result.numSpeakers = 1;
				break;
			case AudioSpeakerMode.Stereo:
				result.channelLayout = ChannelLayout.Stereo;
				result.numSpeakers = 2;
				break;
			case AudioSpeakerMode.Quad:
				result.channelLayout = ChannelLayout.Quadraphonic;
				result.numSpeakers = 4;
				break;
			case AudioSpeakerMode.Mode5point1:
				result.channelLayout = ChannelLayout.FivePointOne;
				result.numSpeakers = 6;
				break;
			case AudioSpeakerMode.Mode7point1:
				result.channelLayout = ChannelLayout.SevenPointOne;
				result.numSpeakers = 8;
				break;
			default:
				Debug.LogWarning("Surround and Prologic mode is not supported. Revert to stereo");
				result.channelLayout = ChannelLayout.Stereo;
				result.numSpeakers = 2;
				break;
			}
			CustomSpeakerLayout customSpeakerLayout = CustomSpeakerLayout();
			if (customSpeakerLayout != null && customSpeakerLayout.speakerPositions.Length == result.numSpeakers)
			{
				result.channelLayout = ChannelLayout.Custom;
				result.speakerDirections = new Vector3[result.numSpeakers];
				for (int i = 0; i < customSpeakerLayout.speakerPositions.Length; i++)
				{
					result.speakerDirections[i].x = customSpeakerLayout.speakerPositions[i].x;
					result.speakerDirections[i].y = customSpeakerLayout.speakerPositions[i].y;
					result.speakerDirections[i].z = customSpeakerLayout.speakerPositions[i].z;
				}
			}
			return result;
		}

		public SceneType RayTracerOption()
		{
			CustomPhononSettings customPhononSettings = CustomPhononSettings();
			if ((bool)customPhononSettings)
			{
				return customPhononSettings.rayTracerOption;
			}
			return SceneType.Phonon;
		}

		public ConvolutionOption ConvolutionOption()
		{
			CustomPhononSettings customPhononSettings = CustomPhononSettings();
			if ((bool)customPhononSettings)
			{
				return customPhononSettings.convolutionOption;
			}
			return Phonon.ConvolutionOption.Phonon;
		}

		public ComputeDeviceType ComputeDeviceSettings(out int numComputeUnits, out bool useOpenCL)
		{
			CustomPhononSettings customPhononSettings = CustomPhononSettings();
			if ((bool)customPhononSettings)
			{
				numComputeUnits = customPhononSettings.numComputeUnits;
				useOpenCL = customPhononSettings.useOpenCL;
				return customPhononSettings.computeDeviceOption;
			}
			numComputeUnits = 0;
			useOpenCL = false;
			return ComputeDeviceType.CPU;
		}

		public AudioListener AudioListener(bool setValue = false)
		{
			if (setValue)
			{
				audioListener = UnityEngine.Object.FindObjectOfType<AudioListener>();
			}
			return audioListener;
		}

		public PhononListener PhononListener(bool setValue = false)
		{
			if (setValue)
			{
				phononListener = UnityEngine.Object.FindObjectOfType<PhononListener>();
			}
			return phononListener;
		}

		public PhononStaticListener PhononStaticListener()
		{
			if (!isSetPhononStaticListener && phononStaticListener == null)
			{
				phononStaticListener = UnityEngine.Object.FindObjectOfType<PhononStaticListener>();
				isSetPhononStaticListener = true;
			}
			return phononStaticListener;
		}

		public CustomSpeakerLayout CustomSpeakerLayout()
		{
			if (!isSetCustomSpeakerLayout && customSpeakerLayout == null)
			{
				customSpeakerLayout = UnityEngine.Object.FindObjectOfType<CustomSpeakerLayout>();
				isSetCustomSpeakerLayout = true;
			}
			return customSpeakerLayout;
		}

		public CustomPhononSettings CustomPhononSettings()
		{
			if (!isSetCustomPhononSettings && customPhononSettings == null)
			{
				customPhononSettings = UnityEngine.Object.FindObjectOfType<CustomPhononSettings>();
				isSetCustomPhononSettings = true;
			}
			return customPhononSettings;
		}

		public void ExportScene()
		{
			Scene scene = new Scene();
			ComputeDevice computeDevice = new ComputeDevice();
			try
			{
				scene.Export(computeDevice, SimulationSettings(), materialValue, GlobalContext());
			}
			catch (Exception ex)
			{
				Debug.LogError("Phonon Geometry not attached. " + ex.Message);
			}
		}

		public void DumpScene()
		{
			Scene scene = new Scene();
			ComputeDevice computeDevice = new ComputeDevice();
			try
			{
				scene.DumpToObj(computeDevice, SimulationSettings(), materialValue, GlobalContext());
			}
			catch (Exception ex)
			{
				Debug.LogError("Phonon Geometry not attached. " + ex.Message);
			}
		}
	}
}
