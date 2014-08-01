using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DougKlassen.Revit.Cron.RCronD
{
    class RCronService : System.ServiceProcess.ServiceBase
    {
        public RCronService()
        {
            this.ServiceName = "RCronD";
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        public static void Main()
        {
            System.ServiceProcess.ServiceBase.Run(new RCronService());
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
