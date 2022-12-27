using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveOxygenCanister : Objective
{
	public Text[] subObjectives;

	public GameObject[] checkMarks;

	public static ObjectiveOxygenCanister instance;

	internal float completeAlpha = 0.25f;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("ObjectiveOxygenCanister"))
		{
			complete = true;
			if (ObjectiveManager.instance.currentObjectiveIndex <= objectiveIndex)
			{
				ObjectiveManager.instance.OnObjectiveComplete(objectiveIndex + 1);
			}
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
	}

	public void OnCannisterComplete()
	{
		if (!complete)
		{
			subObjectives[0].color = new Color(1f, 1f, 1f, completeAlpha);
			checkMarks[0].SetActive(true);
			OnObjectiveComplete();
		}
	}

	public void OnObjectiveComplete()
	{
		if (!complete)
		{
			PreviewLabs.PlayerPrefs.SetBool("ObjectiveOxygenCanister", true);
			ObjectiveManager.instance.OnObjectiveComplete(objectiveIndex + 1);
			complete = true;
		}
	}
}
