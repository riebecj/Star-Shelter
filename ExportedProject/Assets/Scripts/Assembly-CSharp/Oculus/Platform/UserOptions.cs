using System;

namespace Oculus.Platform
{
	public class UserOptions
	{
		private IntPtr Handle;

		public UserOptions()
		{
			Handle = CAPI.ovr_UserOptions_Create();
		}

		public void SetMaxUsers(uint value)
		{
			CAPI.ovr_UserOptions_SetMaxUsers(Handle, value);
		}

		public void SetTimeWindow(TimeWindow value)
		{
			CAPI.ovr_UserOptions_SetTimeWindow(Handle, value);
		}

		public static explicit operator IntPtr(UserOptions options)
		{
			return (options == null) ? IntPtr.Zero : options.Handle;
		}

		~UserOptions()
		{
			CAPI.ovr_UserOptions_Destroy(Handle);
		}
	}
}
