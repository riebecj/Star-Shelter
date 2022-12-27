using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
	public AudioClip[] tracks;

	public AudioSource audioSource;

	private int trackIndex;

	public Text trackInfo;

	public ToggleTrigger playButton;

	public static MusicPlayer instance;

	private void Awake()
	{
		instance = this;
		audioSource.clip = tracks[trackIndex];
	}

	private void Start()
	{
		if ((bool)GetComponent<Rigidbody>())
		{
			Object.Destroy(GetComponent<Rigidbody>());
		}
	}

	private IEnumerator CheckTrackState()
	{
		while (audioSource.isPlaying)
		{
			yield return new WaitForSeconds(1f);
		}
		playButton.Reset();
	}

	public void SwithTrack(bool next)
	{
		if (next)
		{
			trackIndex++;
		}
		else
		{
			trackIndex--;
		}
		if (trackIndex >= tracks.Length)
		{
			trackIndex = 0;
		}
		else if (trackIndex < 0)
		{
			trackIndex = tracks.Length - 1;
		}
		bool flag = false;
		if (audioSource.isPlaying)
		{
			flag = true;
		}
		audioSource.clip = tracks[trackIndex];
		if (flag)
		{
			audioSource.Play();
			PlayInSpeakers();
			StopCoroutine("CheckTrackState");
			StartCoroutine("CheckTrackState");
		}
		trackInfo.text = "Track: " + trackIndex;
	}

	public void TogglePlay(ToggleTrigger trigger)
	{
		StopCoroutine("CheckTrackState");
		if (trigger.On)
		{
			audioSource.Play();
			PlayInSpeakers();
			StartCoroutine("CheckTrackState");
		}
		else
		{
			audioSource.Pause();
			PauseSpeakers();
		}
	}

	private void PlayInSpeakers()
	{
		for (int i = 0; i < Speaker.speakers.Count; i++)
		{
			Speaker.speakers[i].OnPlay(tracks[trackIndex]);
		}
	}

	private void PauseSpeakers()
	{
		for (int i = 0; i < Speaker.speakers.Count; i++)
		{
			Speaker.speakers[i].OnPause();
		}
	}
}
