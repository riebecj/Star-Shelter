using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class AirLeakForce : MonoBehaviour
{
	private Collider[] bods;

	internal List<Rigidbody> bodies = new List<Rigidbody>();

	private Collider baseBounds;

	private float Force = 5f;

	public LayerMask mask;

	private void OnEnable()
	{
		if (!GameManager.instance.loading)
		{
			baseBounds = BaseManager.instance.baseBounds;
			CheckBodies();
		}
	}

	private void CheckBodies()
	{
		bods = Physics.OverlapSphere(base.transform.position, 6f, mask);
		Collider[] array = bods;
		foreach (Collider collider in array)
		{
			if (!collider.GetComponent<VRTK_InteractableObject>() || !collider.GetComponent<Rigidbody>() || collider.GetComponent<Rigidbody>().isKinematic)
			{
				continue;
			}
			if ((bool)collider.GetComponent<Food>())
			{
				if (!collider.GetComponent<VRTK_InteractableObject>().isGrabbable)
				{
					continue;
				}
				collider.GetComponent<Food>().spawner.GetComponent<FruitPlant>().OnLoot();
			}
			bodies.Add(collider.GetComponent<Rigidbody>());
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
					if (Vector3.Distance(base.transform.position, body.position) > 0.5f)
					{
						body.AddForce((base.transform.position - body.transform.position) * Force);
					}
					else
					{
						body.AddForce(base.transform.forward * Force);
					}
				}
			}
			yield return new WaitForSeconds(0.05f);
		}
	}
}
