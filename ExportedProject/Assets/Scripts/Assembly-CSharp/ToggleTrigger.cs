using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class ToggleTrigger : MonoBehaviour
{
	public UnityEvent OnToggle;

	public GameObject[] Objects;

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
				for (int i = 0; i < Objects.Length; i++)
				{
					if (Objects[i].activeSelf)
					{
						Objects[i].SetActive(false);
					}
					else
					{
						Objects[i].SetActive(true);
					}
				}
			}
			else
			{
				On = true;
				for (int j = 0; j < Objects.Length; j++)
				{
					if (Objects[j].activeSelf)
					{
						Objects[j].SetActive(false);
					}
					else
					{
						Objects[j].SetActive(true);
					}
				}
			}
			target = other.transform.root;
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(other.transform.GetComponentInParent<VRTK_ControllerEvents>().gameObject), Vibration);
			OnToggle.Invoke();
			Invoke("Cooldown", 0.5f);
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

	public void Reset()
	{
		On = false;
		for (int i = 0; i < Objects.Length; i++)
		{
			if (Objects[i].activeSelf)
			{
				Objects[i].SetActive(false);
			}
			else
			{
				Objects[i].SetActive(true);
			}
		}
	}

	public void SetState(bool _On)
	{
		if (_On)
		{
			On = true;
			for (int i = 0; i < Objects.Length; i++)
			{
				if (Objects[i].activeSelf)
				{
					Objects[i].SetActive(false);
				}
				else
				{
					Objects[i].SetActive(true);
				}
			}
		}
		else
		{
			On = false;
		}
	}

	public void Cooldown()
	{
	}
}
