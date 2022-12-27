using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace VRTK
{
	public class VRTK_BasicTeleport : MonoBehaviour
	{
		[Header("Base Settings")]
		[Tooltip("The colour to fade to when blinking on teleport.")]
		public Color blinkToColor = Color.black;

		[Tooltip("The fade blink speed can be changed on the basic teleport script to provide a customised teleport experience. Setting the speed to 0 will mean no fade blink effect is present.")]
		public float blinkTransitionSpeed = 0.6f;

		[Tooltip("A range between 0 and 32 that determines how long the blink transition will stay blacked out depending on the distance being teleported. A value of 0 will not delay the teleport blink effect over any distance, a value of 32 will delay the teleport blink fade in even when the distance teleported is very close to the original position. This can be used to simulate time taking longer to pass the further a user teleports. A value of 16 provides a decent basis to simulate this to the user.")]
		[Range(0f, 32f)]
		public float distanceBlinkDelay;

		[Tooltip("If this is checked then the teleported location will be the position of the headset within the play area. If it is unchecked then the teleported location will always be the centre of the play area even if the headset position is not in the centre of the play area.")]
		public bool headsetPositionCompensation = true;

		[Tooltip("A specified VRTK_PolicyList to use to determine whether destination targets will be acted upon by the Teleporter.")]
		public VRTK_PolicyList targetListPolicy;

		[Tooltip("The max distance the teleport destination can be outside the nav mesh to be considered valid. If a value of `0` is given then the nav mesh restrictions will be ignored.")]
		public float navMeshLimitDistance;

		protected Transform headset;

		protected Transform playArea;

		protected bool adjustYForTerrain;

		protected bool enableTeleport = true;

		protected float blinkPause;

		protected float fadeInTime;

		protected float maxBlinkTransitionSpeed = 1.5f;

		protected float maxBlinkDistance = 33f;

		protected Coroutine initaliseListeners;

		public event TeleportEventHandler Teleporting;

		public event TeleportEventHandler Teleported;

		public virtual void InitDestinationSetListener(GameObject markerMaker, bool register)
		{
			if (!markerMaker)
			{
				return;
			}
			VRTK_DestinationMarker[] componentsInChildren = markerMaker.GetComponentsInChildren<VRTK_DestinationMarker>();
			foreach (VRTK_DestinationMarker vRTK_DestinationMarker in componentsInChildren)
			{
				if (register)
				{
					vRTK_DestinationMarker.DestinationMarkerSet += DoTeleport;
					vRTK_DestinationMarker.SetInvalidTarget(targetListPolicy);
					vRTK_DestinationMarker.SetNavMeshCheckDistance(navMeshLimitDistance);
					vRTK_DestinationMarker.SetHeadsetPositionCompensation(headsetPositionCompensation);
				}
				else
				{
					vRTK_DestinationMarker.DestinationMarkerSet -= DoTeleport;
				}
			}
		}

		public virtual void ToggleTeleportEnabled(bool state)
		{
			enableTeleport = state;
		}

		public virtual bool ValidLocation(Transform target, Vector3 destinationPosition)
		{
			if (VRTK_PlayerObject.IsPlayerObject(target.gameObject) || (bool)target.GetComponent<VRTK_UIGraphicRaycaster>())
			{
				return false;
			}
			bool flag = false;
			if ((bool)target)
			{
				NavMeshHit hit;
				flag = NavMesh.SamplePosition(destinationPosition, out hit, navMeshLimitDistance, -1);
			}
			if (navMeshLimitDistance == 0f)
			{
				flag = true;
			}
			return flag && (bool)target && !VRTK_PolicyList.Check(target.gameObject, targetListPolicy);
		}

		protected virtual void OnEnable()
		{
			VRTK_PlayerObject.SetPlayerObject(base.gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
			headset = VRTK_SharedMethods.AddCameraFade();
			playArea = VRTK_DeviceFinder.PlayAreaTransform();
			adjustYForTerrain = false;
			enableTeleport = true;
			initaliseListeners = StartCoroutine(InitListenersAtEndOfFrame());
			VRTK_ObjectCache.registeredTeleporters.Add(this);
		}

		protected virtual void OnDisable()
		{
			if (initaliseListeners != null)
			{
				StopCoroutine(initaliseListeners);
			}
			InitDestinationMarkerListeners(false);
			VRTK_ObjectCache.registeredTeleporters.Remove(this);
		}

		protected virtual void Blink(float transitionSpeed)
		{
			fadeInTime = transitionSpeed;
			if (transitionSpeed > 0f)
			{
				VRTK_SDK_Bridge.HeadsetFade(blinkToColor, 0f);
			}
			Invoke("ReleaseBlink", blinkPause);
		}

		protected virtual void DoTeleport(object sender, DestinationMarkerEventArgs e)
		{
			if (enableTeleport && ValidLocation(e.target, e.destinationPosition) && e.enableTeleport)
			{
				OnTeleporting(sender, e);
				Vector3 newPosition = GetNewPosition(e.destinationPosition, e.target, e.forceDestinationPosition);
				CalculateBlinkDelay(blinkTransitionSpeed, newPosition);
				Blink(blinkTransitionSpeed);
				SetNewPosition(newPosition, e.target, e.forceDestinationPosition);
				SetNewRotation(e.destinationRotation);
				OnTeleported(sender, e);
			}
		}

		protected virtual void SetNewPosition(Vector3 position, Transform target, bool forceDestinationPosition)
		{
			playArea.position = CheckTerrainCollision(position, target, forceDestinationPosition);
		}

		protected virtual void SetNewRotation(Quaternion? rotation)
		{
			if (rotation.HasValue)
			{
				playArea.rotation = rotation.Value;
			}
		}

		protected virtual Vector3 GetNewPosition(Vector3 tipPosition, Transform target, bool returnOriginalPosition)
		{
			if (returnOriginalPosition)
			{
				return tipPosition;
			}
			float x = ((!headsetPositionCompensation) ? tipPosition.x : (tipPosition.x - (headset.position.x - playArea.position.x)));
			float y = playArea.position.y;
			float z = ((!headsetPositionCompensation) ? tipPosition.z : (tipPosition.z - (headset.position.z - playArea.position.z)));
			return new Vector3(x, y, z);
		}

		protected virtual Vector3 CheckTerrainCollision(Vector3 position, Transform target, bool useHeadsetForPosition)
		{
			Terrain component = target.GetComponent<Terrain>();
			if (adjustYForTerrain && component != null)
			{
				Vector3 worldPosition = ((!useHeadsetForPosition) ? position : new Vector3(headset.position.x, position.y, headset.position.z));
				float num = component.SampleHeight(worldPosition);
				position.y = ((!(num > position.y)) ? (component.GetPosition().y + num) : position.y);
			}
			return position;
		}

		protected virtual void OnTeleporting(object sender, DestinationMarkerEventArgs e)
		{
			if (this.Teleporting != null)
			{
				this.Teleporting(this, e);
			}
		}

		protected virtual void OnTeleported(object sender, DestinationMarkerEventArgs e)
		{
			if (this.Teleported != null)
			{
				this.Teleported(this, e);
			}
		}

		protected virtual void CalculateBlinkDelay(float blinkSpeed, Vector3 newPosition)
		{
			blinkPause = 0f;
			if (distanceBlinkDelay > 0f)
			{
				float num = 0.5f;
				float num2 = Vector3.Distance(playArea.position, newPosition);
				blinkPause = Mathf.Clamp(num2 * blinkTransitionSpeed / (maxBlinkDistance - distanceBlinkDelay), num, maxBlinkTransitionSpeed);
				blinkPause = ((!((double)blinkSpeed <= 0.25)) ? blinkPause : num);
			}
		}

		protected virtual void ReleaseBlink()
		{
			VRTK_SDK_Bridge.HeadsetFade(Color.clear, fadeInTime);
			fadeInTime = 0f;
		}

		protected virtual IEnumerator InitListenersAtEndOfFrame()
		{
			yield return new WaitForEndOfFrame();
			if (base.enabled)
			{
				InitDestinationMarkerListeners(true);
			}
		}

		protected virtual void InitDestinationMarkerListeners(bool state)
		{
			GameObject controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
			GameObject controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();
			InitDestinationSetListener(controllerLeftHand, state);
			InitDestinationSetListener(controllerRightHand, state);
			foreach (VRTK_DestinationMarker registeredDestinationMarker in VRTK_ObjectCache.registeredDestinationMarkers)
			{
				if (registeredDestinationMarker.gameObject != controllerLeftHand && registeredDestinationMarker.gameObject != controllerRightHand)
				{
					InitDestinationSetListener(registeredDestinationMarker.gameObject, state);
				}
			}
		}
	}
}
