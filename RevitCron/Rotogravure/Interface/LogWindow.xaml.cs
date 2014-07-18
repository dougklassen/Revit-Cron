using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        public LogWindow(String log)
            : this()
        {
            LogViewTextBox.Text = log;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static void Show(String logString)
        {
            LogWindow logDlg = new LogWindow(logString);
            logDlg.Show();
        }
    }
}
