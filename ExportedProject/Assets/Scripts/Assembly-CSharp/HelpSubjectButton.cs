using UnityEngine;
using UnityEngine.UI;

public class HelpSubjectButton : MonoBehaviour
{
	public Text buttonName;

	public Text buttonNameColor;

	internal HelpManual helpManual;

	private int index;

	public void OnSetup(string _buttonName, int _index, HelpManual _helpManual)
	{
		buttonName.text = _buttonName;
		buttonNameColor.text = _buttonName;
		index = _index;
		helpManual = _helpManual;
	}

	public void OnClick()
	{
		helpManual.UpdateHelpText(index);
	}
}
