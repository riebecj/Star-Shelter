using System.Collections;
using UnityEngine;
using VRTK;

public class HeadLightSwitch : MonoBehaviour
{
	internal bool On;

	public GameObject light;

	public GameObject icon;

	internal float powerDraw = 0.1f;

	internal VRTK_InteractableObject interact;

	public static HeadLightSwitch instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectUsed += OnUse;
	}

	private void OnUse(object sender, InteractableObjectEventArgs e)
	{
		if (!IsInvoking("Cooldown") && icon.activeSelf)
		{
			light.SetActive(!light.activeSelf);
			if (!light.activeSelf)
			{
				StopCoroutine("DrawPower");
			}
			else if (IntroManager.instance == null)
			{
				StartCoroutine("DrawPower");
			}
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(e.interactingObject), 800f);
			Invoke("Cooldown", 0.5f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Controller" && !other.GetComponent<FuelSnapPoint>())
		{
			icon.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Controller" && !other.GetComponent<FuelSnapPoint>())
		{
			icon.SetActive(false);
		}
	}

	private IEnumerator DrawPower()
	{
		while (SuitManager.instance.power > 1f)
		{
			SuitManager.instance.power -= powerDraw;
			yield return new WaitForSeconds(0.5f);
		}
		light.SetActive(false);
	}

	private void Cooldown()
	{
	}
}
