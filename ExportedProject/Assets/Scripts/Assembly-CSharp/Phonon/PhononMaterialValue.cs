using System;
using UnityEngine;

namespace Phonon
{
	[Serializable]
	public class PhononMaterialValue
	{
		[Range(0f, 1f)]
		public float LowFreqAbsorption;

		[Range(0f, 1f)]
		public float MidFreqAbsorption;

		[Range(0f, 1f)]
		public float HighFreqAbsorption;

		[Range(0f, 1f)]
		public float Scattering;

		public PhononMaterialValue()
		{
		}

		public PhononMaterialValue(float aLow, float aMid, float aHigh)
		{
			LowFreqAbsorption = aLow;
			MidFreqAbsorption = aMid;
			HighFreqAbsorption = aHigh;
			Scattering = 0.05f;
		}

		public PhononMaterialValue(float aLow, float aMid, float aHigh, float scattering)
		{
			LowFreqAbsorption = aLow;
			MidFreqAbsorption = aMid;
			HighFreqAbsorption = aHigh;
			Scattering = scattering;
		}

		public PhononMaterialValue(PhononMaterialValue other)
		{
			CopyFrom(other);
		}

		public void CopyFrom(PhononMaterialValue other)
		{
			LowFreqAbsorption = other.LowFreqAbsorption;
			MidFreqAbsorption = other.MidFreqAbsorption;
			HighFreqAbsorption = other.HighFreqAbsorption;
			Scattering = other.Scattering;
		}
	}
}
