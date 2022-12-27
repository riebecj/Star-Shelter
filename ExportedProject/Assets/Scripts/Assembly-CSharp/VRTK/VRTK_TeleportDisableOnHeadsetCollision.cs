using System.Collections;
using UnityEngine;

namespace VRTK
{
	public class VRTK_TeleportDisableOnHeadsetCollision : MonoBehaviour
	{
		protected VRTK_BasicTeleport basicTeleport;

		protected VRTK_HeadsetCollision headsetCollision;

		protected virtual void OnEnable()
		{
			basicTeleport = GetComponent<VRTK_BasicTeleport>();
			StartCoroutine(EnableAtEndOfFrame());
		}

		protected virtual void OnDisable()
		{
			if (!(basicTeleport == null) && (bool)headsetCollision)
			{
				headsetCollision.HeadsetCollisionDetect -= DisableTeleport;
				headsetCollision.HeadsetCollisionEnded -= EnableTeleport;
			}
		}

		protected virtual IEnumerator EnableAtEndOfFrame()
		{
			if (!(basicTeleport == null))
			{
				yield return new WaitForEndOfFrame();
				headsetCollision = VRTK_ObjectCache.registeredHeadsetCollider;
				if ((bool)headsetCollision)
				{
					headsetCollision.HeadsetCollisionDetect += DisableTeleport;
					headsetCollision.HeadsetCollisionEnded += EnableTeleport;
				}
			}
		}

		protected virtual void DisableTeleport(object sender, HeadsetCollisionEventArgs e)
		{
			basicTeleport.ToggleTeleportEnabled(false);
		}

		protected virtual void EnableTeleport(object sender, HeadsetCollisionEventArgs e)
		{
			basicTeleport.ToggleTeleportEnabled(true);
		}
	}
}
