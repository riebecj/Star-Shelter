using System.Runtime.InteropServices;

namespace Valve.VRRenderingPackage
{
	public struct VREvent_Keyboard_t
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
		public string cNewInput;

		public ulong uUserValue;
	}
}
