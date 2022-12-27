using System;
using System.Runtime.InteropServices;

namespace Phonon
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void HRTFLoadCallback(int signalSize, int spectrumSize, FFTHelper fftHelper, IntPtr data);
}
