using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
	[Tooltip("Will fire the event after delay time")]
	public float m_delayTime;

	public UnityEvent m_TriggerEvent;

	public void TriggerWithDelay()
	{
		Invoke("TriggerEvent", m_delayTime);
	}

	public void TriggerEvent()
	{
		m_TriggerEvent.Invoke();
	}
}
