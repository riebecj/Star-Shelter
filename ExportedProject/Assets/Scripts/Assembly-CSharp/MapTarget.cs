using UnityEngine;

public class MapTarget : MonoBehaviour
{
	private AudioSource audioSource;

	public GameObject question;

	internal Transform hand;

	public AudioClip beep;

	internal HoloMap holomap;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (hand == null && other.tag == "Controller")
		{
			question.SetActive(true);
			hand = other.transform;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Controller")
		{
			question.SetActive(false);
			if (other.transform == hand)
			{
				hand = null;
			}
		}
	}

	public void OnSetTarget()
	{
		holomap.SetTarget(base.transform);
		audioSource.PlayOneShot(beep);
	}

	private void OnDestroy()
	{
		if (holomap.targetWreck == base.transform && holomap._worldTarget != null)
		{
			holomap.OnLoseTarget();
		}
	}
}
