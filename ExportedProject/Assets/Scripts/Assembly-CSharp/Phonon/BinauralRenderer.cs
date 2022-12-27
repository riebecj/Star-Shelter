using System;

namespace Phonon
{
	public class BinauralRenderer
	{
		private IntPtr binauralRenderer = IntPtr.Zero;

		public void Create(Environment environment, RenderingSettings renderingSettings, GlobalContext globalContext)
		{
			HRTFParams hRTFParams = default(HRTFParams);
			hRTFParams.type = HRTFDatabaseType.Default;
			hRTFParams.hrtfData = IntPtr.Zero;
			hRTFParams.numHrirSamples = 0;
			hRTFParams.loadCallback = null;
			hRTFParams.unloadCallback = null;
			hRTFParams.lookupCallback = null;
			HRTFParams hrtfParams = hRTFParams;
			Error error = PhononCore.iplCreateBinauralRenderer(globalContext, renderingSettings, hrtfParams, ref binauralRenderer);
			if (error != 0)
			{
				throw new Exception("Unable to create binaural renderer [" + error.ToString() + "]");
			}
		}

		public IntPtr GetBinauralRenderer()
		{
			return binauralRenderer;
		}

		public void Destroy()
		{
			if (binauralRenderer != IntPtr.Zero)
			{
				PhononCore.iplDestroyBinauralRenderer(ref binauralRenderer);
			}
		}
	}
}
