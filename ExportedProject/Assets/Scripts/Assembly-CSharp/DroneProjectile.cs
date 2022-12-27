using UnityEngine;

public class DroneProjectile : MonoBehaviour
{
	public int damage;

	public float speed;

	private float followTime;

	public GameObject impactParticle;

	private void Start()
	{
		Invoke("Destruct", 10f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<ShieldCollisionCheck>())
		{
			other.GetComponent<ShieldCollisionCheck>().OnTakeDamage(damage, base.transform);
			Destruct();
		}
		else if ((bool)other.GetComponentInParent<HoloShield>())
		{
			other.GetComponentInParent<HoloShield>().OnTakeDamage(1);
			Destruct();
		}
		else if ((bool)other.GetComponentInParent<HandShield>())
		{
			other.GetComponentInParent<HandShield>().OnTakeDamage(1, base.transform);
			Destruct();
		}
		else if ((bool)other.GetComponentInParent<Room>())
		{
			other.GetComponentInParent<Room>().OnImpact(base.gameObject, damage);
			Destruct();
		}
		else if (((bool)other.transform.root.GetComponent<Station>() || (bool)other.transform.root.GetComponent<BaseManager>() || (bool)other.transform.root.GetComponent<Wreckage>()) && !other.isTrigger)
		{
			Object.Instantiate(impactParticle, base.transform.position, Quaternion.Inverse(base.transform.rotation));
			Destruct();
		}
		else if (other.transform.root.tag == "Player")
		{
			SuitManager.instance.OnTakeDamage(damage, 4);
			Object.Instantiate(SuitManager.instance.shotDamageAudio, base.transform.position, base.transform.rotation);
			Destruct();
		}
	}

	private void Destruct()
	{
		base.gameObject.SetActive(false);
	}

	public void OnDestruct()
	{
		Object.Instantiate(impactParticle, base.transform.position, Quaternion.Inverse(base.transform.rotation));
		Destruct();
	}

	private void Update()
	{
		followTime += Time.deltaTime;
		if (followTime < 0.5f)
		{
			base.transform.LookAt(GameManager.instance.Head.position);
		}
		base.transform.position += base.transform.forward * Time.deltaTime * speed;
	}
}
