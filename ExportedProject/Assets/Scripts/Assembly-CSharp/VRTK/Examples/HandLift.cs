using UnityEngine;

namespace VRTK.Examples
{
	public class HandLift : VRTK_InteractableObject
	{
		[Header("Hand Lift Options", order = 4)]
		public float speed = 0.1f;

		public Transform handleTop;

		public Transform ropeTop;

		public Transform ropeBottom;

		public GameObject rope;

		public GameObject handle;

		private bool isMoving;

		private bool isMovingUp = true;

		public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
		{
			base.OnInteractableObjectGrabbed(e);
			isMoving = true;
		}

		protected override void Update()
		{
			base.Update();
			if (isMoving)
			{
				Vector3 vector = ((!isMovingUp) ? Vector3.down : Vector3.up) * speed * Time.deltaTime;
				handle.transform.position += vector;
				Vector3 localScale = rope.transform.localScale;
				localScale.y = (ropeTop.position.y - handle.transform.position.y) / 2f;
				Vector3 position = ropeTop.transform.position;
				position.y -= localScale.y;
				rope.transform.localScale = localScale;
				rope.transform.position = position;
				if ((!isMovingUp && handle.transform.position.y <= ropeBottom.position.y) || (isMovingUp && handle.transform.position.y >= handleTop.position.y))
				{
					isMoving = false;
					isMovingUp = !isMovingUp;
				}
			}
		}
	}
}
