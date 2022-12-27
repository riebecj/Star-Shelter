using UnityEngine;

namespace VRTK.Examples
{
	public class RC_Car_Controller : MonoBehaviour
	{
		public GameObject rcCar;

		private RC_Car rcCarScript;

		private void Start()
		{
			rcCarScript = rcCar.GetComponent<RC_Car>();
			GetComponent<VRTK_ControllerEvents>().TriggerAxisChanged += DoTriggerAxisChanged;
			GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += DoTouchpadAxisChanged;
			GetComponent<VRTK_ControllerEvents>().TriggerReleased += DoTriggerReleased;
			GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += DoTouchpadTouchEnd;
			GetComponent<VRTK_ControllerEvents>().AliasMenuOn += DoCarReset;
		}

		private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
		{
			rcCarScript.SetTouchAxis(e.touchpadAxis);
		}

		private void DoTriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
		{
			rcCarScript.SetTriggerAxis(e.buttonPressure);
		}

		private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
		{
			rcCarScript.SetTouchAxis(Vector2.zero);
		}

		private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
		{
			rcCarScript.SetTriggerAxis(0f);
		}

		private void DoCarReset(object sender, ControllerInteractionEventArgs e)
		{
			rcCarScript.ResetCar();
		}
	}
}
