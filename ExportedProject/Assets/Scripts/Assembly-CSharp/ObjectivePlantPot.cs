using System.Collections;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivePlantPot : Objective
{
	public Text[] subObjectives;

	public GameObject[] checkMarks;

	public static ObjectivePlantPot instance;

	internal float completeAlpha = 0.25f;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("ObjectivePlantPot"))
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
		if (!complete && (bool)ObjectiveManager.instance)
		{
			StartCoroutine("CheckState");
		}
	}

	private void UpdateState()
	{
		if (!complete)
		{
			if ((NanoInventory.instance.materialCounts[2] >= 3 && NanoInventory.instance.materialCounts[13] >= 5 && NanoInventory.instance.materialCounts[14] >= 1) || NanoInventory.instance.craftedObjectCounts[1] > 0 || NanoInventory.instance.craftedObjectCounts[2] > 0 || SeedSlot.seedSlots.Count > 0)
			{
				subObjectives[0].color = new Color(1f, 1f, 1f, completeAlpha);
				checkMarks[0].SetActive(true);
				subObjectives[1].gameObject.SetActive(true);
			}
			if (NanoInventory.instance.craftedObjectCounts[1] > 0 || NanoInventory.instance.craftedObjectCounts[2] > 0 || SeedSlot.seedSlots.Count > 0)
			{
				subObjectives[1].color = new Color(1f, 1f, 1f, completeAlpha);
				checkMarks[1].SetActive(true);
				subObjectives[2].gameObject.SetActive(true);
			}
			if (SeedSlot.seedSlots.Count > 0)
			{
				subObjectives[2].color = new Color(1f, 1f, 1f, completeAlpha);
				checkMarks[2].SetActive(true);
				subObjectives[3].gameObject.SetActive(true);
			}
		}
	}

	public void OnPlantSeed()
	{
		if (SeedSlot.seedSlots.Count > 0)
		{
			subObjectives[3].color = new Color(1f, 1f, 1f, completeAlpha);
			checkMarks[3].SetActive(true);
			OnObjectiveComplete();
		}
	}

	public void OnObjectiveComplete()
	{
		if (!complete)
		{
			StopCoroutine("CheckState");
			PreviewLabs.PlayerPrefs.SetBool("ObjectivePlantPot", true);
			ObjectiveManager.instance.OnObjectiveComplete(objectiveIndex + 1);
			complete = true;
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
