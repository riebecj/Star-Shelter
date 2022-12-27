using UnityEngine;

public class GunProjectile : MonoBehaviour
{
	public float speed = 10f;

	public GameObject impactParticle;

	private void Start()
	{
		GetComponent<Rigidbody>().velocity = base.transform.forward * speed;
	}
}
