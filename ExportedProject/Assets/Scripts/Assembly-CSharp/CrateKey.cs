using UnityEngine;

public class CrateKey : MonoBehaviour
{
	private void Start()
	{
		Invoke("Move", 0.5f);
	}

	private void Move()
	{
		GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * 10f);
	}
}
