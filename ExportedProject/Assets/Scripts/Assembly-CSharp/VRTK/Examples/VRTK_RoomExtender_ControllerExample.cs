using UnityEngine;

namespace VRTK.Examples
{
	public class VRTK_RoomExtender_ControllerExample : MonoBehaviour
	{
		protected VRTK_RoomExtender roomExtender;

		private void Start()
		{
			if (GetComponent<VRTK_ControllerEvents>() == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_RoomExtender_ControllerExample", "VRTK_ControllerEvents", "the Controller Alias"));
			}
			else if (Object.FindObjectOfType<VRTK_RoomExtender>() == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_RoomExtender_ControllerExample", "VRTK_RoomExtender"));
			}
			else
			{
				roomExtender = Object.FindObjectOfType<VRTK_RoomExtender>();
				GetComponent<VRTK_ControllerEvents>().TouchpadPressed += DoTouchpadPressed;
				GetComponent<VRTK_ControllerEvents>().TouchpadReleased += DoTouchpadReleased;
				GetComponent<VRTK_ControllerEvents>().AliasMenuOn += DoSwitchMovementFunction;
			}
		}

		private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
		{
			roomExtender.additionalMovementMultiplier = ((!(e.touchpadAxis.magnitude * 5f > 1f)) ? 1f : (e.touchpadAxis.magnitude * 5f));
			if (roomExtender.additionalMovementEnabledOnButtonPress)
			{
				EnableAdditionalMovement();
			}
			else
			{
				DisableAdditionalMovement();
			}
		}

		private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
		{
			if (roomExtender.additionalMovementEnabledOnButtonPress)
			{
				DisableAdditionalMovement();
			}
			else
			{
				EnableAdditionalMovement();
			}
		}

		private void DoSwitchMovementFunction(object sender, ControllerInteractionEventArgs e)
		{
			switch (roomExtender.movementFunction)
			{
			case VRTK_RoomExtender.MovementFunction.Nonlinear:
				roomExtender.movementFunction = VRTK_RoomExtender.MovementFunction.LinearDirect;
				break;
			case VRTK_RoomExtender.MovementFunction.LinearDirect:
				roomExtender.movementFunction = VRTK_RoomExtender.MovementFunction.Nonlinear;
				break;
			}
		}

		private void EnableAdditionalMovement()
		{
			roomExtender.additionalMovementEnabled = true;
		}

		private void DisableAdditionalMovement()
		{
			roomExtender.additionalMovementEnabled = false;
		}
	}
}
