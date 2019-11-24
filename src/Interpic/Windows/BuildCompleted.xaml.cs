using System.Windows;

namespace Interpic.Studio.Windows
{
    /// <summary>
    /// Interaction logic for BuildCompleted.xaml
    /// </summary>
    public partial class BuildCompleted : Window
    {
        string outputFolder;
        public BuildCompleted(string outputFolder)
        {
            InitializeComponent();
            this.outputFolder = outputFolder;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOpenOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = outputFolder,
                UseShellExecute = true,
                Verb = "open"
            });
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
