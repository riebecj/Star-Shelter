using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSFlyerMousePhysics : MonoBehaviour
{
	public float fwdForce = 6f;

	public float sideForce = 6f;

	private Rigidbody myRigidbody;

	private void Start()
	{
		myRigidbody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		myRigidbody.AddForce(base.transform.forward * fwdForce * Input.GetAxis("Vertical"));
		myRigidbody.AddForce(base.transform.right * sideForce * Input.GetAxis("Horizontal"));
	}
}
