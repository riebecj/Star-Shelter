using UnityEngine;

namespace VRTK
{
	public class VRTK_RigidbodyFollow : VRTK_ObjectFollow
	{
		public enum MovementOption
		{
			Set = 0,
			Move = 1,
			Add = 2
		}

		[Tooltip("Specifies how to position and rotate the rigidbody.")]
		public MovementOption movementOption;

		protected Rigidbody rigidbodyToFollow;

		protected Rigidbody rigidbodyToChange;

		public override void Follow()
		{
			CacheRigidbodies();
			base.Follow();
		}

		protected virtual void OnDisable()
		{
			rigidbodyToFollow = null;
			rigidbodyToChange = null;
		}

		protected virtual void FixedUpdate()
		{
			Follow();
		}

		protected override Vector3 GetPositionToFollow()
		{
			return rigidbodyToFollow.position;
		}

		protected override void SetPositionOnGameObject(Vector3 newPosition)
		{
			switch (movementOption)
			{
			case MovementOption.Set:
				rigidbodyToChange.position = newPosition;
				break;
			case MovementOption.Move:
				rigidbodyToChange.MovePosition(newPosition);
				break;
			case MovementOption.Add:
				rigidbodyToChange.AddForce(newPosition - rigidbodyToChange.position);
				break;
			}
		}

		protected override Quaternion GetRotationToFollow()
		{
			return rigidbodyToFollow.rotation;
		}

		protected override void SetRotationOnGameObject(Quaternion newRotation)
		{
			switch (movementOption)
			{
			case MovementOption.Set:
				rigidbodyToChange.rotation = newRotation;
				break;
			case MovementOption.Move:
				rigidbodyToChange.MoveRotation(newRotation);
				break;
			case MovementOption.Add:
				rigidbodyToChange.AddTorque(newRotation * Quaternion.Inverse(rigidbodyToChange.rotation).eulerAngles);
				break;
			}
		}

		protected override Vector3 GetScaleToFollow()
		{
			return rigidbodyToFollow.transform.localScale;
		}

		protected virtual void CacheRigidbodies()
		{
			if (!(gameObjectToFollow == null) && !(gameObjectToChange == null) && (!(rigidbodyToFollow != null) || !(rigidbodyToChange != null)))
			{
				rigidbodyToFollow = gameObjectToFollow.GetComponent<Rigidbody>();
				rigidbodyToChange = gameObjectToChange.GetComponent<Rigidbody>();
			}
		}
	}
}
