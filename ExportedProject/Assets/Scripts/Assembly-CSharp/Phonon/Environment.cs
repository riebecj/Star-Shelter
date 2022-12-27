using System;

namespace Phonon
{
	public class Environment
	{
		private IntPtr environment = IntPtr.Zero;

		public IntPtr GetEnvironment()
		{
			return environment;
		}

		public Error Create(ComputeDevice computeDevice, SimulationSettings simulationSettings, Scene scene, ProbeManager probeManager, GlobalContext globalContext)
		{
			Error error = PhononCore.iplCreateEnvironment(globalContext, computeDevice.GetDevice(), simulationSettings, scene.GetScene(), probeManager.GetProbeManager(), ref environment);
			if (error != 0)
			{
				throw new Exception("Unable to create environment [" + error.ToString() + "]");
			}
			return error;
		}

		public void Destroy()
		{
			if (environment != IntPtr.Zero)
			{
				PhononCore.iplDestroyEnvironment(ref environment);
			}
		}
	}
}
