using System;
using UnityEngine;

namespace VRTK
{
	[RequireComponent(typeof(VRTK_BodyPhysics))]
	[Obsolete("`VRTK_TouchpadMovement` has been replaced with `VRTK_TouchpadControl`. This script will be removed in a future version of VRTK.")]
	public class VRTK_TouchpadMovement : MonoBehaviour
	{
		public enum VerticalAxisMovement
		{
			None = 0,
			Slide = 1,
			Warp = 2,
			WarpWithBlink = 3
		}

		public enum HorizontalAxisMovement
		{
			None = 0,
			Slide = 1,
			Rotate = 2,
			SnapRotate = 3,
			SnapRotateWithBlink = 4,
			Warp = 5,
			WarpWithBlink = 6
		}

		public enum AxisMovementType
		{
			Warp = 0,
			FlipDirection = 1,
			SnapRotate = 2
		}

		public enum AxisMovementDirection
		{
			None = 0,
			Left = 1,
			Right = 2,
			Forward = 3,
			Backward = 4
		}

		[Header("General settings")]
		[Tooltip("If this is checked then the left controller touchpad will be enabled for the selected movement types.")]
		public bool leftController = true;

		[Tooltip("If this is checked then the right controller touchpad will be enabled for the selected movement types.")]
		public bool rightController = true;

		[Tooltip("If a button is defined then the selected movement will only be performed when the specified button is being held down and the touchpad axis changes.")]
		public VRTK_ControllerEvents.ButtonAlias moveOnButtonPress;

		[Tooltip("If the defined movement multiplier button is pressed then the movement will be affected by the axis multiplier value.")]
		public VRTK_ControllerEvents.ButtonAlias movementMultiplierButton;

		[Header("Vertical Axis")]
		[Tooltip("Selects the main movement type to be performed when the vertical axis changes occur.")]
		public VerticalAxisMovement verticalAxisMovement = VerticalAxisMovement.Slide;

		[Tooltip("Dead zone for the vertical axis. High value recommended for warp movement.")]
		[Range(0f, 1f)]
		public float verticalDeadzone = 0.2f;

		[Tooltip("Multiplier for the vertical axis movement when the multiplier button is pressed.")]
		public float verticalMultiplier = 1.5f;

		[Tooltip("The direction that will be moved in is the direction of this device.")]
		public VRTK_DeviceFinder.Devices deviceForDirection;

		[Header("Direction flip")]
		[Tooltip("Enables a secondary action of a direction flip of 180 degrees when the touchpad is pulled downwards.")]
		public bool flipDirectionEnabled;

		[Tooltip("Dead zone for the downwards pull. High value recommended.")]
		[Range(0f, 1f)]
		public float flipDeadzone = 0.7f;

		[Tooltip("The delay before the next direction flip is allowed to happen.")]
		public float flipDelay = 1f;

		[Tooltip("Enables blink on flip.")]
		public bool flipBlink = true;

		[Header("Horizontal Axis")]
		[Tooltip("Selects the movement type to be performed when the horizontal axis changes occur.")]
		public HorizontalAxisMovement horizontalAxisMovement = HorizontalAxisMovement.Slide;

		[Tooltip("Dead zone for the horizontal axis. High value recommended for snap rotate and warp movement.")]
		[Range(0f, 1f)]
		public float horizontalDeadzone = 0.2f;

		[Tooltip("Multiplier for the horizontal axis movement when the multiplier button is pressed.")]
		public float horizontalMultiplier = 1.25f;

		[Tooltip("The delay before the next snap rotation is allowed to happen.")]
		public float snapRotateDelay = 0.5f;

		[Tooltip("The number of degrees to instantly rotate in to the given direction.")]
		public float snapRotateAngle = 30f;

		[Tooltip("The maximum speed the play area will be rotated when the touchpad is being touched at the extremes of the axis. If a lower part of the touchpad axis is touched (nearer the centre) then the rotation speed is slower.")]
		public float rotateMaxSpeed = 3f;

		[Header("Shared Axis Settings")]
		[Tooltip("Blink effect duration multiplier for the movement delay, ie. 1.0 means blink transition lasts until the delay has expired and 0.5 means the effect has completed when half of the delay time is done.")]
		[Range(0.1f, 1f)]
		public float blinkDurationMultiplier = 0.7f;

		[Tooltip("The maximum speed the play area will be moved by sliding when the touchpad is being touched at the extremes of the axis. If a lower part of the touchpad axis is touched (nearer the centre) then the speed is slower.")]
		public float slideMaxSpeed = 3f;

		[Tooltip("The speed in which the play area slows down to a complete stop when the user is no longer touching the touchpad. This deceleration effect can ease any motion sickness that may be suffered.")]
		public float slideDeceleration = 0.1f;

		[Tooltip("The delay before the next warp is allowed to happen.")]
		public float warpDelay = 0.5f;

		[Tooltip("The distance to warp in to the given direction.")]
		public float warpRange = 1f;

		[Tooltip("The maximum altitude change allowed for a warp to happen.")]
		public float warpMaxAltitudeChange = 1f;

