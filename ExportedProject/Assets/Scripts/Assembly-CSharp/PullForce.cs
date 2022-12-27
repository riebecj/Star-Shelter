using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PullForce : MonoBehaviour
{
	private Collider[] bods;

	internal List<Rigidbody> bodies = new List<Rigidbody>();

	private Collider baseBounds;

	public float Force = 5f;

	public LayerMask mask;

	private void Start()
	{
		baseBounds = GetComponent<Collider>();
		CheckBodies();
	}

	public void CheckBodies()
	{
		bods = Physics.OverlapSphere(base.transform.position, 6f, mask);
		Collider[] array = bods;
		foreach (Collider collider in array)
		{
			if ((bool)collider.GetComponent<VRTK_InteractableObject>() && (bool)collider.GetComponent<Rigidbody>() && !collider.GetComponent<Rigidbody>().isKinematic)
			{
				bodies.Add(collider.GetComponent<Rigidbody>());
			}
		}
		StartCoroutine("UpdateForce");
	}

	private IEnumerator UpdateForce()
	{
		while (bodies.Count > 0)
		{
			foreach (Rigidbody body in bodies)
			{
				if (baseBounds.bounds.Contains(body.transform.position))
				{
					body.AddForce((base.transform.position - body.transform.position).normalized * Force);
				}
			}
			yield return new WaitForSeconds(0.05f);
		}
	}
}
