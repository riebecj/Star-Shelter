using UnityEngine;

namespace VRTK.Examples
{
	public class Lamp : VRTK_InteractableObject
	{
		public override void Grabbed(GameObject grabbingObject)
		{
			base.Grabbed(grabbingObject);
			ToggleKinematics(false);
		}

		public override void Ungrabbed(GameObject previousGrabbingObject)
		{
			base.Ungrabbed(previousGrabbingObject);
			ToggleKinematics(true);
		}

		private void ToggleKinematics(bool state)
		{
			Rigidbody[] componentsInChildren = base.transform.parent.GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody rigidbody in componentsInChildren)
			{
				rigidbody.isKinematic = state;
			}
		}
	}
}
