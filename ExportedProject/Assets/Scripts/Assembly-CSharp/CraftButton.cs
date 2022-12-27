using System.Collections.Generic;
using UnityEngine;

public class CraftButton : MonoBehaviour
{
	private List<CraftButton> craftButtons = new List<CraftButton>();

	internal Material storedMaterial;

	private void Start()
	{
		storedMaterial = GetComponentInChildren<MeshRenderer>().material;
		craftButtons.Add(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller")
		{
			OnHighlight();
		}
	}

	public void DisableHighLight()
	{
		GetComponentInChildren<MeshRenderer>().material = storedMaterial;
	}

	public void OnHighlight()
	{
		foreach (CraftButton craftButton in craftButtons)
		{
			craftButton.DisableHighLight();
		}
		GetComponentInChildren<MeshRenderer>().material = Inventory.instance.buttonHighlightedMaterial;
		ArmUIManager.instance.PlayClickAudio();
	}
}
