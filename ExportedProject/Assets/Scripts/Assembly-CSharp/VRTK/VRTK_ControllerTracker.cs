using UnityEngine;

namespace VRTK
{
	public class VRTK_ControllerTracker : MonoBehaviour
	{
		protected VRTK_TrackedController trackedController;

		protected virtual void OnEnable()
		{
			GameObject actualController = VRTK_DeviceFinder.GetActualController(base.gameObject);
			trackedController = ((!(actualController != null)) ? GetComponent<VRTK_TrackedController>() : actualController.GetComponent<VRTK_TrackedController>());
			Update();
		}

		protected virtual void Update()
		{
			if ((bool)trackedController && base.transform.parent != trackedController.transform)
			{
				Vector3 localScale = base.transform.localScale;
				base.transform.SetParent(trackedController.transform);
				base.transform.localPosition = Vector3.zero;
				base.transform.localScale = localScale;
				base.transform.localRotation = Quaternion.identity;
			}
		}
	}
}
