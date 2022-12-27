using System;
using UnityEngine;
using UnityEngine.AI;

namespace VRTK
{
	[Obsolete("`VRTK_BasePointer` has been replaced with `VRTK_Pointer`. This script will be removed in a future version of VRTK.")]
	public abstract class VRTK_BasePointer : VRTK_DestinationMarker
	{
		public enum pointerVisibilityStates
		{
			On_When_Active = 0,
			Always_On = 1,
			Always_Off = 2
		}

		[Serializable]
		public sealed class PointerOriginSmoothingSettings
		{
			[Tooltip("Whether or not to smooth the position of the pointer origin when positioning the pointer tip.")]
			public bool smoothsPosition;

			[Tooltip("The maximum allowed distance between the unsmoothed pointer origin and the smoothed pointer origin per frame to use for smoothing.")]
			public float maxAllowedPerFrameDistanceDifference = 0.003f;

			[Tooltip("Whether or not to smooth the rotation of the pointer origin when positioning the pointer tip.")]
			public bool smoothsRotation;

			[Tooltip("The maximum allowed angle between the unsmoothed pointer origin and the smoothed pointer origin per frame to use for smoothing.")]
			public float maxAllowedPerFrameAngleDifference = 1.5f;
		}

		[Header("Base Pointer Settings", order = 2)]
		[Tooltip("The controller that will be used to toggle the pointer. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
		public VRTK_ControllerEvents controller;

		[Tooltip("A custom transform to use as the origin of the pointer. If no pointer origin transform is provided then the transform the script is attached to is used.")]
		public Transform pointerOriginTransform;

		[Tooltip("Specifies the smoothing to be applied to the pointer origin when positioning the pointer tip.")]
		public PointerOriginSmoothingSettings pointerOriginSmoothingSettings = new PointerOriginSmoothingSettings();

		[Tooltip("The material to use on the rendered version of the pointer. If no material is selected then the default `WorldPointer` material will be used.")]
		public Material pointerMaterial;

		[Tooltip("The colour of the beam when it is colliding with a valid target. It can be set to a different colour for each controller.")]
		public Color pointerHitColor = new Color(0f, 0.5f, 0f, 1f);

		[Tooltip("The colour of the beam when it is not hitting a valid target. It can be set to a different colour for each controller.")]
		public Color pointerMissColor = new Color(0.8f, 0f, 0f, 1f);

		[Tooltip("If this is checked then the pointer beam will be activated on first press of the pointer alias button and will stay active until the pointer alias button is pressed again. The destination set event is emitted when the beam is deactivated on the second button press.")]
		public bool holdButtonToActivate = true;

		[Tooltip("If this is checked then the pointer will be an extension of the controller and able to interact with Interactable Objects.")]
		public bool interactWithObjects;

		[Tooltip("If `Interact With Objects` is checked and this is checked then when an object is grabbed with the pointer touching it, the object will attach to the pointer tip and not snap to the controller.")]
		public bool grabToPointerTip;

		[Tooltip("The time in seconds to delay the pointer beam being able to be active again. Useful for preventing constant teleportation.")]
		public float activateDelay;

		[Tooltip("Determines when the pointer beam should be displayed.")]
		public pointerVisibilityStates pointerVisibility;

		[Tooltip("The layers to ignore when raycasting.")]
		public LayerMask layersToIgnore = 4;

		protected Vector3 destinationPosition;

		protected float pointerContactDistance;

		protected Transform pointerContactTarget;

		protected RaycastHit pointerContactRaycastHit = default(RaycastHit);

		protected uint controllerIndex;

		protected VRTK_PlayAreaCursor playAreaCursor;

		protected Color currentPointerColor;

		protected GameObject objectInteractor;

		protected GameObject objectInteractorAttachPoint;

		private bool isActive;

		private bool destinationSetActive;

		private float activateDelayTimer;

		private int beamEnabledState;

		private VRTK_InteractableObject interactableObject;

		private Rigidbody savedAttachPoint;

		private bool attachedToInteractorAttachPoint;

		private float savedBeamLength;

		private VRTK_InteractGrab controllerGrabScript;

		private GameObject pointerOriginTransformFollowGameObject;

		private VRTK_TransformFollow pointerOriginTransformFollow;

		public virtual bool IsActive()
		{
			return isActive;
		}

		public virtual bool CanActivate()
		{
			return Time.time >= activateDelayTimer;
		}

		public virtual void ToggleBeam(bool state)
		{
			uint index = ((!controller) ? uint.MaxValue : VRTK_DeviceFinder.GetControllerIndex(controller.gameObject));
			if (state)
			{
				TurnOnBeam(index);
			}
			else
			{
				TurnOffBeam(index);
			}
		}

		protected virtual void Awake()
		{
			VRTK_PlayerObject.SetPlayerObject(base.gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			pointerOriginTransform = ((!(pointerOriginTransform == null)) ? pointerOriginTransform : VRTK_SDK_Bridge.GenerateControllerPointerOrigin(base.gameObject));
			AttemptSetController();
			CreatePointerOriginTransformFollow();
			Material source = Resources.Load("WorldPointer") as Material;
			if (pointerMaterial != null)
			{
				source = pointerMaterial;
			}
			pointerMaterial = new Material(source);
			pointerMaterial.color = pointerMissColor;
			playAreaCursor = GetComponent<VRTK_PlayAreaCursor>();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			DisableBeam();
			UnityEngine.Object.Destroy(objectInteractor);
			destinationSetActive = false;
			pointerContactDistance = 0f;
			pointerContactTarget = null;
			destinationPosition = Vector3.zero;
			AliasRegistration(false);
			controllerGrabScript = null;
			UnityEngine.Object.Destroy(pointerOriginTransformFollowGameObject);
		}

		protected virtual void Start()
		{
			SetupController();
		}

		protected virtual void Update()
		{
		}

		protected virtual void FixedUpdate()
		{
			if (interactWithObjects && (bool)objectInteractor && objectInteractor.activeInHierarchy)
			{
				UpdateObjectInteractor();
			}
			if (pointerOriginTransformFollow.isActiveAndEnabled)
			{
				UpdatePointerOriginTransformFollow();
			}
		}

		protected virtual void AliasRegistration(bool state)
		{
			if ((bool)controller)
			{
				if (state)
				{
					controller.AliasPointerOn += EnablePointerBeam;
					controller.AliasPointerOff += DisablePointerBeam;
					controller.AliasPointerSet += SetPointerDestination;
				}
				else
				{
					controller.AliasPointerOn -= EnablePointerBeam;
					controller.AliasPointerOff -= DisablePointerBeam;
					controller.AliasPointerSet -= SetPointerDestination;
				}
			}
		}

		protected virtual void OnValidate()
		{
			pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference = Mathf.Max(0.0001f, pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference);
			pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference = Mathf.Max(0.0001f, pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference);
		}

		protected Transform GetOrigin(bool smoothed = true)
		{
			return (smoothed && isActive) ? pointerOriginTransformFollow.gameObjectToChange.transform : ((!(pointerOriginTransform == null)) ? pointerOriginTransform : base.transform);
		}

		protected virtual void UpdateObjectInteractor()
		{
			objectInteractor.transform.position = destinationPosition;
		}

		protected virtual void UpdatePointerOriginTransformFollow()
		{
			pointerOriginTransformFollow.gameObjectToFollow = ((!(pointerOriginTransform == null)) ? pointerOriginTransform : base.transform).gameObject;
			pointerOriginTransformFollow.smoothsPosition = pointerOriginSmoothingSettings.smoothsPosition;
			pointerOriginTransformFollow.maxAllowedPerFrameDistanceDifference = pointerOriginSmoothingSettings.maxAllowedPerFrameDistanceDifference;
			pointerOriginTransformFollow.smoothsRotation = pointerOriginSmoothingSettings.smoothsRotation;
			pointerOriginTransformFollow.maxAllowedPerFrameAngleDifference = pointerOriginSmoothingSettings.maxAllowedPerFrameAngleDifference;
		}

		protected virtual void InitPointer()
		{
		}

		protected virtual void UpdateDependencies(Vector3 location)
		{
			if ((bool)playAreaCursor)
			{
				playAreaCursor.SetPlayAreaCursorTransform(location);
			}
		}

		protected virtual void EnablePointerBeam(object sender, ControllerInteractionEventArgs e)
		{
			TurnOnBeam(e.controllerIndex);
		}

		protected virtual void DisablePointerBeam(object sender, ControllerInteractionEventArgs e)
		{
			TurnOffBeam(e.controllerIndex);
		}

		protected virtual void SetPointerDestination(object sender, ControllerInteractionEventArgs e)
		{
			PointerSet();
		}

		protected virtual void PointerIn()
		{
			if (base.enabled && (bool)pointerContactTarget)
			{
				OnDestinationMarkerEnter(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerIndex));
				StartUseAction(pointerContactTarget);
			}
		}

		protected virtual void PointerOut()
		{
			if (base.enabled && (bool)pointerContactTarget)
			{
				OnDestinationMarkerExit(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerIndex));
				StopUseAction();
			}
		}

		protected virtual void PointerSet()
		{
			if (!base.enabled || !destinationSetActive || !pointerContactTarget || !CanActivate() || InvalidConstantBeam())
			{
				return;
			}
			activateDelayTimer = Time.time + activateDelay;
			VRTK_InteractableObject component = pointerContactTarget.GetComponent<VRTK_InteractableObject>();
			if (PointerActivatesUseAction(component))
			{
				if (component.IsUsing())
				{
					component.StopUsing(controller.gameObject);
					interactableObject.usingState = 0;
				}
				else if (!component.holdButtonToUse)
				{
					component.StartUsing(controller.gameObject);
					interactableObject.usingState++;
				}
			}
			if ((!playAreaCursor || !playAreaCursor.HasCollided()) && !PointerActivatesUseAction(interactableObject))
			{
				OnDestinationMarkerSet(SetDestinationMarkerEvent(pointerContactDistance, pointerContactTarget, pointerContactRaycastHit, destinationPosition, controllerIndex));
			}
			if (!isActive)
			{
				destinationSetActive = false;
			}
		}

		protected virtual void TogglePointer(bool state)
		{
			ToggleObjectInteraction(state);
			if ((bool)playAreaCursor)
			{
				playAreaCursor.SetHeadsetPositionCompensation(headsetPositionCompensation);
				playAreaCursor.ToggleState(state);
			}
			if (!state && PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse && interactableObject.IsUsing())
			{
				interactableObject.StopUsing(controller.gameObject);
			}
		}

		protected virtual void ToggleObjectInteraction(bool state)
		{
			if (!interactWithObjects)
			{
				return;
			}
			if (state && grabToPointerTip && (bool)controllerGrabScript)
			{
				savedAttachPoint = controllerGrabScript.controllerAttachPoint;
				controllerGrabScript.controllerAttachPoint = objectInteractorAttachPoint.GetComponent<Rigidbody>();
				attachedToInteractorAttachPoint = true;
			}
			if (!state && grabToPointerTip && (bool)controllerGrabScript)
			{
				if (attachedToInteractorAttachPoint)
				{
					controllerGrabScript.ForceRelease(true);
				}
				controllerGrabScript.controllerAttachPoint = savedAttachPoint;
				savedAttachPoint = null;
				attachedToInteractorAttachPoint = false;
				savedBeamLength = 0f;
			}
			if ((bool)objectInteractor)
			{
				objectInteractor.SetActive(state);
			}
		}

		protected virtual void ChangeMaterialColor(GameObject obj, Color color)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if ((bool)renderer.material)
				{
					renderer.material.EnableKeyword("_EMISSION");
					if (renderer.material.HasProperty("_Color"))
					{
						renderer.material.color = color;
					}
					if (renderer.material.HasProperty("_EmissionColor"))
					{
						renderer.material.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(color, 50f));
					}
				}
			}
		}

		protected virtual void SetPointerMaterial(Color color)
		{
			if ((bool)playAreaCursor)
			{
				playAreaCursor.SetMaterialColor(color);
			}
		}

		protected void UpdatePointerMaterial(Color color)
		{
			if (((bool)playAreaCursor && playAreaCursor.HasCollided()) || !ValidDestination(pointerContactTarget, destinationPosition))
			{
				color = pointerMissColor;
			}
			currentPointerColor = color;
			SetPointerMaterial(color);
		}

		protected virtual bool ValidDestination(Transform target, Vector3 destinationPosition)
		{
			bool flag = false;
			if ((bool)target)
			{
				NavMeshHit hit;
				flag = NavMesh.SamplePosition(destinationPosition, out hit, navMeshCheckDistance, -1);
			}
			if (navMeshCheckDistance == 0f)
			{
				flag = true;
			}
			return flag && (bool)target && !VRTK_PolicyList.Check(target.gameObject, invalidListPolicy);
		}

		protected virtual void CreateObjectInteractor()
		{
			objectInteractor = new GameObject(string.Format("[{0}]BasePointer_ObjectInteractor_Holder", base.gameObject.name));
			objectInteractor.transform.SetParent(controller.transform);
			objectInteractor.transform.localPosition = Vector3.zero;
			objectInteractor.layer = LayerMask.NameToLayer("Ignore Raycast");
			VRTK_PlayerObject.SetPlayerObject(objectInteractor, VRTK_PlayerObject.ObjectTypes.Pointer);
			GameObject gameObject = new GameObject(string.Format("[{0}]BasePointer_ObjectInteractor_Collider", base.gameObject.name));
			gameObject.transform.SetParent(objectInteractor.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.Pointer);
			if (grabToPointerTip)
			{
				objectInteractorAttachPoint = new GameObject(string.Format("[{0}]BasePointer_ObjectInteractor_AttachPoint", base.gameObject.name));
				objectInteractorAttachPoint.transform.SetParent(objectInteractor.transform);
				objectInteractorAttachPoint.transform.localPosition = Vector3.zero;
				objectInteractorAttachPoint.layer = LayerMask.NameToLayer("Ignore Raycast");
				Rigidbody rigidbody = objectInteractorAttachPoint.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				rigidbody.freezeRotation = true;
				rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				VRTK_PlayerObject.SetPlayerObject(objectInteractorAttachPoint, VRTK_PlayerObject.ObjectTypes.Pointer);
			}
			float num = 0.025f;
			objectInteractor.transform.localScale = new Vector3(num, num, num);
			objectInteractor.SetActive(false);
		}

		protected virtual void CreatePointerOriginTransformFollow()
		{
			pointerOriginTransformFollowGameObject = new GameObject(string.Format("[{0}]BasePointer_Origin_Smoothed", base.gameObject.name));
			pointerOriginTransformFollowGameObject.SetActive(false);
			pointerOriginTransformFollow = pointerOriginTransformFollowGameObject.AddComponent<VRTK_TransformFollow>();
			pointerOriginTransformFollow.followsScale = false;
			UpdatePointerOriginTransformFollow();
		}

		protected virtual float OverrideBeamLength(float currentLength)
		{
			if (!controllerGrabScript || !controllerGrabScript.GetGrabbedObject())
			{
				savedBeamLength = 0f;
			}
			if (interactWithObjects && grabToPointerTip && attachedToInteractorAttachPoint && (bool)controllerGrabScript && (bool)controllerGrabScript.GetGrabbedObject())
			{
				savedBeamLength = ((savedBeamLength != 0f) ? savedBeamLength : currentLength);
				return savedBeamLength;
			}
			return currentLength;
		}

		private void SetupController()
		{
			if (controller == null)
			{
				controller = GetComponent<VRTK_ControllerEvents>();
				AttemptSetController();
			}
			if (controller == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_BasePointer", "VRTK_ControllerEvents", "controller", "the same"));
			}
		}

		private void AttemptSetController()
		{
			if ((bool)controller)
			{
				AliasRegistration(true);
				controllerGrabScript = controller.GetComponent<VRTK_InteractGrab>();
				if (interactWithObjects)
				{
					CreateObjectInteractor();
				}
			}
		}

		private bool InvalidConstantBeam()
		{
			bool flag = controller.pointerToggleButton == controller.pointerSetButton;
			return !holdButtonToActivate && ((flag && beamEnabledState != 0) || (!flag && !isActive));
		}

		private bool PointerActivatesUseAction(VRTK_InteractableObject io)
		{
			return (bool)io && io.pointerActivatesUseAction && io.IsValidInteractableController(base.gameObject, io.allowedUseControllers);
		}

		private void StartUseAction(Transform target)
		{
			interactableObject = target.GetComponent<VRTK_InteractableObject>();
			bool flag = (bool)interactableObject && interactableObject.useOnlyIfGrabbed && !interactableObject.IsGrabbed();
			if (PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse && !flag && interactableObject.usingState == 0)
			{
				interactableObject.StartUsing(controller.gameObject);
				interactableObject.usingState++;
			}
		}

		private void StopUseAction()
		{
			if (PointerActivatesUseAction(interactableObject) && interactableObject.holdButtonToUse)
			{
				interactableObject.StopUsing(controller.gameObject);
				interactableObject.usingState = 0;
			}
		}

		private void TurnOnBeam(uint index)
		{
			beamEnabledState++;
			if (base.enabled && !isActive && CanActivate())
			{
				if ((bool)playAreaCursor)
				{
					playAreaCursor.SetPlayAreaCursorCollision(false);
				}
				controllerIndex = index;
				TogglePointer(true);
				isActive = true;
				destinationSetActive = true;
				if (pointerOriginTransformFollowGameObject != null)
				{
					pointerOriginTransformFollowGameObject.SetActive(true);
					pointerOriginTransformFollow.Follow();
				}
			}
		}

		private void TurnOffBeam(uint index)
		{
			if (base.enabled && isActive && (holdButtonToActivate || (!holdButtonToActivate && beamEnabledState >= 2)))
			{
				controllerIndex = index;
				DisableBeam();
			}
		}

		private void DisableBeam()
		{
			TogglePointer(false);
			isActive = false;
			beamEnabledState = 0;
			if (pointerOriginTransformFollowGameObject != null)
			{
				pointerOriginTransformFollowGameObject.SetActive(false);
			}
		}
	}
}
