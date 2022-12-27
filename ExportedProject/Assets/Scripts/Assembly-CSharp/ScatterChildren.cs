using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterChildren : MonoBehaviour
{
	private List<Rigidbody> bodies = new List<Rigidbody>();

	public float force = 5000f;

	private void Start()
	{
		IEnumerator enumerator = base.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				if ((bool)transform.GetComponent<Rigidbody>())
				{
					bodies.Add(transform.GetComponent<Rigidbody>());
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		foreach (Rigidbody body in bodies)
		{
			body.AddExplosionForce(force, base.transform.position, 5f);
		}
	}
}
