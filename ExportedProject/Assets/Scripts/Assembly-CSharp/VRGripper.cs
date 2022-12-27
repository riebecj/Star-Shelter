using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRGripper : MonoBehaviour
{
	private SteamVR_TrackedObject CurrentController;

	private AudioSource CurrentAudio;

	private static List<SteamVR_TrackedObject> ControllerList = new List<SteamVR_TrackedObject>();

	private bool isColliding;

	private bool isGripping;

	public float VibrationLength = 0.1f;

	public float HapticPulseStrength = 0.5f;

	public float HapticFrameTime = 0.001f;

	private bool isVibrating;

	public static List<SteamVR_TrackedObject> GetControllers()
	{
		return new List<SteamVR_TrackedObject>(ControllerList);
	}

	private void OnEnable()
	{
		CurrentController = GetComponent<SteamVR_TrackedObject>();
		CurrentAudio = GetComponent<AudioSource>();
		if (!ControllerList.Contains(CurrentController))
		{
			ControllerList.Add(CurrentController);
		}
	}

	private void OnDisable()
	{
		if (ControllerList.Contains(CurrentController))
		{
			ControllerList.Remove(CurrentController);
		}
	}

	private void OnCollisionEnter(Collision _collision)
	{
		Debug.Log("COlliding Dfsdf");
		if (!isColliding && !isGripping)
		{
			StartCoroutine(LongVibration(HapticPulseStrength, VibrationLength));
			CurrentAudio.Play();
			isColliding = true;
		}
	}

	private IEnumerator LongVibration(float _strength, float _duration)
	{
		if (!isVibrating)
		{
			isVibrating = true;
			_strength = Mathf.Clamp(_strength, 0f, 1f);
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)CurrentController.index);
			for (float i = 0f; i < _duration; i += Time.fixedDeltaTime)
			{
				device.TriggerHapticPulse((ushort)(_strength * 3999f));
				yield return null;
			}
			isVibrating = false;
		}
	}

	private void HapticPulse(float _strength)
	{
		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)CurrentController.index);
		device.TriggerHapticPulse((ushort)(_strength * 3999f));
	}

	public void HapticVibration(float _strength, float _duration)
	{
		StartCoroutine(LongVibration(_strength, _duration));
	}

	private void OnCollisionExit(Collision _collision)
	{
		isColliding = false;
	}
}
