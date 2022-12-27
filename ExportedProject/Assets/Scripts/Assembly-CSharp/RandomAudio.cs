using UnityEngine;

public class RandomAudio : MonoBehaviour
{
	private AudioSource audioSource;

	public AudioClip[] clips;

	public float maxPich = 1f;

	public float minPich = 1f;

	public bool UseRandomPich;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
	}

	private void OnEnable()
	{
		if (UseRandomPich)
		{
			audioSource.pitch = Random.Range(maxPich, minPich);
		}
	}
}
