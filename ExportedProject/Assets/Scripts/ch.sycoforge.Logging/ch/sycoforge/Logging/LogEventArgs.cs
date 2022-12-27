using System;

namespace ch.sycoforge.Logging
{
	public class LogEventArgs : EventArgs
	{
		public string Message;

		public DateTime LogTime;

		public LogEventArgs(string message)
		{
			Message = message;
			LogTime = DateTime.Now;
		}
	}
}
