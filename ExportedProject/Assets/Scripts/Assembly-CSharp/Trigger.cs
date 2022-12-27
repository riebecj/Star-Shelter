using System;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
	[Serializable]
	public class UnityAction : Action<Collider>
	{
	}

	[Action(typeof(void), new Type[] { typeof(Collider) })]
	public UnityAction enters;

	[Action(typeof(void), new Type[] { typeof(Collider) })]
	public UnityAction stays;

	[Action(typeof(void), new Type[] { typeof(Collider) })]
	public UnityAction exits;

	public UnityEvent<int> MyEvent;

	public MyIntEvent m_MyEvent;

	public void OnTriggerEnter(Collider collider)
	{
		Debug.Log("here");
		enters.action(collider);
	}

	public void OnTriggerStays(Collider collider)
	{
		stays.action(collider);
	}

	public void OnTriggerExit(Collider collider)
	{
		exits.action(collider);
	}

	public void Awake()
	{
		enters.Awake();
		stays.Awake();
		exits.Awake();
	}
}
