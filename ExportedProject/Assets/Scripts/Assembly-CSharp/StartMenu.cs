using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
	private AudioSource audioSource;

	public AudioClip beep;

	public Text volumeNumber;

	public static StartMenu instance;

	public GameObject main;

	public GameObject options;

	internal bool shadowsOn;

	private void Awake()
	{
		instance = this;
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		if (!PreviewLabs.PlayerPrefs.HasKey("MasterVolume"))
		{
			AudioListener.volume = 0.5f;
		}
		else
		{
			AudioListener.volume = PreviewLabs.PlayerPrefs.GetFloat("MasterVolume");
		}
		volumeNumber.text = (AudioListener.volume * 10f).ToString("F0");
	}

	public void ChangeVolume(float value)
	{
		AudioListener.volume += value;
		AudioListener.volume = Mathf.Clamp(AudioListener.volume, 0f, 1f);
		volumeNumber.text = (AudioListener.volume * 10f).ToString("F0");
		PlayClickAudio();
	}

	public void ToggleShadows(bool value)
	{
		foreach (LightSource lightSource in LightSource.lightSources)
		{
			if (lightSource.castsShadows)
			{
				if (!value)
				{
					lightSource.light.shadows = LightShadows.None;
				}
				else
				{
					lightSource.light.shadows = LightShadows.Hard;
				}
			}
		}
	}

	public void ShowOptions()
	{
		options.SetActive(true);
		main.SetActive(false);
	}

	public void ShowMain()
	{
		options.SetActive(false);
		main.SetActive(true);
	}

	public void OnExit()
	{
		Application.Quit();
	}

	public void PlayClickAudio()
	{
		audioSource.PlayOneShot(beep);
	}
}
