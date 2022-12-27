using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentUI : MonoBehaviour
{
	public Image[] icons;

	public Text[] counters;

	public static List<ComponentUI> componentUIs = new List<ComponentUI>();

	private Transform head;

	internal CraftComponent _craftComponent;

	private void Start()
	{
		componentUIs.Add(this);
		head = GameManager.instance.Head;
		base.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		StartCoroutine("LookAtCamera");
		if ((bool)_craftComponent)
		{
			_craftComponent.ToggleOutline(true);
		}
	}

	public void Setup(CraftComponent craftComponent)
	{
		for (int i = 0; i < 6; i++)
		{
			icons[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < craftComponent.craftMaterials.Count && j < icons.Length; j++)
		{
			icons[j].gameObject.SetActive(true);
			_craftComponent = craftComponent;
			for (int k = 0; k < NanoInventory.instance.craftMaterials.Count; k++)
			{
				if (craftComponent.craftMaterials[j] == NanoInventory.instance.craftMaterials[k])
				{
					icons[j].sprite = NanoInventory.instance.craftMaterials[k].icon;
					counters[j].text = craftComponent.materialCounts[j].ToString();
				}
			}
		}
	}

	public void Setup(RoomLeak leakComponent)
	{
		for (int i = 0; i < 6; i++)
		{
			icons[i].gameObject.SetActive(false);
		}
		icons[0].gameObject.SetActive(true);
		icons[0].sprite = NanoInventory.instance.craftMaterials[0].icon;
		counters[0].text = leakComponent.leakRate.ToString();
	}

	private IEnumerator LookAtCamera()
	{
		while (true)
		{
			base.transform.LookAt(head);
			yield return new WaitForSeconds(0.025f);
		}
	}

	private void OnDestroy()
	{
		componentUIs.Remove(this);
	}

	private void OnDisable()
	{
		if ((bool)_craftComponent)
		{
			_craftComponent.ToggleOutline(false);
		}
	}
}
