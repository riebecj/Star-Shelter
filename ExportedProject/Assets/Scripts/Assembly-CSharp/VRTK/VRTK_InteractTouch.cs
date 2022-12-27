using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	public class VRTK_InteractTouch : MonoBehaviour
	{
		[Tooltip("An optional GameObject that contains the compound colliders to represent the touching object. If this is empty then the collider will be auto generated at runtime to match the SDK default controller.")]
		public GameObject customColliderContainer;

		protected GameObject touchedObject;

		protected List<Collider> touchedObjectColliders = new List<Collider>();

		protected List<Collider> touchedObjectActiveColliders = new List<Collider>();

		protected GameObject controllerCollisionDetector;

		protected bool triggerRumble;

		protected bool destroyColliderOnDisable;

		protected bool triggerIsColliding;

		protected bool triggerWasColliding;

		protected bool rigidBodyForcedActive;

		protected Rigidbody touchRigidBody;

		protected Object defaultColliderPrefab;

		public event ObjectInteractEventHandler ControllerTouchInteractableObject;

		public event ObjectInteractEventHandler ControllerUntouchInteractableObject;

		public virtual void OnControllerTouchInteractableObject(ObjectInteractEventArgs e)
		{
			if (this.ControllerTouchInteractableObject != null)
			{
				this.ControllerTouchInteractableObject(this, e);
			}
		}

		public virtual void OnControllerUntouchInteractableObject(ObjectInteractEventArgs e)
		{
			if (this.ControllerUntouchInteractableObject != null)
			{
				this.ControllerUntouchInteractableObject(this, e);
			}
		}

		public virtual ObjectInteractEventArgs SetControllerInteractEvent(GameObject target)
		{
			ObjectInteractEventArgs result = default(ObjectInteractEventArgs);
			result.controllerIndex = VRTK_DeviceFinder.GetControllerIndex(base.gameObject);
			result.target = target;
			return result;
		}

		public virtual void ForceTouch(GameObject obj)
		{
			Collider componentInChildren = obj.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				OnTriggerStay(componentInChildren);
			}
		}

		public virtual GameObject GetTouchedObject()
		{
			return touchedObject;
		}

		public virtual bool IsObjectInteractable(GameObject obj)
		{
			if (obj != null)
			{
				VRTK_InteractableObject componentInParent = obj.GetComponentInParent<VRTK_InteractableObject>();
				if (componentInParent != null)
				{
					if (componentInParent.disableWhenIdle && !componentInParent.enabled)
					{
						return true;
					}
					return componentInParent.enabled;
				}
			}
			return false;
		}

		public virtual void ToggleControllerRigidBody(bool state, bool forceToggle = false)
		{
			if (controllerCollisionDetector != null && touchRigidBody != null)
			{
				touchRigidBody.isKinematic = !state;
				rigidBodyForcedActive = forceToggle;
				Collider[] componentsInChildren = controllerCollisionDetector.GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					collider.isTrigger = !state;
				}
			}
		}

		public virtual bool IsRigidBodyActive()
		{
			return !touchRigidBody.isKinematic;
		}

		public virtual bool IsRigidBodyForcedActive()
		{
			return IsRigidBodyActive() && rigidBodyForcedActive;
		}

		public virtual void ForceStopTouching()
		{
			if (touchedObject != null)
			{
				StopTouching(touchedObject);
			}
		}

		public virtual Collider[] ControllerColliders()
		{
			return (controllerCollisionDetector.GetComponents<Collider>().Length <= 0) ? controllerCollisionDetector.GetComponentsInChildren<Collider>() : controllerCollisionDetector.GetComponents<Collider>();
		}

		protected virtual void Awake()
		{
			destroyColliderOnDisable = false;
			SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(base.gameObject);
			defaultColliderPrefab = Resources.Load(VRTK_SDK_Bridge.GetControllerDefaultColliderPath(controllerHand));
		}

		protected virtual void OnEnable()
		{
			VRTK_PlayerObject.SetPlayerObject(base.gameObject, VRTK_PlayerObject.ObjectTypes.Controller);
			triggerRumble = false;
			CreateTouchCollider();
			CreateTouchRigidBody();
		}

		protected virtual void OnDisable()
		{
			ForceStopTouching();
			DestroyTouchCollider();
		}

		protected virtual void OnTriggerEnter(Collider collider)
		{
			GameObject gameObject = TriggerStart(collider);
			if (touchedObject != null && (bool)gameObject && touchedObject != gameObject && !touchedObject.GetComponent<VRTK_InteractableObject>().IsGrabbed())
			{
				CancelInvoke("ResetTriggerRumble");
				ResetTriggerRumble();
				ForceStopTouching();
				triggerIsColliding = true;
			}
		}

		protected virtual void OnTriggerExit(Collider collider)
		{
			if (touchedObjectActiveColliders.Contains(collider))
			{
				touchedObjectActiveColliders.Remove(collider);
			}
		}

		protected virtual void OnTriggerStay(Collider collider)
		{
			GameObject gameObject = TriggerStart(collider);
			if (touchedObject == null || collider.transform.IsChildOf(touchedObject.transform))
			{
				triggerIsColliding = true;
			}
			if (touchedObject == null && (bool)gameObject && IsObjectInteractable(collider.gameObject))
			{
				touchedObject = gameObject;
				VRTK_InteractableObject component = touchedObject.GetComponent<VRTK_InteractableObject>();
				GameObject currentTouchingObject = base.gameObject;
				if (!component.IsValidInteractableController(base.gameObject, component.allowedTouchControllers))
				{
					CleanupEndTouch();
					return;
				}
				StoreTouchedObjectColliders(collider);
				component.ToggleHighlight(true);
				ToggleControllerVisibility(false);
				CheckRumbleController(component);
				component.StartTouching(currentTouchingObject);
				OnControllerTouchInteractableObject(SetControllerInteractEvent(touchedObject));
			}
		}

		protected virtual void FixedUpdate()
		{
			if (!triggerIsColliding && !triggerWasColliding)
			{
				CheckStopTouching();
			}
			triggerWasColliding = triggerIsColliding;
			triggerIsColliding = false;
		}

		protected virtual void LateUpdate()
		{
			if (touchedObjectActiveColliders.Count == 0)
			{
				CheckStopTouching();
			}
		}

		protected virtual GameObject GetColliderInteractableObject(Collider collider)
		{
			VRTK_InteractableObject vRTK_InteractableObject = null;
			vRTK_InteractableObject = (((bool)collider.GetComponent<VRTK_InteractableObject>() || (bool)collider.GetComponent<Rigidbody>() || (collider.isTrigger && !(collider.tag == "InteractWithParent"))) ? collider.GetComponent<VRTK_InteractableObject>() : collider.GetComponentInParent<VRTK_InteractableObject>());
			return (!vRTK_InteractableObject) ? null : vRTK_InteractableObject.gameObject;
		}

		protected virtual void AddActiveCollider(Collider collider)
		{
			if (touchedObject != null && !touchedObjectActiveColliders.Contains(collider) && touchedObjectColliders.Contains(collider))
			{
				touchedObjectActiveColliders.Add(collider);
			}
		}

		protected virtual void StoreTouchedObjectColliders(Collider collider)
		{
			touchedObjectColliders.Clear();
			touchedObjectActiveColliders.Clear();
			Collider[] componentsInChildren = touchedObject.GetComponentsInChildren<Collider>();
			foreach (Collider item in componentsInChildren)
			{
				touchedObjectColliders.Add(item);
			}
			touchedObjectActiveColliders.Add(collider);
		}

		protected virtual void ToggleControllerVisibility(bool visible)
		{
			GameObject modelAliasController = VRTK_DeviceFinder.GetModelAliasController(base.gameObject);
			if (touchedObject != null)
			{
				VRTK_InteractControllerAppearance[] componentsInParent = touchedObject.GetComponentsInParent<VRTK_InteractControllerAppearance>(true);
				if (componentsInParent.Length > 0)
				{
					componentsInParent[0].ToggleControllerOnTouch(visible, modelAliasController, touchedObject);
				}
			}
			else if (visible)
			{
				VRTK_SharedMethods.SetRendererVisible(modelAliasController, touchedObject);
			}
		}

		protected virtual void CheckRumbleController(VRTK_InteractableObject touchedObjectScript)
		{
			VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(base.gameObject), 1500f);
			if (triggerRumble)
			{
			}
		}

		protected virtual void CheckStopTouching()
		{
			if (touchedObject != null)
			{
				VRTK_InteractableObject component = touchedObject.GetComponent<VRTK_InteractableObject>();
				GameObject gameObject = base.gameObject;
				if (component != null && component.GetGrabbingObject() != gameObject)
				{
					StopTouching(touchedObject);
				}
			}
		}

		protected virtual GameObject TriggerStart(Collider collider)
		{
			if (IsSnapDropZone(collider))
			{
				return null;
			}
			AddActiveCollider(collider);
			return GetColliderInteractableObject(collider);
		}

		protected virtual bool IsSnapDropZone(Collider collider)
		{
			if ((bool)collider.GetComponent<VRTK_SnapDropZone>())
			{
				return true;
			}
			return false;
		}

		protected virtual void ResetTriggerRumble()
		{
			triggerRumble = false;
		}

		protected virtual void StopTouching(GameObject untouched)
		{
			if (IsObjectInteractable(untouched))
			{
				GameObject previousTouchingObject = base.gameObject;
				VRTK_InteractableObject component = untouched.GetComponent<VRTK_InteractableObject>();
				component.StopTouching(previousTouchingObject);
				if (!component.IsTouched())
				{
					component.ToggleHighlight(false);
				}
			}
			ToggleControllerVisibility(true);
			OnControllerUntouchInteractableObject(SetControllerInteractEvent(untouched));
			CleanupEndTouch();
		}

		protected virtual void CleanupEndTouch()
		{
			touchedObject = null;
			touchedObjectActiveColliders.Clear();
			touchedObjectColliders.Clear();
		}

		protected virtual void DestroyTouchCollider()
		{
			if (destroyColliderOnDisable)
			{
				Object.Destroy(controllerCollisionDetector);
			}
		}

		protected virtual bool CustomRigidBodyIsChild()
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (transform != base.transform && transform == customColliderContainer.transform)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void CreateTouchCollider()
		{
			if (!(customColliderContainer == null))
			{
				controllerCollisionDetector = customColliderContainer;
				destroyColliderOnDisable = false;
			}
		}

		protected virtual void CreateTouchRigidBody()
		{
			touchRigidBody = GetComponent<Rigidbody>();
			if (touchRigidBody == null)
			{
				touchRigidBody = base.gameObject.AddComponent<Rigidbody>();
				touchRigidBody.isKinematic = true;
				touchRigidBody.useGravity = false;
				touchRigidBody.constraints = RigidbodyConstraints.FreezeAll;
				touchRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			}
		}
	}
}
