using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
	[RequireComponent(typeof(AeroplaneController))]
	public class AeroplaneUserControl2Axis : MonoBehaviour
	{
		public float maxRollAngle = 80f;

		public float maxPitchAngle = 80f;

		private AeroplaneController m_Aeroplane;

		private void Awake()
		{
			m_Aeroplane = GetComponent<AeroplaneController>();
		}

		private void FixedUpdate()
		{
		}

		private void AdjustInputForMobileControls(ref float roll, ref float pitch, ref float throttle)
		{
			float num = roll * maxRollAngle * ((float)Math.PI / 180f);
			float num2 = pitch * maxPitchAngle * ((float)Math.PI / 180f);
			roll = Mathf.Clamp(num - m_Aeroplane.RollAngle, -1f, 1f);
			pitch = Mathf.Clamp(num2 - m_Aeroplane.PitchAngle, -1f, 1f);
			float num3 = throttle * 0.5f + 0.5f;
			throttle = Mathf.Clamp(num3 - m_Aeroplane.Throttle, -1f, 1f);
		}
	}
}
