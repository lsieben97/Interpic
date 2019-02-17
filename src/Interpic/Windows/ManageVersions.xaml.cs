using Interpic.Alerts;
using Interpic.Extensions;
using Interpic.Models;
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
    /// Interaction logic for ManageVersions.xaml
    /// </summary>
    public partial class ManageVersions : Window
    {
        public Project Project { get; set; }
        private string selectedVersionId;
        private IProjectTypeProvider provider;
        public ManageVersions(Project project, IProjectTypeProvider provider)
        {
            InitializeComponent();
            LoadVersions(project);
            Project = project;
            this.provider = provider;
        }

        private void LoadVersions(Project project)
        {
            lsbVersions.Items.Clear();
            foreach(Models.Version version in project.Versions)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = version.Name;
                item.Tag = version.Id;
                lsbVersions.Items.Add(item);
            }
            lsbVersions.SelectedIndex = 0;
        }

        private void LsbVersions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lsbVersions.SelectedItem != null)
            {
                selectedVersionId = (lsbVersions.SelectedItem as ListBoxItem).Tag.ToString();
                btnEdit.IsEnabled = true;
                btnRemove.IsEnabled = true;
            }
            else
            {
                btnEdit.IsEnabled = false;
                btnRemove.IsEnabled = false;
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            NewVersion dialog = new NewVersion(provider.GetDefaultVersionSettings());
            if (dialog.ShowDialog().Value)
            {
                dialog.Version.Parent = Project;
                Project.Versions.Add(dialog.Version);
                LoadVersions(Project);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Models.Version selectedVersion = Project.Versions.Single(version => version.Id == selectedVersionId);
            NewVersion dialog = new NewVersion(provider.GetDefaultVersionSettings(), selectedVersion);
            if (dialog.ShowDialog().Value)
            {
                Project.Versions[Project.Versions.IndexOf(selectedVersion)] = dialog.Version;
                LoadVersions(Project);
            }
            
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if ( Project.Versions.Count() != 1)
            {
                Models.Version selectedVersion = Project.Versions.Single(version => version.Id == selectedVersionId);
                if (ConfirmAlert.Show("Version '" + selectedVersion.Name + "' Will be permenently removed together with all containing pages!").Result)
                {
                    Project.Versions.Remove(selectedVersion);
                    LoadVersions(Project);
                }
            }
            else
            {
                ErrorAlert.Show("A manual must have at least 1 version!");
            }
        }
    }
}
