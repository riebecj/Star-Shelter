using UnityEngine;

public class ShieldCollisionCheck : MonoBehaviour
{
	private HandShield handShield;

	private void Start()
	{
		handShield = GetComponentInParent<HandShield>();
	}

	public void OnTakeDamage(int damage, Transform impactTransform)
	{
		handShield.OnTakeDamage(damage, impactTransform);
	}
}
