using System.Windows;

namespace Interpic.Alerts
{
    /// <summary>
    /// Interaction logic for ErrorAlert.xaml
    /// </summary>
    public partial class WarningAlert : Window
    {
        public bool Result { get; set; }

        public WarningAlert(string message, bool showCancelButton = false)
        {
            InitializeComponent();
            lbMessage.Text = message;
            btnCancel.Visibility = showCancelButton ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        public static WarningAlert Show(string message, bool showCancelButton = false)
        {
            WarningAlert alert = new WarningAlert(message, showCancelButton);
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
