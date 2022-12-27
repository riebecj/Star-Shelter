using PreviewLabs;
using UnityEngine;

public class EntranceEvent : MonoBehaviour
{
	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("EntranceEvent"))
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.root.tag == "Player")
		{
			TutorialManager.instance.ToggleScavenging();
			GetComponent<Animation>().Play();
			PreviewLabs.PlayerPrefs.SetBool("EntranceEvent", true);
			GetComponent<Collider>().enabled = false;
		}
	}
}
