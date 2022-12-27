using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRTK.Examples.Utilities
{
	public class SceneChanger : MonoBehaviour
	{
		private bool canPress;

		private uint controllerIndex;

		private void Awake()
		{
			canPress = false;
			Invoke("ResetPress", 1f);
			DynamicGI.UpdateEnvironment();
		}

		private bool ForwardPressed()
		{
			if (controllerIndex >= uint.MaxValue)
			{
				return false;
			}
			if (canPress && VRTK_SDK_Bridge.IsTriggerPressedOnIndex(controllerIndex) && VRTK_SDK_Bridge.IsGripPressedOnIndex(controllerIndex) && VRTK_SDK_Bridge.IsTouchpadPressedOnIndex(controllerIndex))
			{
				return true;
			}
			return false;
		}

		private bool BackPressed()
		{
			if (controllerIndex >= uint.MaxValue)
			{
				return false;
			}
			if (canPress && VRTK_SDK_Bridge.IsTriggerPressedOnIndex(controllerIndex) && VRTK_SDK_Bridge.IsGripPressedOnIndex(controllerIndex) && VRTK_SDK_Bridge.IsButtonTwoPressedOnIndex(controllerIndex))
			{
				return true;
			}
			return false;
		}

		private void ResetPress()
		{
			canPress = true;
		}

		private void Update()
		{
			GameObject controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand(true);
			controllerIndex = VRTK_DeviceFinder.GetControllerIndex(controllerRightHand);
			if (ForwardPressed() || Input.GetKeyUp(KeyCode.Space))
			{
				int num = SceneManager.GetActiveScene().buildIndex + 1;
				if (num >= SceneManager.sceneCountInBuildSettings)
				{
					num = 0;
				}
				SceneManager.LoadScene(num);
			}
			if (BackPressed() || Input.GetKeyUp(KeyCode.Backspace))
			{
				int num2 = SceneManager.GetActiveScene().buildIndex - 1;
				if (num2 < 0)
				{
					num2 = SceneManager.sceneCountInBuildSettings - 1;
				}
				SceneManager.LoadScene(num2);
			}
		}
	}
}
