using UnityEngine;

namespace VRTK.Examples
{
	public class VRTK_ControllerPointerEvents_ListenerExample : MonoBehaviour
	{
		private void Start()
		{
			if (GetComponent<VRTK_DestinationMarker>() == null)
			{
				VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerPointerEvents_ListenerExample", "VRTK_DestinationMarker", "the Controller Alias"));
			}
			else
			{
				GetComponent<VRTK_DestinationMarker>().DestinationMarkerEnter += DoPointerIn;
				GetComponent<VRTK_DestinationMarker>().DestinationMarkerExit += DoPointerOut;
				GetComponent<VRTK_DestinationMarker>().DestinationMarkerSet += DoPointerDestinationSet;
			}
		}

		private void DebugLogger(uint index, string action, Transform target, RaycastHit raycastHit, float distance, Vector3 tipPosition)
		{
			string text = ((!target) ? "<NO VALID TARGET>" : target.name);
			string text2 = ((!raycastHit.collider) ? "<NO VALID COLLIDER>" : raycastHit.collider.name);
			VRTK_Logger.Info("Controller on index '" + index + "' is " + action + " at a distance of " + distance + " on object named [" + text + "] on the collider named [" + text2 + "] - the pointer tip position is/was: " + tipPosition);
		}

		private void DoPointerIn(object sender, DestinationMarkerEventArgs e)
		{
			DebugLogger(e.controllerIndex, "POINTER IN", e.target, e.raycastHit, e.distance, e.destinationPosition);
		}

		private void DoPointerOut(object sender, DestinationMarkerEventArgs e)
		{
			DebugLogger(e.controllerIndex, "POINTER OUT", e.target, e.raycastHit, e.distance, e.destinationPosition);
		}

		private void DoPointerDestinationSet(object sender, DestinationMarkerEventArgs e)
		{
			DebugLogger(e.controllerIndex, "POINTER DESTINATION", e.target, e.raycastHit, e.distance, e.destinationPosition);
		}
	}
}
