using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Phonon
{
	[AddComponentMenu("Phonon/Phonon Listener")]
	public class PhononListener : MonoBehaviour
	{
		public bool processMixedAudio;

		public bool acceleratedMixing;

		public bool enableReverb;

		public ReverbSimulationType reverbSimulationType;

		[Range(0f, 1f)]
		public float dryMixFraction = 1f;

		[Range(0f, 10f)]
		public float reverbMixFraction = 1f;

		public bool indirectBinauralEnabled;

		public bool useAllProbeBoxes;

		public ProbeBox[] probeBoxes;

		public List<string> bakedProbeNames = new List<string>();

		public List<int> bakedProbeDataSizes = new List<int>();

		public int bakedDataSize;

		private PhononManager phononManager;

		private PhononManagerContainer phononContainer;

		private IndirectMixer indirectMixer = new IndirectMixer();

		private IndirectSimulator indirectSimulator = new IndirectSimulator();

		public PhononBaker phononBaker = new PhononBaker();

		private Vector3 listenerPosition;

		private Vector3 listenerAhead;

		private Vector3 listenerUp;

		private Mutex mutex = new Mutex();

		private bool initialized;

		private bool destroying;

		private bool errorLogged;

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
			indirectMixer.Flush();
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
			indirectSimulator.Initialize(phononManager.AudioFormat(), phononManager.SimulationSettings());
			indirectMixer.Initialize(phononManager.AudioFormat(), phononManager.SimulationSettings());
		}

		private void LazyInitialize()
		{
			if (phononManager != null && phononContainer != null)
			{
				indirectSimulator.LazyInitialize(phononContainer.BinauralRenderer(), enableReverb && !acceleratedMixing, indirectBinauralEnabled, phononManager.RenderingSettings(), false, SourceSimulationType.Realtime, "__reverb__", phononManager.PhononStaticListener(), reverbSimulationType, phononContainer.EnvironmentalRenderer());
				indirectMixer.LazyInitialize(phononContainer.BinauralRenderer(), acceleratedMixing, indirectBinauralEnabled, phononManager.RenderingSettings());
			}
		}

		private void Destroy()
		{
			mutex.WaitOne();
			destroying = true;
			indirectMixer.Destroy();
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
				if (!errorLogged && phononManager != null && phononContainer != null && phononContainer.Scene().GetScene() == IntPtr.Zero && enableReverb)
				{
					Debug.LogError("Scene not found. Make sure to pre-export the scene.");
					errorLogged = true;
				}
				if (!initialized && phononManager != null && phononContainer != null && phononContainer.EnvironmentalRenderer().GetEnvironmentalRenderer() != IntPtr.Zero)
				{
					initialized = true;
				}
				if (phononManager != null)
				{
					listenerPosition = Common.ConvertVector(base.transform.position);
					listenerAhead = Common.ConvertVector(base.transform.forward);
					listenerUp = Common.ConvertVector(base.transform.up);
					indirectSimulator.FrameUpdate(false, SourceSimulationType.Realtime, reverbSimulationType, phononManager.PhononStaticListener(), phononManager.PhononListener());
				}
				yield return new WaitForEndOfFrame();
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
			if (!initialized || destroying || (acceleratedMixing && !processMixedAudio))
			{
				mutex.ReleaseMutex();
				Array.Clear(data, 0, data.Length);
				return;
			}
			if (acceleratedMixing)
			{
				indirectMixer.AudioFrameUpdate(data, channels, phononContainer.EnvironmentalRenderer().GetEnvironmentalRenderer(), listenerPosition, listenerAhead, listenerUp, indirectBinauralEnabled);
			}
			else if (enableReverb)
			{
				float[] array = indirectSimulator.AudioFrameUpdate(data, channels, listenerPosition, listenerPosition, listenerAhead, listenerUp, enableReverb, reverbMixFraction, indirectBinauralEnabled, phononManager.PhononListener());
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < data.Length; i++)
					{
						data[i] = data[i] * dryMixFraction + array[i];
					}
				}
			}
			mutex.ReleaseMutex();
		}

		private void OnDrawGizmosSelected()
		{
			Color color = Gizmos.color;
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

		public void BeginBake()
		{
			if (useAllProbeBoxes)
			{
				phononBaker.BeginBake(UnityEngine.Object.FindObjectsOfType<ProbeBox>(), BakingMode.Reverb, "__reverb__");
			}
			else
			{
				phononBaker.BeginBake(probeBoxes, BakingMode.Reverb, "__reverb__");
			}
		}

		public void EndBake()
		{
			phononBaker.EndBake();
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
				if (probeBox == null)
				{
					continue;
				}
				int num2 = 0;
				list.Add(probeBox.name);
				for (int j = 0; j < probeBox.probeDataName.Count; j++)
				{
					if ("__reverb__" == probeBox.probeDataName[j])
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
