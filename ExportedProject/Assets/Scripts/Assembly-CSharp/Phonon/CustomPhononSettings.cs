using UnityEngine;

namespace Phonon
{
	[AddComponentMenu("Phonon/Custom Phonon Settings")]
	public class CustomPhononSettings : MonoBehaviour
	{
		public SceneType rayTracerOption;

		public ConvolutionOption convolutionOption;

		public bool useOpenCL;

		public ComputeDeviceType computeDeviceOption = ComputeDeviceType.Any;

		[Range(0f, 8f)]
		public int numComputeUnits = 4;
	}
}
