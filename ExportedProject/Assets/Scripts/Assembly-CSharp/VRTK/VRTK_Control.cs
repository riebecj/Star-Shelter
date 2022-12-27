using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRTK
{
	[ExecuteInEditMode]
	public abstract class VRTK_Control : MonoBehaviour
	{
		[Serializable]
		[Obsolete("`VRTK_Control.ValueChangedEvent` has been replaced with delegate events. `VRTK_Control_UnityEvents` is now required to access Unity events. This method will be removed in a future version of VRTK.")]
		public class ValueChangedEvent : UnityEvent<float, float>
		{
		}

		[Serializable]
		[Obsolete("`VRTK_Control.DefaultControlEvents` has been replaced with delegate events. `VRTK_Control_UnityEvents` is now required to access Unity events. This method will be removed in a future version of VRTK.")]
		public class DefaultControlEvents
		{
			public ValueChangedEvent OnValueChanged;
		}

		public struct ControlValueRange
		{
			public float controlMin;

			public float controlMax;
		}

		public enum Direction
		{
			autodetect = 0,
			x = 1,
			y = 2,
			z = 3
		}

		[Tooltip("The default events for the control. This parameter is deprecated and will be removed in a future version of VRTK.")]
		[Obsolete("`VRTK_Control.defaultEvents` has been replaced with delegate events. `VRTK_Control_UnityEvents` is now required to access Unity events. This method will be removed in a future version of VRTK.")]
		public DefaultControlEvents defaultEvents;

		[Tooltip("If active the control will react to the controller without the need to push the grab button.")]
		public bool interactWithoutGrab;

		protected Bounds bounds;

		protected bool setupSuccessful = true;

		protected VRTK_ControllerRigidbodyActivator autoTriggerVolume;

		protected float value;

		protected static Color COLOR_OK = Color.yellow;

		protected static Color COLOR_ERROR = new Color(1f, 0f, 0f, 0.9f);

		protected const float MIN_OPENING_DISTANCE = 20f;

		protected ControlValueRange valueRange;

		protected GameObject controlContent;

		protected bool hideControlContent;

		public event Control3DEventHandler ValueChanged;

		protected abstract void InitRequiredComponents();

		protected abstract bool DetectSetup();

		protected abstract ControlValueRange RegisterValueRange();

		public virtual void OnValueChanged(Control3DEventArgs e)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, e);
			}
		}

		public virtual float GetValue()
		{
			return value;
		}

		public virtual float GetNormalizedValue()
		{
			return Mathf.Abs(Mathf.Round((value - valueRange.controlMin) / (valueRange.controlMax - valueRange.controlMin) * 100f));
		}

		public virtual void SetContent(GameObject content, bool hideContent)
		{
			controlContent = content;
			hideControlContent = hideContent;
		}

		public virtual GameObject GetContent()
		{
			return controlContent;
		}

		protected abstract void HandleUpdate();

		protected virtual void Awake()
		{
			if (Application.isPlaying)
			{
				InitRequiredComponents();
				if (interactWithoutGrab)
				{
					CreateTriggerVolume();
				}
			}
			setupSuccessful = DetectSetup();
			if (Application.isPlaying)
			{
				valueRange = RegisterValueRange();
				HandleInteractables();
			}
		}

		protected virtual void Update()
		{
			if (!Application.isPlaying)
			{
				setupSuccessful = DetectSetup();
			}
			else if (setupSuccessful)
			{
				float num = value;
				HandleUpdate();
				if (value != num)
				{
					HandleInteractables();
					defaultEvents.OnValueChanged.Invoke(GetValue(), GetNormalizedValue());
					OnValueChanged(SetControlEvent());
				}
			}
		}

		protected virtual Control3DEventArgs SetControlEvent()
		{
			Control3DEventArgs result = default(Control3DEventArgs);
			result.value = GetValue();
			result.normalizedValue = GetNormalizedValue();
			return result;
		}

		protected virtual void OnDrawGizmos()
		{
			if (base.enabled)
			{
				bounds = VRTK_SharedMethods.GetBounds(base.transform);
				Gizmos.color = ((!setupSuccessful) ? COLOR_ERROR : COLOR_OK);
				if (setupSuccessful)
				{
					Gizmos.DrawWireCube(bounds.center, bounds.size);
				}
				else
				{
					Gizmos.DrawCube(bounds.center, bounds.size * 1.01f);
				}
			}
		}

		protected virtual void CreateTriggerVolume()
		{
			GameObject gameObject = new GameObject(base.name + "-Trigger");
			gameObject.transform.SetParent(base.transform);
			autoTriggerVolume = gameObject.AddComponent<VRTK_ControllerRigidbodyActivator>();
			Bounds bounds = VRTK_SharedMethods.GetBounds(base.transform);
			bounds.Expand(bounds.size * 0.2f);
			gameObject.transform.position = bounds.center;
			BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.isTrigger = true;
			boxCollider.size = bounds.size;
		}

		protected Vector3 GetThirdDirection(Vector3 axis1, Vector3 axis2)
		{
			bool flag = axis1.x != 0f || axis2.x != 0f;
			bool flag2 = axis1.y != 0f || axis2.y != 0f;
			bool flag3 = axis1.z != 0f || axis2.z != 0f;
			if (flag && flag2)
			{
				return Vector3.forward;
			}
			if (flag && flag3)
			{
				return Vector3.up;
			}
			return Vector3.right;
		}

		protected virtual void HandleInteractables()
		{
			if (!(controlContent == null))
			{
				if (hideControlContent)
				{
					controlContent.SetActive(value > 0f);
				}
				VRTK_InteractableObject[] componentsInChildren = controlContent.GetComponentsInChildren<VRTK_InteractableObject>(true);
				foreach (VRTK_InteractableObject vRTK_InteractableObject in componentsInChildren)
				{
					vRTK_InteractableObject.enabled = value > 20f;
				}
			}
		}
	}
}
