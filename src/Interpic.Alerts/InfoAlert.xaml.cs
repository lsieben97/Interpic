using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Interpic.Alerts
{
    /// <summary>
    /// Interaction logic for ErrorAlert.xaml
    /// </summary>
    public partial class InfoAlert : Window
    {
        public bool Result { get; set; }

        public InfoAlert(string message)
        {
            InitializeComponent();
            lbMessage.Text = message;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        public static InfoAlert Show(string message)
        {
            InfoAlert alert = new InfoAlert(message);
            alert.ShowDialog();
            return alert;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }
    }
}
