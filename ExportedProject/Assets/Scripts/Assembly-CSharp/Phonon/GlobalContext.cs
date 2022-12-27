using System;

namespace Phonon
{
	public struct GlobalContext
	{
		public IntPtr logCallback;

		public IntPtr allocateCallback;

		public IntPtr freeCallback;
	}
}
