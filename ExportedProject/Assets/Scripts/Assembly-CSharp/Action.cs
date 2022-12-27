using System;
using UnityEngine;

[Serializable]
public class Action<T>
{
	public UnityEngine.Object target;

	public string method;

	public string[] candidates = new string[0];

	public int index;

	public System.Action<T> action;

	public void Awake()
	{
		action = Delegate.CreateDelegate(typeof(System.Action<T>), target, target.GetType().GetMethod(method)) as System.Action<T>;
	}
}
