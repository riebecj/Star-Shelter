using PreviewLabs;
using UnityEngine;

public class DisableOnSalvage : MonoBehaviour
{
	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("IWasDisabled" + base.transform.position))
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OnSalvage()
	{
		PreviewLabs.PlayerPrefs.SetBool("IWasDisabled" + base.transform.position, true);
	}
}
