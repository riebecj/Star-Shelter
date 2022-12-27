using PreviewLabs;
using UnityEngine;

public class PieIntroEvent : MonoBehaviour
{
	public AudioClip pieIntro;

	public AudioClip scavengeAudio;

	internal bool triggerd;

	public static PieIntroEvent instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("PieIntroEvent"))
		{
			base.gameObject.SetActive(false);
			instance = null;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!GameManager.instance.loading && !triggerd && other.transform.root.tag == "Player")
		{
			triggerd = true;
			GameAudioManager.instance.AddToSuitQueue(pieIntro);
			PreviewLabs.PlayerPrefs.SetBool("PieIntroEvent", true);
			TutorialManager.instance.Invoke("DisableTimer", 40f);
			Invoke("CometEvent", 14f);
		}
	}

	private void CometEvent()
	{
		CometManager.instance.TestComet();
		Invoke("Repair", 15f);
	}

	private void Repair()
	{
		TutorialManager.instance.ToggleRepair();
	}

	public void OnRepaired()
	{
		Debug.Log("OnRepaired");
		if (!TutorialManager.instance.Repaired)
		{
			GameAudioManager.instance.AddToSuitQueue(scavengeAudio);
			TutorialManager.instance.OnRepairComplete();
			TutorialManager.instance.Invoke("DisableTimer", 15f);
		}
	}
}
