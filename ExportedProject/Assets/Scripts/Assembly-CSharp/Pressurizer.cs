using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;
using UnityEngine.UI;

public class Pressurizer : MonoBehaviour
{
	public float oxygen;

	public float maxOxygen;

	internal Room room;

	internal bool full;

	public Image oxygenBar;

	public Text oxygenText;

	public Text buttonText;

	public static List<Pressurizer> pressurizers = new List<Pressurizer>();

	private void OnEnable()
	{
		pressurizers.Add(this);
	}

	private void OnDisable()
	{
		pressurizers.Remove(this);
	}

	private void Start()
	{
		room = GetComponentInParent<Room>();
		OnLoad();
	}

	public void OnSave()
	{
		string text = base.transform.position.ToString();
		PreviewLabs.PlayerPrefs.SetFloat(text + "oxygen", oxygen);
	}

	public void OnLoad()
	{
		string text = base.transform.position.ToString();
		if (PreviewLabs.PlayerPrefs.HasKey(text + "oxygen"))
		{
			oxygen = PreviewLabs.PlayerPrefs.GetFloat(text + "oxygen");
		}
		full = true;
		buttonText.text = "Release Oxygen";
		UpdateUI();
	}

	public void TogglePressure()
	{
		float num = 0f;
		if (full)
		{
			num = Mathf.Clamp(oxygen, 0f, room.OxgenCapacity - room.Oxygen);
			room.Oxygen += num;
			oxygen -= num;
			full = false;
			buttonText.text = "Contain Oxygen";
		}
		else
		{
			num = Mathf.Clamp(room.Oxygen, 0f, maxOxygen - oxygen);
			oxygen += num;
			if (room.Oxygen - num > 0f)
			{
				room.Oxygen -= num;
			}
			else
			{
				room.Oxygen = 0f;
			}
			full = true;
			buttonText.text = "Release Oxygen";
		}
		UpdateUI();
	}

	private void UpdateUI()
	{
		oxygenBar.fillAmount = oxygen / maxOxygen;
		oxygenText.text = oxygen.ToString("F0") + "/" + maxOxygen.ToString("F0");
	}
}
