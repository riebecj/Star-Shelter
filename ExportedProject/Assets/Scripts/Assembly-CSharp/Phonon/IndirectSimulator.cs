using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Phonon
{
	public class IndirectSimulator
	{
		private AudioFormat inputFormat;

		private AudioFormat outputFormat;

		private AudioFormat ambisonicsFormat;

		private float[] wetData;

		private IntPtr[] wetDataMarshal;

		private IntPtr[] wetAmbisonicsDataMarshal;

		private bool fourierMixingEnabled;

		private IntPtr propagationPanningEffect = IntPtr.Zero;

		private IntPtr propagationBinauralEffect = IntPtr.Zero;

		private IntPtr propagationAmbisonicsEffect = IntPtr.Zero;

		public void Initialize(AudioFormat audioFormat, SimulationSettings simulationSettings)
		{
			inputFormat = audioFormat;
			outputFormat = audioFormat;
			ambisonicsFormat.channelLayoutType = ChannelLayoutType.Ambisonics;
			ambisonicsFormat.ambisonicsOrder = simulationSettings.ambisonicsOrder;
			ambisonicsFormat.numSpeakers = (ambisonicsFormat.ambisonicsOrder + 1) * (ambisonicsFormat.ambisonicsOrder + 1);
			ambisonicsFormat.ambisonicsOrdering = AmbisonicsOrdering.ACN;
			ambisonicsFormat.ambisonicsNormalization = AmbisonicsNormalization.N3D;
			ambisonicsFormat.channelOrder = ChannelOrder.Deinterleaved;
		}

		public void LazyInitialize(BinauralRenderer binauralRenderer, bool reflectionEnabled, bool indirectBinauralEnabled, RenderingSettings renderingSettings, bool sourceUpdate, SourceSimulationType sourceSimulationType, string uniqueIdentifier, PhononStaticListener phononStaticListener, ReverbSimulationType reverbSimualtionType, EnvironmentalRenderer environmentalRenderer)
		{
			AudioFormat audioFormat = outputFormat;
			audioFormat.channelOrder = ChannelOrder.Deinterleaved;
			if (reflectionEnabled && propagationPanningEffect == IntPtr.Zero && binauralRenderer.GetBinauralRenderer() != IntPtr.Zero && PhononCore.iplCreateAmbisonicsPanningEffect(binauralRenderer.GetBinauralRenderer(), ambisonicsFormat, audioFormat, ref propagationPanningEffect) != 0)
			{
				Debug.Log("Unable to create Ambisonics panning effect. Please check the log file for details.");
				return;
			}
			if (outputFormat.channelLayout == ChannelLayout.Stereo && reflectionEnabled && indirectBinauralEnabled && propagationBinauralEffect == IntPtr.Zero && binauralRenderer.GetBinauralRenderer() != IntPtr.Zero && PhononCore.iplCreateAmbisonicsBinauralEffect(binauralRenderer.GetBinauralRenderer(), ambisonicsFormat, audioFormat, ref propagationBinauralEffect) != 0)
			{
				Debug.Log("Unable to create propagation binaural effect. Please check the log file for details.");
				return;
			}
			if (reflectionEnabled && propagationAmbisonicsEffect == IntPtr.Zero && environmentalRenderer.GetEnvironmentalRenderer() != IntPtr.Zero)
			{
				string s = string.Empty;
				if (sourceUpdate && sourceSimulationType == SourceSimulationType.BakedStaticSource)
				{
					s = uniqueIdentifier;
				}
				else if (sourceUpdate && sourceSimulationType == SourceSimulationType.BakedStaticListener)
				{
					if (phononStaticListener == null)
					{
						Debug.LogError("No Phonon Static Listener component found.");
					}
					else if (phononStaticListener.currentStaticListenerNode == null)
					{
						Debug.LogError("Current static listener node is not specified in Phonon Static Listener.");
					}
					else
					{
						s = phononStaticListener.currentStaticListenerNode.GetUniqueIdentifier();
					}
				}
				else if (!sourceUpdate && reverbSimualtionType == ReverbSimulationType.BakedReverb)
				{
					s = "__reverb__";
				}
				SimulationType simulationType = ((!sourceUpdate) ? ((reverbSimualtionType != 0) ? SimulationType.Baked : SimulationType.Realtime) : ((sourceSimulationType != 0) ? SimulationType.Baked : SimulationType.Realtime));
				if (PhononCore.iplCreateConvolutionEffect(environmentalRenderer.GetEnvironmentalRenderer(), Common.ConvertString(s), simulationType, inputFormat, ambisonicsFormat, ref propagationAmbisonicsEffect) != 0)
				{
					Debug.LogError("Unable to create propagation effect for object");
				}
			}
			if (reflectionEnabled && wetData == null)
			{
				wetData = new float[renderingSettings.frameSize * outputFormat.numSpeakers];
			}
			if (reflectionEnabled && wetAmbisonicsDataMarshal == null)
			{
				wetAmbisonicsDataMarshal = new IntPtr[ambisonicsFormat.numSpeakers];
				for (int i = 0; i < ambisonicsFormat.numSpeakers; i++)
				{
					wetAmbisonicsDataMarshal[i] = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(float)) * renderingSettings.frameSize);
				}
			}
			if (reflectionEnabled && wetDataMarshal == null)
			{
				wetDataMarshal = new IntPtr[outputFormat.numSpeakers];
				for (int j = 0; j < outputFormat.numSpeakers; j++)
				{
					wetDataMarshal[j] = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(float)) * renderingSettings.frameSize);
				}
			}
		}

		public void Destroy()
		{
			PhononCore.iplDestroyConvolutionEffect(ref propagationAmbisonicsEffect);
			propagationAmbisonicsEffect = IntPtr.Zero;
			PhononCore.iplDestroyAmbisonicsBinauralEffect(ref propagationBinauralEffect);
			propagationBinauralEffect = IntPtr.Zero;
			PhononCore.iplDestroyAmbisonicsPanningEffect(ref propagationPanningEffect);
			propagationPanningEffect = IntPtr.Zero;
			wetData = null;
			if (wetDataMarshal != null)
			{
				for (int i = 0; i < outputFormat.numSpeakers; i++)
				{
					Marshal.FreeCoTaskMem(wetDataMarshal[i]);
				}
			}
			wetDataMarshal = null;
			if (wetAmbisonicsDataMarshal != null)
			{
				for (int j = 0; j < ambisonicsFormat.numSpeakers; j++)
				{
					Marshal.FreeCoTaskMem(wetAmbisonicsDataMarshal[j]);
				}
			}
			wetAmbisonicsDataMarshal = null;
		}

		public float[] AudioFrameUpdate(float[] data, int channels, Vector3 sourcePosition, Vector3 listenerPosition, Vector3 listenerAhead, Vector3 listenerUp, bool enableReflections, float indirectMixFraction, bool indirectBinauralEnabled, PhononListener phononListener)
		{
			AudioBuffer audioBuffer = default(AudioBuffer);
			audioBuffer.audioFormat = inputFormat;
			audioBuffer.numSamples = data.Length / channels;
			audioBuffer.deInterleavedBuffer = null;
			audioBuffer.interleavedBuffer = data;
			AudioBuffer audioBuffer2 = default(AudioBuffer);
			audioBuffer2.audioFormat = outputFormat;
			audioBuffer2.numSamples = data.Length / channels;
			audioBuffer2.deInterleavedBuffer = null;
			audioBuffer2.interleavedBuffer = data;
			if (enableReflections && wetData != null && wetDataMarshal != null && wetAmbisonicsDataMarshal != null && propagationAmbisonicsEffect != IntPtr.Zero)
			{
				for (int i = 0; i < data.Length; i++)
				{
					wetData[i] = data[i] * indirectMixFraction;
				}
				AudioBuffer dryAudio = default(AudioBuffer);
				dryAudio.audioFormat = inputFormat;
				dryAudio.numSamples = wetData.Length / channels;
				dryAudio.deInterleavedBuffer = null;
				dryAudio.interleavedBuffer = wetData;
				PhononCore.iplSetDryAudioForConvolutionEffect(propagationAmbisonicsEffect, sourcePosition, dryAudio);
				if (fourierMixingEnabled)
				{
					phononListener.processMixedAudio = true;
					return null;
				}
				AudioBuffer audioBuffer3 = default(AudioBuffer);
				audioBuffer3.audioFormat = ambisonicsFormat;
				audioBuffer3.numSamples = data.Length / channels;
				audioBuffer3.deInterleavedBuffer = wetAmbisonicsDataMarshal;
				audioBuffer3.interleavedBuffer = null;
				PhononCore.iplGetWetAudioForConvolutionEffect(propagationAmbisonicsEffect, listenerPosition, listenerAhead, listenerUp, audioBuffer3);
				AudioBuffer audioBuffer4 = default(AudioBuffer);
				audioBuffer4.audioFormat = outputFormat;
				audioBuffer4.audioFormat.channelOrder = ChannelOrder.Deinterleaved;
				audioBuffer4.numSamples = data.Length / channels;
				audioBuffer4.deInterleavedBuffer = wetDataMarshal;
				audioBuffer4.interleavedBuffer = null;
				if (outputFormat.channelLayout == ChannelLayout.Stereo && indirectBinauralEnabled)
				{
					PhononCore.iplApplyAmbisonicsBinauralEffect(propagationBinauralEffect, audioBuffer3, audioBuffer4);
				}
				else
				{
					PhononCore.iplApplyAmbisonicsPanningEffect(propagationPanningEffect, audioBuffer3, audioBuffer4);
				}
				AudioBuffer outputAudio = default(AudioBuffer);
				outputAudio.audioFormat = outputFormat;
				outputAudio.numSamples = data.Length / channels;
				outputAudio.deInterleavedBuffer = null;
				outputAudio.interleavedBuffer = wetData;
				PhononCore.iplInterleaveAudioBuffer(audioBuffer4, outputAudio);
				return wetData;
			}
			return null;
		}

		public void FrameUpdate(bool sourceUpdate, SourceSimulationType sourceSimulationType, ReverbSimulationType reverbSimulationType, PhononStaticListener phononStaticListener, PhononListener phononListener)
		{
			if (sourceUpdate && sourceSimulationType == SourceSimulationType.BakedStaticListener && phononStaticListener != null && phononStaticListener.currentStaticListenerNode != null)
			{
				UpdateEffectName(phononStaticListener.currentStaticListenerNode.GetUniqueIdentifier());
			}
			if ((bool)phononListener && phononListener.acceleratedMixing)
			{
				fourierMixingEnabled = true;
			}
			else
			{
				fourierMixingEnabled = false;
			}
		}

		public void Flush()
		{
			PhononCore.iplFlushAmbisonicsPanningEffect(propagationPanningEffect);
			PhononCore.iplFlushAmbisonicsBinauralEffect(propagationBinauralEffect);
			PhononCore.iplFlushConvolutionEffect(propagationAmbisonicsEffect);
		}

		public void UpdateEffectName(string effectName)
		{
			if (propagationAmbisonicsEffect != IntPtr.Zero)
			{
				PhononCore.iplSetConvolutionEffectName(propagationAmbisonicsEffect, Common.ConvertString(effectName));
			}
		}
	}
}
