using System;
using UnityEngine;

namespace ch.sycoforge.Decal
{
	[Serializable]
	internal class TransformObserver
	{
		[HideInInspector]
		[SerializeField]
		private Vector3 lastPosition;

		[SerializeField]
		[HideInInspector]
		private Vector3 lastScale;

		[HideInInspector]
		[SerializeField]
		private Quaternion lastRotation;

		[SerializeField]
		[HideInInspector]
		private Transform transform;

		public TransformObserver(Transform transform)
		{
			this.transform = transform;
		}

		public OrientationChange CheckTransformChange()
		{
			OrientationChange orientationChange = OrientationChange.None;
			if (transform != null)
			{
				Vector3 position = transform.position;
				Quaternion rotation = transform.rotation;
				Vector3 localScale = transform.localScale;
				if (lastPosition != position)
				{
					orientationChange |= OrientationChange.Translation;
				}
				if (lastRotation != rotation)
				{
					orientationChange |= OrientationChange.Rotation;
				}
				if (lastScale != localScale)
				{
					orientationChange |= OrientationChange.Scale;
				}
				lastPosition = position;
				lastScale = localScale;
				lastRotation = rotation;
			}
			return orientationChange;
		}
	}
}
