using PreviewLabs;
using UnityEngine;

public class AudioEvent : MonoBehaviour
{
	public AudioClip audioClip;

	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("AudioEvent" + base.transform.position))
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.root.tag == "Player")
		{
			GameAudioManager.instance.AddToSuitQueue(audioClip);
			PreviewLabs.PlayerPrefs.SetBool("AudioEvent" + base.transform.position, true);
			base.gameObject.SetActive(false);
		}
	}
}
