using UnityEngine;

namespace VRTK
{
	[RequireComponent(typeof(VRTK_HeadsetCollision))]
	public class VRTK_PositionRewind : MonoBehaviour
	{
		[Header("Rewind Settings")]
		[Tooltip("The amount of time from original headset collision until the rewind to the last good known position takes place.")]
		public float rewindDelay = 0.5f;

		[Tooltip("The additional distance to push the play area back upon rewind to prevent being right next to the wall again.")]
		public float pushbackDistance = 0.5f;

		[Tooltip("The threshold to determine how low the headset has to be before it is considered the user is crouching. The last good position will only be recorded in a non-crouching position.")]
		public float crouchThreshold = 0.5f;

		[Tooltip("The threshold to determind how low the headset can be to perform a position rewind. If the headset Y position is lower than this threshold then a rewind won't occur.")]
		public float crouchRewindThreshold = 0.1f;

		[Header("Custom Settings")]
		[Tooltip("The VRTK Body Physics script to use for the collisions and rigidbodies. If this is left blank then the first Body Physics script found in the scene will be used.")]
		public VRTK_BodyPhysics bodyPhysics;

		[Tooltip("The VRTK Headset Collision script to use to determine if the headset is colliding. If this is left blank then the script will need to be applied to the same GameObject.")]
		public VRTK_HeadsetCollision headsetCollision;

		protected Transform headset;

		protected Transform playArea;

		protected Vector3 lastGoodStandingPosition;

		protected Vector3 lastGoodHeadsetPosition;

		protected float highestHeadsetY;

		protected float lastPlayAreaY;

		protected bool lastGoodPositionSet;

		protected bool hasCollided;

		protected bool isColliding;

		protected bool isRewinding;

		protected float collideTimer;

		protected virtual void OnEnable()
		{
			lastGoodPositionSet = false;
			headset = VRTK_DeviceFinder.HeadsetTransform();
			playArea = VRTK_DeviceFinder.PlayAreaTransform();
			if (playArea == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
			}
			bodyPhysics = ((!(bodyPhysics != null)) ? Object.FindObjectOfType<VRTK_BodyPhysics>() : bodyPhysics);
			headsetCollision = ((!(headsetCollision != null)) ? GetComponentInChildren<VRTK_HeadsetCollision>() : headsetCollision);
			ManageHeadsetListeners(true);
		}

		protected virtual void OnDisable()
		{
			ManageHeadsetListeners(false);
		}

		protected virtual void Update()
		{
			if (isColliding)
			{
				if (collideTimer > 0f)
				{
					collideTimer -= Time.deltaTime;
					return;
				}
				collideTimer = 0f;
				isColliding = false;
				RewindPosition();
			}
		}

		protected virtual void FixedUpdate()
		{
			if (!isColliding && playArea != null)
			{
				float num = 0.005f;
				if (playArea.position.y > lastPlayAreaY + num || playArea.position.y < lastPlayAreaY - num)
				{
					highestHeadsetY = crouchThreshold;
				}
				if (headset.localPosition.y > highestHeadsetY)
				{
					highestHeadsetY = headset.localPosition.y;
				}
				float num2 = highestHeadsetY - crouchThreshold;
				if (headset.localPosition.y > num2 && num2 > crouchThreshold)
				{
					lastGoodPositionSet = true;
					lastGoodStandingPosition = playArea.position;
					lastGoodHeadsetPosition = headset.position;
				}
				lastPlayAreaY = playArea.position.y;
			}
		}

		protected virtual void StartCollision()
		{
			isColliding = true;
			if (!hasCollided && collideTimer <= 0f)
			{
				hasCollided = true;
				collideTimer = rewindDelay;
			}
		}

		protected virtual void EndCollision()
		{
			isColliding = false;
			hasCollided = false;
			isRewinding = false;
		}

		protected virtual bool BodyCollisionsEnabled()
		{
			return bodyPhysics == null || bodyPhysics.enableBodyCollisions;
		}

		protected virtual void RewindPosition()
		{
			if (!isRewinding && ((playArea != null) & lastGoodPositionSet) && headset.localPosition.y > crouchRewindThreshold && BodyCollisionsEnabled())
			{
				isRewinding = true;
				Vector3 vector = lastGoodHeadsetPosition - headset.position;
				float num = Vector2.Distance(new Vector2(headset.position.x, headset.position.z), new Vector2(lastGoodHeadsetPosition.x, lastGoodHeadsetPosition.z));
				playArea.Translate(vector.normalized * (num + pushbackDistance));
				playArea.position = new Vector3(playArea.position.x, lastGoodStandingPosition.y, playArea.position.z);
				if (bodyPhysics != null)
				{
					bodyPhysics.ResetVelocities();
				}
			}
		}

		protected virtual void ManageHeadsetListeners(bool state)
		{
			if (headsetCollision != null)
			{
				if (state)
				{
					headsetCollision.HeadsetCollisionDetect += HeadsetCollision_HeadsetCollisionDetect;
					headsetCollision.HeadsetCollisionEnded += HeadsetCollision_HeadsetCollisionEnded;
				}
				else
				{
					headsetCollision.HeadsetCollisionDetect -= HeadsetCollision_HeadsetCollisionDetect;
					headsetCollision.HeadsetCollisionEnded -= HeadsetCollision_HeadsetCollisionEnded;
				}
			}
		}

		protected virtual void HeadsetCollision_HeadsetCollisionDetect(object sender, HeadsetCollisionEventArgs e)
		{
			StartCollision();
		}

		protected virtual void HeadsetCollision_HeadsetCollisionEnded(object sender, HeadsetCollisionEventArgs e)
		{
			EndCollision();
		}
	}
}
