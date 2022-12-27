using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	public class VRTK_BodyPhysics : VRTK_DestinationMarker
	{
		public enum FallingRestrictors
		{
			NoRestriction = 0,
			LeftController = 1,
			RightController = 2,
			EitherController = 3,
			BothControllers = 4
		}

		[Header("Body Collision Settings")]
		[Tooltip("If checked then the body collider and rigidbody will be used to check for rigidbody collisions.")]
		public bool enableBodyCollisions = true;

		[Tooltip("If this is checked then any items that are grabbed with the controller will not collide with the body collider. This is very useful if the user is required to grab and wield objects because if the collider was active they would bounce off the collider.")]
		public bool ignoreGrabbedCollisions = true;

		[Tooltip("The collider which is created for the user is set at a height from the user's headset position. If the collider is required to be lower to allow for room between the play area collider and the headset then this offset value will shorten the height of the generated collider.")]
		public float headsetYOffset = 0.2f;

		[Tooltip("The amount of movement of the headset between the headset's current position and the current standing position to determine if the user is walking in play space and to ignore the body physics collisions if the movement delta is above this threshold.")]
		public float movementThreshold = 0.0015f;

		[Tooltip("The amount of movement of the play area between the play area's current position and the previous position to determine if the user is moving play space.")]
		public float playAreaMovementThreshold = 0.00075f;

		[Tooltip("The maximum number of samples to collect of headset position before determining if the current standing position within the play space has changed.")]
		public int standingHistorySamples = 5;

		[Tooltip("The `y` distance between the headset and the object being leaned over, if object being leaned over is taller than this threshold then the current standing position won't be updated.")]
		public float leanYThreshold = 0.5f;

		[Header("Step Settings")]
		[Tooltip("The maximum height to consider when checking if an object can be stepped upon to.")]
		public float stepUpYOffset = 0.15f;

		[Tooltip("The width/depth of the foot collider in relation to the radius of the body collider.")]
		[Range(0.1f, 0.9f)]
		public float stepThicknessMultiplier = 0.5f;

		[Tooltip("The distance between the current play area Y position and the new stepped up Y position to consider a valid step up. A higher number can help with juddering on slopes or small increases in collider heights.")]
		public float stepDropThreshold = 0.08f;

		[Header("Snap To Floor Settings")]
		[Tooltip("A custom raycaster to use when raycasting to find floors.")]
		public VRTK_CustomRaycast customRaycast;

		[Tooltip("**OBSOLETE [Use customRaycast]** The layers to ignore when raycasting to find floors.")]
		[Obsolete("`VRTK_BodyPhysics.layersToIgnore` is no longer used in the `VRTK_BodyPhysics` class, use the `customRaycast` parameter instead. This parameter will be removed in a future version of VRTK.")]
		public LayerMask layersToIgnore = 4;

		[Tooltip("A check to see if the drop to nearest floor should take place. If the selected restrictor is still over the current floor then the drop to nearest floor will not occur. Works well for being able to lean over ledges and look down. Only works for falling down not teleporting up.")]
		public FallingRestrictors fallRestriction;

		[Tooltip("When the `y` distance between the floor and the headset exceeds this distance and `Enable Body Collisions` is true then the rigidbody gravity will be used instead of teleport to drop to nearest floor.")]
		public float gravityFallYThreshold = 1f;

		[Tooltip("The `y` distance between the floor and the headset that must change before a fade transition is initiated. If the new user location is at a higher distance than the threshold then the headset blink transition will activate on teleport. If the new user location is within the threshold then no blink transition will happen, which is useful for walking up slopes, meshes and terrains to prevent constant blinking.")]
		public float blinkYThreshold = 0.15f;

		[Tooltip("The amount the `y` position needs to change by between the current floor `y` position and the previous floor `y` position before a change in floor height is considered to have occurred. A higher value here will mean that a `Drop To Floor` will be less likely to happen if the `y` of the floor beneath the user hasn't changed as much as the given threshold.")]
		public float floorHeightTolerance = 0.001f;

		[Range(1f, 10f)]
		[Tooltip("The amount of rounding on the play area Y position to be applied when checking if falling is occuring.")]
		public int fallCheckPrecision = 5;

		protected Transform playArea;

		protected Transform headset;

		protected Rigidbody bodyRigidbody;

		protected GameObject bodyColliderContainer;

		protected CapsuleCollider bodyCollider;

		protected CapsuleCollider footCollider;

		protected VRTK_CollisionTracker collisionTracker;

		protected bool currentBodyCollisionsSetting;

		protected GameObject currentCollidingObject;

		protected GameObject currentValidFloorObject;

		protected VRTK_BasicTeleport teleporter;

		protected float lastFrameFloorY;

		protected float hitFloorYDelta;

		protected bool initialFloorDrop;

		protected bool resetPhysicsAfterTeleport;

		protected bool storedCurrentPhysics;

		protected bool retogglePhysicsOnCanFall;

		protected bool storedRetogglePhysics;

		protected Vector3 lastPlayAreaPosition = Vector3.zero;

		protected Vector2 currentStandingPosition;

		protected List<Vector2> standingPositionHistory = new List<Vector2>();

		protected float playAreaHeightAdjustment = 0.009f;

		protected float bodyMass = 100f;

		protected float bodyRadius = 0.15f;

		internal float motionTimer;

		protected float leanForwardLengthAddition = 0.05f;

		protected float playAreaPositionThreshold = 0.002f;

		protected float gravityPush = -0.001f;

		protected int decimalPrecision = 3;

		protected bool isFalling;

		protected bool isMoving;

		protected bool isLeaning;

		protected bool onGround = true;

		protected bool inMotion = true;

		protected bool preventSnapToFloor;

		protected bool generateCollider;

		protected bool generateRigidbody;

		protected Vector3 playAreaVelocity = Vector3.zero;

		protected string footColliderContainerNameCheck;

		protected const string BODY_COLLIDER_CONTAINER_NAME = "BodyColliderContainer";

		protected const string FOOT_COLLIDER_CONTAINER_NAME = "FootColliderContainer";

		public static VRTK_BodyPhysics instance;

		protected bool drawDebugGizmo;

		public event BodyPhysicsEventHandler StartFalling;

		public event BodyPhysicsEventHandler StopFalling;

		public event BodyPhysicsEventHandler StartMoving;

		public event BodyPhysicsEventHandler StopMoving;

		public event BodyPhysicsEventHandler StartColliding;

		public event BodyPhysicsEventHandler StopColliding;

		public virtual bool ArePhysicsEnabled()
		{
			return bodyRigidbody != null && !bodyRigidbody.isKinematic;
		}

		public virtual void ApplyBodyVelocity(Vector3 velocity, bool forcePhysicsOn = false, bool applyMomentum = false)
		{
			if (enableBodyCollisions && forcePhysicsOn)
			{
				TogglePhysics(true);
			}
			if (ArePhysicsEnabled())
			{
				bodyRigidbody.velocity = velocity;
				StartFall(currentValidFloorObject);
			}
		}

		public virtual void ToggleOnGround(bool state)
		{
			onGround = state;
		}

		public virtual void TogglePreventSnapToFloor(bool state)
		{
			preventSnapToFloor = state;
		}

		public virtual bool IsFalling()
		{
			return isFalling;
		}

		public virtual bool IsMoving()
		{
			return isMoving;
		}

		public virtual bool IsLeaning()
		{
			return isLeaning;
		}

		public virtual bool OnGround()
		{
			return onGround;
		}

		public virtual Vector3 GetVelocity()
		{
			return (!(bodyRigidbody != null)) ? Vector3.zero : bodyRigidbody.velocity;
		}

		public virtual Vector3 GetAngularVelocity()
		{
			return (!(bodyRigidbody != null)) ? Vector3.zero : bodyRigidbody.angularVelocity;
		}

		public virtual void ResetVelocities()
		{
			bodyRigidbody.velocity = Vector3.zero;
			bodyRigidbody.angularVelocity = Vector3.zero;
		}

		protected override void OnEnable()
		{
			instance = this;
			base.OnEnable();
			SetupPlayArea();
			SetupHeadset();
			footColliderContainerNameCheck = VRTK_SharedMethods.GenerateVRTKObjectName(true, "FootColliderContainer");
			EnableDropToFloor();
			EnableBodyPhysics();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			DisableDropToFloor();
			DisableBodyPhysics();
			ManageCollisionListeners(false);
		}

		protected virtual void FixedUpdate()
		{
			CheckBodyCollisionsSetting();
			CalculateVelocity();
			UpdateCollider();
			lastPlayAreaPosition = ((!(playArea != null)) ? Vector3.zero : playArea.position);
		}

		protected virtual void OnCollisionEnter(Collision collision)
		{
			VRTK_PlayerClimb.instance.movingMode = false;
			if (collision.relativeVelocity.magnitude > 7.5f && motionTimer > 1f)
			{
				Debug.Log("collision.relativeVelocity.magnitude " + collision.relativeVelocity.magnitude);
				SuitManager.instance.OnBodyImpact(Mathf.Clamp((int)collision.relativeVelocity.magnitude * 2, 5, 15));
				VRTK_ScreenFade.Start(Color.black, 0.2f);
				SpaceMask.instance.Invoke("FadeBack", 0.3f);
			}
			if (!VRTK_PlayerObject.IsPlayerObject(collision.gameObject) && currentValidFloorObject != null && !currentValidFloorObject.Equals(collision.gameObject))
			{
				CheckStepUpCollision(collision);
				currentCollidingObject = collision.gameObject;
				OnStartColliding(SetBodyPhysicsEvent(currentCollidingObject));
			}
		}

		protected virtual void OnTriggerEnter(Collider collider)
		{
			if (!VRTK_PlayerObject.IsPlayerObject(collider.gameObject) && currentValidFloorObject != null && !currentValidFloorObject.Equals(collider.gameObject))
			{
				currentCollidingObject = collider.gameObject;
				OnStartColliding(SetBodyPhysicsEvent(currentCollidingObject));
			}
		}

		protected virtual void OnCollisionExit(Collision collision)
		{
			if (currentCollidingObject != null && currentCollidingObject.Equals(collision.gameObject))
			{
				OnStopColliding(SetBodyPhysicsEvent(currentCollidingObject));
				currentCollidingObject = null;
			}
		}

		protected virtual void OnTriggerExit(Collider collider)
		{
			if (currentCollidingObject != null && currentCollidingObject.Equals(collider.gameObject))
			{
				OnStopColliding(SetBodyPhysicsEvent(currentCollidingObject));
				currentCollidingObject = null;
			}
		}

		protected virtual void OnDrawGizmos()
		{
			if (drawDebugGizmo && headset != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(new Vector3(headset.position.x, headset.position.y - 0.3f, headset.position.z), 0.075f);
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(new Vector3(currentStandingPosition.x, headset.position.y - 0.3f, currentStandingPosition.y), 0.05f);
			}
		}

		protected virtual void SetupPlayArea()
		{
			playArea = VRTK_DeviceFinder.PlayAreaTransform();
			if (playArea != null)
			{
				lastPlayAreaPosition = playArea.position;
				collisionTracker = playArea.GetComponent<VRTK_CollisionTracker>();
				if (collisionTracker == null)
				{
					collisionTracker = playArea.gameObject.AddComponent<VRTK_CollisionTracker>();
				}
				ManageCollisionListeners(true);
			}
		}

		protected virtual void SetupHeadset()
		{
			headset = VRTK_DeviceFinder.HeadsetTransform();
			if (headset != null)
			{
				currentStandingPosition = new Vector2(headset.position.x, headset.position.z);
			}
		}

		protected virtual void ManageCollisionListeners(bool state)
		{
			if (collisionTracker != null)
			{
				if (state)
				{
					collisionTracker.CollisionEnter += CollisionTracker_CollisionEnter;
					collisionTracker.CollisionExit += CollisionTracker_CollisionExit;
					collisionTracker.TriggerEnter += CollisionTracker_TriggerEnter;
					collisionTracker.TriggerExit += CollisionTracker_TriggerExit;
				}
				else
				{
					collisionTracker.CollisionEnter -= CollisionTracker_CollisionEnter;
					collisionTracker.CollisionExit -= CollisionTracker_CollisionExit;
					collisionTracker.TriggerEnter -= CollisionTracker_TriggerEnter;
					collisionTracker.TriggerExit -= CollisionTracker_TriggerExit;
				}
			}
		}

		protected virtual void CollisionTracker_TriggerExit(object sender, CollisionTrackerEventArgs e)
		{
			OnTriggerExit(e.collider);
		}

		protected virtual void CollisionTracker_TriggerEnter(object sender, CollisionTrackerEventArgs e)
		{
			OnTriggerEnter(e.collider);
		}

		protected virtual void CollisionTracker_CollisionExit(object sender, CollisionTrackerEventArgs e)
		{
			OnCollisionExit(e.collision);
		}

		protected virtual void CollisionTracker_CollisionEnter(object sender, CollisionTrackerEventArgs e)
		{
			OnCollisionEnter(e.collision);
		}

		protected virtual void OnStartFalling(BodyPhysicsEventArgs e)
		{
			if (this.StartFalling != null)
			{
				this.StartFalling(this, e);
			}
		}

		protected virtual void OnStopFalling(BodyPhysicsEventArgs e)
		{
			if (this.StopFalling != null)
			{
				this.StopFalling(this, e);
			}
		}

		protected virtual void OnStartMoving(BodyPhysicsEventArgs e)
		{
			if (this.StartMoving != null)
			{
				this.StartMoving(this, e);
			}
		}

		protected virtual void OnStopMoving(BodyPhysicsEventArgs e)
		{
			if (this.StopMoving != null)
			{
				this.StopMoving(this, e);
			}
		}

		protected virtual void OnStartColliding(BodyPhysicsEventArgs e)
		{
			if (this.StartColliding != null)
			{
				this.StartColliding(this, e);
			}
		}

		protected virtual void OnStopColliding(BodyPhysicsEventArgs e)
		{
			if (this.StopColliding != null)
			{
				this.StopColliding(this, e);
			}
		}

		protected virtual BodyPhysicsEventArgs SetBodyPhysicsEvent(GameObject target)
		{
			BodyPhysicsEventArgs result = default(BodyPhysicsEventArgs);
			result.target = target;
			return result;
		}

		protected virtual void CalculateVelocity()
		{
			playAreaVelocity = ((!(playArea != null)) ? Vector3.zero : ((playArea.position - lastPlayAreaPosition) / Time.fixedDeltaTime));
			if (playAreaVelocity.magnitude > 7.5f)
			{
				SpaceMask.instance.ToggleSpeedUI(true);
			}
			else if (playAreaVelocity.magnitude < 7.5f && SpaceMask.instance.speedUI.activeSelf)
			{
				SpaceMask.instance.ToggleSpeedUI(false);
			}
		}

		protected virtual void TogglePhysics(bool state)
		{
			if (bodyRigidbody != null)
			{
			}
			if (bodyCollider != null)
			{
			}
			if (footCollider != null)
			{
			}
			currentBodyCollisionsSetting = state;
		}

		protected virtual void ManageFalling()
		{
			if (!isFalling)
			{
				CheckHeadsetMovement();
				SnapToNearestFloor();
			}
			else
			{
				CheckFalling();
			}
		}

		protected virtual void CheckBodyCollisionsSetting()
		{
			if (enableBodyCollisions != currentBodyCollisionsSetting)
			{
				TogglePhysics(enableBodyCollisions);
			}
		}

		protected virtual void CheckFalling()
		{
			if (isFalling && VRTK_SharedMethods.RoundFloat(lastPlayAreaPosition.y, fallCheckPrecision) == VRTK_SharedMethods.RoundFloat(playArea.position.y, fallCheckPrecision))
			{
				StopFall();
			}
		}

		protected virtual void SetCurrentStandingPosition()
		{
			if (playArea != null && !playArea.transform.position.Equals(lastPlayAreaPosition))
			{
				Vector3 vector = playArea.transform.position - lastPlayAreaPosition;
				currentStandingPosition = new Vector2(currentStandingPosition.x + vector.x, currentStandingPosition.y + vector.z);
			}
		}

		protected virtual void SetIsMoving(Vector2 currentHeadsetPosition)
		{
			float num = Vector2.Distance(currentHeadsetPosition, currentStandingPosition);
			float num2 = ((!(playArea != null)) ? 0f : Vector3.Distance(playArea.transform.position, lastPlayAreaPosition));
			isMoving = ((num > movementThreshold) ? true : false);
			if (playArea != null && (num2 > playAreaMovementThreshold || !onGround))
			{
				isMoving = false;
			}
		}

		protected virtual void CheckLean()
		{
			Vector3 vector = ((!(headset != null)) ? Vector3.zero : new Vector3(currentStandingPosition.x, headset.position.y, currentStandingPosition.y));
			Vector3 direction = ((!(playArea != null)) ? Vector3.zero : (-playArea.up));
			RaycastHit hitData;
			currentValidFloorObject = ((!VRTK_CustomRaycast.Raycast(ray: new Ray(vector, direction), customCast: customRaycast, hitData: out hitData, ignoreLayers: layersToIgnore)) ? null : hitData.collider.gameObject);
			if (!(headset == null) && !(playArea == null) && enableBodyCollisions)
			{
				Quaternion rotation = headset.rotation;
				headset.rotation = new Quaternion(0f, headset.rotation.y, headset.rotation.z, headset.rotation.w);
				Ray ray2 = new Ray(headset.position, headset.forward);
				float num = bodyCollider.radius + leanForwardLengthAddition;
				RaycastHit hitData2;
				if (!VRTK_CustomRaycast.Raycast(customRaycast, ray2, out hitData2, layersToIgnore, num) && currentValidFloorObject != null)
				{
					CalculateLean(vector, num, hitData.distance);
				}
				headset.rotation = rotation;
			}
		}

		protected virtual void CalculateLean(Vector3 startPosition, float forwardLength, float originalRayDistance)
		{
			Vector3 vector = startPosition + headset.forward * forwardLength;
			vector = new Vector3(vector.x, startPosition.y, vector.z);
			RaycastHit hitData;
			if (VRTK_CustomRaycast.Raycast(ray: new Ray(vector, -playArea.up), customCast: customRaycast, hitData: out hitData, ignoreLayers: layersToIgnore))
			{
				float num = VRTK_SharedMethods.RoundFloat(originalRayDistance - hitData.distance, decimalPrecision);
				float num2 = VRTK_SharedMethods.RoundFloat(Vector3.Distance(playArea.transform.position, lastPlayAreaPosition), decimalPrecision);
				isMoving = (onGround && num2 <= playAreaPositionThreshold && num > 0f) || isMoving;
				isLeaning = ((onGround && num > leanYThreshold) ? true : false);
			}
		}

		protected virtual void UpdateStandingPosition(Vector2 currentHeadsetPosition)
		{
			standingPositionHistory.Add(currentHeadsetPosition);
			if (standingPositionHistory.Count <= standingHistorySamples)
			{
				return;
			}
			if (!isLeaning && currentCollidingObject == null)
			{
				bool flag = true;
				for (int i = 0; i < standingHistorySamples; i++)
				{
					float num = Vector2.Distance(standingPositionHistory[i], standingPositionHistory[standingHistorySamples]);
					flag = num <= movementThreshold && flag;
				}
				currentStandingPosition = ((!flag) ? currentStandingPosition : currentHeadsetPosition);
			}
			standingPositionHistory.Clear();
		}

		protected virtual void CheckHeadsetMovement()
		{
			bool flag = isMoving;
			Vector2 currentHeadsetPosition = ((!(headset != null)) ? Vector2.zero : new Vector2(headset.position.x, headset.position.z));
			SetCurrentStandingPosition();
			SetIsMoving(currentHeadsetPosition);
			CheckLean();
			UpdateStandingPosition(currentHeadsetPosition);
			if (enableBodyCollisions)
			{
				TogglePhysics(!isMoving);
			}
			if (flag != isMoving)
			{
				MovementChanged(isMoving);
			}
		}

		protected virtual void MovementChanged(bool movementState)
		{
			if (movementState)
			{
				OnStartMoving(SetBodyPhysicsEvent(null));
			}
			else
			{
				OnStopMoving(SetBodyPhysicsEvent(null));
			}
		}

		protected virtual void EnableDropToFloor()
		{
			initialFloorDrop = false;
			retogglePhysicsOnCanFall = false;
			teleporter = GetComponent<VRTK_BasicTeleport>();
			if (teleporter != null)
			{
				teleporter.Teleported += Teleporter_Teleported;
			}
		}

		protected virtual void DisableDropToFloor()
		{
			if (teleporter != null)
			{
				teleporter.Teleported -= Teleporter_Teleported;
			}
		}

		protected virtual void Teleporter_Teleported(object sender, DestinationMarkerEventArgs e)
		{
			initialFloorDrop = false;
			StopFall();
			if (resetPhysicsAfterTeleport)
			{
				TogglePhysics(storedCurrentPhysics);
			}
		}

		protected virtual void EnableBodyPhysics()
		{
			currentBodyCollisionsSetting = enableBodyCollisions;
			CreateCollider();
			InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), true);
			InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), true);
		}

		protected virtual void DisableBodyPhysics()
		{
			DestroyCollider();
			InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), false);
			InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), false);
		}

		protected virtual void CheckStepUpCollision(Collision collision)
		{
			if (!(footCollider != null) || collision.contacts.Length <= 0 || !(collision.contacts[0].thisCollider.transform.name == footColliderContainerNameCheck))
			{
				return;
			}
			float num = 0.55f;
			float y = 0.01f;
			Vector3 vector = playArea.TransformPoint(footCollider.center);
			Vector3 center = new Vector3(vector.x, vector.y + CalculateStepUpYOffset() * num, vector.z);
			Vector3 halfExtents = new Vector3(bodyCollider.radius, y, bodyCollider.radius);
			float maxDistance = center.y - playArea.position.y;
			RaycastHit hitInfo;
			if (Physics.BoxCast(center, halfExtents, Vector3.down, out hitInfo, Quaternion.identity, maxDistance) && hitInfo.point.y - playArea.position.y > stepDropThreshold)
			{
				if (teleporter != null && enableTeleport)
				{
					hitFloorYDelta = playArea.position.y - hitInfo.point.y;
					TeleportFall(hitInfo.point.y, hitInfo);
					lastFrameFloorY = hitInfo.point.y;
				}
				else
				{
					playArea.position = new Vector3(hitInfo.point.x - (headset.position.x - playArea.position.x), hitInfo.point.y, hitInfo.point.z - (headset.position.z - playArea.position.z));
				}
			}
		}

		protected virtual GameObject CreateColliderContainer(string name, Transform parent)
		{
			GameObject gameObject = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, name));
			gameObject.transform.SetParent(parent);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Collider);
			return gameObject;
		}

		protected virtual void CreateCollider()
		{
			generateCollider = false;
			generateRigidbody = false;
			if (playArea == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
				return;
			}
			VRTK_PlayerObject.SetPlayerObject(playArea.gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
			bodyRigidbody = playArea.GetComponent<Rigidbody>();
			if (bodyRigidbody == null)
			{
				generateRigidbody = true;
				bodyRigidbody = playArea.gameObject.AddComponent<Rigidbody>();
				bodyRigidbody.mass = bodyMass;
				bodyRigidbody.freezeRotation = true;
			}
			bodyRigidbody.useGravity = false;
			bodyRigidbody.freezeRotation = true;
			if (bodyColliderContainer == null)
			{
				generateCollider = true;
				bodyColliderContainer = CreateColliderContainer("BodyColliderContainer", playArea);
				bodyCollider = bodyColliderContainer.AddComponent<CapsuleCollider>();
				bodyCollider.radius = bodyRadius;
				bodyCollider.height = 0.3f;
				bodyColliderContainer.gameObject.layer = LayerMask.NameToLayer("Player");
				VRTK_PlayerObject.SetPlayerObject(bodyColliderContainer, VRTK_PlayerObject.ObjectTypes.Collider);
			}
			if (playArea.gameObject.layer == 0)
			{
				playArea.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			}
			TogglePhysics(enableBodyCollisions);
		}

		protected virtual void DestroyCollider()
		{
			if (generateRigidbody)
			{
				UnityEngine.Object.Destroy(bodyRigidbody);
			}
			if (generateCollider)
			{
				UnityEngine.Object.Destroy(bodyColliderContainer);
			}
		}

		protected virtual void UpdateCollider()
		{
			if (!(bodyColliderContainer != null) || !(headset != null))
			{
				return;
			}
			float num = ((!headset) ? 0f : (headset.transform.localPosition.y - (headsetYOffset + CalculateStepUpYOffset())));
			float y = Mathf.Max(num * 0.5f + CalculateStepUpYOffset() + playAreaHeightAdjustment, bodyCollider.radius + playAreaHeightAdjustment);
			bodyCollider.center = new Vector3(headset.localPosition.x, y, headset.localPosition.z);
			if (bodyRigidbody.velocity.magnitude > 7.5f)
			{
				if (!inMotion && ArmUIManager.instance.speedVignette)
				{
					Vignette.instance.FadeIn();
				}
				inMotion = true;
				motionTimer += Time.deltaTime;
			}
			else
			{
				if (inMotion)
				{
					Vignette.instance.FadeOut();
				}
				motionTimer = 0f;
				inMotion = false;
			}
			if (footCollider != null)
			{
				float radius = bodyCollider.radius * stepThicknessMultiplier;
				footCollider.radius = radius;
				footCollider.height = CalculateStepUpYOffset();
				footCollider.center = new Vector3(headset.localPosition.x, CalculateStepUpYOffset() * 0.5f, headset.localPosition.z);
			}
		}

		protected virtual float CalculateStepUpYOffset()
		{
			return stepUpYOffset * 2f;
		}

		protected virtual void InitControllerListeners(GameObject mappedController, bool state)
		{
			if (!(mappedController != null))
			{
				return;
			}
			IgnoreCollisions(mappedController.GetComponentsInChildren<Collider>(), true);
			VRTK_InteractGrab component = mappedController.GetComponent<VRTK_InteractGrab>();
			if (component != null && ignoreGrabbedCollisions)
			{
				if (state)
				{
					component.ControllerGrabInteractableObject += OnGrabObject;
					component.ControllerUngrabInteractableObject += OnUngrabObject;
				}
				else
				{
					component.ControllerGrabInteractableObject -= OnGrabObject;
					component.ControllerUngrabInteractableObject -= OnUngrabObject;
				}
			}
		}

		protected virtual IEnumerator RestoreCollisions(GameObject obj)
		{
			yield return new WaitForEndOfFrame();
			if (obj != null)
			{
				VRTK_InteractableObject component = obj.GetComponent<VRTK_InteractableObject>();
				if (component != null && !component.IsGrabbed())
				{
					IgnoreCollisions(obj.GetComponentsInChildren<Collider>(), false);
				}
			}
		}

		protected virtual void IgnoreCollisions(Collider[] colliders, bool state)
		{
		}

		protected virtual void OnGrabObject(object sender, ObjectInteractEventArgs e)
		{
			if (e.target != null)
			{
				StopCoroutine("RestoreCollisions");
				IgnoreCollisions(e.target.GetComponentsInChildren<Collider>(), true);
			}
		}

		protected virtual void OnUngrabObject(object sender, ObjectInteractEventArgs e)
		{
			if (base.gameObject.activeInHierarchy && playArea.gameObject.activeInHierarchy)
			{
				StartCoroutine(RestoreCollisions(e.target));
			}
		}

		protected virtual bool FloorIsGrabbedObject(RaycastHit collidedObj)
		{
			VRTK_InteractableObject component = collidedObj.transform.GetComponent<VRTK_InteractableObject>();
			return component != null && component.IsGrabbed();
		}

		protected virtual bool FloorHeightChanged(float currentY)
		{
			float num = Mathf.Abs(currentY - lastFrameFloorY);
			return num > floorHeightTolerance;
		}

		protected virtual bool ValidDrop(bool rayHit, RaycastHit rayCollidedWith, float floorY)
		{
			return rayHit && teleporter != null && teleporter.ValidLocation(rayCollidedWith.transform, rayCollidedWith.point) && !FloorIsGrabbedObject(rayCollidedWith) && FloorHeightChanged(floorY);
		}

		protected virtual float ControllerHeightCheck(GameObject controllerObj)
		{
			RaycastHit hitData;
			VRTK_CustomRaycast.Raycast(ray: new Ray(controllerObj.transform.position, -playArea.up), customCast: customRaycast, hitData: out hitData, ignoreLayers: layersToIgnore);
			return controllerObj.transform.position.y - hitData.distance;
		}

		protected virtual bool ControllersStillOverPreviousFloor()
		{
			if (fallRestriction == FallingRestrictors.NoRestriction)
			{
				return false;
			}
			float num = 0.05f;
			GameObject controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();
			GameObject controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
			float y = playArea.position.y;
			bool flag = controllerRightHand.activeInHierarchy && Mathf.Abs(ControllerHeightCheck(controllerRightHand) - y) < num;
			bool flag2 = controllerLeftHand.activeInHierarchy && Mathf.Abs(ControllerHeightCheck(controllerLeftHand) - y) < num;
			if (fallRestriction == FallingRestrictors.LeftController)
			{
				flag = false;
			}
			if (fallRestriction == FallingRestrictors.RightController)
			{
				flag2 = false;
			}
			if (fallRestriction == FallingRestrictors.BothControllers)
			{
				return flag && flag2;
			}
			return flag || flag2;
		}

		protected virtual void SnapToNearestFloor()
		{
			if (!preventSnapToFloor && (enableBodyCollisions || enableTeleport) && (bool)headset && headset.transform.position.y > playArea.position.y)
			{
				RaycastHit hitData;
				bool rayHit = VRTK_CustomRaycast.Raycast(ray: new Ray(headset.transform.position, -playArea.up), customCast: customRaycast, hitData: out hitData, ignoreLayers: layersToIgnore);
				hitFloorYDelta = playArea.position.y - hitData.point.y;
				if (initialFloorDrop && (ValidDrop(rayHit, hitData, hitData.point.y) || retogglePhysicsOnCanFall))
				{
					storedCurrentPhysics = ArePhysicsEnabled();
					resetPhysicsAfterTeleport = false;
					TogglePhysics(false);
					HandleFall(hitData.point.y, hitData);
				}
				initialFloorDrop = true;
				lastFrameFloorY = hitData.point.y;
			}
		}

		protected virtual bool PreventFall(float hitFloorY)
		{
			return hitFloorY < playArea.position.y && ControllersStillOverPreviousFloor();
		}

		protected virtual void HandleFall(float hitFloorY, RaycastHit rayCollidedWith)
		{
			if (PreventFall(hitFloorY))
			{
				if (!retogglePhysicsOnCanFall)
				{
					retogglePhysicsOnCanFall = true;
					storedRetogglePhysics = storedCurrentPhysics;
				}
				return;
			}
			if (retogglePhysicsOnCanFall)
			{
				storedCurrentPhysics = storedRetogglePhysics;
				retogglePhysicsOnCanFall = false;
			}
			if (enableBodyCollisions && (teleporter == null || !enableTeleport || hitFloorYDelta > gravityFallYThreshold))
			{
				GravityFall(rayCollidedWith);
			}
			else if (teleporter != null && enableTeleport)
			{
				TeleportFall(hitFloorY, rayCollidedWith);
			}
		}

		protected virtual void StartFall(GameObject targetFloor)
		{
			isFalling = true;
			isMoving = false;
			isLeaning = false;
			onGround = false;
			OnStartFalling(SetBodyPhysicsEvent(targetFloor));
		}

		protected virtual void StopFall()
		{
			isFalling = false;
			onGround = true;
			OnStopFalling(SetBodyPhysicsEvent(null));
		}

		protected virtual void GravityFall(RaycastHit rayCollidedWith)
		{
		}

		protected virtual void TeleportFall(float floorY, RaycastHit rayCollidedWith)
		{
			StartFall(rayCollidedWith.collider.gameObject);
			GameObject gameObject = rayCollidedWith.transform.gameObject;
			Vector3 position = new Vector3(playArea.position.x, floorY, playArea.position.z);
			float blinkTransitionSpeed = teleporter.blinkTransitionSpeed;
			teleporter.blinkTransitionSpeed = ((!(Mathf.Abs(hitFloorYDelta) > blinkYThreshold)) ? 0f : blinkTransitionSpeed);
			OnDestinationMarkerSet(SetDestinationMarkerEvent(rayCollidedWith.distance, gameObject.transform, rayCollidedWith, position, uint.MaxValue, true));
			teleporter.blinkTransitionSpeed = blinkTransitionSpeed;
			resetPhysicsAfterTeleport = true;
		}

		protected virtual void ApplyBodyMomentum(bool applyMomentum = false)
		{
			if (applyMomentum)
			{
				float magnitude = bodyRigidbody.velocity.magnitude;
				Vector3 force = playAreaVelocity / ((!(magnitude < 1f)) ? magnitude : 1f);
				bodyRigidbody.AddRelativeForce(force, ForceMode.VelocityChange);
			}
		}
	}
}
