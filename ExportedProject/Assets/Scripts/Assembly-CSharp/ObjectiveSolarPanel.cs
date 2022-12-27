using System.Collections;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveSolarPanel : Objective
{
	public Text[] subObjectives;

	public GameObject[] checkMarks;

	public static ObjectiveSolarPanel instance;

	internal float completeAlpha = 0.25f;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("ObjectiveSolarPanel"))
		{
			if (ObjectiveManager.instance.currentObjectiveIndex <= objectiveIndex)
			{
				ObjectiveManager.instance.OnObjectiveComplete(objectiveIndex + 1);
			}
			complete = true;
			return;
		}
		GameObject[] array = checkMarks;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		Text[] array2 = subObjectives;
		foreach (Text text in array2)
		{
			text.gameObject.SetActive(false);
		}
		subObjectives[0].gameObject.SetActive(true);
		UpdateState();
		StartCoroutine("CheckState");
	}

	private void OnEnable()
	{
		if (!complete)
		{
			Invoke("StartCheck", 0.25f);
		}
	}

	private void StartCheck()
	{
		StartCoroutine("CheckState");
	}

	private void UpdateState()
	{
		if (!complete && !IntroManager.instance && (bool)NanoInventory.instance && (bool)ObjectiveManager.instance)
		{
			if ((NanoInventory.instance.materialCounts[0] >= 5 && NanoInventory.instance.materialCounts[4] >= 4 && NanoInventory.instance.materialCounts[12] >= 2 && NanoInventory.instance.materialCounts[13] >= 3) || NanoInventory.instance.craftedObjectCounts[43] > 0 || BaseManager.instance.SolarPanels.Count > 0)
			{
				subObjectives[0].color = new Color(1f, 1f, 1f, completeAlpha);
				checkMarks[0].SetActive(true);
				subObjectives[1].gameObject.SetActive(true);
			}
			if (NanoInventory.instance.craftedObjectCounts[43] > 0 || BaseManager.instance.SolarPanels.Count > 0)
			{
				subObjectives[1].color = new Color(1f, 1f, 1f, completeAlpha);
				checkMarks[1].SetActive(true);
				subObjectives[2].gameObject.SetActive(true);
			}
			if (BaseManager.instance.SolarPanels.Count > 0 && ObjectiveManager.instance.currentObjectiveIndex == objectiveIndex)
			{
				subObjectives[2].color = new Color(1f, 1f, 1f, completeAlpha);
				checkMarks[2].SetActive(true);
				OnObjectiveComplete();
			}
		}
	}

	public void OnObjectiveComplete()
	{
		if (!complete)
		{
			complete = true;
			PreviewLabs.PlayerPrefs.SetBool("ObjectiveSolarPanel", true);
			ObjectiveManager.instance.OnObjectiveComplete(objectiveIndex + 1);
			StopCoroutine("CheckState");
		}
	}

	private IEnumerator CheckState()
	{
		while (true)
		{
			UpdateState();
			yield return new WaitForSeconds(1f);
		}
	}

	private void OnDeactive()
	{
	}
}
