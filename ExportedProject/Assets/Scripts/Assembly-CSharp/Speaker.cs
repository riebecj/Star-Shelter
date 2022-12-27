using System.Collections.Generic;
using UnityEngine;

public class Speaker : MonoBehaviour
{
	public AudioSource audioSource;

	public static List<Speaker> speakers = new List<Speaker>();

	private void Start()
	{
		speakers.Add(this);
	}

	private void OnEnable()
	{
		if ((bool)MusicPlayer.instance && MusicPlayer.instance.audioSource.isPlaying)
		{
			OnPlay(MusicPlayer.instance.audioSource.clip);
		}
	}

	public void OnPlay(AudioClip clip)
	{
		audioSource.gameObject.SetActive(true);
		audioSource.clip = clip;
		audioSource.time = MusicPlayer.instance.audioSource.time;
		audioSource.Play();
	}

	public void OnPause()
	{
		audioSource.gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		speakers.Remove(this);
	}
}
