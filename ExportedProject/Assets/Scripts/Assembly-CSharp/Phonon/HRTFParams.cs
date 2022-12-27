using System;

namespace Phonon
{
	public struct HRTFParams
	{
		public HRTFDatabaseType type;

		public IntPtr hrtfData;

		public int numHrirSamples;

		public HRTFLoadCallback loadCallback;

		public HRTFUnloadCallback unloadCallback;

		public HRTFLookupCallback lookupCallback;
	}
}
