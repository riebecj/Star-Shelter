using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Archive : MonoBehaviour
{
	public List<string> logList = new List<string>();

	public List<string> titleList = new List<string>();

	internal List<string> textBodies = new List<string>();

	internal List<string> titles = new List<string>();

	public List<Transform> buttons = new List<Transform>();

	public Text currentText;

	public GameObject buttonBase;

	public Transform buttonParent;

	public static Archive instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		textBodies = logList;
		titles = titleList;
		currentText.text = logList[0];
		CreateButtons();
		PlaceButtons();
	}

	public void ShowText(string newText)
	{
		currentText.text = newText;
	}

	private void CreateButtons()
	{
		for (int i = 0; i < textBodies.Count; i++)
		{
			GameObject gameObject = Object.Instantiate(buttonBase, base.transform.position, base.transform.rotation);
			buttons.Add(gameObject.transform);
			gameObject.transform.SetParent(buttonParent);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.localScale = Vector3.one;
			component.anchoredPosition3D = Vector3.zero;
			ArchiveNode component2 = component.GetComponent<ArchiveNode>();
			component2.title = titles[i];
			component2.textBody = textBodies[i];
			component2.UpdateUI();
		}
	}

	private void PlaceButtons()
	{
		float num = 360 / buttons.Count;
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].localEulerAngles = new Vector3(0f, 0f, (float)i * num);
			buttons[i].GetChild(0).localEulerAngles = new Vector3(0f, 0f, 0f - buttons[i].localEulerAngles.z);
		}
	}
}
