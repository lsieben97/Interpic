using System.Windows;

namespace Interpic.Alerts
{
    /// <summary>
    /// Interaction logic for ErrorAlert.xaml
    /// </summary>
    public partial class ConfirmAlert : Window
    {
        public bool Result { get; set; }

        public ConfirmAlert(string message, bool showCancelButton = true)
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

        public static ConfirmAlert Show(string message, bool showCancelButton = true)
        {
            ConfirmAlert alert = new ConfirmAlert(message, showCancelButton);
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
