using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace VRTK
{
	public class SDK_InputSimulator : MonoBehaviour
	{
		public enum MouseInputMode
		{
			Always = 0,
			RequiresButtonPress = 1
		}

		[Tooltip("Show control information in the upper left corner of the screen.")]
		public bool showControlHints = true;

		[Tooltip("Hide hands when disabling them.")]
		public bool hideHandsAtSwitch;

		[Tooltip("Reset hand position and rotation when enabling them.")]
		public bool resetHandsAtSwitch = true;

		[Tooltip("Whether mouse movement always acts as input or requires a button press.")]
		public MouseInputMode mouseMovementInput;

		[Tooltip("Lock the mouse cursor to the game window when the mouse movement key is pressed.")]
		public bool lockMouseToView = true;

		[Header("Adjustments")]
		[Tooltip("Adjust hand movement speed.")]
		public float handMoveMultiplier = 0.002f;

		[Tooltip("Adjust hand rotation speed.")]
		public float handRotationMultiplier = 0.5f;

		[Tooltip("Adjust player movement speed.")]
		public float playerMoveMultiplier = 5f;

		[Tooltip("Adjust player rotation speed.")]
		public float playerRotationMultiplier = 0.5f;

		[Header("Operation Key Bindings")]
		[Tooltip("Key used to enable mouse input if a button press is required.")]
		public KeyCode mouseMovementKey = KeyCode.Mouse1;

		[Tooltip("Key used to toggle control hints on/off.")]
		public KeyCode toggleControlHints = KeyCode.F1;

		[Tooltip("Key used to switch between left and righ hand.")]
		public KeyCode changeHands = KeyCode.Tab;

		[Tooltip("Key used to switch hands On/Off.")]
		public KeyCode handsOnOff = KeyCode.LeftAlt;

		[Tooltip("Key used to switch between positional and rotational movement.")]
		public KeyCode rotationPosition = KeyCode.LeftShift;

		[Tooltip("Key used to switch between X/Y and X/Z axis.")]
		public KeyCode changeAxis = KeyCode.LeftControl;

		[Header("Movement Key Bindings")]
		[Tooltip("Key used to move forward.")]
		public KeyCode moveForward = KeyCode.W;

		[Tooltip("Key used to move to the left.")]
		public KeyCode moveLeft = KeyCode.A;

		[Tooltip("Key used to move backwards.")]
		public KeyCode moveBackward = KeyCode.S;

		[Tooltip("Key used to move to the right.")]
		public KeyCode moveRight = KeyCode.D;

		[Header("Controller Key Bindings")]
		[Tooltip("Key used to simulate trigger button.")]
		public KeyCode triggerAlias = KeyCode.Mouse1;

		[Tooltip("Key used to simulate grip button.")]
		public KeyCode gripAlias = KeyCode.Mouse0;

		[Tooltip("Key used to simulate touchpad button.")]
		public KeyCode touchpadAlias = KeyCode.Q;

		[Tooltip("Key used to simulate button one.")]
		public KeyCode buttonOneAlias = KeyCode.E;

		[Tooltip("Key used to simulate button two.")]
		public KeyCode buttonTwoAlias = KeyCode.R;

		[Tooltip("Key used to simulate start menu button.")]
		public KeyCode startMenuAlias = KeyCode.F;

		[Tooltip("Key used to switch between button touch and button press mode.")]
		public KeyCode touchModifier = KeyCode.T;

		[Tooltip("Key used to switch between hair touch mode.")]
		public KeyCode hairTouchModifier = KeyCode.H;

		private bool isHand;

		private GameObject hintCanvas;

		private Text hintText;

		private Transform rightHand;

		private Transform leftHand;

		private Transform currentHand;

		private Vector3 oldPos;

		private Transform myCamera;

		private SDK_ControllerSim rightController;

		private SDK_ControllerSim leftController;

		private static GameObject cachedCameraRig;

		private static bool destroyed;

		[CompilerGenerated]
		private static Func<KeyCode, string> _003C_003Ef__am_0024cache0;

		public static GameObject FindInScene()
		{
			if (cachedCameraRig == null && !destroyed)
			{
				cachedCameraRig = VRTK_SharedMethods.FindEvenInactiveGameObject<SDK_InputSimulator>();
				if (!cachedCameraRig)
				{
					VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRSimulatorCameraRig", "SDK_InputSimulator", ". check that the `VRTK/Prefabs/VRSimulatorCameraRig` prefab been added to the scene."));
				}
			}
			return cachedCameraRig;
		}

		private void Awake()
		{
			hintCanvas = base.transform.Find("Control Hints").gameObject;
			hintText = hintCanvas.GetComponentInChildren<Text>();
			hintCanvas.SetActive(showControlHints);
			rightHand = base.transform.Find("RightHand");
			rightHand.gameObject.SetActive(false);
			leftHand = base.transform.Find("LeftHand");
			leftHand.gameObject.SetActive(false);
			currentHand = rightHand;
			oldPos = Input.mousePosition;
			myCamera = base.transform.Find("Camera");
			leftHand.Find("Hand").GetComponent<Renderer>().material.color = Color.red;
			rightHand.Find("Hand").GetComponent<Renderer>().material.color = Color.green;
			rightController = rightHand.GetComponent<SDK_ControllerSim>();
			leftController = leftHand.GetComponent<SDK_ControllerSim>();
			rightController.Selected = true;
			leftController.Selected = false;
			destroyed = false;
			SDK_SimController sDK_SimController = VRTK_SDK_Bridge.GetControllerSDK() as SDK_SimController;
			if (sDK_SimController != null)
			{
				Dictionary<string, KeyCode> dictionary = new Dictionary<string, KeyCode>();
				dictionary.Add("Trigger", triggerAlias);
				dictionary.Add("Grip", gripAlias);
				dictionary.Add("TouchpadPress", touchpadAlias);
				dictionary.Add("ButtonOne", buttonOneAlias);
				dictionary.Add("ButtonTwo", buttonTwoAlias);
				dictionary.Add("StartMenu", startMenuAlias);
				dictionary.Add("TouchModifier", touchModifier);
				dictionary.Add("HairTouchModifier", hairTouchModifier);
				Dictionary<string, KeyCode> keyMappings = dictionary;
				sDK_SimController.SetKeyMappings(keyMappings);
			}
		}

		private void OnDestroy()
		{
			destroyed = true;
		}

		private void Update()
		{
			if (Input.GetKeyDown(toggleControlHints))
			{
				showControlHints = !showControlHints;
				hintCanvas.SetActive(showControlHints);
			}
			if (mouseMovementInput == MouseInputMode.RequiresButtonPress)
			{
				if (lockMouseToView)
				{
					Cursor.lockState = (Input.GetKey(mouseMovementKey) ? CursorLockMode.Locked : CursorLockMode.None);
				}
				else if (Input.GetKeyDown(mouseMovementKey))
				{
					oldPos = Input.mousePosition;
				}
			}
			if (Input.GetKeyDown(handsOnOff))
			{
				if (isHand)
				{
					SetMove();
				}
				else
				{
					SetHand();
				}
			}
			if (Input.GetKeyDown(changeHands))
			{
				if (currentHand.name == "LeftHand")
				{
					currentHand = rightHand;
					rightController.Selected = true;
					leftController.Selected = false;
				}
				else
				{
					currentHand = leftHand;
					rightController.Selected = false;
					leftController.Selected = true;
				}
			}
			if (isHand)
			{
				UpdateHands();
			}
			else
			{
				UpdateRotation();
			}
			UpdatePosition();
			if (showControlHints)
			{
				UpdateHints();
			}
		}

		private void UpdateHands()
		{
			Vector3 mouseDelta = GetMouseDelta();
			if (!IsAcceptingMouseInput())
			{
				return;
			}
			if (Input.GetKey(rotationPosition))
			{
				if (Input.GetKey(changeAxis))
				{
					Vector3 zero = Vector3.zero;
					zero.x += (mouseDelta * handRotationMultiplier).y;
					zero.y += (mouseDelta * handRotationMultiplier).x;
					currentHand.transform.Rotate(zero * Time.deltaTime);
				}
				else
				{
					Vector3 zero2 = Vector3.zero;
					zero2.z += (mouseDelta * handRotationMultiplier).x;
					zero2.x += (mouseDelta * handRotationMultiplier).y;
					currentHand.transform.Rotate(zero2 * Time.deltaTime);
				}
			}
			else if (Input.GetKey(changeAxis))
			{
				Vector3 zero3 = Vector3.zero;
				zero3 += mouseDelta * handMoveMultiplier;
				currentHand.transform.Translate(zero3 * Time.deltaTime);
			}
			else
			{
				Vector3 zero4 = Vector3.zero;
				zero4.x += (mouseDelta * handMoveMultiplier).x;
				zero4.z += (mouseDelta * handMoveMultiplier).y;
				currentHand.transform.Translate(zero4 * Time.deltaTime);
			}
		}

		private void UpdateRotation()
		{
			Vector3 mouseDelta = GetMouseDelta();
			if (IsAcceptingMouseInput())
			{
				Vector3 eulerAngles = base.transform.rotation.eulerAngles;
				eulerAngles.y += (mouseDelta * playerRotationMultiplier).x;
				base.transform.localRotation = Quaternion.Euler(eulerAngles);
				eulerAngles = myCamera.rotation.eulerAngles;
				if (eulerAngles.x > 180f)
				{
					eulerAngles.x -= 360f;
				}
				if (eulerAngles.x < 80f && eulerAngles.x > -80f)
				{
					eulerAngles.x += (mouseDelta * playerRotationMultiplier).y * -1f;
					eulerAngles.x = Mathf.Clamp(eulerAngles.x, -79f, 79f);
					myCamera.rotation = Quaternion.Euler(eulerAngles);
				}
			}
		}

		private void UpdatePosition()
		{
			if (Input.GetKey(moveForward))
			{
				base.transform.Translate(base.transform.forward * Time.deltaTime * playerMoveMultiplier, Space.World);
			}
			else if (Input.GetKey(moveBackward))
			{
				base.transform.Translate(-base.transform.forward * Time.deltaTime * playerMoveMultiplier, Space.World);
			}
			if (Input.GetKey(moveLeft))
			{
				base.transform.Translate(-base.transform.right * Time.deltaTime * playerMoveMultiplier, Space.World);
			}
			else if (Input.GetKey(moveRight))
			{
				base.transform.Translate(base.transform.right * Time.deltaTime * playerMoveMultiplier, Space.World);
			}
		}

		private void SetHand()
		{
			Cursor.visible = false;
			isHand = true;
			rightHand.gameObject.SetActive(true);
			leftHand.gameObject.SetActive(true);
			oldPos = Input.mousePosition;
			if (resetHandsAtSwitch)
			{
				rightHand.transform.localPosition = new Vector3(0.2f, 1.2f, 0.5f);
				rightHand.transform.localRotation = Quaternion.identity;
				leftHand.transform.localPosition = new Vector3(-0.2f, 1.2f, 0.5f);
				leftHand.transform.localRotation = Quaternion.identity;
			}
		}

		private void SetMove()
		{
			Cursor.visible = true;
			isHand = false;
			if (hideHandsAtSwitch)
			{
				rightHand.gameObject.SetActive(false);
				leftHand.gameObject.SetActive(false);
			}
		}

		private void UpdateHints()
		{
			string empty = string.Empty;
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CUpdateHints_003Em__0;
			}
			Func<KeyCode, string> func = _003C_003Ef__am_0024cache0;
			string text = string.Empty;
			if (mouseMovementInput == MouseInputMode.RequiresButtonPress)
			{
				text = " (" + func(mouseMovementKey) + ")";
			}
			string text2 = moveForward.ToString() + moveLeft.ToString() + moveBackward.ToString() + moveRight;
			empty = empty + "<b>" + text2 + "</b>: Move Player/Playspace\n";
			if (isHand)
			{
				empty = ((!Input.GetKey(rotationPosition)) ? (empty + "Mouse: Controller Position" + text + "\n") : (empty + "Mouse: Controller Rotation" + text + "\n"));
				string text3 = empty;
				empty = text3 + "Modes: HMD (" + func(handsOnOff) + "), Rotation (" + func(rotationPosition) + ")\n";
				text3 = empty;
				empty = text3 + "Controller Hand: " + currentHand.name.Replace("Hand", string.Empty) + " (" + func(changeHands) + ")\n";
				string text4 = ((!Input.GetKey(changeAxis)) ? "X/Z" : "X/Y");
				text3 = empty;
				empty = text3 + "Axis: " + text4 + " (" + func(changeAxis) + ")\n";
				string text5 = "Press";
				if (Input.GetKey(hairTouchModifier))
				{
					text5 = "Hair Touch";
				}
				else if (Input.GetKey(touchModifier))
				{
					text5 = "Touch";
				}
				text3 = empty;
				empty = text3 + "Button Press Mode Modifiers: Touch (" + func(touchModifier) + "), Hair Touch (" + func(hairTouchModifier) + ")\n";
				text3 = empty;
				empty = text3 + "Trigger " + text5 + ": " + func(triggerAlias) + "\n";
				text3 = empty;
				empty = text3 + "Grip " + text5 + ": " + func(gripAlias) + "\n";
				if (!Input.GetKey(hairTouchModifier))
				{
					text3 = empty;
					empty = text3 + "Touchpad " + text5 + ": " + func(touchpadAlias) + "\n";
					text3 = empty;
					empty = text3 + "Button One " + text5 + ": " + func(buttonOneAlias) + "\n";
					text3 = empty;
					empty = text3 + "Button Two " + text5 + ": " + func(buttonTwoAlias) + "\n";
					text3 = empty;
					empty = text3 + "Start Menu " + text5 + ": " + func(startMenuAlias) + "\n";
				}
			}
			else
			{
				empty = empty + "Mouse: HMD Rotation" + text + "\n";
				empty = empty + "Modes: Controller (" + func(handsOnOff) + ")\n";
			}
			hintText.text = empty.TrimEnd();
		}

		private bool IsAcceptingMouseInput()
		{
			return mouseMovementInput == MouseInputMode.Always || Input.GetKey(mouseMovementKey);
		}

		private Vector3 GetMouseDelta()
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				return new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			}
			Vector3 result = Input.mousePosition - oldPos;
			oldPos = Input.mousePosition;
			return result;
		}

		[CompilerGenerated]
		private static string _003CUpdateHints_003Em__0(KeyCode k)
		{
			return "<b>" + k.ToString() + "</b>";
		}
	}
}
