using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Phonon
{
	[AddComponentMenu("Phonon/Phonon Source")]
	[RequireComponent(typeof(AudioSource))]
	public class PhononSource : MonoBehaviour
	{
		public bool directBinauralEnabled = true;

		public HRTFInterpolation hrtfInterpolation;

		public OcclusionOption directOcclusionOption;

		[Range(0.1f, 32f)]
		public float partialOcclusionRadius = 1f;

		public bool physicsBasedAttenuation = true;

		[Range(0f, 1f)]
		public float directMixFraction = 1f;

		public bool enableReflections;

		public SourceSimulationType sourceSimulationType;

		[Range(0f, 10f)]
		public float indirectMixFraction = 1f;

		public bool indirectBinauralEnabled;

		public string uniqueIdentifier = string.Empty;

		[Range(1f, 1024f)]
		public float bakingRadius = 16f;

		public bool useAllProbeBoxes;

		public ProbeBox[] probeBoxes;

		public PhononBaker phononBaker = new PhononBaker();

		public List<string> bakedProbeNames = new List<string>();

		public List<int> bakedProbeDataSizes = new List<int>();

		public int bakedDataSize;

		private PhononManager phononManager;

		private PhononManagerContainer phononContainer;

		private AudioFormat inputFormat;

		private AudioFormat outputFormat;

		private AudioFormat ambisonicsFormat;

		private Vector3 sourcePosition;

		private Vector3 listenerPosition;

		private Vector3 listenerAhead;

		private Vector3 listenerUp;

		private Mutex mutex = new Mutex();

		private bool initialized;

		private bool destroying;

		private bool errorLogged;

		private DirectSimulator directSimulator = new DirectSimulator();

		private IndirectSimulator indirectSimulator = new IndirectSimulator();

		private void Awake()
		{
			Initialize();
			LazyInitialize();
		}

		private void OnEnable()
		{
			StartCoroutine(EndOfFrameUpdate());
		}

		private void OnDisable()
		{
			directSimulator.Flush();
			indirectSimulator.Flush();
		}

		private void OnDestroy()
		{
			Destroy();
		}

		private void Initialize()
		{
			initialized = false;
			destroying = false;
			errorLogged = false;
			phononManager = UnityEngine.Object.FindObjectOfType<PhononManager>();
			if (phononManager == null)
			{
				Debug.LogError("Phonon Manager Settings object not found in the scene! Click Window > Phonon");
				return;
			}
			bool initializeRenderer = true;
			phononManager.Initialize(initializeRenderer);
			phononContainer = phononManager.PhononManagerContainer();
			phononContainer.Initialize(initializeRenderer, phononManager);
			directSimulator.Initialize(phononManager.AudioFormat());
			indirectSimulator.Initialize(phononManager.AudioFormat(), phononManager.SimulationSettings());
		}

		private void LazyInitialize()
		{
			if (phononManager != null && phononContainer != null)
			{
				directSimulator.LazyInitialize(phononContainer.BinauralRenderer(), directBinauralEnabled);
				indirectSimulator.LazyInitialize(phononContainer.BinauralRenderer(), enableReflections, indirectBinauralEnabled, phononManager.RenderingSettings(), true, sourceSimulationType, uniqueIdentifier, phononManager.PhononStaticListener(), ReverbSimulationType.RealtimeReverb, phononContainer.EnvironmentalRenderer());
			}
		}

		private void Destroy()
		{
			mutex.WaitOne();
			destroying = true;
			directSimulator.Destroy();
			indirectSimulator.Destroy();
			if (phononContainer != null)
			{
				phononContainer.Destroy();
				phononContainer = null;
			}
			mutex.ReleaseMutex();
		}

		private IEnumerator EndOfFrameUpdate()
		{
			while (true)
			{
				LazyInitialize();
				if (!errorLogged && phononManager != null && phononContainer != null && phononContainer.Scene().GetScene() == IntPtr.Zero && (directOcclusionOption != 0 || enableReflections))
				{
					Debug.LogError("Scene not found. Make sure to pre-export the scene.");
					errorLogged = true;
				}
				if (phononManager != null && !errorLogged)
				{
					UpdateRelativeDirection();
					directSimulator.FrameUpdate(phononContainer.EnvironmentalRenderer().GetEnvironmentalRenderer(), sourcePosition, listenerPosition, listenerAhead, listenerUp, partialOcclusionRadius, directOcclusionOption);
					indirectSimulator.FrameUpdate(true, sourceSimulationType, ReverbSimulationType.RealtimeReverb, phononManager.PhononStaticListener(), phononManager.PhononListener());
					initialized = true;
				}
				yield return new WaitForEndOfFrame();
			}
		}

		private void UpdateRelativeDirection()
		{
			AudioListener audioListener = phononManager.AudioListener();
			if (!(audioListener == null))
			{
				sourcePosition = Common.ConvertVector(base.transform.position);
				listenerPosition = Common.ConvertVector(audioListener.transform.position);
				listenerAhead = Common.ConvertVector(audioListener.transform.forward);
				listenerUp = Common.ConvertVector(audioListener.transform.up);
			}
		}

		private void OnAudioFilterRead(float[] data, int channels)
		{
			mutex.WaitOne();
			if (data == null)
			{
				mutex.ReleaseMutex();
				return;
			}
			if (!initialized || destroying)
			{
				mutex.ReleaseMutex();
				Array.Clear(data, 0, data.Length);
				return;
			}
			float[] array = indirectSimulator.AudioFrameUpdate(data, channels, sourcePosition, listenerPosition, listenerAhead, listenerUp, enableReflections, indirectMixFraction, indirectBinauralEnabled, phononManager.PhononListener());
			directSimulator.AudioFrameUpdate(data, channels, physicsBasedAttenuation, directMixFraction, directBinauralEnabled, hrtfInterpolation);
			if (array != null && array.Length != 0)
			{
				for (int i = 0; i < data.Length; i++)
				{
					data[i] += array[i];
				}
			}
			mutex.ReleaseMutex();
		}

		public void BeginBake()
		{
			Vector3 vector = Common.ConvertVector(base.gameObject.transform.position);
			Sphere sphere = default(Sphere);
			sphere.centerx = vector.x;
			sphere.centery = vector.y;
			sphere.centerz = vector.z;
			sphere.radius = bakingRadius;
			if (useAllProbeBoxes)
			{
				phononBaker.BeginBake(UnityEngine.Object.FindObjectsOfType<ProbeBox>(), BakingMode.StaticSource, uniqueIdentifier, sphere);
			}
			else
			{
				phononBaker.BeginBake(probeBoxes, BakingMode.StaticSource, uniqueIdentifier, sphere);
			}
		}

		public void EndBake()
		{
			phononBaker.EndBake();
		}

		private void OnDrawGizmosSelected()
		{
			if (sourceSimulationType != SourceSimulationType.BakedStaticSource)
			{
				return;
			}
			Color color = Gizmos.color;
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.gameObject.transform.position, bakingRadius);
			Gizmos.color = Color.magenta;
			ProbeBox[] array = probeBoxes;
			if (useAllProbeBoxes)
			{
				array = UnityEngine.Object.FindObjectsOfType<ProbeBox>();
			}
			if (array != null)
			{
				ProbeBox[] array2 = array;
				foreach (ProbeBox probeBox in array2)
				{
					if (probeBox != null)
					{
						Gizmos.DrawWireCube(probeBox.transform.position, probeBox.transform.localScale);
					}
				}
			}
			Gizmos.color = color;
		}

		public void UpdateBakedDataStatistics()
		{
			ProbeBox[] array = probeBoxes;
			if (useAllProbeBoxes)
			{
				array = UnityEngine.Object.FindObjectsOfType<ProbeBox>();
			}
			if (array == null)
			{
				return;
			}
			int num = 0;
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			ProbeBox[] array2 = array;
			foreach (ProbeBox probeBox in array2)
			{
				if (probeBox == null || uniqueIdentifier.Length == 0)
				{
					continue;
				}
				int num2 = 0;
				list.Add(probeBox.name);
				for (int j = 0; j < probeBox.probeDataName.Count; j++)
				{
					if (uniqueIdentifier == probeBox.probeDataName[j])
					{
						num2 = probeBox.probeDataNameSizes[j];
						num += num2;
					}
				}
				list2.Add(num2);
			}
			bakedDataSize = num;
			bakedProbeNames = list;
			bakedProbeDataSizes = list2;
		}
	}
}
