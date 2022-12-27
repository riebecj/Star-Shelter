using UnityEngine;
using UnityEngine.Events;

public class EventToggleOnTrigger : MonoBehaviour
{
	[Tooltip("Will fire the event after delay time")]
	public float m_delayTime;

	[Tooltip("Interval before button can be pressed again")]
	public float cooldown;

	[Tooltip("Will toggle on and off every other interaction")]
	public bool m_isToggledOn;

	public UnityEvent m_toggleOnEvent;

	public UnityEvent m_toggleOffEvent;

	private void OnTriggerEnter(Collider other)
	{
		if (!IsInvoking("Cooldown"))
		{
			if (cooldown != 0f)
			{
				Invoke("Cooldown", cooldown);
			}
			if (m_isToggledOn)
			{
				Invoke("TriggerOffEvent", m_delayTime);
				m_isToggledOn = false;
			}
			else
			{
				Invoke("TriggerOnEvent", m_delayTime);
				m_isToggledOn = true;
			}
		}
	}

	private void TriggerOnEvent()
	{
		m_toggleOnEvent.Invoke();
	}

	private void TriggerOffEvent()
	{
		m_toggleOffEvent.Invoke();
	}

	private void Cooldown()
	{
	}
}
