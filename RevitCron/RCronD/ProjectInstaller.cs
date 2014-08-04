using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace DougKlassen.Revit.Cron.Daemon
{
	[RunInstaller(true)]
	public class RCronDInstaller : Installer
	{
		private ServiceInstaller rCronDServiceInstaller;
		private ServiceProcessInstaller processInstaller;

		public RCronDInstaller()
		{
			processInstaller = new ServiceProcessInstaller();
			rCronDServiceInstaller = new ServiceInstaller();

			processInstaller.Account = ServiceAccount.LocalSystem;

			rCronDServiceInstaller.StartType = ServiceStartMode.Manual;
			rCronDServiceInstaller.ServiceName = "RCronD";

			Installers.Add(rCronDServiceInstaller);
			Installers.Add(processInstaller);
		}

		//public static void Main()
		//{
		//	Console.WriteLine("Usage: InstallUtil.exe rcrond.exe");
		//}
	}
}
