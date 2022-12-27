using UnityEngine;

public class RepairEventTrigger : MonoBehaviour
{
	internal bool triggerd;

	private void OnTriggerEnter(Collider other)
	{
		if (!triggerd && other.transform.root.tag == "Player")
		{
			triggerd = true;
			IntroManager.instance.SalvageComplete();
			IntroManager.instance.ToggleRepair();
			IntroEventRepair.instance.TogglePointer();
		}
	}
}
