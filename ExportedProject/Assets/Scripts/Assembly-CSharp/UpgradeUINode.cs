using UnityEngine;
using VRTK;

public class UpgradeUINode : MonoBehaviour
{
	public int index;

	internal UpgradeManager upgradeManager;

	private void Start()
	{
		upgradeManager = base.transform.GetComponentInParent<UpgradeManager>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<ResearchNode>() && other.GetComponent<VRTK_InteractableObject>().IsGrabbed())
		{
			other.GetComponent<VRTK_InteractableObject>().ForceStopInteracting();
			base.transform.GetComponentInParent<UpgradeManager>().AttemptUpgrade(index);
			ObjectiveResearchPoint.instance.OnResearch();
		}
	}
}
