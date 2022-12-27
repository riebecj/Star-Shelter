using UnityEngine;

namespace VRTK
{
	public struct CollisionTrackerEventArgs
	{
		public bool isTrigger;

		public Collision collision;

		public Collider collider;
	}
}
