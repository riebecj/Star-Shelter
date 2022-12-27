using UnityEngine;
using UnityEngine.Events;

public class EventOnEnable : MonoBehaviour
{
	public float m_delayTime;

	public UnityEvent m_onEnableEvent;

	private void OnEnable()
	{
		Invoke("StartEvent", m_delayTime);
	}

	private void StartEvent()
	{
		m_onEnableEvent.Invoke();
	}
}
