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
    /// Interaction logic for DoneAlert.xaml
    /// </summary>
    public partial class DoneAlert : Window
    {
        public bool Result { get; set; }

        public DoneAlert(string message)
        {
            InitializeComponent();
            lbMessage.Text = message;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        public static DoneAlert Show(string message)
        {
            DoneAlert alert = new DoneAlert(message);
            alert.ShowDialog();
            return alert;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }
    }
}
