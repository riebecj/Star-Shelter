using UnityEngine;

public class Upgrade : MonoBehaviour
{
	public enum SuitUpgrade
	{
		oxygen = 0,
		power = 1,
		thrusters = 2
	}

	public SuitUpgrade suitUpgrade;

	private void Start()
	{
		OnUpgrade();
	}

	private void OnUpgrade()
	{
		if (suitUpgrade == SuitUpgrade.oxygen)
		{
			SuitManager.instance.UpgradeOxygenCapacity();
		}
		else if (suitUpgrade == SuitUpgrade.power)
		{
			SuitManager.instance.UpgradePowerCapacity();
		}
		else if (suitUpgrade == SuitUpgrade.thrusters)
		{
			SuitManager.instance.UpgradeThrusterSpeed();
		}
		Object.Destroy(base.gameObject);
	}
}
