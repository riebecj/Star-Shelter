using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
	public GameObject[] Objectives;

	private Objective[] allObjectives;

	public List<Objective> activeObjectives = new List<Objective>();

	public GameObject ObjectiveNotice;

	public GameObject objectiveButton;

	public GameObject buttonTab;

	public GameObject objectiveTab;

	public static ObjectiveManager instance;

	internal int currentObjectiveIndex;

	public Transform buttonList;

	public Transform objectives;

	public List<ObjectiveButton> objectiveButtons = new List<ObjectiveButton>();

	private void Awake()
	{
		instance = this;
		if (Application.loadedLevelName != "MainScene")
		{
			return;
		}
		PreviewLabs.PlayerPrefs.SetBool("ObjectiveStarted0", true);
		if (PreviewLabs.PlayerPrefs.HasKey("CurrentObjective") && PreviewLabs.PlayerPrefs.GetInt("CurrentObjective") < Objectives.Length && Objectives.Length > 0)
		{
			if (Objectives.Length >= PreviewLabs.PlayerPrefs.GetInt("CurrentObjective"))
			{
				Objectives[PreviewLabs.PlayerPrefs.GetInt("CurrentObjective")].SetActive(true);
			}
			currentObjectiveIndex = PreviewLabs.PlayerPrefs.GetInt("CurrentObjective");
		}
		LoadObjectives();
		LoadButtons();
	}

	private void LoadButtons()
	{
		for (int num = allObjectives.Length - 1; num >= 0; num--)
		{
			for (int num2 = activeObjectives.Count - 1; num2 >= 0; num2--)
			{
				if (!activeObjectives[num2].hasButton && allObjectives[num] == activeObjectives[num2])
				{
					activeObjectives[num2].hasButton = true;
					GameObject gameObject = Object.Instantiate(objectiveButton, buttonList);
					if (!BaseLoader.instance.isLoading)
					{
						gameObject.transform.SetAsFirstSibling();
					}
					ObjectiveButton component = gameObject.GetComponent<ObjectiveButton>();
					component.OnSetup(activeObjectives[num2].objectiveName, num);
					component.objective = activeObjectives[num2];
					component.UpdateObjectiveState();
					objectiveButtons.Add(component);
				}
			}
		}
		UpdateButtonsStates();
	}

	private void LoadObjectives()
	{
		allObjectives = objectives.GetComponentsInChildren<Objective>();
		for (int i = 0; i < allObjectives.Length; i++)
		{
			if (PreviewLabs.PlayerPrefs.GetBool("ObjectiveStarted" + allObjectives[i].objectiveIndex))
			{
				activeObjectives.Add(allObjectives[i]);
			}
		}
	}

	public void UpdateButtonsStates()
	{
		for (int i = 0; i < objectiveButtons.Count; i++)
		{
			objectiveButtons[i].UpdateObjectiveState();
		}
	}

	public void OnObjectiveComplete(int newIndex)
	{
		if (Application.loadedLevelName != "MainScene")
		{
			return;
		}
		PreviewLabs.PlayerPrefs.SetBool("ObjectiveStarted" + newIndex, true);
		PreviewLabs.PlayerPrefs.SetInt("CurrentObjective", newIndex);
		currentObjectiveIndex = newIndex;
		ObjectiveNotice.SetActive(false);
		GameObject[] array = Objectives;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		AddObjective(currentObjectiveIndex);
		if (newIndex < 4)
		{
			Objectives[newIndex].SetActive(true);
			if ((bool)ArmUIManager.instance && !ArmUIManager.instance.tabs[4].activeSelf)
			{
				ObjectiveNotice.SetActive(true);
			}
			else
			{
				ObjectiveNotice.SetActive(false);
			}
		}
	}

	public void AddObjective(int index)
	{
		if (!activeObjectives.Contains(allObjectives[index]))
		{
			PreviewLabs.PlayerPrefs.SetBool("ObjectiveStarted" + index, true);
			activeObjectives.Add(allObjectives[index]);
			LoadButtons();
		}
	}

	public void ShowObjective(int index)
	{
		buttonTab.SetActive(false);
		objectiveTab.SetActive(true);
		for (int i = 0; i < Objectives.Length; i++)
		{
			Objectives[i].SetActive(false);
		}
		Objectives[index].SetActive(true);
	}

	public void ObjectiveNoticed()
	{
		UpdateButtonsStates();
		ObjectiveNotice.SetActive(false);
	}
}
