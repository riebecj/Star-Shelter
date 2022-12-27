using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RadiationShower : MonoBehaviour
{
	public float delay = 5f;

	public Animation anim;

	public Text powerCost;

	public Text radiationLevel;

	private void Start()
	{
		StartCoroutine("UpdateState");
	}

	private void OnEnable()
	{
		if ((bool)ObjectiveRadiation.instance)
		{
			ObjectiveRadiation.instance.showerCrafted = true;
		}
	}

	public void OnStartScan()
	{
		if (BaseManager.instance.power < SuitManager.instance.radiation * 2f)
		{
			SuitManager.instance.LowPowerPrompt();
			return;
		}
		Invoke("OnClearRadiation", delay);
		anim.Play();
	}

	private IEnumerator UpdateState()
	{
		while (true)
		{
			powerCost.text = (SuitManager.instance.radiation * 2f).ToString("F0");
			radiationLevel.text = SuitManager.instance.radiation.ToString("F0");
			yield return new WaitForSeconds(5f);
		}
	}

	private void OnClearRadiation()
	{
		SuitManager.instance.radiation = 0f;
		radiationLevel.text = "0";
		BaseManager.instance.power -= SuitManager.instance.radiation * 2f;
		ObjectiveRadiation.instance.showerUsed = true;
	}
}
