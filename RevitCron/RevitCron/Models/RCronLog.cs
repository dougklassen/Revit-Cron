using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace DougKlassen.Revit.Cron.Models
{
	public sealed class RCronLog
	{
		private static readonly RCronLog instance = new RCronLog(); //singleton static initializer

		private StringBuilder logText = new StringBuilder(String.Empty);

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

		public RCronLog(String text)
		{
			logText = new StringBuilder(text);
		}

		public void AppendLine()
		{
			logText.Append('\n');
		}

		public void AppendLine(String text)
		{
			logText.AppendLine(text);
		}

		public void AppendLine(String text, params object[] args)
		{
			logText.AppendLine(String.Format(text, args.Select(o => o.ToString()).ToArray()));
		}

		public void LogException(Exception exception)
		{
			AppendLine("\n!! {0} {1}", DateTime.Now, exception.GetType());
			AppendLine("  {0}", exception.Message);
			AppendLine("  {0}", exception.StackTrace);
		}

		public void LogThreadInfo()
		{
			Thread currThrd = Thread.CurrentThread;
			AppendLine("!! current thread: {0}", currThrd.Name);
			AppendLine("!! Id: {0}", currThrd.ManagedThreadId);
			AppendLine("!! app domain: {0}", AppDomain.CurrentDomain.FriendlyName);
		}
	}
}