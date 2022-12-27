using System.Collections;
using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using UnityEngine.Audio;

public class GameAudioManager : MonoBehaviour
{
	public static GameAudioManager instance;

	public AudioMixer mainMixer;

	public AudioMixerSnapshot normal;

	public AudioMixerSnapshot space;

	public AudioMixerSnapshot maskOff;

	internal bool inSpace;

	public bool suitQueueIsPlaying;

	public List<AudioClip> suitQueue = new List<AudioClip>();

	public AudioClip errorSound;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		OnLoad();
	}

	private void OnLoad()
	{
		if (!PreviewLabs.PlayerPrefs.HasKey("Master"))
		{
			PreviewLabs.PlayerPrefs.SetInt("Master", 8);
		}
		if (!PreviewLabs.PlayerPrefs.HasKey("SFX"))
		{
			PreviewLabs.PlayerPrefs.SetInt("SFX", 5);
		}
		if (!PreviewLabs.PlayerPrefs.HasKey("Voice"))
		{
			PreviewLabs.PlayerPrefs.SetInt("Voice", 5);
		}
		UpdateMainVolume(PreviewLabs.PlayerPrefs.GetInt("Master"));
		UpdateSFXVolume(PreviewLabs.PlayerPrefs.GetInt("SFX"));
		UpdateVoiceVolume(PreviewLabs.PlayerPrefs.GetInt("Voice"));
	}

	public void Restart()
	{
		suitQueue.Clear();
		StartCoroutine("UpdateSuitQueue");
	}

	public void ToggleSpace()
	{
		if (!inSpace)
		{
			space.TransitionTo(0.25f);
			inSpace = true;
		}
		else
		{
			normal.TransitionTo(0.25f);
			inSpace = false;
		}
	}

	public void OnNormal()
	{
		normal.TransitionTo(0.25f);
		inSpace = false;
	}

	public void OnSpace()
	{
		normal.TransitionTo(0.25f);
		inSpace = false;
	}

	public void ToggleMask(bool Open)
	{
		if (Open)
		{
			maskOff.TransitionTo(0.25f);
		}
		else if (inSpace)
		{
			space.TransitionTo(0.25f);
		}
		else
		{
			normal.TransitionTo(0.25f);
		}
	}

	public void AddToSuitQueue(AudioClip newClip)
	{
		if (!suitQueue.Contains(newClip))
		{
			suitQueue.Add(newClip);
			if (!suitQueueIsPlaying)
			{
				suitQueueIsPlaying = true;
				StartCoroutine("UpdateSuitQueue");
			}
		}
	}

	private IEnumerator UpdateSuitQueue()
	{
		while (suitQueueIsPlaying && !GameManager.instance.dead)
		{
			if (suitQueue.Count > 0)
			{
				if (!SuitManager.instance.audioSource.isPlaying)
				{
					SuitManager.instance.audioSource.PlayOneShot(suitQueue[0]);
					suitQueue.Remove(suitQueue[0]);
				}
			}
			else
			{
				suitQueueIsPlaying = false;
			}
			yield return new WaitForSeconds(0.25f);
		}
	}

	public void UpdateMainVolume(int value)
	{
		float value2 = -40 + value * 6;
		if (value == 0)
		{
			value2 = -80f;
		}
		mainMixer.SetFloat("Master", value2);
		PreviewLabs.PlayerPrefs.SetInt("Master", value);
	}

	public void UpdateSFXVolume(int value)
	{
		float value2 = -40 + value * 6;
		if (value == 0)
		{
			value2 = -80f;
		}
		mainMixer.SetFloat("SFX", value2);
		PreviewLabs.PlayerPrefs.SetInt("SFX", value);
	}

	public void UpdateVoiceVolume(int value)
	{
		float value2 = -40 + value * 6;
		if (value == 0)
		{
			value2 = -80f;
		}
		mainMixer.SetFloat("Voice", value2);
		PreviewLabs.PlayerPrefs.SetInt("Voice", value);
	}
}
