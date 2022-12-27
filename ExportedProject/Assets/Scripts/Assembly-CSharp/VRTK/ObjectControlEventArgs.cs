using UnityEngine;

namespace VRTK
{
	public struct ObjectControlEventArgs
	{
		public GameObject controlledGameObject;

		public Transform directionDevice;

		public Vector3 axisDirection;

		public float axis;

		public float deadzone;

		public bool currentlyFalling;

		public bool modifierActive;
	}
}
