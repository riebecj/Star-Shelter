using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Examples
{
	public class RendererOffOnDash : MonoBehaviour
	{
		private bool wasSwitchedOff;

		private List<VRTK_DashTeleport> dashTeleporters = new List<VRTK_DashTeleport>();

		private void OnEnable()
		{
			foreach (VRTK_BasicTeleport registeredTeleporter in VRTK_ObjectCache.registeredTeleporters)
			{
				VRTK_DashTeleport component = registeredTeleporter.GetComponent<VRTK_DashTeleport>();
				if ((bool)component)
				{
					dashTeleporters.Add(component);
				}
			}
			foreach (VRTK_DashTeleport dashTeleporter in dashTeleporters)
			{
				dashTeleporter.WillDashThruObjects += RendererOff;
				dashTeleporter.DashedThruObjects += RendererOn;
			}
		}

		private void OnDisable()
		{
			foreach (VRTK_DashTeleport dashTeleporter in dashTeleporters)
			{
				dashTeleporter.WillDashThruObjects -= RendererOff;
				dashTeleporter.DashedThruObjects -= RendererOn;
			}
		}

		private void RendererOff(object sender, DashTeleportEventArgs e)
		{
			GameObject gameObject = base.transform.gameObject;
			RaycastHit[] hits = e.hits;
			foreach (RaycastHit raycastHit in hits)
			{
				if (raycastHit.collider.gameObject == gameObject)
				{
					SwitchRenderer(gameObject, false);
					break;
				}
			}
		}

		private void RendererOn(object sender, DashTeleportEventArgs e)
		{
			GameObject go = base.transform.gameObject;
			if (wasSwitchedOff)
			{
				SwitchRenderer(go, true);
			}
		}

		private void SwitchRenderer(GameObject go, bool enable)
		{
			go.GetComponent<Renderer>().enabled = enable;
			wasSwitchedOff = !enable;
		}
	}
}
