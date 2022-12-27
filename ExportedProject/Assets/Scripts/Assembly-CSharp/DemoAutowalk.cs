using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DemoAutowalk : MonoBehaviour
{
	public float moveSpeed = 3f;

	public bool allowMovement = true;

	private CharacterController myCC;

	private bool moveForward;

	private void Start()
	{
		myCC = GetComponent<CharacterController>();
	}

	private void Update()
	{
		if (!allowMovement)
		{
			if (moveForward)
			{
				moveForward = false;
			}
		}
		else if (Input.GetButtonDown("Fire1"))
		{
			moveForward = !moveForward;
			if (!moveForward)
			{
				myCC.SimpleMove(Vector3.zero);
			}
		}
		if (moveForward)
		{
			Vector3 vector = Camera.main.transform.TransformDirection(Vector3.forward);
			myCC.SimpleMove(vector * moveSpeed);
		}
	}

	public void AllowMovement(bool status)
	{
		allowMovement = status;
		if (moveForward)
		{
			moveForward = false;
		}
	}
}
