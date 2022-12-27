using System.Collections;
using UnityEngine;

namespace VRTK
{
	public class VRTK_DashTeleport : VRTK_HeightAdjustTeleport
	{
		[Header("Dash Settings")]
		[Tooltip("The fixed time it takes to dash to a new position.")]
		public float normalLerpTime = 0.1f;

		[Tooltip("The minimum speed for dashing in meters per second.")]
		public float minSpeedMps = 50f;

		[Tooltip("The Offset of the CapsuleCast above the camera.")]
		public float capsuleTopOffset = 0.2f;

		[Tooltip("The Offset of the CapsuleCast below the camera.")]
		public float capsuleBottomOffset = 0.5f;

		[Tooltip("The radius of the CapsuleCast.")]
		public float capsuleRadius = 0.5f;

		protected float minDistanceForNormalLerp;

		protected float lerpTime = 0.1f;

		public event DashTeleportEventHandler WillDashThruObjects;

		public event DashTeleportEventHandler DashedThruObjects;

		public virtual void OnWillDashThruObjects(DashTeleportEventArgs e)
		{
			if (this.WillDashThruObjects != null)
			{
				this.WillDashThruObjects(this, e);
			}
		}

		public virtual void OnDashedThruObjects(DashTeleportEventArgs e)
		{
			if (this.DashedThruObjects != null)
			{
				this.DashedThruObjects(this, e);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			minDistanceForNormalLerp = minSpeedMps * normalLerpTime;
		}

		protected override void SetNewPosition(Vector3 position, Transform target, bool forceDestinationPosition)
		{
			Vector3 targetPosition = CheckTerrainCollision(position, target, forceDestinationPosition);
			StartCoroutine(lerpToPosition(targetPosition, target));
		}

		protected virtual IEnumerator lerpToPosition(Vector3 targetPosition, Transform target)
		{
			enableTeleport = false;
			bool gameObjectInTheWay2 = false;
			Vector3 eyeCameraPosition = headset.transform.position;
			Vector3 eyeCameraPositionOnGround = new Vector3(eyeCameraPosition.x, playArea.position.y, eyeCameraPosition.z);
			Vector3 eyeCameraRelativeToRig = eyeCameraPosition - playArea.position;
			Vector3 targetEyeCameraPosition = targetPosition + eyeCameraRelativeToRig;
			Vector3 direction = (targetEyeCameraPosition - eyeCameraPosition).normalized;
			Vector3 bottomPoint = eyeCameraPositionOnGround + Vector3.up * capsuleBottomOffset + direction;
			Vector3 topPoint = eyeCameraPosition + Vector3.up * capsuleTopOffset + direction;
			float maxDistance = Vector3.Distance(playArea.position, targetPosition - direction * 0.5f);
			RaycastHit[] allHits = Physics.CapsuleCastAll(bottomPoint, topPoint, capsuleRadius, direction, maxDistance);
			RaycastHit[] array = allHits;
			foreach (RaycastHit raycastHit in array)
			{
				gameObjectInTheWay2 = ((raycastHit.collider.gameObject != target.gameObject) ? true : false);
			}
			if (gameObjectInTheWay2)
			{
				OnWillDashThruObjects(SetDashTeleportEvent(allHits));
			}
			if (maxDistance >= minDistanceForNormalLerp)
			{
				lerpTime = normalLerpTime;
			}
			else
			{
				lerpTime = 1f / minSpeedMps * maxDistance;
			}
			Vector3 startPosition = new Vector3(playArea.position.x, playArea.position.y, playArea.position.z);
			float elapsedTime = 0f;
			float t = 0f;
			while (t < 1f)
			{
				playArea.position = Vector3.Lerp(startPosition, targetPosition, t);
				elapsedTime += Time.deltaTime;
				t = elapsedTime / lerpTime;
				if (t > 1f)
				{
					if (playArea.position != targetPosition)
					{
						playArea.position = targetPosition;
					}
					t = 1f;
				}
				yield return new WaitForEndOfFrame();
			}
			if (gameObjectInTheWay2)
			{
				OnDashedThruObjects(SetDashTeleportEvent(allHits));
			}
			gameObjectInTheWay2 = false;
			enableTeleport = true;
		}

		protected virtual DashTeleportEventArgs SetDashTeleportEvent(RaycastHit[] hits)
		{
			DashTeleportEventArgs result = default(DashTeleportEventArgs);
			result.hits = hits;
			return result;
		}
	}
}
