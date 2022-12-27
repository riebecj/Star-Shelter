using PreviewLabs;
using UnityEngine;
using VRTK;

public class FusionSlot : MonoBehaviour
{
	private Transform core;

	public AudioClip addCoreAudio;

	private AudioSource audioSource;

	public GameObject coreProxy;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (PreviewLabs.PlayerPrefs.GetBool(string.Concat(base.transform.position, "FusionSlot")))
		{
			coreProxy.SetActive(true);
		}
		else
		{
			coreProxy.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<FusionCore>() && other.GetComponent<VRTK_InteractableObject>().IsGrabbed() && !GameManager.instance.inTitanEvent && core == null)
		{
			core = other.transform;
			core.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
			core.GetComponent<VRTK_InteractableObject>().isGrabbable = false;
			core.GetComponent<VRTK_InteractableObject>().previousParent = base.transform;
			core.GetComponent<VRTK_InteractableObject>().previousKinematicState = true;
			core.GetComponent<Collider>().enabled = false;
			other.isTrigger = true;
			core.position = base.transform.position;
			core.rotation = base.transform.rotation;
			audioSource.PlayOneShot(addCoreAudio, 0.8f);
			PreviewLabs.PlayerPrefs.SetBool(string.Concat(base.transform.position, "FusionSlot"), true);
			GameManager.instance.looseCores--;
			BaseManager.instance.OnAddCore();
		}
	}
}
