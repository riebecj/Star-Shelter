using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeakPrompt : MonoBehaviour
{
	private Transform head;

	public Text powerCost;

	public Text metalCost;

	public Image repairBar;

	public GameObject repairObject;

	public GameObject repairInfo;

	public Transform info;

	public static List<LeakPrompt> leakPrompts = new List<LeakPrompt>();

	public Animator anim;

	private void Start()
	{
		leakPrompts.Add(this);
	}

	private void OnEnable()
	{
		if (!(Application.loadedLevelName != "MainScene"))
		{
			Adjust();
			head = GameManager.instance.Head;
			info.gameObject.SetActive(true);
			StartCoroutine("CheckStatus");
		}
	}

	private void Adjust()
	{
		if (!BaseManager.instance.inBase)
		{
			info.transform.localPosition = new Vector3(0f, 0f, 0.2f);
			info.transform.localEulerAngles = Vector3.zero;
		}
		else
		{
			info.transform.localPosition = new Vector3(0f, 0f, -0.2f);
			info.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
	}

	public void RepairCheck(bool value)
	{
	}

	public void DisableInfo()
	{
		repairObject.gameObject.SetActive(true);
		repairInfo.SetActive(false);
	}

	private IEnumerator CheckStatus()
	{
		while (true)
		{
			if (!BaseManager.instance.inBase)
			{
				info.transform.localPosition = new Vector3(0f, 0f, 0.2f);
				info.transform.localEulerAngles = Vector3.zero;
			}
			else
			{
				info.transform.localPosition = new Vector3(0f, 0f, -0.2f);
				info.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			}
			yield return new WaitForSeconds(3f);
		}
	}

	private void OnDestroy()
	{
		leakPrompts.Remove(this);
	}
}
