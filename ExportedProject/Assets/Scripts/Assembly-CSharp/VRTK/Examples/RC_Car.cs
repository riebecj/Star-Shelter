using UnityEngine;

namespace VRTK.Examples
{
	public class RC_Car : MonoBehaviour
	{
		public float maxAcceleration = 3f;

		public float jumpPower = 10f;

		private float acceleration = 0.05f;

		private float movementSpeed;

		private float rotationSpeed = 180f;

		private bool isJumping;

		private Vector2 touchAxis;

		private float triggerAxis;

		private Rigidbody rb;

		private Vector3 defaultPosition;

		private Quaternion defaultRotation;

		public void SetTouchAxis(Vector2 data)
		{
			touchAxis = data;
		}

		public void SetTriggerAxis(float data)
		{
			triggerAxis = data;
		}

		public void ResetCar()
		{
			base.transform.position = defaultPosition;
			base.transform.rotation = defaultRotation;
		}

		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
			defaultPosition = base.transform.position;
			defaultRotation = base.transform.rotation;
		}

		private void FixedUpdate()
		{
			if (isJumping)
			{
				touchAxis.x = 0f;
			}
			CalculateSpeed();
			Move();
			Turn();
			Jump();
		}

		private void CalculateSpeed()
		{
			if (touchAxis.y != 0f)
			{
				movementSpeed += acceleration * touchAxis.y;
				movementSpeed = Mathf.Clamp(movementSpeed, 0f - maxAcceleration, maxAcceleration);
			}
			else
			{
				Decelerate();
			}
		}

		private void Decelerate()
		{
			if (movementSpeed > 0f)
			{
				movementSpeed -= Mathf.Lerp(acceleration, maxAcceleration, 0f);
			}
			else if (movementSpeed < 0f)
			{
				movementSpeed += Mathf.Lerp(acceleration, 0f - maxAcceleration, 0f);
			}
			else
			{
				movementSpeed = 0f;
			}
		}

		private void Move()
		{
			Vector3 vector = base.transform.forward * movementSpeed * Time.deltaTime;
			rb.MovePosition(rb.position + vector);
		}

		private void Turn()
		{
			float y = touchAxis.x * rotationSpeed * Time.deltaTime;
			Quaternion quaternion = Quaternion.Euler(0f, y, 0f);
			rb.MoveRotation(rb.rotation * quaternion);
		}

		private void Jump()
		{
			if (!isJumping && triggerAxis > 0f)
			{
				float num = triggerAxis * jumpPower;
				rb.AddRelativeForce(Vector3.up * num);
				triggerAxis = 0f;
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			isJumping = false;
		}

		private void OnTriggerExit(Collider collider)
		{
			isJumping = true;
		}
	}
}
