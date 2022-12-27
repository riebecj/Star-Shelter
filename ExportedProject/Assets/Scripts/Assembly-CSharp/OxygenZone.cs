using UnityEngine;

public class OxygenZone : MonoBehaviour
{
	private Transform player;

	public Collider bounds;

	public bool enableOxygen;

	internal bool inZone;

	private void Start()
	{
		player = GameManager.instance.Head;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.root.tag == "Player" && !GameManager.instance.dead)
		{
			if (enableOxygen)
			{
				inZone = true;
				SuitManager.instance.inOxygenZone = true;
			}
			else
			{
				inZone = false;
				SuitManager.instance.inOxygenZone = false;
			}
		}
	}
}
