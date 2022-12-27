using System.Collections;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveRadiation : Objective
{
	public Text[] subObjectives;

	public GameObject[] checkMarks;

	public static ObjectiveRadiation instance;

	internal bool showerCrafted;

	internal bool showerUsed;

	internal float completeAlpha = 0.25f;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("ObjectiveRadiation"))
		{
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
		if (!complete && !IntroManager.instance)
		{
			if (showerCrafted)
			{
				subObjectives[0].color = new Color(1f, 1f, 1f, completeAlpha);
				checkMarks[0].SetActive(true);
				subObjectives[1].gameObject.SetActive(true);
			}
			if (showerUsed)
			{
				subObjectives[1].color = new Color(1f, 1f, 1f, completeAlpha);
				checkMarks[1].SetActive(true);
				OnObjectiveComplete();
			}
		}
	}

	public void OnObjectiveComplete()
	{
		if (!complete)
		{
			complete = true;
			PreviewLabs.PlayerPrefs.SetBool("ObjectiveRadiation", true);
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
