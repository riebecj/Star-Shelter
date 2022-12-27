using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRTK;

public class GateFillButton : MonoBehaviour
{
	public UnityEvent OnClick;

	private Transform target;

	internal float fillValue;

	internal int Vibration = 800;

	public Image[] fillImages;

	internal bool triggered;

	private void OnTriggerStay(Collider other)
	{
		if (triggered || !(other.tag == "Controller"))
		{
			return;
		}
		if (target != other.transform.root)
		{
			target = other.transform.root;
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(other.transform.parent.gameObject), Vibration);
			fillValue += Time.deltaTime;
			Image[] array = fillImages;
			foreach (Image image in array)
			{
				image.fillAmount = fillValue;
			}
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
			Image[] array = fillImages;
			foreach (Image image in array)
			{
				image.fillAmount = fillValue;
			}
			target = null;
			triggered = false;
		}
	}

	private void OnComplete()
	{
		fillValue = 0f;
		OnClick.Invoke();
		triggered = true;
	}
}
