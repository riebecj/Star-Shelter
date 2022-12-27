using System;

namespace Phonon
{
	public struct AudioBuffer
	{
		public AudioFormat audioFormat;

		public int numSamples;

		public float[] interleavedBuffer;

		public IntPtr[] deInterleavedBuffer;
	}
}
