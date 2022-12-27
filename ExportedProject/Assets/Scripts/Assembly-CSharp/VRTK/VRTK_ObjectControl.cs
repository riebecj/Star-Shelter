using UnityEngine;

namespace VRTK
{
	public abstract class VRTK_ObjectControl : MonoBehaviour
	{
		public enum DirectionDevices
		{
			Headset = 0,
			LeftController = 1,
			RightController = 2,
			ControlledObject = 3
		}

		[Header("Control Settings")]
		[Tooltip("The controller to read the controller events from. If this is blank then it will attempt to get a controller events script from the same GameObject.")]
		public VRTK_ControllerEvents controller;

		[Tooltip("The direction that will be moved in is the direction of this device.")]
		public DirectionDevices deviceForDirection;

		[Tooltip("If this is checked then whenever the axis on the attached controller is being changed, all other object control scripts of the same type on other controllers will be disabled.")]
		public bool disableOtherControlsOnActive = true;

		[Tooltip("If a `VRTK_BodyPhysics` script is present and this is checked, then the object control will affect the play area whilst it is falling.")]
		public bool affectOnFalling;

		[Tooltip("An optional game object to apply the object control to. If this is blank then the PlayArea will be controlled.")]
		public GameObject controlOverrideObject;

		protected VRTK_ControllerEvents controllerEvents;

		protected VRTK_BodyPhysics bodyPhysics;

		protected VRTK_ObjectControl otherObjectControl;

		protected GameObject controlledGameObject;

		protected GameObject setControlOverrideObject;

		protected Transform directionDevice;

		protected DirectionDevices previousDeviceForDirection;

		protected Vector2 currentAxis;

		protected Vector2 storedAxis;

		protected bool currentlyFalling;

		protected bool modifierActive;

		protected float controlledGameObjectPreviousY;

		protected float controlledGameObjectPreviousYOffset = 0.01f;

		public event ObjectControlEventHandler XAxisChanged;

		public event ObjectControlEventHandler YAxisChanged;

		public virtual void OnXAxisChanged(ObjectControlEventArgs e)
		{
			if (this.XAxisChanged != null)
			{
				this.XAxisChanged(this, e);
			}
		}

		public virtual void OnYAxisChanged(ObjectControlEventArgs e)
		{
			if (this.YAxisChanged != null)
			{
				this.YAxisChanged(this, e);
			}
		}

		protected abstract void ControlFixedUpdate();

		protected abstract VRTK_ObjectControl GetOtherControl();

		protected abstract bool IsInAction();

		protected abstract void SetListeners(bool state);

		protected virtual void OnEnable()
		{
			currentAxis = Vector2.zero;
			storedAxis = Vector2.zero;
			controllerEvents = ((!(controller != null)) ? GetComponent<VRTK_ControllerEvents>() : controller);
			if (!controllerEvents)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_ObjectControl", "VRTK_ControllerEvents", "controller", "the same"));
			}
			else
			{
				SetControlledObject();
				bodyPhysics = (controlOverrideObject ? null : Object.FindObjectOfType<VRTK_BodyPhysics>());
				directionDevice = GetDirectionDevice();
				SetListeners(true);
				otherObjectControl = GetOtherControl();
			}
		}

		protected virtual void OnDisable()
		{
			SetListeners(false);
		}

		protected virtual void Update()
		{
			if (controlOverrideObject != setControlOverrideObject)
			{
				SetControlledObject();
			}
		}

		protected virtual void FixedUpdate()
		{
			CheckDirectionDevice();
			CheckFalling();
			ControlFixedUpdate();
		}

		protected virtual ObjectControlEventArgs SetEventArguements(Vector3 axisDirection, float axis, float axisDeadzone)
		{
			ObjectControlEventArgs result = default(ObjectControlEventArgs);
			result.controlledGameObject = controlledGameObject;
			result.directionDevice = directionDevice;
			result.axisDirection = axisDirection;
			result.axis = axis;
			result.deadzone = axisDeadzone;
			result.currentlyFalling = currentlyFalling;
			result.modifierActive = modifierActive;
			return result;
		}

		protected virtual void SetControlledObject()
		{
			setControlOverrideObject = controlOverrideObject;
			controlledGameObject = ((!controlOverrideObject) ? VRTK_DeviceFinder.PlayAreaTransform().gameObject : controlOverrideObject);
			controlledGameObjectPreviousY = controlledGameObject.transform.position.y;
		}

		protected virtual void CheckFalling()
		{
			if ((bool)bodyPhysics && bodyPhysics.IsFalling() && ObjectHeightChange())
			{
				if (!affectOnFalling)
				{
					if (storedAxis == Vector2.zero)
					{
						storedAxis = new Vector2(currentAxis.x, currentAxis.y);
					}
					currentAxis = Vector2.zero;
				}
				currentlyFalling = true;
			}
			if ((bool)bodyPhysics && !bodyPhysics.IsFalling() && currentlyFalling)
			{
				currentAxis = ((!IsInAction()) ? Vector2.zero : storedAxis);
				storedAxis = Vector2.zero;
				currentlyFalling = false;
			}
		}

		protected virtual bool ObjectHeightChange()
		{
			bool result = controlledGameObjectPreviousY - controlledGameObjectPreviousYOffset > controlledGameObject.transform.position.y;
			controlledGameObjectPreviousY = controlledGameObject.transform.position.y;
			return result;
		}

		protected virtual Transform GetDirectionDevice()
		{
			switch (deviceForDirection)
			{
			case DirectionDevices.ControlledObject:
				return controlledGameObject.transform;
			case DirectionDevices.Headset:
				return VRTK_DeviceFinder.HeadsetTransform();
			case DirectionDevices.LeftController:
				return VRTK_DeviceFinder.GetControllerLeftHand(true).transform;
			case DirectionDevices.RightController:
				return VRTK_DeviceFinder.GetControllerRightHand(true).transform;
			default:
				return null;
			}
		}

		protected virtual void CheckDirectionDevice()
		{
			if (previousDeviceForDirection != deviceForDirection)
			{
				directionDevice = GetDirectionDevice();
			}
			previousDeviceForDirection = deviceForDirection;
		}
	}
}
