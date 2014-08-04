using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DougKlassen.Revit.Cron.Daemon
{
	public class RCronD : System.ServiceProcess.ServiceBase
	{
		public RCronD()
		{
			this.ServiceName = "RCronD";
			this.CanStop = true;
			this.CanPauseAndContinue = true;
			this.AutoLog = true;
		}

		public static void Main()
		{
			System.ServiceProcess.ServiceBase.Run(new RCronD());
		}

		protected override void OnStart(string[] args)
		{
			base.OnStart(args);
		}

		protected override void OnStop()
		{
			base.OnStop();
		}
	}
}
