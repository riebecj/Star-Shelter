using UnityEngine;

namespace VRTK.Examples
{
	public class RealGun_Slide : VRTK_InteractableObject
	{
		private float restPosition;

		private float fireTimer;

		private float fireDistance = 0.05f;

		private float boltSpeed = 0.01f;

		public void Fire()
		{
			fireTimer = fireDistance;
		}

		protected override void Awake()
		{
			base.Awake();
			restPosition = base.transform.localPosition.z;
		}

		protected override void Update()
		{
			base.Update();
			if (base.transform.localPosition.z >= restPosition)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, restPosition);
			}
			if (fireTimer == 0f && base.transform.localPosition.z < restPosition && !IsGrabbed())
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z + boltSpeed);
			}
			if (fireTimer > 0f)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z - boltSpeed);
				fireTimer -= boltSpeed;
			}
			if (fireTimer < 0f)
			{
				fireTimer = 0f;
			}
		}
	}
}
