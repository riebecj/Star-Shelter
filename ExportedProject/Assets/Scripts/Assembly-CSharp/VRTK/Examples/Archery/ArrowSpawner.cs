using UnityEngine;

namespace VRTK.Examples.Archery
{
	public class ArrowSpawner : MonoBehaviour
	{
		public GameObject arrowPrefab;

		public float spawnDelay = 1f;

		private float spawnDelayTimer;

		private BowAim bow;

		private void Start()
		{
			spawnDelayTimer = 0f;
		}

		private void OnTriggerStay(Collider collider)
		{
			VRTK_InteractGrab vRTK_InteractGrab = ((!collider.gameObject.GetComponent<VRTK_InteractGrab>()) ? collider.gameObject.GetComponentInParent<VRTK_InteractGrab>() : collider.gameObject.GetComponent<VRTK_InteractGrab>());
			if (CanGrab(vRTK_InteractGrab) && NoArrowNotched(vRTK_InteractGrab.gameObject) && Time.time >= spawnDelayTimer)
			{
				GameObject gameObject = Object.Instantiate(arrowPrefab);
				gameObject.name = "ArrowClone";
				vRTK_InteractGrab.GetComponent<VRTK_InteractTouch>().ForceTouch(gameObject);
				vRTK_InteractGrab.AttemptGrab();
				spawnDelayTimer = Time.time + spawnDelay;
			}
		}

		private bool CanGrab(VRTK_InteractGrab grabbingController)
		{
			return (bool)grabbingController && grabbingController.GetGrabbedObject() == null && grabbingController.IsGrabButtonPressed();
		}

		private bool NoArrowNotched(GameObject controller)
		{
			if (VRTK_DeviceFinder.IsControllerLeftHand(controller))
			{
				bow = VRTK_DeviceFinder.GetControllerRightHand(true).GetComponentInChildren<BowAim>();
			}
			else if (VRTK_DeviceFinder.IsControllerRightHand(controller))
			{
				bow = VRTK_DeviceFinder.GetControllerLeftHand(true).GetComponentInChildren<BowAim>();
			}
			return bow == null || !bow.HasArrow();
		}
	}
}
