using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
	public class VRTK_Haptics : MonoBehaviour
	{
		protected Dictionary<uint, Coroutine> hapticLoopCoroutines = new Dictionary<uint, Coroutine>();

		public virtual void TriggerHapticPulse(uint controllerIndex, float strength)
		{
			CancelHapticPulse(controllerIndex);
			float strength2 = Mathf.Clamp(strength, 0f, 1f);
			VRTK_SDK_Bridge.HapticPulseOnIndex(controllerIndex, strength2);
		}

		public virtual void TriggerHapticPulse(uint controllerIndex, float strength, float duration, float pulseInterval)
		{
			CancelHapticPulse(controllerIndex);
			float hapticPulseStrength = Mathf.Clamp(strength, 0f, 1f);
			SDK_ControllerHapticModifiers hapticModifiers = VRTK_SDK_Bridge.GetHapticModifiers();
			Coroutine value = StartCoroutine(HapticPulse(controllerIndex, duration * hapticModifiers.durationModifier, hapticPulseStrength, pulseInterval * hapticModifiers.intervalModifier));
			if (!hapticLoopCoroutines.ContainsKey(controllerIndex))
			{
				hapticLoopCoroutines.Add(controllerIndex, value);
			}
		}

		protected virtual void OnDisable()
		{
			foreach (KeyValuePair<uint, Coroutine> hapticLoopCoroutine in hapticLoopCoroutines)
			{
				CancelHapticPulse(hapticLoopCoroutine.Key);
			}
		}

		protected virtual void CancelHapticPulse(uint controllerIndex)
		{
			if (hapticLoopCoroutines.ContainsKey(controllerIndex) && hapticLoopCoroutines[controllerIndex] != null)
			{
				StopCoroutine(hapticLoopCoroutines[controllerIndex]);
			}
		}

		protected virtual IEnumerator HapticPulse(uint controllerIndex, float duration, float hapticPulseStrength, float pulseInterval)
		{
			if (!(pulseInterval <= 0f))
			{
				while (duration > 0f)
				{
					VRTK_SDK_Bridge.HapticPulseOnIndex(controllerIndex, hapticPulseStrength);
					yield return new WaitForSeconds(pulseInterval);
					duration -= pulseInterval;
				}
			}
		}
	}
}
