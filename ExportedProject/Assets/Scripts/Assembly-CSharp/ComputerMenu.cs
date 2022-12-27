using UnityEngine;
using UnityEngine.UI;

public class ComputerMenu : MonoBehaviour
{
	public Text volumeNumber;

	private AudioSource audioSource;

	public AudioClip beep;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void ChangeVolume(float value)
	{
		AudioListener.volume += value;
		AudioListener.volume = Mathf.Clamp(AudioListener.volume, 0f, 1f);
		volumeNumber.text = (AudioListener.volume * 10f).ToString("F0");
		PlayClickAudio();
	}

	public void PlayClickAudio()
	{
		audioSource.PlayOneShot(beep);
	}

	public void OnExit()
	{
		PlayClickAudio();
		Application.Quit();
	}
}
