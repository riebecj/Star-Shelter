using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRTK;

public class FillButton : MonoBehaviour
{
	public UnityEvent OnClick;

	private Transform target;

	internal float fillValue;

	internal int Vibration = 800;

	public Image fillImage;

	public Color completeColor = Color.green;

	internal Color oldColor;

	private void Start()
	{
		oldColor = fillImage.color;
	}

	private void OnTriggerStay(Collider other)
	{
		if (IsInvoking("Cooldown") || !(other.tag == "Controller"))
		{
			return;
		}
		if (target != other.transform.root)
		{
			target = other.transform.root;
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(other.transform.parent.gameObject), Vibration);
			fillValue += Time.deltaTime;
			fillImage.fillAmount = fillValue;
			if (fillValue > 1f)
			{
				OnComplete();
			}
		}
		else
		{
			target = null;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Controller")
		{
			fillValue = 0f;
			fillImage.fillAmount = fillValue;
			target = null;
			fillImage.color = oldColor;
		}
	}

	private void OnComplete()
	{
		fillValue = 0f;
		OnClick.Invoke();
		Invoke("Cooldown", 1f);
		fillImage.color = completeColor;
	}

	private void OnEable()
	{
		fillValue = 0f;
		fillImage.fillAmount = fillValue;
		target = null;
	}

	private void OnDisable()
	{
		fillValue = 0f;
		fillImage.fillAmount = fillValue;
		target = null;
	}

	private void Cooldown()
	{
	}
}
