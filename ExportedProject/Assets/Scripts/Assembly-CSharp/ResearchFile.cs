using UnityEngine;
using UnityEngine.UI;

public class ResearchFile : MonoBehaviour
{
	public int type;

	public Image symbol;

	public Image symbolColor;

	public void SetSymbol(Sprite newSymbol, int newType)
	{
		symbol.sprite = newSymbol;
		symbolColor.sprite = newSymbol;
		type = newType;
	}
}
