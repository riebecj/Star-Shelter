using System;
using UnityEngine;

namespace VRTK
{
	[Obsolete("`VRTK_TouchpadWalking` has been replaced with `VRTK_TouchpadControl`. This script will be removed in a future version of VRTK.")]
	public class VRTK_TouchpadWalking : MonoBehaviour
	{
		[Tooltip("If this is checked then the left controller touchpad will be enabled to move the play area.")]
		public bool leftController = true;

		[Tooltip("If this is checked then the right controller touchpad will be enabled to move the play area.")]
		public bool rightController = true;

		[Tooltip("The maximum speed the play area will be moved when the touchpad is being touched at the extremes of the axis. If a lower part of the touchpad axis is touched (nearer the centre) then the walk speed is slower.")]
		public float maxWalkSpeed = 3f;

		[Tooltip("The speed in which the play area slows down to a complete stop when the user is no longer touching the touchpad. This deceleration effect can ease any motion sickness that may be suffered.")]
		public float deceleration = 0.1f;

		[Tooltip("If a button is defined then movement will only occur when the specified button is being held down and the touchpad axis changes.")]
		public VRTK_ControllerEvents.ButtonAlias moveOnButtonPress;

		[Tooltip("The direction that will be moved in is the direction of this device.")]
		public VRTK_DeviceFinder.Devices deviceForDirection;

		[Tooltip("If the defined speed multiplier button is pressed then the current movement speed will be multiplied by the `Speed Multiplier` value.")]
		public VRTK_ControllerEvents.ButtonAlias speedMultiplierButton;

		[Tooltip("The amount to mmultiply the movement speed by if the `Speed Multiplier Button` is pressed.")]
		public float speedMultiplier = 1f;

		private GameObject controllerLeftHand;

		private GameObject controllerRightHand;

		private Transform playArea;

		private Vector2 touchAxis;

		private float movementSpeed;

		private float strafeSpeed;

		private bool leftSubscribed;

		private bool rightSubscribed;

		private ControllerInteractionEventHandler touchpadAxisChanged;

		private ControllerInteractionEventHandler touchpadUntouched;

		private bool multiplySpeed;

		private VRTK_ControllerEvents controllerEvents;

		private VRTK_BodyPhysics bodyPhysics;

		private bool wasFalling;

		private bool previousLeftControllerState;

		private bool previousRightControllerState;

		protected virtual void Awake()
		{
			touchpadAxisChanged = DoTouchpadAxisChanged;
			touchpadUntouched = DoTouchpadTouchEnd;
			playArea = VRTK_DeviceFinder.PlayAreaTransform();
			controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
			controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();
			if (!playArea)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
			}
			VRTK_PlayerObject.SetPlayerObject(base.gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
		}

		protected virtual void OnEnable()
		{
			SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed);
			SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed);
			bodyPhysics = GetComponent<VRTK_BodyPhysics>();
			movementSpeed = 0f;
			strafeSpeed = 0f;
			multiplySpeed = false;
		}

		protected virtual void OnDisable()
		{
			SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed, true);
			SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed, true);
			bodyPhysics = null;
		}

		protected virtual void Update()
		{
			multiplySpeed = (bool)controllerEvents && speedMultiplierButton != 0 && controllerEvents.IsButtonPressed(speedMultiplierButton);
			CheckControllerState(controllerLeftHand, leftController, ref leftSubscribed, ref previousLeftControllerState);
			CheckControllerState(controllerRightHand, rightController, ref rightSubscribed, ref previousRightControllerState);
		}

		protected virtual void FixedUpdate()
		{
			HandleFalling();
			CalculateSpeed(ref movementSpeed, touchAxis.y);
			CalculateSpeed(ref strafeSpeed, touchAxis.x);
			Move();
		}

		protected virtual void HandleFalling()
		{
			if ((bool)bodyPhysics && bodyPhysics.IsFalling())
			{
				touchAxis = Vector2.zero;
				wasFalling = true;
			}
			if ((bool)bodyPhysics && !bodyPhysics.IsFalling() && wasFalling)
			{
				touchAxis = Vector2.zero;
				wasFalling = false;
				strafeSpeed = 0f;
				movementSpeed = 0f;
			}
		}

		protected virtual void CheckControllerState(GameObject controller, bool controllerState, ref bool subscribedState, ref bool previousState)
		{
			if (controllerState != previousState)
			{
				SetControllerListeners(controller, controllerState, ref subscribedState);
			}
			previousState = controllerState;
		}

		private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
		{
			controllerEvents = (VRTK_ControllerEvents)sender;
			if (moveOnButtonPress != 0 && !controllerEvents.IsButtonPressed(moveOnButtonPress))
			{
				touchAxis = Vector2.zero;
				controllerEvents = null;
			}
			else
			{
				touchAxis = e.touchpadAxis;
			}
		}

		private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
		{
			touchAxis = Vector2.zero;
			controllerEvents = null;
		}

		private void CalculateSpeed(ref float speed, float inputValue)
		{
			if (inputValue != 0f)
			{
				speed = maxWalkSpeed * inputValue;
				speed = ((!multiplySpeed) ? speed : (speed * speedMultiplier));
			}
			else
			{
				Decelerate(ref speed);
			}
		}

		private void Decelerate(ref float speed)
		{
			if (speed > 0f)
			{
				speed -= Mathf.Lerp(deceleration, maxWalkSpeed, 0f);
			}
			else if (speed < 0f)
			{
				speed += Mathf.Lerp(deceleration, 0f - maxWalkSpeed, 0f);
			}
			else
			{
				speed = 0f;
			}
			float num = 0.1f;
			if (speed < num && speed > 0f - num)
			{
				speed = 0f;
			}
		}

		private void Move()
		{
			Transform transform = VRTK_DeviceFinder.DeviceTransform(deviceForDirection);
			if ((bool)transform)
			{
				Vector3 vector = transform.forward * movementSpeed * Time.deltaTime;
				Vector3 vector2 = transform.right * strafeSpeed * Time.deltaTime;
				float y = playArea.position.y;
				playArea.position += vector + vector2;
				playArea.position = new Vector3(playArea.position.x, y, playArea.position.z);
			}
		}

		private void SetControllerListeners(GameObject controller, bool controllerState, ref bool subscribedState, bool forceDisabled = false)
		{
			if ((bool)controller)
			{
				bool toggle = !forceDisabled && controllerState;
				ToggleControllerListeners(controller, toggle, ref subscribedState);
			}
		}

		private void ToggleControllerListeners(GameObject controller, bool toggle, ref bool subscribed)
		{
			VRTK_ControllerEvents component = controller.GetComponent<VRTK_ControllerEvents>();
			if ((bool)component && toggle && !subscribed)
			{
				component.TouchpadAxisChanged += touchpadAxisChanged;
				component.TouchpadTouchEnd += touchpadUntouched;
				subscribed = true;
			}
			else if ((bool)component && !toggle && subscribed)
			{
				component.TouchpadAxisChanged -= touchpadAxisChanged;
				component.TouchpadTouchEnd -= touchpadUntouched;
				touchAxis = Vector2.zero;
				subscribed = false;
			}
		}
	}
}
