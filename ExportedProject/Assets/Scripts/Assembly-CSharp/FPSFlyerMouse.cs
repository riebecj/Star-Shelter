using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSFlyerMouse : MonoBehaviour
{
	public float speed = 6f;

	private Vector3 moveDirection = Vector3.zero;

	private CharacterController controller;

	private CollisionFlags flags;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
	}

	private void FixedUpdate()
	{
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		moveDirection = base.transform.TransformDirection(moveDirection);
		moveDirection *= speed;
		flags = controller.Move(moveDirection * Time.deltaTime);
	}
}
