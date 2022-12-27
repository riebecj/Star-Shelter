using UnityEngine;

public class GunHolster : MonoBehaviour
{
	public static GunHolster instance;

	public Vector3 Offset;

	public Transform head;

	internal Collider collider;

	internal bool hasGun;

	private float Vibration = 15000000f;

	public Rigidbody gunPos;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		collider = GetComponent<Collider>();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(base.transform.position, 0.25f);
	}
}
