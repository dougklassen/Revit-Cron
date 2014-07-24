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

using DougKlassen.Revit.Cron.Models;

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
