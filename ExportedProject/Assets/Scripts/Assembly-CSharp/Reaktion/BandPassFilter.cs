using System;
using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Utility/Band Pass Filter")]
	public class BandPassFilter : MonoBehaviour
	{
		[Range(0f, 1f)]
		public float cutoff = 0.5f;

		[Range(1f, 10f)]
		public float q = 1f;

		private float vF;

		private float vD;

		private float vZ1;

		private float vZ2;

		private float vZ3;

		public float CutoffFrequency
		{
			get
			{
				return Mathf.Pow(2f, 10f * cutoff - 10f) * 15000f;
			}
		}

		private void Awake()
		{
			Update();
		}

		private void Update()
		{
			float num = 1.081081f * Mathf.Sin((float)Math.PI * CutoffFrequency / (float)AudioSettings.outputSampleRate);
			vD = 1f / q;
			vF = (1.85f - 0.75f * vD * num) * num;
		}

		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				float num = data[i];
				float num2 = 0.5f * num;
				float num3 = vZ2 * vF + vZ3;
				float num4 = (num2 + vZ1 - num3 - vZ2 * vD) * vF + vZ2;
				for (int j = 0; j < channels; j++)
				{
					data[i + j] = num4;
				}
				vZ1 = num2;
				vZ2 = num4;
				vZ3 = num3;
			}
		}
	}
}
