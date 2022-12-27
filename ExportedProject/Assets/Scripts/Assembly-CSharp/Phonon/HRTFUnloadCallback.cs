using System.Runtime.InteropServices;

namespace Phonon
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void HRTFUnloadCallback();
}
