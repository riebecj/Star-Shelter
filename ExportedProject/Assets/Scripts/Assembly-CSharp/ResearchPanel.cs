using UnityEngine;

public class ResearchPanel : MonoBehaviour
{
	public Animation animation;

	internal bool triggered;

	public GameObject[] disabledObjects;

	public GameObject[] enabledObjects;

	public void DownloadResearch()
	{
		if (!triggered)
		{
			GameObject[] array = disabledObjects;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(false);
			}
			triggered = true;
			animation.gameObject.SetActive(true);
			Invoke("AddResearchPoint", animation.clip.length);
		}
	}

	private void AddResearchPoint()
	{
		BaseManager.researchPoints++;
		for (int i = 0; i < UpgradeManager.upgradeManagers.Count; i++)
		{
			UpgradeManager.upgradeManagers[i].UpdateState();
		}
		GameObject[] array = enabledObjects;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(true);
		}
	}
}
