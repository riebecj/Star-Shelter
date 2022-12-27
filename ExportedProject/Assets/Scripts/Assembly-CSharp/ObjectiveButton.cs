using UnityEngine;
using UnityEngine.UI;

public class ObjectiveButton : MonoBehaviour
{
	public Text buttonName;

	public Text symbol;

	internal Objective objective;

	private int index;

	public void OnSetup(string _buttonName, int _index)
	{
		buttonName.text = _buttonName;
		index = _index;
	}

	public void OnClick()
	{
		ObjectiveManager.instance.ShowObjective(index);
	}

	public void UpdateObjectiveState()
	{
		if (objective.complete)
		{
			symbol.text = "x";
			symbol.color = Color.gray;
			buttonName.color = Color.gray;
		}
	}
}
