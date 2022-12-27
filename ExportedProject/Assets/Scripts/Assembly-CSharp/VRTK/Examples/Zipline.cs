using UnityEngine;

namespace VRTK.Examples
{
	public class Zipline : VRTK_InteractableObject
	{
		[Header("Zipline Options", order = 4)]
		public float downStartSpeed = 0.2f;

		public float acceleration = 1f;

		public float upSpeed = 1f;

		public Transform handleEndPosition;

		public Transform handleStartPosition;

		public GameObject handle;

		private bool isMoving;

		private bool isMovingDown = true;

		private float currentSpeed;

		public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
		{
			base.OnInteractableObjectGrabbed(e);
			isMoving = true;
		}

		protected override void Awake()
		{
			base.Awake();
			currentSpeed = downStartSpeed;
		}

		protected override void Update()
		{
			base.Update();
			if (isMoving)
			{
				Vector3 vector;
				if (isMovingDown)
				{
					currentSpeed += acceleration * Time.deltaTime;
					vector = Vector3.down * currentSpeed * Time.deltaTime;
				}
				else
				{
					vector = Vector3.up * upSpeed * Time.deltaTime;
				}
				handle.transform.localPosition += vector;
				if ((isMovingDown && handle.transform.localPosition.y <= handleEndPosition.localPosition.y) || (!isMovingDown && handle.transform.localPosition.y >= handleStartPosition.localPosition.y))
				{
					isMoving = false;
					isMovingDown = !isMovingDown;
					currentSpeed = downStartSpeed;
				}
			}
		}
	}
}
