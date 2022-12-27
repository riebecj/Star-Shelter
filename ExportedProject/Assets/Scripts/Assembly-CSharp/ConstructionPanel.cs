using System.Collections.Generic;
using PreviewLabs;
using UnityEngine;

public class ConstructionPanel : MonoBehaviour
{
	public List<CraftMaterial> craftMaterials = new List<CraftMaterial>();

	public int[] materialCosts = new int[1];

	public AudioClip notEnoughMaterials;

	public AudioClip successBeep;

	public AudioClip failBeep;

	public AudioSource audioSource;

	public Animator anim;

	public GameObject constructionPart;

	public GameObject button;

	private void Start()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("PartCraft" + base.transform.position))
		{
			constructionPart.SetActive(true);
			constructionPart.GetComponentInChildren<ConstructionPart>().craftTime = 0f;
			constructionPart.GetComponentInParent<JumpShip>().CheckIfComplete();
			button.SetActive(false);
		}
	}

	public void OnCraft()
	{
		if (CanCraft())
		{
			if ((bool)anim)
			{
				anim.SetBool("Crafting", true);
				Invoke("EndCraft", 10f);
			}
			for (int i = 0; i < NanoInventory.instance.craftMaterials.Count; i++)
			{
				for (int j = 0; j < craftMaterials.Count; j++)
				{
					if (NanoInventory.instance.craftMaterials[i] == craftMaterials[j])
					{
						NanoInventory.instance.materialCounts[i] -= materialCosts[j];
					}
				}
			}
			audioSource.PlayOneShot(successBeep);
			constructionPart.SetActive(true);
			constructionPart.GetComponentInParent<JumpShip>().CheckIfComplete();
			Invoke("DisableButton", 2f);
			PreviewLabs.PlayerPrefs.SetBool("PartCraft" + base.transform.position, true);
		}
		else
		{
			OnFail();
		}
	}

	private void DisableButton()
	{
		button.SetActive(false);
	}

	private void EndCraft()
	{
		anim.SetBool("Crafting", false);
	}

	public bool CanCraft()
	{
		for (int i = 0; i < craftMaterials.Count; i++)
		{
			for (int j = 0; j < NanoInventory.instance.craftMaterials.Count; j++)
			{
				if (NanoInventory.instance.craftMaterials[j] == craftMaterials[i] && NanoInventory.instance.materialCounts[j] < materialCosts[i])
				{
					return false;
				}
			}
		}
		return true;
	}

	public void OnFail()
	{
		GameAudioManager.instance.AddToSuitQueue(notEnoughMaterials);
		audioSource.PlayOneShot(successBeep);
	}
}
