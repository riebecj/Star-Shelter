using UnityEngine;

public class AudioOnTrigger : MonoBehaviour
{
	public AudioSource audioSource;

	public AudioClip audioClip;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller")
		{
			audioSource.PlayOneShot(audioClip);
		}
	}
}
