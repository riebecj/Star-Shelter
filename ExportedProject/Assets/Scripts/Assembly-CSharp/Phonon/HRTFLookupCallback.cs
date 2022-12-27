using System;
using System.Runtime.InteropServices;

namespace Phonon
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void HRTFLookupCallback(IntPtr direction, IntPtr leftHrtf, IntPtr rightHrtf);
}
