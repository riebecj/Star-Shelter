using UnityEngine;
using VRTK;

public class ResearchNode : MonoBehaviour
{
	internal VRTK_InteractableObject interact;

	internal bool isheld;

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += DoObjectGrab;
		interact.InteractableObjectUngrabbed += DoObjectDrop;
		Invoke("ResetPosition", 5f);
	}

	private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
	{
		for (int i = 0; i < UpgradeManager.upgradeManagers.Count; i++)
		{
			UpgradeManager.upgradeManagers[i].returnIcon = false;
		}
		isheld = true;
	}

	private void DoObjectDrop(object sender, InteractableObjectEventArgs e)
	{
		for (int i = 0; i < UpgradeManager.upgradeManagers.Count; i++)
		{
			UpgradeManager.upgradeManagers[i].returnIcon = true;
			UpgradeManager.upgradeManagers[i].StartCoroutine("ReturnIcon");
		}
		isheld = false;
	}

	private void ResetPosition()
	{
		if (!isheld)
		{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			base.transform.localPosition = Vector3.zero;
		}
		Invoke("ResetPosition", 5f);
	}
}
