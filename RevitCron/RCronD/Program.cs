using System.Windows.Forms;

namespace DougKlassen.Revit.Cron.Daemon
{
	class Program
	{
		static void Main(string[] args)
		{
			ApplicationContext appContext = new RCronDAppContext();
			Application.Run(appContext);
		}
	}
}
