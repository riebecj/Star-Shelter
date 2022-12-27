using UnityEngine;

namespace VRTK
{
	public struct ControllerInteractionEventArgs
	{
		public uint controllerIndex;

		public float buttonPressure;

		public Vector2 touchpadAxis;

		public float touchpadAngle;
	}
}
