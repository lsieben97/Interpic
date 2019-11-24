using System.Windows;

namespace Interpic.Alerts
{
    /// <summary>
    /// Interaction logic for ErrorAlert.xaml
    /// </summary>
    public partial class ErrorAlert : Window
    {
        public ErrorAlert(string message, bool showCancelButton = false)
        {
            InitializeComponent();
            lbMessage.Text = message;
            btnCancel.Visibility = showCancelButton ? Visibility.Visible : Visibility.Hidden;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
            DialogResult = false;
        }

        public static ErrorAlert Show(string message)
        {
            ErrorAlert alert = new ErrorAlert(message);
            alert.ShowDialog();
            return alert;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
