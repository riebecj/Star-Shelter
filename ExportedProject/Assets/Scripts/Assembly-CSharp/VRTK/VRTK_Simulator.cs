using System;
using UnityEngine;

namespace VRTK
{
	public class VRTK_Simulator : MonoBehaviour
	{
		[Serializable]
		public class Keys
		{
			public KeyCode forward = KeyCode.W;

			public KeyCode backward = KeyCode.S;

			public KeyCode strafeLeft = KeyCode.A;

			public KeyCode strafeRight = KeyCode.D;

			public KeyCode left = KeyCode.Q;

			public KeyCode right = KeyCode.E;

			public KeyCode up = KeyCode.Y;

			public KeyCode down = KeyCode.C;

			public KeyCode reset = KeyCode.X;
		}

		[Tooltip("Per default the keys on the left-hand side of the keyboard are used (WASD). They can be individually set as needed. The reset key brings the camera to its initial location.")]
		public Keys keys;

		[Tooltip("Typically the simulator should be turned off when not testing anymore. This option will do this automatically when outside the editor.")]
		public bool onlyInEditor = true;

		[Tooltip("Depending on the scale of the world the step size can be defined to increase or decrease movement speed.")]
		public float stepSize = 0.05f;

		[Tooltip("An optional game object marking the position and rotation at which the camera should be initially placed.")]
		public Transform camStart;

		protected Transform headset;

		protected Transform playArea;

		protected Vector3 initialPosition;

		protected Quaternion initialRotation;

		protected virtual void Start()
		{
			if (onlyInEditor && !Application.isEditor)
			{
				base.enabled = false;
				return;
			}
			headset = VRTK_DeviceFinder.HeadsetTransform();
			playArea = VRTK_DeviceFinder.PlayAreaTransform();
			if (!headset)
			{
				VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_Simulator", "Headset Camera", ". Simulator deactivated."));
				base.enabled = false;
				return;
			}
			if ((bool)camStart && camStart.gameObject.activeInHierarchy)
			{
				playArea.position = camStart.position;
				playArea.rotation = camStart.rotation;
			}
			initialPosition = playArea.position;
			initialRotation = playArea.rotation;
		}

		protected virtual void Update()
		{
			Vector3 vector = Vector3.zero;
			Vector3 eulerAngles = Vector3.zero;
			if (Input.GetKey(keys.forward))
			{
				vector = overwriteY(headset.forward, 0f);
			}
			else if (Input.GetKey(keys.backward))
			{
				vector = overwriteY(-headset.forward, 0f);
			}
			else if (Input.GetKey(keys.strafeLeft))
			{
				vector = overwriteY(-headset.right, 0f);
			}
			else if (Input.GetKey(keys.strafeRight))
			{
				vector = overwriteY(headset.right, 0f);
			}
			else if (Input.GetKey(keys.up))
			{
				vector = new Vector3(0f, 1f, 0f);
			}
			else if (Input.GetKey(keys.down))
			{
				vector = new Vector3(0f, -1f, 0f);
			}
			else if (Input.GetKey(keys.left))
			{
				eulerAngles = new Vector3(0f, -1f, 0f);
			}
			else if (Input.GetKey(keys.right))
			{
				eulerAngles = new Vector3(0f, 1f, 0f);
			}
			else if (Input.GetKey(keys.reset))
			{
				playArea.position = initialPosition;
				playArea.rotation = initialRotation;
			}
			playArea.Translate(vector * stepSize, Space.World);
			playArea.Rotate(eulerAngles);
		}

		protected virtual Vector3 overwriteY(Vector3 vector, float value)
		{
			return new Vector3(vector.x, value, vector.z);
		}
	}
}
