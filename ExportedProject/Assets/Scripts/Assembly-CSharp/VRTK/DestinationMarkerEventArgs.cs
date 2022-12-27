using UnityEngine;

namespace VRTK
{
	public struct DestinationMarkerEventArgs
	{
		public float distance;

		public Transform target;

		public RaycastHit raycastHit;

		public Vector3 destinationPosition;

		public Quaternion? destinationRotation;

		public bool forceDestinationPosition;

		public bool enableTeleport;

		public uint controllerIndex;
	}
}
