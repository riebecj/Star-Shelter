using System;
using UnityEngine;

namespace Phonon
{
	public class ProbeManager
	{
		private IntPtr probeManager = IntPtr.Zero;

		private IntPtr probeBatch = IntPtr.Zero;

		public IntPtr GetProbeManager()
		{
			return probeManager;
		}

		public Error Create()
		{
			if (PhononCore.iplCreateProbeBatch(ref probeBatch) != 0)
			{
				throw new Exception("Unable to create probe batch.");
			}
			Error error = PhononCore.iplCreateProbeManager(ref probeManager);
			if (error != 0)
			{
				throw new Exception("Unable to create probe batch.");
			}
			ProbeBox[] array = UnityEngine.Object.FindObjectsOfType<ProbeBox>();
			ProbeBox[] array2 = array;
			foreach (ProbeBox probeBox in array2)
			{
				if (probeBox.probeBoxData != null && probeBox.probeBoxData.Length != 0)
				{
					IntPtr probeBox2 = IntPtr.Zero;
					try
					{
						PhononCore.iplLoadProbeBox(probeBox.probeBoxData, probeBox.probeBoxData.Length, ref probeBox2);
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.Message);
					}
					int num = PhononCore.iplGetProbeSpheres(probeBox2, null);
					for (int j = 0; j < num; j++)
					{
						PhononCore.iplAddProbeToBatch(probeBatch, probeBox2, j);
					}
					PhononCore.iplDestroyProbeBox(ref probeBox2);
				}
			}
			PhononCore.iplAddProbeBatch(probeManager, probeBatch);
			PhononCore.iplFinalizeProbeBatch(probeBatch);
			return error;
		}

		public void Destroy()
		{
			if (probeBatch != IntPtr.Zero)
			{
				PhononCore.iplDestroyProbeBatch(ref probeBatch);
				probeBatch = IntPtr.Zero;
			}
			if (probeManager != IntPtr.Zero)
			{
				PhononCore.iplDestroyProbeManager(ref probeManager);
				probeManager = IntPtr.Zero;
			}
		}
	}
}
