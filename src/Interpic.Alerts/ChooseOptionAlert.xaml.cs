using System.Windows;

namespace Interpic.Alerts
{
    /// <summary>
    /// Interaction logic for DoneAlert.xaml
    /// </summary>
    public partial class ChooseOptionAlert : Window
    {
        /// <summary>
        /// the option picked. 1 = option 1, 2 = option 2.
        /// </summary>
        public int OptionPicked { get; set; }


        public ChooseOptionAlert(string message, string option1, string option2)
        {
            InitializeComponent();
            lbMessage.Text = message;
            btnOption1.Content = option1;
            btnOption2.Content = option2;
        }

        public static ChooseOptionAlert Show(string message, string option1, string option2)
        {
            ChooseOptionAlert alert = new ChooseOptionAlert(message, option1, option2);
            alert.ShowDialog();
            return alert;
        }

        private void btnOption2_Click(object sender, RoutedEventArgs e)
        {
            OptionPicked = 2;
            Close();
        }

        private void btnOption1_Click(object sender, RoutedEventArgs e)
        {
            OptionPicked = 1;
            Close();
        }
    }
}
