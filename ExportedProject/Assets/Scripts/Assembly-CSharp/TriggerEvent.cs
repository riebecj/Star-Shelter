using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
	public UnityEvent OnClick;

	internal bool triggered;

	private void OnTriggerEnter(Collider other)
	{
		if (!triggered && other.transform.root.tag == "Player")
		{
			triggered = true;
			OnClick.Invoke();
		}
	}
}
