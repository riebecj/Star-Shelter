using System;
using PreviewLabs;
using UnityEngine;

public class ShieldSnapPoint : MonoBehaviour
{
	internal bool full;

	public GameObject UI;

	public GameObject placeHolder;

	public GameObject holoplace;

	internal HandShield shield;

	internal bool On;

	private void Start()
	{
		Load();
		GameManager instance = GameManager.instance;
		instance.deathDelegate = (GameManager.DeathDelegate)Delegate.Combine(instance.deathDelegate, new GameManager.DeathDelegate(OnDeath));
	}

	private void Load()
	{
		if (PreviewLabs.PlayerPrefs.GetBool("HandShield" + base.transform.parent.name))
		{
			placeHolder.SetActive(true);
			shield = placeHolder.GetComponent<HandShield>();
			shield.snapPoint = this;
			shield.Invoke("EnableUI", 0.5f);
			full = true;
			shield.LockHandInteraction();
		}
	}

	public void OnSave()
	{
		PreviewLabs.PlayerPrefs.SetBool("HandShield" + base.transform.parent.name, full);
	}

	public void UpdateState(bool state)
	{
		full = state;
		OnSave();
	}

	public void Toggle()
	{
		On = !On;
		shield.ToggleShield(On);
	}

	public void OnClear()
	{
		full = false;
		PreviewLabs.PlayerPrefs.SetBool("HandShield" + base.transform.parent.name, full);
	}

	private void OnDeath()
	{
		PreviewLabs.PlayerPrefs.SetBool("HandShield" + base.transform.parent.name, false);
		if ((bool)shield)
		{
			shield.gameObject.SetActive(false);
		}
		Debug.Log("OnDeath");
	}
}
