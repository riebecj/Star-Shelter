using System;
using System.Collections.Generic;
using System.IO;

namespace ch.sycoforge.Logging
{
	public sealed class Logger
	{
		public delegate void LoggedEventHandler(LogEventArgs e);

		private static LoggedEventHandler _OnLogged;

		private static string logPath = string.Empty;

		public static List<string> LogMessages = new List<string>();

		public static bool LogToFile { get; set; }

		public static string Path
		{
			get
			{
				return logPath;
			}
			private set
			{
				logPath = value;
			}
		}

		public static event LoggedEventHandler OnLogged
		{
			add
			{
				_OnLogged = (LoggedEventHandler)Delegate.Combine(_OnLogged, value);
			}
			remove
			{
				_OnLogged = (LoggedEventHandler)Delegate.Remove(_OnLogged, value);
			}
		}

		public static void Initialize(string path)
		{
			logPath = path + string.Format("\\{0}_log.txt", DateTime.Now.ToString("u").Replace(':', '_'));
		}

		public static void LogFile(string lines, bool append = true)
		{
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(Path, append))
				{
					streamWriter.WriteLine(lines);
					streamWriter.Close();
				}
			}
			catch
			{
			}
		}

		public static void Log(string message)
		{
			try
			{
				LogMessages.Add(message);
				if (LogToFile)
				{
					LogFile(message + "\n\r");
				}
				FireOnLogged(message);
			}
			catch
			{
			}
		}

		private static void FireOnLogged(string message)
		{
			if (_OnLogged != null)
			{
				LogEventArgs e = new LogEventArgs(message);
				_OnLogged(e);
			}
		}
	}
}
