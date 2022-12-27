using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
	public GameObject particle;

	public float blastRadius = 4f;

	public float explosionForce = 1000f;

	public int damage = 5;

	public float speed = 3f;

	internal Transform target;

	private Rigidbody rigidbody;

	private List<Transform> Enemies = new List<Transform>();

	public LayerMask layerMask;

	private void Start()
	{
		DroneAI[] array = Object.FindObjectsOfType(typeof(DroneAI)) as DroneAI[];
		DroneAI[] array2 = array;
		foreach (DroneAI droneAI in array2)
		{
			if (droneAI.health > 0)
			{
				Enemies.Add(droneAI.transform);
			}
		}
		WallTurret[] array3 = Object.FindObjectsOfType(typeof(WallTurret)) as WallTurret[];
		WallTurret[] array4 = array3;
		foreach (WallTurret wallTurret in array4)
		{
			if (wallTurret.health > 0)
			{
				Enemies.Add(wallTurret.transform);
			}
		}
		TitanWeakPoint[] array5 = Object.FindObjectsOfType(typeof(TitanWeakPoint)) as TitanWeakPoint[];
		TitanWeakPoint[] array6 = array5;
		foreach (TitanWeakPoint titanWeakPoint in array6)
		{
			if (titanWeakPoint.health > 0)
			{
				Enemies.Add(titanWeakPoint.transform);
			}
		}
		GetComponent<Rigidbody>().AddForce(base.transform.forward * speed, ForceMode.VelocityChange);
		rigidbody = GetComponent<Rigidbody>();
		Invoke("StartSearch", 1.5f);
	}

	private void StartSearch()
	{
		StartCoroutine("UpdateState");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.root.tag != "Player" && !other.isTrigger)
		{
			base.gameObject.SetActive(false);
			Object.Instantiate(particle, base.transform.position, Quaternion.identity);
			OnExplode();
		}
	}

	private void OnExplode()
	{
		Object.Instantiate(particle, base.transform.position, Quaternion.identity);
		Collider[] array = Physics.OverlapSphere(base.transform.position, blastRadius, layerMask);
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
					else if ((bool)component.GetComponent<DroneAI>())
					{
						component.GetComponent<DroneAI>().OnTakeDamage(damage);
					}
					else if ((bool)collider.GetComponent<WallTurret>())
					{
						collider.GetComponent<WallTurret>().OnTakeDamage(damage);
					}
				}
				else if ((bool)collider.GetComponent<TitanWeakPoint>())
				{
					collider.GetComponent<TitanWeakPoint>().TakeDamage(collider.GetComponent<TitanWeakPoint>().health);
				}
			}
			else if ((bool)collider.GetComponent<Plate>())
			{
				collider.GetComponent<Plate>().TakeDamage(Random.Range(1, 2));
			}
			else if ((bool)collider.GetComponent<SolarPanel>())
			{
				collider.GetComponent<SolarPanel>().TakeDamage(Random.Range(1, 2));
			}
			else if ((bool)collider.GetComponent<Turret>())
			{
				collider.GetComponent<Turret>().TakeDamage(Random.Range(1, 2));
			}
			else if ((bool)collider.GetComponent<MonoBehaviour>())
			{
				collider.SendMessage("OnExplosion", SendMessageOptions.DontRequireReceiver);
			}
		}
		base.gameObject.SetActive(false);
	}

	private IEnumerator UpdateState()
	{
		GetClosestEnemy();
		rigidbody.drag = 5f;
		while (target != null)
		{
			base.transform.LookAt(target);
			if (rigidbody.velocity.magnitude < 8f)
			{
				rigidbody.AddForce(base.transform.forward, ForceMode.VelocityChange);
			}
			if (Vector3.Distance(base.transform.position * 15f, target.position) < 1.5f)
			{
				OnExplode();
			}
			yield return new WaitForSeconds(0.02f);
		}
	}

	private void GetClosestEnemy()
	{
		float num = float.PositiveInfinity;
		Transform transform = null;
		for (int i = 0; i < Enemies.Count; i++)
		{
			float num2 = Vector3.Distance(base.transform.position, Enemies[i].position);
			if (num2 < num)
			{
				transform = Enemies[i];
				num = num2;
			}
		}
		target = transform;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, blastRadius);
	}
}
