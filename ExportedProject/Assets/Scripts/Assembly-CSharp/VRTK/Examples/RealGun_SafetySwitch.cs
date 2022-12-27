using UnityEngine;

namespace VRTK.Examples
{
	public class RealGun_SafetySwitch : VRTK_InteractableObject
	{
		public bool safetyOff = true;

		private float offAngle = 40f;

		private Vector3 fixedPosition;

		public override void StartUsing(GameObject currentUsingObject)
		{
			base.StartUsing(currentUsingObject);
			SetSafety(!safetyOff);
		}

		protected void Start()
		{
			fixedPosition = base.transform.localPosition;
			SetSafety(safetyOff);
		}

		protected override void Update()
		{
			base.Update();
			base.transform.localPosition = fixedPosition;
			if (safetyOff)
			{
				base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			else
			{
				base.transform.localEulerAngles = new Vector3(0f, offAngle, 0f);
			}
		}

		private void SetSafety(bool safety)
		{
			safetyOff = safety;
		}
	}
}
