using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Phonon
{
	public class IndirectMixer
	{
		private AudioFormat ambisonicsFormat;

		private AudioFormat outputFormat;

		private IntPtr propagationPanningEffect = IntPtr.Zero;

		private IntPtr propagationBinauralEffect = IntPtr.Zero;

		private float[] wetData;

		private IntPtr[] wetDataMarshal;

		private IntPtr[] wetAmbisonicsDataMarshal;

		public void Initialize(AudioFormat audioFormat, SimulationSettings simulationSettings)
		{
			outputFormat = audioFormat;
			ambisonicsFormat.channelLayoutType = ChannelLayoutType.Ambisonics;
			ambisonicsFormat.ambisonicsOrder = simulationSettings.ambisonicsOrder;
			ambisonicsFormat.numSpeakers = (ambisonicsFormat.ambisonicsOrder + 1) * (ambisonicsFormat.ambisonicsOrder + 1);
			ambisonicsFormat.ambisonicsOrdering = AmbisonicsOrdering.ACN;
			ambisonicsFormat.ambisonicsNormalization = AmbisonicsNormalization.N3D;
			ambisonicsFormat.channelOrder = ChannelOrder.Deinterleaved;
		}

		public void LazyInitialize(BinauralRenderer binauralRenderer, bool acceleratedMixing, bool indirectBinauralEnabled, RenderingSettings renderingSettings)
		{
			AudioFormat audioFormat = outputFormat;
			audioFormat.channelOrder = ChannelOrder.Deinterleaved;
			if (acceleratedMixing && propagationPanningEffect == IntPtr.Zero && binauralRenderer.GetBinauralRenderer() != IntPtr.Zero && PhononCore.iplCreateAmbisonicsPanningEffect(binauralRenderer.GetBinauralRenderer(), ambisonicsFormat, audioFormat, ref propagationPanningEffect) != 0)
			{
				Debug.Log("Unable to create Ambisonics panning effect. Please check the log file for details.");
				return;
			}
			if (outputFormat.channelLayout == ChannelLayout.Stereo && acceleratedMixing && indirectBinauralEnabled && propagationBinauralEffect == IntPtr.Zero && binauralRenderer.GetBinauralRenderer() != IntPtr.Zero && PhononCore.iplCreateAmbisonicsBinauralEffect(binauralRenderer.GetBinauralRenderer(), ambisonicsFormat, audioFormat, ref propagationBinauralEffect) != 0)
			{
				Debug.Log("Unable to create propagation binaural effect. Please check the log file for details.");
				return;
			}
			if (acceleratedMixing && wetData == null)
			{
				wetData = new float[renderingSettings.frameSize * outputFormat.numSpeakers];
			}
			if (acceleratedMixing && wetAmbisonicsDataMarshal == null)
			{
				wetAmbisonicsDataMarshal = new IntPtr[ambisonicsFormat.numSpeakers];
				for (int i = 0; i < ambisonicsFormat.numSpeakers; i++)
				{
					wetAmbisonicsDataMarshal[i] = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(float)) * renderingSettings.frameSize);
				}
			}
			if (acceleratedMixing && wetDataMarshal == null)
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
			PhononCore.iplDestroyAmbisonicsBinauralEffect(ref propagationBinauralEffect);
			PhononCore.iplDestroyAmbisonicsPanningEffect(ref propagationPanningEffect);
			propagationBinauralEffect = IntPtr.Zero;
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

		public void AudioFrameUpdate(float[] data, int channels, IntPtr environmentalRenderer, Vector3 listenerPosition, Vector3 listenerAhead, Vector3 listenerUp, bool indirectBinauralEnabled)
		{
			AudioBuffer audioBuffer = default(AudioBuffer);
			audioBuffer.audioFormat = ambisonicsFormat;
			audioBuffer.numSamples = data.Length / channels;
			audioBuffer.deInterleavedBuffer = wetAmbisonicsDataMarshal;
			audioBuffer.interleavedBuffer = null;
			PhononCore.iplGetMixedEnvironmentalAudio(environmentalRenderer, listenerPosition, listenerAhead, listenerUp, audioBuffer);
			AudioBuffer audioBuffer2 = default(AudioBuffer);
			audioBuffer2.audioFormat = outputFormat;
			audioBuffer2.audioFormat.channelOrder = ChannelOrder.Deinterleaved;
			audioBuffer2.numSamples = data.Length / channels;
			audioBuffer2.deInterleavedBuffer = wetDataMarshal;
			audioBuffer2.interleavedBuffer = null;
			if (outputFormat.channelLayout == ChannelLayout.Stereo && indirectBinauralEnabled)
			{
				PhononCore.iplApplyAmbisonicsBinauralEffect(propagationBinauralEffect, audioBuffer, audioBuffer2);
			}
			else
			{
				PhononCore.iplApplyAmbisonicsPanningEffect(propagationPanningEffect, audioBuffer, audioBuffer2);
			}
			AudioBuffer outputAudio = default(AudioBuffer);
			outputAudio.audioFormat = outputFormat;
			outputAudio.numSamples = data.Length / channels;
			outputAudio.deInterleavedBuffer = null;
			outputAudio.interleavedBuffer = wetData;
			PhononCore.iplInterleaveAudioBuffer(audioBuffer2, outputAudio);
			for (int i = 0; i < data.Length; i++)
			{
				data[i] += wetData[i];
			}
		}

		public void Flush()
		{
			PhononCore.iplFlushAmbisonicsPanningEffect(propagationPanningEffect);
			PhononCore.iplFlushAmbisonicsBinauralEffect(propagationBinauralEffect);
		}
	}
}
