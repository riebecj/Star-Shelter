using System.Collections;
using UnityEngine;

public class SparkZone : MonoBehaviour
{
	private Collider collider;

	public int damage = 15;

	internal bool updating;

	private void Start()
	{
		collider = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!updating && other.transform.root.tag == "Player")
		{
			updating = true;
			StartCoroutine("CheckDamage");
		}
	}

	private IEnumerator CheckDamage()
	{
		while (updating)
		{
			if (collider.bounds.Contains(GameManager.instance.Head.position) || collider.bounds.Contains(GameManager.instance.leftController.position) || collider.bounds.Contains(GameManager.instance.rightController.position))
			{
				SuitManager.instance.OnTakeDamage(damage, 9);
				SuitManager.instance.power = Mathf.Clamp(SuitManager.instance.power + 7f, 0f, SuitManager.instance.maxPower);
			}
			yield return new WaitForSeconds(1f);
		}
	}
}
