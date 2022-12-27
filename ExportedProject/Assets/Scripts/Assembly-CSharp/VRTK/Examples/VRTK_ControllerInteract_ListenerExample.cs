using UnityEngine;

namespace VRTK.Examples
{
	public class VRTK_ControllerInteract_ListenerExample : MonoBehaviour
	{
		private void Start()
		{
			if (GetComponent<VRTK_InteractTouch>() == null || GetComponent<VRTK_InteractGrab>() == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerInteract_ListenerExample", "VRTK_InteractTouch and VRTK_InteractGrab", "the Controller Alias"));
			}
			else
			{
				GetComponent<VRTK_InteractTouch>().ControllerTouchInteractableObject += DoInteractTouch;
				GetComponent<VRTK_InteractTouch>().ControllerUntouchInteractableObject += DoInteractUntouch;
				GetComponent<VRTK_InteractGrab>().ControllerGrabInteractableObject += DoInteractGrab;
				GetComponent<VRTK_InteractGrab>().ControllerUngrabInteractableObject += DoInteractUngrab;
			}
		}

		private void DebugLogger(uint index, string action, GameObject target)
		{
			VRTK_Logger.Info("Controller on index '" + index + "' is " + action + " an object named " + target.name);
		}

		private void DoInteractTouch(object sender, ObjectInteractEventArgs e)
		{
			if ((bool)e.target)
			{
				DebugLogger(e.controllerIndex, "TOUCHING", e.target);
			}
		}

		private void DoInteractUntouch(object sender, ObjectInteractEventArgs e)
		{
			if ((bool)e.target)
			{
				DebugLogger(e.controllerIndex, "NO LONGER TOUCHING", e.target);
			}
		}

		private void DoInteractGrab(object sender, ObjectInteractEventArgs e)
		{
			if ((bool)e.target)
			{
				DebugLogger(e.controllerIndex, "GRABBING", e.target);
			}
		}

		private void DoInteractUngrab(object sender, ObjectInteractEventArgs e)
		{
			if ((bool)e.target)
			{
				DebugLogger(e.controllerIndex, "NO LONGER GRABBING", e.target);
			}
		}
	}
}
