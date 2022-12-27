using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PatchManager : MonoBehaviour
{
	public int year;

	public int month;

	public int day;

	public int hour;

	public Text timeToPatchLabel;

	private string timeString;

	private void Start()
	{
	}

	private IEnumerator UpdateInfo()
	{
		while (true)
		{
			TimeSpan ts = new DateTime(year, month, day, hour, 0, 0).Subtract(DateTime.Now);
			timeString = string.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds \n til new Update", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
			timeToPatchLabel.text = timeString;
			yield return new WaitForSeconds(1f);
		}
	}
}
