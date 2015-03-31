using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace DougKlassen.Revit.Cron.Models
{
	/// <summary>
	/// A class modelling a log for RevitCron events
	/// </summary>
	public sealed class RCronLog
	{
		private static readonly RCronLog instance = new RCronLog(); //singleton static initializer

		private StringBuilder logText = new StringBuilder(String.Empty);

		/// <summary>
		/// The public instance of the singleton instance
		/// </summary>
		public static RCronLog Instance
		{
			get
			{
				return instance;
			}
		}

		private RCronLog()
		{
			logText = new StringBuilder(String.Empty);
		}

		/// <summary>
		/// The texts contents of the log
		/// </summary>
		public String Text
		{
			get
			{
				return logText.ToString();
			}
			set
			{
				logText = new StringBuilder(value);
			}
		}

		/// <summary>
		/// Create a new instance of RCronLog
		/// </summary>
		/// <param name="text"></param>
		public RCronLog(String text)
		{
			logText = new StringBuilder(text);
		}

		/// <summary>
		/// Add a blank line to the log
		/// </summary>
		public void AppendLine()
		{
			logText.Append('\n');
		}

		/// <summary>
		/// Add a line of text to the log
		/// </summary>
		/// <param name="text">The text to be added to the log</param>
		public void AppendLine(String text)
		{
			logText.AppendLine(text);
		}

		/// <summary>
		/// Add a line of text to the log
		/// </summary>
		/// <param name="text">The text to be added to the log with place holder elements</param>
		/// <param name="args">Variables to be written into the text string</param>
		public void AppendLine(String text, params object[] args)
		{
			logText.AppendLine(String.Format(text, args.Select(o => o.ToString()).ToArray()));
		}

		/// <summary>
		/// Write information about an exception to the log
		/// </summary>
		/// <param name="exception">The exception to record</param>
		public void LogException(Exception exception)
		{
			AppendLine("\n!! {0} {1}", DateTime.Now, exception.GetType());
			AppendLine("  {0}", exception.Message);
			AppendLine("  {0}", exception.StackTrace);
		}

		/// <summary>
		/// Record information about the current thread to the log
		/// </summary>
		public void LogThreadInfo()
		{
			Thread currThrd = Thread.CurrentThread;
			AppendLine("  ** current thread: {0}", currThrd.Name);
			AppendLine("  -- Id: {0}", currThrd.ManagedThreadId);
			AppendLine("  -- app domain: {0}", AppDomain.CurrentDomain.FriendlyName);
		}
	}
}