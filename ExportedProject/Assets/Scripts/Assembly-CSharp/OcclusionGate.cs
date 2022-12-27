using UnityEngine;
using VRTK;

public class OcclusionGate : MonoBehaviour
{
	public float drawDistance = 1000f;

	public bool disableSunLight;

	public bool changeAudio;

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.root.tag == "Player")
		{
			ToggleOcclusion();
		}
		else if ((bool)other.GetComponent<VRTK_InteractableObject>() && (bool)other.GetComponent<Rigidbody>() && !other.GetComponent<Rigidbody>().isKinematic)
		{
			other.gameObject.transform.SetParent(GameManager.instance.stationObjects);
		}
	}

	private void ToggleOcclusion()
	{
		if (drawDistance != 0f)
		{
			GameManager.instance.Head.GetComponent<Camera>().farClipPlane = drawDistance;
		}
		else
		{
			for (int i = 0; i < Comet.unActiveComets.Count; i++)
			{
				Object.Destroy(Comet.unActiveComets[i]);
			}
			Comet.unActiveComets.Clear();
		}
		if (disableSunLight)
		{
			GameManager.instance.sunLight.enabled = false;
			GameManager.instance.Head.GetComponent<Camera>().farClipPlane = 100f;
			if (changeAudio)
			{
				GameAudioManager.instance.OnNormal();
			}
		}
		else
		{
			GameManager.instance.sunLight.enabled = true;
			GameManager.instance.Head.GetComponent<Camera>().farClipPlane = drawDistance;
			if (changeAudio)
			{
				GameAudioManager.instance.OnSpace();
			}
		}
	}
}
