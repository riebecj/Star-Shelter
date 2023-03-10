using UnityEngine;

namespace VRTK.Examples
{
	public class UseRotate : VRTK_InteractableObject
	{
		[Header("Rotation when in use")]
		[SerializeField]
		[Tooltip("Rotation speed when not in use (deg/sec)")]
		private float idleSpinSpeed;

		[SerializeField]
		[Tooltip("Rotation speed when in use (deg/sec)")]
		private float activeSpinSpeed = 360f;

		[Tooltip("Game object to rotate\n(leave empty to use this object)")]
		[SerializeField]
		private Transform rotatingObject;

		[SerializeField]
		private Vector3 rotationAxis = Vector3.up;

		private float spinSpeed;

		public override void StartUsing(GameObject usingObject)
		{
			base.StartUsing(usingObject);
			spinSpeed = activeSpinSpeed;
		}

		public override void StopUsing(GameObject usingObject)
		{
			base.StopUsing(usingObject);
			spinSpeed = idleSpinSpeed;
		}

		protected void Start()
		{
			if (rotatingObject == null)
			{
				rotatingObject = base.transform;
			}
			spinSpeed = idleSpinSpeed;
		}

		protected override void Update()
		{
			base.Update();
			rotatingObject.Rotate(rotationAxis, spinSpeed * Time.deltaTime);
		}
	}
}
