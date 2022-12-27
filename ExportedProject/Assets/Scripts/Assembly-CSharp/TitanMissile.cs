using System.Collections;
using UnityEngine;

public class TitanMissile : MonoBehaviour
{
	public GameObject particle;

	public float blastRadius = 4f;

	public float explosionForce = 1000f;

	public int damage = 5;

	public float speed = 1f;

	public Transform target;

	private Rigidbody rigidbody;

	internal bool targetPlayer;

	public LayerMask layerMask;

	internal static Transform currentMissile;

	private void Start()
	{
		GetComponent<Rigidbody>().AddForce(base.transform.forward * speed, ForceMode.VelocityChange);
		rigidbody = GetComponent<Rigidbody>();
		Invoke("StartSearch", 1.5f);
		currentMissile = base.transform;
	}

	private void StartSearch()
	{
		StartCoroutine("UpdateState");
	}

	public void OnTakeDamage(int value)
	{
		base.gameObject.SetActive(false);
		Object.Instantiate(particle, base.transform.position, Quaternion.identity);
		OnExplode();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.transform.root.GetComponent<TitanAI>() && !other.isTrigger)
		{
			base.gameObject.SetActive(false);
			Object.Instantiate(particle, base.transform.position, Quaternion.identity);
			OnExplode();
		}
	}

	public void OnExplode()
	{
		Object.Instantiate(particle, base.transform.position, Quaternion.identity);
		Collider[] array = Physics.OverlapSphere(base.transform.position, blastRadius, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.attachedRigidbody != null)
			{
				Rigidbody attachedRigidbody = collider.attachedRigidbody;
				if (!attachedRigidbody.isKinematic)
				{
					attachedRigidbody.AddExplosionForce(explosionForce, base.transform.position, blastRadius);
					if ((bool)attachedRigidbody.GetComponent<FootController>() && !IsInvoking("PlayerDamageCooldown"))
					{
						GameManager.instance.GetComponent<SuitManager>().OnTakeDamage(damage, 5);
						Invoke("PlayerDamageCooldown", 1f);
					}
					else if ((bool)attachedRigidbody.GetComponent<DroneAI>())
					{
						attachedRigidbody.GetComponent<DroneAI>().OnTakeDamage(damage);
					}
				}
			}
			else if ((bool)collider.GetComponent<Plate>())
			{
				collider.GetComponent<Plate>().TakeDamage(1);
			}
			else if ((bool)collider.GetComponent<SolarPanel>())
			{
				collider.GetComponent<SolarPanel>().TakeDamage(1);
			}
			else if ((bool)collider.GetComponent<Turret>())
			{
				collider.GetComponent<Turret>().TakeDamage(1);
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
		rigidbody.drag = 5f;
		while (target != null)
		{
			if (!targetPlayer && Vector3.Distance(base.transform.position, target.position) < 2f)
			{
				targetPlayer = true;
				target = GameManager.instance.Head;
			}
			base.transform.LookAt(target);
			if (rigidbody.velocity.magnitude < 8f)
			{
				rigidbody.AddForce(base.transform.forward, ForceMode.VelocityChange);
			}
			if (Vector3.Distance(base.transform.position * 15f, target.position) < 1.5f && targetPlayer)
			{
				OnExplode();
			}
			yield return new WaitForSeconds(0.02f);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, blastRadius);
	}

	private void PlayerDamageCooldown()
	{
	}

	private void OnDisable()
	{
		currentMissile = null;
	}
}
