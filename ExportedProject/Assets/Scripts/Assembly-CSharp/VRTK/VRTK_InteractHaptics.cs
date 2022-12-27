using UnityEngine;

namespace VRTK
{
	public class VRTK_InteractHaptics : MonoBehaviour
	{
		[Header("Haptics On Touch")]
		[Tooltip("Denotes how strong the rumble in the controller will be on touch.")]
		[Range(0f, 1f)]
		public float strengthOnTouch;

		[Tooltip("Denotes how long the rumble in the controller will last on touch.")]
		public float durationOnTouch;

		[Tooltip("Denotes interval betweens rumble in the controller on touch.")]
		public float intervalOnTouch = 0.05f;

		[Header("Haptics On Grab")]
		[Tooltip("Denotes how strong the rumble in the controller will be on grab.")]
		[Range(0f, 1f)]
		public float strengthOnGrab;

		[Tooltip("Denotes how long the rumble in the controller will last on grab.")]
		public float durationOnGrab;

		[Tooltip("Denotes interval betweens rumble in the controller on grab.")]
		public float intervalOnGrab = 0.05f;

		[Header("Haptics On Use")]
		[Tooltip("Denotes how strong the rumble in the controller will be on use.")]
		[Range(0f, 1f)]
		public float strengthOnUse;

		[Tooltip("Denotes how long the rumble in the controller will last on use.")]
		public float durationOnUse;

		[Tooltip("Denotes interval betweens rumble in the controller on use.")]
		public float intervalOnUse = 0.05f;

		protected const float minInterval = 0.05f;

		public virtual void HapticsOnTouch(uint controllerIndex)
		{
			if (strengthOnTouch > 0f && durationOnTouch > 0f)
			{
				TriggerHapticPulse(controllerIndex, strengthOnTouch, durationOnTouch, intervalOnTouch);
			}
		}

		public virtual void HapticsOnGrab(uint controllerIndex)
		{
			if (strengthOnGrab > 0f && durationOnGrab > 0f)
			{
				TriggerHapticPulse(controllerIndex, strengthOnGrab, durationOnGrab, intervalOnGrab);
			}
		}

		public virtual void HapticsOnUse(uint controllerIndex)
		{
			if (strengthOnUse > 0f && durationOnUse > 0f)
			{
				TriggerHapticPulse(controllerIndex, strengthOnUse, durationOnUse, intervalOnUse);
			}
		}

		protected virtual void OnEnable()
		{
			if (!GetComponent<VRTK_InteractableObject>())
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractHaptics", "VRTK_InteractableObject", "the same"));
			}
		}

		protected virtual void TriggerHapticPulse(uint controllerIndex, float strength, float duration, float interval)
		{
			VRTK_SharedMethods.TriggerHapticPulse(controllerIndex, strength, duration, (!(interval >= 0.05f)) ? 0.05f : interval);
		}
	}
}
