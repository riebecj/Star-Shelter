using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CheckTime : MonoBehaviour
{
	public Text TimeLabel;

	public Text LabelCurrentTime;

	public Text score;

	private string timeString;

	private void OnEnable()
	{
		StartCoroutine("UpdateState");
	}

	private IEnumerator UpdateState()
	{
		while (true)
		{
			TimeSpan currentTime = DateTime.Now.TimeOfDay;
			LabelCurrentTime.text = DateTime.Now.ToString("hh:mm");
			if (currentTime.Hours == 1 || currentTime.Hours <= 11)
			{
				TimeLabel.text = "AM";
			}
			else
			{
				TimeLabel.text = "PM";
			}
			if ((bool)GameManager.instance && !GameManager.instance.loading && (Application.isEditor || ((bool)GameManager.instance && GameManager.instance.debugMode)) && Application.loadedLevelName == "MainScene")
			{
				score.text = DifficultyManager.instance.GetScore().ToString();
			}
			yield return new WaitForSeconds(5f);
		}
	}
}
