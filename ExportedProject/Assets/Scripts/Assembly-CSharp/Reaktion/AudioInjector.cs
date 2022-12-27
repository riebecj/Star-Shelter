using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Injector/Audio Injector")]
	public class AudioInjector : InjectorBase
	{
		public bool mute = true;

		private const float zeroOffset = 1.5849E-13f;

		private const float refLevel = 0.70710677f;

		private const float minDB = -60f;

		private float squareSum;

		private int sampleCount;

		private void Update()
		{
			if (sampleCount >= 1)
			{
				float num = Mathf.Min(1f, Mathf.Sqrt(squareSum / (float)sampleCount));
				dbLevel = 20f * Mathf.Log10(num / 0.70710677f + 1.5849E-13f);
				squareSum = 0f;
				sampleCount = 0;
			}
		}

		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				float num = data[i];
				squareSum += num * num;
			}
			sampleCount += data.Length / channels;
			if (mute)
			{
				for (int j = 0; j < data.Length; j++)
				{
					data[j] = 0f;
				}
			}
		}
	}
}
