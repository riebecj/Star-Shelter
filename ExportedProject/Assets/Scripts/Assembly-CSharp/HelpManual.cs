using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HelpManual : MonoBehaviour
{
	public GameObject buttonProxy;

	public GameObject infoPanel;

	public GameObject buttonList;

	public GameObject archiveButtons;

	public HelpDocument[] helpDocuments;

	public Text title;

	public Text titleColor;

	public Text breadText;

	private void Start()
	{
		Load();
	}

	private void Load()
	{
		helpDocuments = Resources.LoadAll("Help", typeof(HelpDocument)).Cast<HelpDocument>().ToArray();
		for (int i = 0; i < helpDocuments.Length; i++)
		{
			GameObject gameObject = Object.Instantiate(buttonProxy, buttonList.transform);
			gameObject.GetComponent<HelpSubjectButton>().OnSetup(helpDocuments[i].title, i, this);
		}
	}

	public void UpdateHelpText(int index)
	{
		buttonList.SetActive(false);
		infoPanel.SetActive(true);
		archiveButtons.SetActive(false);
		title.text = helpDocuments[index].title;
		titleColor.text = helpDocuments[index].title;
		breadText.text = helpDocuments[index].breadText;
	}

	public void OnBack()
	{
		buttonList.SetActive(true);
		infoPanel.SetActive(false);
		archiveButtons.SetActive(true);
	}
}
