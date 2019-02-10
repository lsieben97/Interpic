using Interpic.Models;
using System;
using System.Collections.Generic;
using Interpic.Studio.Functional;
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
    /// Interaction logic for ExtensionManager.xaml
    /// </summary>
    public partial class ExtensionManager : Window
    {
        private Project project;
        public ExtensionManager(ref Project project)
        {
            this.project = project;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadExtensions();
        }

        private void LoadExtensions()
        {
            if (project.ProjectExtensions == null && Functional.Extensions.GlobalExtensionsAvailable() == false)
            {
                Extension empty = new Models.Extension();
                empty.Name = "No active extensions";
                lsbActiveExtensions.Items.Add(empty);
                btnRemove.IsEnabled = false;
            }
            else
            {
                List<Extension> activeExtensions = new List<Extension>();
                if (project.ProjectExtensions != null)
                {
                    activeExtensions.AddRange(project.ProjectExtensions);
                }

                if (Functional.Extensions.GlobalExtensionsAvailable() == true)
                {
                    activeExtensions.AddRange(Functional.Extensions.GetGlobalExtensions());
                }

                lsbActiveExtensions.ItemsSource = activeExtensions;
            }
        }

        private void lsbActiveExtensions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: display extension contents
        }
    }
}
