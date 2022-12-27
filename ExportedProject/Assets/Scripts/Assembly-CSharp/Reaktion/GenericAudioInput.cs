using UnityEngine;

namespace Reaktion
{
	[AddComponentMenu("Reaktion/Utility/Generic Audio Input")]
	public class GenericAudioInput : MonoBehaviour
	{
		private AudioSource audioSource;

		public float estimatedLatency { get; protected set; }

		private void Awake()
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.loop = true;
			StartInput();
		}

		private void OnApplicationPause(bool paused)
		{
			if (paused)
			{
				audioSource.Stop();
				Microphone.End(null);
				audioSource.clip = null;
			}
			else
			{
				StartInput();
			}
		}

		private void StartInput()
		{
			int outputSampleRate = AudioSettings.outputSampleRate;
			audioSource.clip = Microphone.Start(null, true, 1, outputSampleRate);
			if (audioSource.clip != null)
			{
				int num;
				for (num = 0; num <= 0; num = Microphone.GetPosition(null))
				{
				}
				audioSource.Play();
				estimatedLatency = (float)num / (float)outputSampleRate;
			}
			else
			{
				Debug.LogWarning("GenericAudioInput: Initialization failed.");
			}
		}
	}
}
