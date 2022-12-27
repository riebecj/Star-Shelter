using UnityEngine;

public class GasTank : MonoBehaviour
{
	public GameObject particle;

	public GameObject broken;

	public float blastRadius = 4f;

	public float explosionForce = 1000f;

	public int damage = 5;

	public void OnTakeDamage()
	{
		OnExplode();
	}

	private void OnExplode()
	{
		Object.Instantiate(particle, base.transform.position, Quaternion.identity);
		GameObject gameObject = Object.Instantiate(broken, base.transform.position, base.transform.rotation);
		Rigidbody[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = componentsInChildren;
		foreach (Rigidbody rigidbody in array)
		{
			rigidbody.AddForce(Random.onUnitSphere * 250f);
		}
		Collider[] array2 = Physics.OverlapSphere(base.transform.position, blastRadius, GameManager.instance.physical);
		Collider[] array3 = array2;
		foreach (Collider collider in array3)
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
			else if ((bool)collider.GetComponent<MonoBehaviour>())
			{
				collider.SendMessage("OnExplosion", SendMessageOptions.DontRequireReceiver);
			}
		}
		base.gameObject.SetActive(false);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, blastRadius);
	}
}
