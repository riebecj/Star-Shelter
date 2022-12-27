using System.Collections;
using UnityEngine;

namespace VRTK
{
	[RequireComponent(typeof(VRTK_HeadsetControllerAware))]
	public class VRTK_TeleportDisableOnControllerObscured : MonoBehaviour
	{
		protected VRTK_BasicTeleport basicTeleport;

		protected VRTK_HeadsetControllerAware headset;

		protected virtual void OnEnable()
		{
			basicTeleport = GetComponent<VRTK_BasicTeleport>();
			StartCoroutine(EnableAtEndOfFrame());
		}

		protected virtual void OnDisable()
		{
			if (!(basicTeleport == null) && (bool)headset)
			{
				headset.ControllerObscured -= DisableTeleport;
				headset.ControllerUnobscured -= EnableTeleport;
			}
		}

		protected virtual IEnumerator EnableAtEndOfFrame()
		{
			if (!(basicTeleport == null))
			{
				yield return new WaitForEndOfFrame();
				headset = VRTK_ObjectCache.registeredHeadsetControllerAwareness;
				if ((bool)headset)
				{
					headset.ControllerObscured += DisableTeleport;
					headset.ControllerUnobscured += EnableTeleport;
				}
			}
		}

		protected virtual void DisableTeleport(object sender, HeadsetControllerAwareEventArgs e)
		{
			basicTeleport.ToggleTeleportEnabled(false);
		}

		protected virtual void EnableTeleport(object sender, HeadsetControllerAwareEventArgs e)
		{
			basicTeleport.ToggleTeleportEnabled(true);
		}
	}
}