		private GameObject controllerLeftHand;

		private GameObject controllerRightHand;

		private Transform playArea;

		private Vector2 touchAxis;

		private float movementSpeed;

		private float strafeSpeed;

		private float blinkFadeInTime;

		private float lastWarp;

		private float lastFlip;

		private float lastSnapRotate;

		private bool multiplyMovement;

		private CapsuleCollider bodyCollider;

		private Transform headset;

		private bool leftSubscribed;

		private bool rightSubscribed;

		private ControllerInteractionEventHandler touchpadAxisChanged;

		private ControllerInteractionEventHandler touchpadUntouched;

		private VRTK_ControllerEvents controllerEvents;

		private VRTK_BodyPhysics bodyPhysics;

		private bool wasFalling;

		private bool previousLeftControllerState;

		private bool previousRightControllerState;

		public event TouchpadMovementAxisEventHandler AxisMovement;

		protected virtual void Awake()
		{
			touchpadAxisChanged = DoTouchpadAxisChanged;
			touchpadUntouched = DoTouchpadTouchEnd;
			controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
			controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();
			playArea = VRTK_DeviceFinder.PlayAreaTransform();
			if (!playArea)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
			}
			headset = VRTK_DeviceFinder.HeadsetTransform();
			if (!headset)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "HeadsetTransform", "Headset SDK"));
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
			blinkFadeInTime = 0f;
			lastWarp = 0f;
			lastFlip = 0f;
			lastSnapRotate = 0f;
			multiplyMovement = false;
		}

		protected virtual void Start()
		{
			bodyCollider = playArea.GetComponentInChildren<CapsuleCollider>();
			if (!bodyCollider)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_TouchpadMovement", "CapsuleCollider", "the PlayArea"));
			}
		}

		protected virtual void OnDisable()
		{
			SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed, true);
			SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed, true);
			bodyPhysics = null;
		}

		protected virtual void Update()
		{
			multiplyMovement = (bool)controllerEvents && movementMultiplierButton != 0 && controllerEvents.IsButtonPressed(movementMultiplierButton);
			CheckControllerState(controllerLeftHand, leftController, ref leftSubscribed, ref previousLeftControllerState);
			CheckControllerState(controllerRightHand, rightController, ref rightSubscribed, ref previousRightControllerState);
		}

		protected virtual void FixedUpdate()
		{
			bool flag = false;
			HandleFalling();
			if (horizontalAxisMovement == HorizontalAxisMovement.Slide)
			{
				CalculateSpeed(true, ref strafeSpeed, touchAxis.x);
				flag = true;
			}
			else if (horizontalAxisMovement == HorizontalAxisMovement.Rotate)
			{
				Rotate();
			}
			else if ((horizontalAxisMovement == HorizontalAxisMovement.SnapRotate || horizontalAxisMovement == HorizontalAxisMovement.SnapRotateWithBlink) && Mathf.Abs(touchAxis.x) > horizontalDeadzone && lastSnapRotate < Time.timeSinceLevelLoad)
			{
				SnapRotate(horizontalAxisMovement == HorizontalAxisMovement.SnapRotateWithBlink);
			}
			else if ((horizontalAxisMovement == HorizontalAxisMovement.Warp || horizontalAxisMovement == HorizontalAxisMovement.WarpWithBlink) && Mathf.Abs(touchAxis.x) > horizontalDeadzone && lastWarp < Time.timeSinceLevelLoad)
			{
				Warp(horizontalAxisMovement == HorizontalAxisMovement.WarpWithBlink, true);
			}
			if (flipDirectionEnabled && touchAxis.y < 0f)
			{
				if (touchAxis.y < 0f - flipDeadzone && lastFlip < Time.timeSinceLevelLoad)
				{
					lastFlip = Time.timeSinceLevelLoad + flipDelay;
					SnapRotate(flipBlink, true);
				}
			}
			else if (verticalAxisMovement == VerticalAxisMovement.Slide)
			{
				CalculateSpeed(false, ref movementSpeed, touchAxis.y);
				flag = true;
			}
			else if ((verticalAxisMovement == VerticalAxisMovement.Warp || verticalAxisMovement == VerticalAxisMovement.WarpWithBlink) && Mathf.Abs(touchAxis.y) > verticalDeadzone && lastWarp < Time.timeSinceLevelLoad)
			{
				Warp(verticalAxisMovement == VerticalAxisMovement.WarpWithBlink);
			}
			if (flag)
			{
				Move();
			}
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
				return;
			}
			Vector2 touchpadAxis = e.touchpadAxis;
			Vector2 normalized = touchpadAxis.normalized;
			float magnitude = touchpadAxis.magnitude;
			if (touchpadAxis.y < verticalDeadzone && touchpadAxis.y > 0f - verticalDeadzone)
			{
				touchpadAxis.y = 0f;
			}
			else
			{
				touchpadAxis.y = (normalized * ((magnitude - verticalDeadzone) / (1f - verticalDeadzone))).y;
			}
			if (touchpadAxis.x < horizontalDeadzone && touchpadAxis.x > 0f - horizontalDeadzone)
			{
				touchpadAxis.x = 0f;
			}
			else
			{
				touchpadAxis.x = (normalized * ((magnitude - horizontalDeadzone) / (1f - horizontalDeadzone))).x;
			}
			touchAxis = touchpadAxis;
		}

		private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
		{
			touchAxis = Vector2.zero;
			controllerEvents = null;
		}

		private void OnAxisMovement(AxisMovementType givenMovementType, AxisMovementDirection givenDirection)
		{
			if (this.AxisMovement != null)
			{
				TouchpadMovementAxisEventArgs e = default(TouchpadMovementAxisEventArgs);
				e.movementType = givenMovementType;
				e.direction = givenDirection;
				this.AxisMovement(this, e);
			}
		}

		private void CalculateSpeed(bool horizontal, ref float speed, float inputValue)
		{
			if (inputValue != 0f)
			{
				speed = slideMaxSpeed * inputValue;
				speed = ((!multiplyMovement) ? speed : (speed * ((!horizontal) ? verticalMultiplier : horizontalMultiplier)));
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
				speed -= Mathf.Lerp(slideDeceleration, slideMaxSpeed, 0f);
			}
			else if (speed < 0f)
			{
				speed += Mathf.Lerp(slideDeceleration, 0f - slideMaxSpeed, 0f);
			}
			else
			{
				speed = 0f;
			}
			if (speed < verticalDeadzone && speed > 0f - verticalDeadzone)
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

		private void Warp(bool blink = false, bool horizontal = false)
		{
			float num = warpRange * ((!multiplyMovement) ? 1f : ((!horizontal) ? verticalMultiplier : horizontalMultiplier));
			Transform transform = VRTK_DeviceFinder.DeviceTransform(deviceForDirection);
			Vector3 vector = playArea.TransformPoint(bodyCollider.center);
			Vector3 vector2 = vector + ((!horizontal) ? transform.forward : transform.right) * num * ((!(((!horizontal) ? touchAxis.y : touchAxis.x) < 0f)) ? 1 : (-1));
			float num2 = 0.2f;
			Vector3 vector3 = (horizontal ? ((!(touchAxis.x < 0f)) ? (headset.right * -1f) : headset.right) : ((!(touchAxis.y > 0f)) ? (headset.forward * -1f) : headset.forward));
			RaycastHit hitInfo;
			if (Physics.Raycast(headset.position + Vector3.up * num2, vector3, out hitInfo, num))
			{
				vector2 = hitInfo.point - vector3 * bodyCollider.radius;
			}
			if (Physics.Raycast(vector2 + Vector3.up * (warpMaxAltitudeChange + num2), Vector3.down, out hitInfo, (warpMaxAltitudeChange + num2) * 2f))
			{
				vector2.y = hitInfo.point.y + bodyCollider.height / 2f;
				lastWarp = Time.timeSinceLevelLoad + warpDelay;
				playArea.position = vector2 - vector + playArea.position;
				if (blink)
				{
					blinkFadeInTime = warpDelay * blinkDurationMultiplier;
					VRTK_SDK_Bridge.HeadsetFade(Color.black, 0f);
					Invoke("ReleaseBlink", 0.01f);
				}
				OnAxisMovement(AxisMovementType.Warp, horizontal ? ((touchAxis.x < 0f) ? AxisMovementDirection.Left : AxisMovementDirection.Right) : ((!(touchAxis.y < 0f)) ? AxisMovementDirection.Forward : AxisMovementDirection.Backward));
			}
		}

		private void RotateAroundPlayer(float angle)
		{
			Vector3 vector = playArea.TransformPoint(bodyCollider.center);
			playArea.Rotate(Vector3.up, angle);
			vector -= playArea.TransformPoint(bodyCollider.center);
			playArea.position += vector;
		}

		private void Rotate()
		{
			float angle = touchAxis.x * rotateMaxSpeed * Time.deltaTime * ((!multiplyMovement) ? 1f : horizontalMultiplier) * 10f;
			RotateAroundPlayer(angle);
		}

		private void SnapRotate(bool blink = false, bool flipDirection = false)
		{
			lastSnapRotate = Time.timeSinceLevelLoad + snapRotateDelay;
			float angle = ((!flipDirection) ? (snapRotateAngle * ((!multiplyMovement) ? 1f : horizontalMultiplier) * (float)((!(touchAxis.x < 0f)) ? 1 : (-1))) : 180f);
			RotateAroundPlayer(angle);
			if (blink)
			{
				blinkFadeInTime = snapRotateDelay * blinkDurationMultiplier;
				VRTK_SDK_Bridge.HeadsetFade(Color.black, 0f);
				Invoke("ReleaseBlink", 0.01f);
			}
			OnAxisMovement(flipDirection ? AxisMovementType.FlipDirection : AxisMovementType.SnapRotate, (touchAxis.x < 0f) ? AxisMovementDirection.Left : AxisMovementDirection.Right);
		}

		private void ReleaseBlink()
		{
			VRTK_SDK_Bridge.HeadsetFade(Color.clear, blinkFadeInTime);
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
