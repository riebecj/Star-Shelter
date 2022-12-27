using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace Phonon
{
	public class PhononBaker
	{
		public static bool oneBakeActive;

		private int probeBoxBakingCurrently;

		private bool bakeConvolution = true;

		private bool bakeParameteric;

		private bool cancelBake;

		private BakeStatus bakeStatus;

		private PhononManager duringBakePhononManager;

		private PhononManagerContainer duringBakePhononContainer;

		private ProbeBox[] duringBakeProbeBoxes;

		private Sphere duringBakeSphere;

		private BakingMode duringBakeMode;

		private string duringBakeIdentifier;

		private string bakedListenerPrefix = "__staticlistener__";

		private Thread bakeThread;

		private PhononCore.BakeProgressCallback bakeCallback;

		private IntPtr bakeCallbackPointer;

		private GCHandle bakeCallbackHandle;

		public void BakeEffectThread()
		{
			BakingSettings bakingSettings = default(BakingSettings);
			bakingSettings.bakeConvolution = (bakeConvolution ? Bool.True : Bool.False);
			bakingSettings.bakeParametric = (bakeParameteric ? Bool.True : Bool.False);
			ProbeBox[] array = duringBakeProbeBoxes;
			foreach (ProbeBox probeBox in array)
			{
				if (cancelBake)
				{
					return;
				}
				if (probeBox == null)
				{
					Debug.LogError("Probe Box specified in list of Probe Boxes to bake is null.");
					continue;
				}
				if (probeBox.probeBoxData == null || probeBox.probeBoxData.Length == 0)
				{
					Debug.LogError("Skipping probe box, because probes have not been generated for it.");
					continue;
				}
				IntPtr probeBox2 = IntPtr.Zero;
				try
				{
					PhononCore.iplLoadProbeBox(probeBox.probeBoxData, probeBox.probeBoxData.Length, ref probeBox2);
					probeBoxBakingCurrently++;
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.Message);
				}
				if (duringBakeMode == BakingMode.Reverb)
				{
					PhononCore.iplBakeReverb(duringBakePhononContainer.Environment().GetEnvironment(), probeBox2, bakingSettings, bakeCallback);
				}
				else if (duringBakeMode == BakingMode.StaticListener)
				{
					PhononCore.iplBakeStaticListener(duringBakePhononContainer.Environment().GetEnvironment(), probeBox2, duringBakeSphere, Common.ConvertString(duringBakeIdentifier), bakingSettings, bakeCallback);
				}
				else if (duringBakeMode == BakingMode.StaticSource)
				{
					PhononCore.iplBakePropagation(duringBakePhononContainer.Environment().GetEnvironment(), probeBox2, duringBakeSphere, Common.ConvertString(duringBakeIdentifier), bakingSettings, bakeCallback);
				}
				if (cancelBake)
				{
					return;
				}
				int num = PhononCore.iplSaveProbeBox(probeBox2, null);
				probeBox.probeBoxData = new byte[num];
				PhononCore.iplSaveProbeBox(probeBox2, probeBox.probeBoxData);
				string text = "__reverb__";
				if (duringBakeMode == BakingMode.StaticListener)
				{
					text = bakedListenerPrefix + duringBakeIdentifier;
				}
				else if (duringBakeMode == BakingMode.StaticSource)
				{
					text = duringBakeIdentifier;
				}
				int size = PhononCore.iplGetBakedDataSizeByName(probeBox2, Common.ConvertString(text));
				probeBox.UpdateProbeDataMapping(text, size);
				PhononCore.iplDestroyProbeBox(ref probeBox2);
			}
			bakeStatus = BakeStatus.Complete;
		}

		public void BeginBake(ProbeBox[] probeBoxes, BakingMode bakingMode, string identifier = null, Sphere sphere = default(Sphere))
		{
			oneBakeActive = true;
			bakeStatus = BakeStatus.InProgress;
			duringBakeProbeBoxes = probeBoxes;
			duringBakeMode = bakingMode;
			duringBakeIdentifier = identifier;
			duringBakeSphere = sphere;
			if (probeBoxes.Length == 0)
			{
				Debug.LogError("Probe Box component not attached or no probe boxes selected for baking.");
			}
			try
			{
				duringBakePhononManager = UnityEngine.Object.FindObjectOfType<PhononManager>();
				if (duringBakePhononManager == null)
				{
					throw new Exception("Phonon Manager Settings object not found in the scene! Click Window > Phonon");
				}
				bool initializeRenderer = false;
				duringBakePhononManager.Initialize(initializeRenderer);
				duringBakePhononContainer = duringBakePhononManager.PhononManagerContainer();
				duringBakePhononContainer.Initialize(initializeRenderer, duringBakePhononManager);
				if (duringBakePhononContainer.Scene().GetScene() == IntPtr.Zero)
				{
					throw new Exception("Make sure to pre-export the scene before baking.");
				}
			}
			catch (Exception ex)
			{
				bakeStatus = BakeStatus.Complete;
				Debug.LogError(ex.Message);
				return;
			}
			bakeCallback = AdvanceProgress;
			bakeCallbackPointer = Marshal.GetFunctionPointerForDelegate(bakeCallback);
			bakeCallbackHandle = GCHandle.Alloc(bakeCallbackPointer);
			GC.Collect();
			bakeThread = new Thread(BakeEffectThread);
			bakeThread.Start();
		}

		public void EndBake()
		{
			if (bakeThread != null)
			{
				bakeThread.Join();
			}
			if (bakeCallbackHandle.IsAllocated)
			{
				bakeCallbackHandle.Free();
			}
			if ((bool)duringBakePhononManager)
			{
				duringBakePhononManager.Destroy();
			}
			if (duringBakePhononContainer != null)
			{
				duringBakePhononContainer.Destroy();
			}
			duringBakePhononManager = null;
			duringBakeProbeBoxes = null;
			probeBoxBakingCurrently = 0;
			bakeStatus = BakeStatus.Ready;
			oneBakeActive = false;
		}

		public void CancelBake()
		{
			cancelBake = true;
			PhononCore.iplCancelBake();
			EndBake();
			oneBakeActive = false;
			cancelBake = false;
		}

		public bool IsBakeActive()
		{
			return oneBakeActive;
		}

		public BakeStatus GetBakeStatus()
		{
			return bakeStatus;
		}

		private void AdvanceProgress(float bakeProgressFraction)
		{
		}

		public void DrawProgressBar()
		{
		}
	}
}
