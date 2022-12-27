using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneListener : MonoBehaviour
{
	public bool startMicOnStartup = true;

	public bool stopMicrophoneListener;

	public bool startMicrophoneListener;

	private bool microphoneListenerOn;

	public bool disableOutputSound;

	private AudioSource src;

	public AudioMixer masterMixer;

	private float timeSinceRestart;

	private void Start()
	{
		if (startMicOnStartup)
		{
			RestartMicrophoneListener();
			StartMicrophoneListener();
		}
	}

	private void Update()
	{
		if (stopMicrophoneListener)
		{
			StopMicrophoneListener();
		}
		if (startMicrophoneListener)
		{
			StartMicrophoneListener();
		}
		stopMicrophoneListener = false;
		startMicrophoneListener = false;
		MicrophoneIntoAudioSource(microphoneListenerOn);
		DisableSound(!disableOutputSound);
	}

	public void StopMicrophoneListener()
	{
		microphoneListenerOn = false;
		disableOutputSound = false;
		src.Stop();
		src.clip = null;
		Microphone.End(null);
	}

	public void StartMicrophoneListener()
	{
		microphoneListenerOn = true;
		disableOutputSound = true;
		RestartMicrophoneListener();
	}

	public void DisableSound(bool SoundOn)
	{
		float num = 0f;
		num = ((!SoundOn) ? (-80f) : 0f);
		masterMixer.SetFloat("MasterVolume", num);
	}

	public void RestartMicrophoneListener()
	{
		src = GetComponent<AudioSource>();
		src.clip = null;
		timeSinceRestart = Time.time;
	}

	private void MicrophoneIntoAudioSource(bool MicrophoneListenerOn)
	{
		if (MicrophoneListenerOn && Time.time - timeSinceRestart > 0.5f && !Microphone.IsRecording(null))
		{
			src.clip = Microphone.Start(null, true, 10, 44100);
			while (Microphone.GetPosition(null) <= 0)
			{
			}
			src.Play();
		}
	}
}
