using System;
using UnityEngine;

namespace Phonon
{
	public class DirectSimulator
	{
		private AudioFormat inputFormat;

		private AudioFormat outputFormat;

		private DirectSoundPath directSoundPath;

		private AttenuationInterpolator directAttnLerp = new AttenuationInterpolator();

		private int directAttnLerpFrames = 4;

		private IntPtr directBinauralEffect = IntPtr.Zero;

		private IntPtr directCustomPanningEffect = IntPtr.Zero;

		public void Initialize(AudioFormat audioFormat)
		{
			directAttnLerp.Init(directAttnLerpFrames);
			inputFormat = audioFormat;
			outputFormat = audioFormat;
		}

		public void LazyInitialize(BinauralRenderer binauralRenderer, bool directBinauralEnabled)
		{
			if (directBinauralEffect == IntPtr.Zero && outputFormat.channelLayout == ChannelLayout.Stereo && directBinauralEnabled && binauralRenderer.GetBinauralRenderer() != IntPtr.Zero && PhononCore.iplCreateBinauralEffect(binauralRenderer.GetBinauralRenderer(), inputFormat, outputFormat, ref directBinauralEffect) != 0)
			{
				Debug.Log("Unable to create binaural effect. Please check the log file for details.");
			}
			else if (directCustomPanningEffect == IntPtr.Zero && outputFormat.channelLayout == ChannelLayout.Custom && !directBinauralEnabled && binauralRenderer.GetBinauralRenderer() != IntPtr.Zero && PhononCore.iplCreatePanningEffect(binauralRenderer.GetBinauralRenderer(), inputFormat, outputFormat, ref directCustomPanningEffect) != 0)
			{
				Debug.Log("Unable to create custom panning effect. Please check the log file for details.");
			}
		}

		public void Destroy()
		{
			directAttnLerp.Reset();
			PhononCore.iplDestroyBinauralEffect(ref directBinauralEffect);
			directBinauralEffect = IntPtr.Zero;
			PhononCore.iplDestroyPanningEffect(ref directCustomPanningEffect);
			directCustomPanningEffect = IntPtr.Zero;
		}

		public void AudioFrameUpdate(float[] data, int channels, bool physicsBasedAttenuation, float directMixFraction, bool directBinauralEnabled, HRTFInterpolation hrtfInterpolation)
		{
			float num = ((!physicsBasedAttenuation) ? 1f : directSoundPath.distanceAttenuation);
			directAttnLerp.Set(directSoundPath.occlusionFactor * directMixFraction * num);
			int num2 = data.Length / channels;
			float perSampleIncrement;
			float num3 = directAttnLerp.Update(out perSampleIncrement, num2);
			Vector3 direction = directSoundPath.direction;
			AudioBuffer inputAudio = default(AudioBuffer);
			inputAudio.audioFormat = inputFormat;
			inputAudio.numSamples = data.Length / channels;
			inputAudio.deInterleavedBuffer = null;
			inputAudio.interleavedBuffer = data;
			AudioBuffer outputAudio = default(AudioBuffer);
			outputAudio.audioFormat = outputFormat;
			outputAudio.numSamples = data.Length / channels;
			outputAudio.deInterleavedBuffer = null;
			outputAudio.interleavedBuffer = data;
			if (outputFormat.channelLayout == ChannelLayout.Stereo && directBinauralEnabled)
			{
				PhononCore.iplApplyBinauralEffect(directBinauralEffect, inputAudio, direction, hrtfInterpolation, outputAudio);
			}
			else if (outputFormat.channelLayout == ChannelLayout.Custom)
			{
				PhononCore.iplApplyPanningEffect(directCustomPanningEffect, inputAudio, direction, outputAudio);
			}
			int i = 0;
			int num4 = 0;
			for (; i < num2; i++)
			{
				int num5 = 0;
				while (num5 < channels)
				{
					data[num4] *= num3;
					num5++;
					num4++;
				}
				num3 += perSampleIncrement;
			}
		}

		public void Flush()
		{
			PhononCore.iplFlushBinauralEffect(directBinauralEffect);
			PhononCore.iplFlushPanningEffect(directCustomPanningEffect);
		}

		public void FrameUpdate(IntPtr envRenderer, Vector3 sourcePosition, Vector3 listenerPosition, Vector3 listenerAhead, Vector3 listenerUp, float partialOcclusionRadius, OcclusionOption directOcclusionOption)
		{
			directSoundPath = PhononCore.iplGetDirectSoundPath(envRenderer, listenerPosition, listenerAhead, listenerUp, sourcePosition, partialOcclusionRadius, directOcclusionOption);
		}
	}
}
