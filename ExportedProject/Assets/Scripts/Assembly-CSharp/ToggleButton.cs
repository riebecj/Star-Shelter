using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRTK;

public class ToggleButton : MonoBehaviour
{
	public UnityEvent OnToggle;

	public Sprite onSprite;

	public Sprite offSprite;

	public Image icon;

	public bool On;

	public bool toggleOnAwake;

	private Transform target;

	internal int Vibration = 800;

	private void OnEnable()
	{
		Invoke("Cooldown", 0.5f);
		if (toggleOnAwake)
		{
			Invoke("ToggleOnAwake", 0.25f);
		}
	}

	private void ToggleOnAwake()
	{
		OnToggle.Invoke();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (IsInvoking("Cooldown") || !(other.tag == "Controller"))
		{
			return;
		}
		if (target != other.transform.root)
		{
			if (On)
			{
				On = false;
				icon.sprite = offSprite;
			}
			else
			{
				On = true;
				icon.sprite = onSprite;
			}
			target = other.transform.root;
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(other.transform.GetComponentInParent<VRTK_ControllerEvents>().gameObject), Vibration);
			OnToggle.Invoke();
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
			target = null;
		}
	}

	public void TurnOff()
	{
		if (On)
		{
			On = false;
			icon.sprite = offSprite;
		}
		OnToggle.Invoke();
	}

	public void SetState(bool _On)
	{
		if (_On)
		{
			On = true;
			icon.sprite = onSprite;
		}
		else
		{
			On = false;
			icon.sprite = offSprite;
		}
	}

	public void Cooldown()
	{
	}
}
