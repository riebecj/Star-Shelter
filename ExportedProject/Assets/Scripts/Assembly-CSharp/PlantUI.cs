using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantUI : MonoBehaviour
{
	public static List<PlantUI> plantUIs = new List<PlantUI>();

	public Text infoHeader;

	public Text infoBody;

	public Text growthInfo;

	public Image healthBar;

	private Transform head;

	private void Start()
	{
		head = GameManager.instance.Head;
		base.gameObject.SetActive(false);
	}

	public void Setup(string header, string body)
	{
		infoHeader.text = header;
		infoBody.text = body;
		growthInfo.text = string.Empty;
	}

	private void OnEnable()
	{
		if ((bool)base.transform.parent.GetComponentInChildren<FruitPlant>())
		{
			UpdateHealth(base.transform.parent.GetComponentInChildren<FruitPlant>().health);
		}
		else if ((bool)base.transform.parent.GetComponentInChildren<OxygenPlant>())
		{
			UpdateHealth(base.transform.parent.GetComponentInChildren<OxygenPlant>().health);
		}
	}

	private void OnDestroy()
	{
		plantUIs.Remove(this);
	}

	public void UpdateHealth(float health)
	{
		healthBar.fillAmount = health / 100f;
	}

	public void UpdateGrowth(float growth)
	{
		growthInfo.text = (growth * 100f).ToString("F1") + "%";
	}

	public void Reset()
	{
		healthBar.fillAmount = 0f;
		growthInfo.text = string.Empty;
		infoHeader.text = string.Empty;
		infoBody.text = string.Empty;
	}
}
