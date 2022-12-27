using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
	public int damage;

	public float speed;

	public GameObject impactParticle;

	private void Start()
	{
		Invoke("Destruct", 8f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.root.tag == "Player")
		{
			SuitManager.instance.OnTakeDamage(damage, 6);
			Object.Instantiate(SuitManager.instance.shotDamageAudio, base.transform.position, base.transform.rotation);
			Destruct();
		}
		else if ((bool)other.GetComponent<ShieldCollisionCheck>())
		{
			other.GetComponent<ShieldCollisionCheck>().OnTakeDamage(damage, base.transform);
			Destruct();
		}
		else if (((bool)other.transform.root.GetComponent<Station>() || (bool)other.transform.root.GetComponent<BaseManager>() || (bool)other.transform.root.GetComponent<Wreckage>()) && !other.isTrigger)
		{
			Object.Instantiate(impactParticle, base.transform.position, Quaternion.Inverse(base.transform.rotation));
			Destruct();
		}
	}

	private void Destruct()
	{
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		base.transform.position += base.transform.forward * Time.deltaTime * speed;
	}
}
