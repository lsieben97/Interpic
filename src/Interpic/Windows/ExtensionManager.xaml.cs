using Interpic.Models;
using System.Windows;
using System.Windows.Controls;

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
            //if (project.ProjectExtensions == null && Functional.Extensions.GlobalExtensionsAvailable() == false)
            //{
            //    ExtensionDeclaration empty = new Models.ExtensionDeclaration();
            //    empty.Name = "No active extensions";
            //    lsbActiveExtensions.Items.Add(empty);
            //    btnRemove.IsEnabled = false;
            //}
            //else
            //{
            //    List<ExtensionDeclaration> activeExtensions = new List<ExtensionDeclaration>();
            //    if (project.ProjectExtensions != null)
            //    {
            //        activeExtensions.AddRange(project.ProjectExtensions);
            //    }

            //    if (Functional.Extensions.GlobalExtensionsAvailable() == true)
            //    {
            //        activeExtensions.AddRange(Functional.Extensions.GetGlobalExtensions());
            //    }

            //    lsbActiveExtensions.ItemsSource = activeExtensions;
            //}
        }

        private void lsbActiveExtensions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: display extension contents
        }
    }
}
