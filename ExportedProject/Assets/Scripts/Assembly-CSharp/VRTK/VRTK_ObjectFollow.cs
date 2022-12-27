using UnityEngine;

namespace VRTK
{
	public abstract class VRTK_ObjectFollow : MonoBehaviour
	{
		[Tooltip("The game object to follow. The followed property values will be taken from this one.")]
		public GameObject gameObjectToFollow;

		[Tooltip("The game object to change the property values of. If left empty the game object this script is attached to will be changed.")]
		public GameObject gameObjectToChange;

		[Tooltip("Whether to follow the position of the given game object.")]
		public bool followsPosition = true;

		[Tooltip("Whether to smooth the position when following `gameObjectToFollow`.")]
		public bool smoothsPosition;

		[Tooltip("The maximum allowed distance between the unsmoothed source and the smoothed target per frame to use for smoothing.")]
		public float maxAllowedPerFrameDistanceDifference = 0.003f;

		[Tooltip("Whether to follow the rotation of the given game object.")]
		public bool followsRotation = true;

		[Tooltip("Whether to smooth the rotation when following `gameObjectToFollow`.")]
		public bool smoothsRotation;

		[Tooltip("The maximum allowed angle between the unsmoothed source and the smoothed target per frame to use for smoothing.")]
		public float maxAllowedPerFrameAngleDifference = 1.5f;

		[Tooltip("Whether to follow the scale of the given game object.")]
		public bool followsScale = true;

		[Tooltip("Whether to smooth the scale when following `gameObjectToFollow`.")]
		public bool smoothsScale;

		[Tooltip("The maximum allowed size between the unsmoothed source and the smoothed target per frame to use for smoothing.")]
		public float maxAllowedPerFrameSizeDifference = 0.003f;

		public Vector3 targetPosition { get; private set; }

		public Quaternion targetRotation { get; private set; }

		public Vector3 targetScale { get; private set; }

		public virtual void Follow()
		{
			if (!(gameObjectToFollow == null))
			{
				if (followsPosition)
				{
					FollowPosition();
				}
				if (followsRotation)
				{
					FollowRotation();
				}
				if (followsScale)
				{
					FollowScale();
				}
			}
		}

		protected virtual void OnEnable()
		{
			gameObjectToChange = ((!(gameObjectToChange != null)) ? base.gameObject : gameObjectToChange);
		}

		protected virtual void OnValidate()
		{
			maxAllowedPerFrameDistanceDifference = Mathf.Max(0.0001f, maxAllowedPerFrameDistanceDifference);
			maxAllowedPerFrameAngleDifference = Mathf.Max(0.0001f, maxAllowedPerFrameAngleDifference);
			maxAllowedPerFrameSizeDifference = Mathf.Max(0.0001f, maxAllowedPerFrameSizeDifference);
		}

		protected abstract Vector3 GetPositionToFollow();

		protected abstract void SetPositionOnGameObject(Vector3 newPosition);

		protected abstract Quaternion GetRotationToFollow();

		protected abstract void SetRotationOnGameObject(Quaternion newRotation);

		protected virtual Vector3 GetScaleToFollow()
		{
			return gameObjectToFollow.transform.localScale;
		}

		protected virtual void SetScaleOnGameObject(Vector3 newScale)
		{
			gameObjectToChange.transform.localScale = newScale;
		}

		protected virtual void FollowPosition()
		{
			Vector3 positionToFollow = GetPositionToFollow();
			Vector3 positionOnGameObject;
			if (smoothsPosition)
			{
				float t = Mathf.Clamp01(Vector3.Distance(targetPosition, positionToFollow) / maxAllowedPerFrameDistanceDifference);
				positionOnGameObject = Vector3.Lerp(targetPosition, positionToFollow, t);
			}
			else
			{
				positionOnGameObject = positionToFollow;
			}
			targetPosition = positionOnGameObject;
			SetPositionOnGameObject(positionOnGameObject);
		}

		protected virtual void FollowRotation()
		{
			Quaternion rotationToFollow = GetRotationToFollow();
			Quaternion rotationOnGameObject;
			if (smoothsRotation)
			{
				float t = Mathf.Clamp01(Quaternion.Angle(targetRotation, rotationToFollow) / maxAllowedPerFrameAngleDifference);
				rotationOnGameObject = Quaternion.Lerp(targetRotation, rotationToFollow, t);
			}
			else
			{
				rotationOnGameObject = rotationToFollow;
			}
			targetRotation = rotationOnGameObject;
			SetRotationOnGameObject(rotationOnGameObject);
		}

		protected virtual void FollowScale()
		{
			Vector3 scaleToFollow = GetScaleToFollow();
			Vector3 scaleOnGameObject;
			if (smoothsScale)
			{
				float t = Mathf.Clamp01(Vector3.Distance(targetScale, scaleToFollow) / maxAllowedPerFrameSizeDifference);
				scaleOnGameObject = Vector3.Lerp(targetScale, scaleToFollow, t);
			}
			else
			{
				scaleOnGameObject = scaleToFollow;
			}
			targetScale = scaleOnGameObject;
			SetScaleOnGameObject(scaleOnGameObject);
		}
	}
}
