using System.Runtime.InteropServices;

namespace Valve.VRRenderingPackage
{
	public struct VREvent_Process_t
	{
		public uint pid;

		public uint oldPid;

		[MarshalAs(UnmanagedType.I1)]
		public bool bForced;
	}
}
