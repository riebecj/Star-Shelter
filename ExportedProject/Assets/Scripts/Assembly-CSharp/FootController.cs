using UnityEngine;

public class FootController : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
	}

	private void OnParticleCollision(GameObject other)
	{
		if (!IsInvoking("Cooldown"))
		{
			Invoke("Cooldown", 0.5f);
			SuitManager.instance.OnTakeDamage(5, 3);
			SuitManager.instance.OnBodyImpact(0);
		}
	}

	private void Cooldown()
	{
	}
}
