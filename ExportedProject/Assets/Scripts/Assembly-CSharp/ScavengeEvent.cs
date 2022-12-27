using PreviewLabs;
using UnityEngine;

public class ScavengeEvent : MonoBehaviour
{
	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("ScavengeEvent"))
		{
			base.transform.parent.gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		PreviewLabs.PlayerPrefs.SetBool("ScavengeEvent", true);
		if ((bool)TutorialManager.instance && TutorialManager.instance.tutorials != null)
		{
			TutorialManager.instance.OnScavengingComplete();
		}
	}
}
