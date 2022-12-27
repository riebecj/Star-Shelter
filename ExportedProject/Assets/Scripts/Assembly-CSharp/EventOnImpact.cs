using UnityEngine;
using UnityEngine.Events;

public class EventOnImpact : MonoBehaviour
{
	public float impactThreshhold;

	public UnityEvent m_onCollisionEvent;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > impactThreshhold)
		{
			m_onCollisionEvent.Invoke();
		}
	}
}
