using UnityEngine;

namespace VRTK
{
	public abstract class VRTK_BaseObjectControlAction : MonoBehaviour
	{
		public enum AxisListeners
		{
			XAxisChanged = 0,
			YAxisChanged = 1
		}

		[Tooltip("The Object Control script to receive axis change events from.")]
		public VRTK_ObjectControl objectControlScript;

		[Tooltip("Determines which Object Control Axis event to listen to.")]
		public AxisListeners listenOnAxisChange;

		protected Collider centerCollider;

		protected Vector3 colliderCenter = Vector3.zero;

		protected float colliderRadius;

		protected float colliderHeight;

		protected Transform controlledTransform;

		protected Transform playArea;

		protected abstract void Process(GameObject controlledGameObject, Transform directionDevice, Vector3 axisDirection, float axis, float deadzone, bool currentlyFalling, bool modifierActive);

		protected virtual void OnEnable()
		{
			playArea = VRTK_DeviceFinder.PlayAreaTransform();
			if ((bool)objectControlScript)
			{
				switch (listenOnAxisChange)
				{
				case AxisListeners.XAxisChanged:
					objectControlScript.XAxisChanged += AxisChanged;
					break;
				case AxisListeners.YAxisChanged:
					objectControlScript.YAxisChanged += AxisChanged;
					break;
				}
			}
		}

		protected virtual void OnDisable()
		{
			if ((bool)objectControlScript)
			{
				switch (listenOnAxisChange)
				{
				case AxisListeners.XAxisChanged:
					objectControlScript.XAxisChanged -= AxisChanged;
					break;
				case AxisListeners.YAxisChanged:
					objectControlScript.YAxisChanged -= AxisChanged;
					break;
				}
			}
		}

		protected virtual void AxisChanged(object sender, ObjectControlEventArgs e)
		{
			Process(e.controlledGameObject, e.directionDevice, e.axisDirection, e.axis, e.deadzone, e.currentlyFalling, e.modifierActive);
		}

		protected virtual void RotateAroundPlayer(GameObject controlledGameObject, float angle)
		{
			Vector3 objectCenter = GetObjectCenter(controlledGameObject.transform);
			Vector3 vector = controlledGameObject.transform.TransformPoint(objectCenter);
			controlledGameObject.transform.Rotate(Vector3.up, angle);
			vector -= controlledGameObject.transform.TransformPoint(objectCenter);
			controlledGameObject.transform.position += vector;
		}

		protected virtual void Blink(float blinkSpeed)
		{
			if (blinkSpeed > 0f)
			{
				VRTK_SDK_Bridge.HeadsetFade(Color.black, 0f);
				ReleaseBlink(blinkSpeed);
			}
		}

		protected virtual void ReleaseBlink(float blinkSpeed)
		{
			VRTK_SDK_Bridge.HeadsetFade(Color.clear, blinkSpeed);
		}

		protected virtual Vector3 GetObjectCenter(Transform checkObject)
		{
			if (centerCollider == null || checkObject != controlledTransform)
			{
				controlledTransform = checkObject;
				if (checkObject == playArea)
				{
					CapsuleCollider capsuleCollider = (CapsuleCollider)(centerCollider = playArea.GetComponentInChildren<CapsuleCollider>());
					if (capsuleCollider != null)
					{
						colliderRadius = capsuleCollider.radius;
						colliderHeight = capsuleCollider.height;
						colliderCenter = capsuleCollider.center;
					}
					else
					{
						VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "PlayArea", "CapsuleCollider", "the same or child"));
					}
				}
				else
				{
					centerCollider = checkObject.GetComponentInChildren<Collider>();
					if (centerCollider == null)
					{
						VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "CheckObject", "Collider", "the same or child"));
					}
					colliderRadius = 0.1f;
					colliderHeight = 0.1f;
				}
			}
			return colliderCenter;
		}

		protected virtual int GetAxisDirection(float axis)
		{
			int result = 0;
			if (axis < 0f)
			{
				result = -1;
			}
			else if (axis > 0f)
			{
				result = 1;
			}
			return result;
		}
	}
}
