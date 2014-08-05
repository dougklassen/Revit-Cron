using DougKlassen.Revit.Cron.Models;

using System.Windows;

namespace DougKlassen.Revit.Cron.Rotogravure.Interface
{
	/// <summary>
	/// Interaction logic for LogWindow.xaml
	/// </summary>
	public partial class LogWindow : Window
	{
		private LogWindow()
		{
			InitializeComponent();
		}

		public LogWindow(RCronLog log)
			: this()
		{
			LogViewTextBox.Text = log.Text;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		public static void Show(RCronLog log)
		{
			LogWindow logDlg = new LogWindow(log);
			logDlg.Show();
		}
	}
}
