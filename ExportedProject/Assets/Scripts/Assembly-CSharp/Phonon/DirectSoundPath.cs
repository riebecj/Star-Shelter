using System.Runtime.InteropServices;

namespace Phonon
{
	public struct DirectSoundPath
	{
		public Vector3 direction;

		public float distanceAttenuation;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] airAbsorption;

		public float propagationDelay;

		public float occlusionFactor;
	}
}
