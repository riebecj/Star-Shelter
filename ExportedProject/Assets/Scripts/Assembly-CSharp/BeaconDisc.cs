using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using VRTK;

public class BeaconDisc : MonoBehaviour
{
	public int code = 1337;

	public static List<Transform> beacons = new List<Transform>();

	private VRTK_InteractableObject interact;

	public AudioClip infoAudio;

	private void Awake()
	{
		beacons.Add(base.transform);
		code = 1000 * Random.Range(1, 9) + 100 * Random.Range(1, 9) + 10 * Random.Range(1, 9) + Random.Range(1, 9);
	}

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		if ((bool)interact)
		{
			interact.InteractableObjectGrabbed += DoObjectGrab;
		}
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		if (!PreviewLabs.PlayerPrefs.GetBool("DiscGrabbed"))
		{
			GameAudioManager.instance.AddToSuitQueue(infoAudio);
			PreviewLabs.PlayerPrefs.SetBool("DiscGrabbed", true);
		}
		if (!ObjectiveBeacon.instance.complete)
		{
			ObjectiveManager.instance.AddObjective(5);
		}
	}
}
