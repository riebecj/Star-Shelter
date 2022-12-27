using System;

namespace Phonon
{
	public class ComputeDevice
	{
		private IntPtr device = IntPtr.Zero;

		public IntPtr GetDevice()
		{
			return device;
		}

		public Error Create(bool useOpenCL, ComputeDeviceType deviceType, int numComputeUnits)
		{
			Error error = Error.None;
			if (useOpenCL)
			{
				error = PhononCore.iplCreateComputeDevice(deviceType, numComputeUnits, ref device);
				if (error != 0)
				{
					throw new Exception("Unable to create OpenCL compute device (" + deviceType.ToString() + ", " + numComputeUnits + " CUs): [" + error.ToString() + "]");
				}
			}
			return error;
		}

		public void Destroy()
		{
			if (device != IntPtr.Zero)
			{
				PhononCore.iplDestroyComputeDevice(ref device);
			}
		}
	}
}
