using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileHackA : MonoBehaviour
{
	public FileBox[] filesBoxes;

	public Transform[] files;

	public Sprite[] symbols;

	public Text powerUI;

	internal bool active;

	public GameObject hackUI;

	public GameObject hackButton;

	public GameObject completePanel;

	public GameObject cancelButton;

	public int powerCost = 10;

	internal AudioSource audioSource;

	internal Animator animator;

	internal bool hasTarget;

	public AudioClip StartHackAudio;

	public AudioClip HackCompleteAudio;

	public AudioClip HackFailedAudio;

	public AudioClip filesortedAudio;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		AssignGenerator();
	}

	private void AssignGenerator()
	{
		foreach (PowerGenerator powerGenerator in PowerGenerator.powerGenerators)
		{
			if (Vector3.Distance(base.transform.position, powerGenerator.transform.position) < (float)PowerGenerator.generatorDistance)
			{
				powerGenerator.togglePower = (PowerGenerator.TogglePower)Delegate.Combine(powerGenerator.togglePower, new PowerGenerator.TogglePower(TogglePower));
			}
		}
	}

	public void TogglePower(bool on)
	{
		if (on)
		{
			hackButton.SetActive(true);
		}
		else
		{
			hackButton.SetActive(false);
		}
	}

	public void Shuffle()
	{
		for (int i = 0; i < filesBoxes.Length; i++)
		{
			filesBoxes[i].StopAllCoroutines();
		}
		Transform[] array = files;
		foreach (Transform transform in array)
		{
			transform.localPosition = Vector3.zero;
		}
		List<Sprite> list = new List<Sprite>();
		Sprite[] array2 = symbols;
		foreach (Sprite item in array2)
		{
			list.Add(item);
		}
		int[] array3 = new int[9] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
		Shuffle(array3);
		for (int l = 0; l < filesBoxes.Length; l++)
		{
			filesBoxes[l].source = this;
			filesBoxes[l].SetSymbol(symbols[array3[l]], array3[l]);
			files[l].GetComponent<ResearchFile>().SetSymbol(symbols[array3[l]], array3[l]);
		}
		Transform[] array4 = files;
		for (int m = 0; m < array4.Length; m++)
		{
			array4[m].position += new Vector3(UnityEngine.Random.Range(-0.1f, 0.3f), UnityEngine.Random.Range(-0.15f, 0.25f), UnityEngine.Random.Range(-0.1f, 0.3f));
		}
	}

	private void CheckBoxes()
	{
		bool flag = true;
		for (int i = 0; i < filesBoxes.Length; i++)
		{
			if (!filesBoxes[i].sorted)
			{
				flag = false;
			}
		}
		if (flag)
		{
			OnComplete();
		}
	}

	private void OnComplete()
	{
		StopCoroutine("CountDown");
		StopCoroutine("UpdateState");
		ScaleOut();
		animator.SetBool("Success", true);
	}

	private void ScaleOut()
	{
		hackUI.SetActive(false);
		completePanel.SetActive(true);
		audioSource.PlayOneShot(HackCompleteAudio);
		AddResearchPoint();
	}

	private void AddResearchPoint()
	{
		BaseManager.researchPoints++;
		for (int i = 0; i < UpgradeManager.upgradeManagers.Count; i++)
		{
			UpgradeManager.upgradeManagers[i].AddResearchPoint();
		}
	}

	private IEnumerator UpdateState()
	{
		while (true)
		{
			CheckBoxes();
			yield return new WaitForSeconds(0.02f);
		}
	}

	private IEnumerator CountDown()
	{
		while (SuitManager.instance.power > 1.1f)
		{
			SuitManager.instance.power -= 1f;
			powerUI.text = SuitManager.instance.power.ToString();
			if (SuitManager.instance.power < 1f)
			{
				Reset();
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public void StartHack()
	{
		if (!hackUI.activeSelf)
		{
			if (SuitManager.instance.power < 1f)
			{
				SuitManager.instance.LowPowerPrompt();
				return;
			}
			Shuffle();
			hackUI.SetActive(true);
			hackButton.SetActive(false);
			Invoke("EnableCancel", 1.5f);
			StartCoroutine("CountDown");
			StartCoroutine("UpdateState");
			audioSource.PlayOneShot(StartHackAudio);
			animator.SetBool("Active", true);
		}
	}

	private void EnableCancel()
	{
		cancelButton.SetActive(true);
	}

	public void QuitHack()
	{
		hackUI.SetActive(false);
	}

	public void Reset()
	{
		hackUI.SetActive(false);
		cancelButton.SetActive(false);
		hackButton.SetActive(true);
		audioSource.PlayOneShot(HackFailedAudio);
		animator.SetBool("Active", false);
	}

	public void OnFileSorted()
	{
		audioSource.PlayOneShot(filesortedAudio);
	}

	public static void Shuffle<T>(T[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			T val = array[i];
			int num = UnityEngine.Random.Range(i, array.Length);
			array[i] = array[num];
			array[num] = val;
		}
	}

	public void FailAudio()
	{
		audioSource.PlayOneShot(HackFailedAudio);
	}
}
