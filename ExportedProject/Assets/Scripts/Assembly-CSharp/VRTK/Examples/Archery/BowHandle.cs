using UnityEngine;

namespace VRTK.Examples.Archery
{
	public class BowHandle : MonoBehaviour
	{
		public Transform arrowNockingPoint;

		public BowAim aim;

		[HideInInspector]
		public Transform nockSide;
	}
}
