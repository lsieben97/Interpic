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
