using UnityEngine;

namespace VRTK
{
	public class VRTK_UIPointerAutoActivator : MonoBehaviour
	{
		protected virtual void OnTriggerEnter(Collider collider)
		{
			VRTK_PlayerObject componentInParent = collider.GetComponentInParent<VRTK_PlayerObject>();
			VRTK_UIPointer componentInParent2 = collider.GetComponentInParent<VRTK_UIPointer>();
			if ((bool)componentInParent2 && (bool)componentInParent && componentInParent.objectType == VRTK_PlayerObject.ObjectTypes.Collider)
			{
				componentInParent2.autoActivatingCanvas = base.gameObject;
			}
		}

		protected virtual void OnTriggerExit(Collider collider)
		{
			VRTK_UIPointer componentInParent = collider.GetComponentInParent<VRTK_UIPointer>();
			if ((bool)componentInParent && componentInParent.autoActivatingCanvas == base.gameObject)
			{
				componentInParent.autoActivatingCanvas = null;
			}
		}
	}
}
