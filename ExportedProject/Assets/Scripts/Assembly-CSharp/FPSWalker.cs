using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSWalker : MonoBehaviour
{
	public float speed = 6f;

	public float jumpSpeed = 8f;

	public float gravity = 20f;

	private CharacterController controller;

	private Transform myTransform;

	private Vector3 moveDirection = Vector3.zero;

	private CollisionFlags flags;

	private bool grounded;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
		myTransform = base.transform;
	}

	private void FixedUpdate()
	{
		if (grounded)
		{
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
			moveDirection = myTransform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetButton("Jump"))
			{
				moveDirection.y = jumpSpeed;
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		flags = controller.Move(moveDirection * Time.deltaTime);
		grounded = (flags & CollisionFlags.Below) != 0;
	}
}
