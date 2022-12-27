using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxyMine : MonoBehaviour
{
	public float aggroDistance;

	public float triggerDistance;

	public float speed = 3f;

	public GameObject particle;

	public float blastRadius = 4f;

	public float explosionForce = 1000f;

	public int damage = 5;

	public static List<ProxyMine> proxyMines = new List<ProxyMine>();

	private Transform target;

	internal bool active;

	private void Awake()
	{
		proxyMines.Add(this);
	}

	public void OnTakeDamage()
	{
		OnExplode();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!active)
		{
			if ((bool)other.GetComponent<Comet>())
			{
				target = other.transform;
			}
			if ((bool)other.GetComponent<DroneAI>())
			{
				target = other.transform;
			}
			else if ((bool)other.GetComponent<TitanAI>())
			{
				target = other.transform;
			}
			else if ((bool)other.GetComponent<WallTurret>())
			{
				target = other.transform;
			}
			if ((bool)target)
			{
				active = true;
				StartCoroutine("Chase");
			}
		}
	}

	private IEnumerator Chase()
	{
		float refreshTime = 0.03f;
		while (active)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, target.position, speed * refreshTime);
			if (Vector3.Distance(base.transform.position, target.position) < triggerDistance)
			{
				OnExplode();
			}
			yield return new WaitForSeconds(refreshTime);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (active)
		{
			OnExplode();
		}
	}

	private void OnExplode()
	{
		Object.Instantiate(particle, base.transform.position, Quaternion.identity);
		Collider[] array = Physics.OverlapSphere(base.transform.position, blastRadius, GameManager.instance.physical);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if ((bool)collider.GetComponent<Rigidbody>())
			{
				Rigidbody component = collider.GetComponent<Rigidbody>();
				if (!component.isKinematic)
				{
					component.AddExplosionForce(explosionForce, base.transform.position, blastRadius);
					if ((bool)component.GetComponent<FootController>())
					{
						GameManager.instance.GetComponent<SuitManager>().OnTakeDamage(damage, 5);
					}
				}
			}
			if ((bool)collider.GetComponent<DroneAI>())
			{
				collider.GetComponent<DroneAI>().OnTakeDamage(damage);
			}
			else if ((bool)collider.GetComponent<TitanWeakPoint>())
			{
				collider.GetComponent<TitanWeakPoint>().TakeDamage(damage);
			}
			else if ((bool)collider.GetComponent<WallTurret>())
			{
				collider.GetComponent<WallTurret>().OnTakeDamage(damage);
			}
			else if ((bool)collider.GetComponent<Comet>())
			{
				collider.GetComponent<Comet>().OnGetShot();
			}
		}
		base.gameObject.SetActive(false);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, blastRadius);
	}

	private void OnDisable()
	{
		proxyMines.Remove(this);
	}
}
