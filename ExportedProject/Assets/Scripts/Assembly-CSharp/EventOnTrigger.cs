using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class EventOnTrigger : MonoBehaviour
{
	[Tooltip("Will fire the event after delay time")]
	public float m_delayTime;

	[Tooltip("Interval before button can be pressed again")]
	public float cooldown;

	[ShowIf("useSingleObject", true)]
	public Collider m_triggerCollider;

	public bool useSingleObject;

	public UnityEvent m_onTriggerEnterEvent;

	public UnityEvent m_onTriggerExitEvent;

	private void Start()
	{
		Invoke("Cooldown", 0.5f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (IsInvoking("Cooldown"))
		{
			return;
		}
		if (cooldown != 0f)
		{
			Invoke("Cooldown", cooldown);
		}
		if (useSingleObject)
		{
			if (other == m_triggerCollider)
			{
				Invoke("TriggerEnterEvent", m_delayTime);
			}
		}
		else
		{
			Invoke("TriggerEnterEvent", m_delayTime);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (useSingleObject)
		{
			if (other == m_triggerCollider)
			{
				Invoke("TriggerExitEvent", m_delayTime);
			}
		}
		else
		{
			Invoke("TriggerExitEvent", m_delayTime);
		}
	}

	public void TriggerEnterEvent()
	{
		m_onTriggerEnterEvent.Invoke();
	}

	public void TriggerExitEvent()
	{
		m_onTriggerExitEvent.Invoke();
	}

	private void Cooldown()
	{
	}
}
