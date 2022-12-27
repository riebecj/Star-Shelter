using System.Collections;
using UnityEngine;

namespace VRTK.Examples.Archery
{
	public class BowAim : MonoBehaviour
	{
		public float powerMultiplier;

		public float pullMultiplier;

		public float pullOffset;

		public float maxPullDistance = 1.1f;

		public float bowVibration = 0.062f;

		public float stringVibration = 0.087f;

		private BowAnimation bowAnimation;

		private GameObject currentArrow;

		private BowHandle handle;

		private VRTK_InteractableObject interact;

		private VRTK_InteractGrab holdControl;

		private VRTK_InteractGrab stringControl;

		private Quaternion releaseRotation;

		private Quaternion baseRotation;

		private bool fired;

		private float fireOffset;

		private float currentPull;

		private float previousPull;

		public VRTK_InteractGrab GetPullHand()
		{
			return stringControl;
		}

		public bool IsHeld()
		{
			return interact.IsGrabbed();
		}

		public bool HasArrow()
		{
			return currentArrow != null;
		}

		public void SetArrow(GameObject arrow)
		{
			currentArrow = arrow;
		}

		private void Start()
		{
			bowAnimation = GetComponent<BowAnimation>();
			handle = GetComponentInChildren<BowHandle>();
			interact = GetComponent<VRTK_InteractableObject>();
			interact.InteractableObjectGrabbed += DoObjectGrab;
		}

		private void DoObjectGrab(object sender, InteractableObjectEventArgs e)
		{
			if (VRTK_DeviceFinder.IsControllerLeftHand(e.interactingObject))
			{
				holdControl = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractGrab>();
				stringControl = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractGrab>();
			}
			else
			{
				stringControl = VRTK_DeviceFinder.GetControllerLeftHand().GetComponent<VRTK_InteractGrab>();
				holdControl = VRTK_DeviceFinder.GetControllerRightHand().GetComponent<VRTK_InteractGrab>();
			}
			StartCoroutine("GetBaseRotation");
		}

		private IEnumerator GetBaseRotation()
		{
			yield return new WaitForEndOfFrame();
			baseRotation = base.transform.localRotation;
		}

		private void Update()
		{
			if (currentArrow != null && IsHeld())
			{
				AimArrow();
				AimBow();
				PullString();
				if (!stringControl.IsGrabButtonPressed())
				{
					currentArrow.GetComponent<Arrow>().Fired();
					fired = true;
					releaseRotation = base.transform.localRotation;
					Release();
				}
			}
			else if (IsHeld())
			{
				if (fired)
				{
					fired = false;
					fireOffset = Time.time;
				}
				if (!releaseRotation.Equals(baseRotation))
				{
					base.transform.localRotation = Quaternion.Lerp(releaseRotation, baseRotation, (Time.time - fireOffset) * 8f);
				}
			}
			if (!IsHeld() && currentArrow != null)
			{
				Release();
			}
		}

		private void Release()
		{
			bowAnimation.SetFrame(0f);
			currentArrow.transform.SetParent(null);
			Collider[] componentsInChildren = currentArrow.GetComponentsInChildren<Collider>();
			Collider[] componentsInChildren2 = GetComponentsInChildren<Collider>();
			Collider[] array = componentsInChildren;
			foreach (Collider collider in array)
			{
				collider.enabled = true;
				Collider[] array2 = componentsInChildren2;
				foreach (Collider collider2 in array2)
				{
					Physics.IgnoreCollision(collider, collider2);
				}
			}
			currentArrow.GetComponent<Rigidbody>().isKinematic = false;
			currentArrow.GetComponent<Rigidbody>().velocity = currentPull * powerMultiplier * currentArrow.transform.TransformDirection(Vector3.forward);
			currentArrow.GetComponent<Arrow>().inFlight = true;
			currentArrow = null;
			currentPull = 0f;
			ReleaseArrow();
		}

		private void ReleaseArrow()
		{
			if ((bool)stringControl)
			{
				stringControl.ForceRelease();
			}
		}

		private void AimArrow()
		{
			currentArrow.transform.localPosition = Vector3.zero;
			currentArrow.transform.LookAt(handle.nockSide.position);
		}

		private void AimBow()
		{
			base.transform.rotation = Quaternion.LookRotation(holdControl.transform.position - stringControl.transform.position, holdControl.transform.TransformDirection(Vector3.forward));
		}

		private void PullString()
		{
			currentPull = Mathf.Clamp((Vector3.Distance(holdControl.transform.position, stringControl.transform.position) - pullOffset) * pullMultiplier, 0f, maxPullDistance);
			bowAnimation.SetFrame(currentPull);
			if (!currentPull.ToString("F2").Equals(previousPull.ToString("F2")))
			{
				VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(holdControl.gameObject), bowVibration);
				VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(stringControl.gameObject), stringVibration);
			}
			previousPull = currentPull;
		}
	}
}
