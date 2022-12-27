using UnityEngine;

namespace VRTK
{
	public class VRTK_ControllerRigidbodyActivator : MonoBehaviour
	{
		[Tooltip("If this is checked then the collider will have it's rigidbody toggled on and off during a collision.")]
		public bool isEnabled = true;

		protected virtual void OnTriggerEnter(Collider collider)
		{
			ToggleRigidbody(collider, true);
		}

		protected virtual void OnTriggerExit(Collider collider)
		{
			ToggleRigidbody(collider, false);
		}

		private void ToggleRigidbody(Collider collider, bool state)
		{
			VRTK_InteractTouch componentInParent = collider.GetComponentInParent<VRTK_InteractTouch>();
			if ((bool)componentInParent && (isEnabled || !state))
			{
				componentInParent.ToggleControllerRigidBody(state, state);
			}
		}
	}
}
