using UnityEngine;

namespace VRTK.Examples
{
	public class Controller_Hand : MonoBehaviour
	{
		public enum Hands
		{
			Right = 0,
			Left = 1
		}

		public Hands hand;

		private Transform pointerFinger;

		private Transform gripFingers;

		private float maxRotation = 90f;

		private float originalPointerRotation;

		private float originalGripRotation;

		private float targetPointerRotation;

		private float targetGripRotation;

		private void Start()
		{
			GetComponentInParent<VRTK_InteractGrab>().GrabButtonPressed += DoGrabOn;
			GetComponentInParent<VRTK_InteractGrab>().GrabButtonReleased += DoGrabOff;
			GetComponentInParent<VRTK_InteractUse>().UseButtonPressed += DoUseOn;
			GetComponentInParent<VRTK_InteractUse>().UseButtonReleased += DoUseOff;
			string text = "ModelPieces";
			pointerFinger = base.transform.Find(text + "/PointerFingerContainer");
			gripFingers = base.transform.Find(text + "/GripFingerContainer");
			if (hand == Hands.Left)
			{
				InversePosition(pointerFinger);
				InversePosition(gripFingers);
				InversePosition(base.transform.Find(text + "/Palm"));
				InversePosition(base.transform.Find(text + "/Thumb"));
			}
			originalPointerRotation = pointerFinger.localEulerAngles.y;
			originalGripRotation = gripFingers.localEulerAngles.y;
			targetPointerRotation = originalPointerRotation;
			targetGripRotation = originalGripRotation;
		}

		private void InversePosition(Transform givenTransform)
		{
			givenTransform.localPosition = new Vector3(givenTransform.localPosition.x * -1f, givenTransform.localPosition.y, givenTransform.localPosition.z);
			givenTransform.localEulerAngles = new Vector3(givenTransform.localEulerAngles.x, givenTransform.localEulerAngles.y * -1f, givenTransform.localEulerAngles.z);
		}

		private void DoGrabOn(object sender, ControllerInteractionEventArgs e)
		{
			targetGripRotation = maxRotation;
		}

		private void DoGrabOff(object sender, ControllerInteractionEventArgs e)
		{
			targetGripRotation = originalGripRotation;
		}

		private void DoUseOn(object sender, ControllerInteractionEventArgs e)
		{
			targetPointerRotation = maxRotation;
		}

		private void DoUseOff(object sender, ControllerInteractionEventArgs e)
		{
			targetPointerRotation = originalPointerRotation;
		}

		private void Update()
		{
			pointerFinger.localEulerAngles = new Vector3(targetPointerRotation, 0f, 0f);
			gripFingers.localEulerAngles = new Vector3(targetGripRotation, 0f, 0f);
		}
	}
}
