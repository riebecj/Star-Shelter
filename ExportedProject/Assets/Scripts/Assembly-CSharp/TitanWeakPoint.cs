using UnityEngine;

public class TitanWeakPoint : MonoBehaviour
{
	public TitanAI titan;

	public GameObject hitVFX;

	public GameObject breakVFX;

	public int health = 15;

	private int startHealth;

	public AudioClip onHit;

	private void Start()
	{
		startHealth = health;
	}

	public void TakeDamage(int damage)
	{
		if (!IsInvoking("Recharge"))
		{
			health -= damage;
			titan.audioSource.PlayOneShot(onHit);
			titan.anim.SetBool("Hit", true);
			titan.PieWarning();
			Invoke("EndHit", 0.1f);
			if (health <= 0)
			{
				OnGetStunned();
				breakVFX.SetActive(true);
				Invoke("Recharge", 8f);
			}
			else
			{
				hitVFX.SetActive(true);
				Invoke("DisableFX", 0.8f);
			}
		}
	}

	private void OnGetStunned()
	{
		titan.OnStun();
	}

	private void Recharge()
	{
		health = startHealth;
		breakVFX.SetActive(false);
	}

	private void EndHit()
	{
		titan.anim.SetBool("Hit", false);
	}

	private void DisableFX()
	{
		hitVFX.SetActive(false);
	}
}
