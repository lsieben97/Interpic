using Interpic.Alerts;
using Interpic.Models.Extensions;
using Interpic.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Interpic.Studio.Windows;
using Interpic.UI.Controls;

namespace Interpic.Studio.StudioViews
{
    /// <summary>
    /// Interaction logic for ManageVersions.xaml
    /// </summary>
    public partial class ManageVersionsView : UserControl, IStudioViewHandler
    {
        public Project Project { get; set; }
        public IStudioEnvironment Studio { get; set; }
        public IStudioTab StudioTab { get; set; }
        private string selectedVersionId;
        private IProjectTypeProvider provider;

        public ManageVersionsView(Project project, IProjectTypeProvider provider, IStudioEnvironment studio)
        {
            InitializeComponent();
            Project = project;
            this.provider = provider;
        }

        private void LoadVersions()
        {
            lsbVersions.Items.Clear();
            foreach(Models.Version version in Project.Versions)
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
            Studio.AddVersion(true);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Models.Version selectedVersion = Project.Versions.Single(version => version.Id == selectedVersionId);
            NewVersion dialog = new NewVersion(provider.GetDefaultVersionSettings(), selectedVersion);
            if (dialog.ShowDialog().Value)
            {
                Project.Versions[Project.Versions.IndexOf(selectedVersion)] = dialog.Version;
                LoadVersions();
            }
            
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if ( Project.Versions.Count() != 1)
            {
                Models.Version selectedVersion = Project.Versions.Single(version => version.Id == selectedVersionId);
                Studio.RemoveManualItem(selectedVersion, true);
            }
            else
            {
                ErrorAlert.Show("A manual must have at least 1 version!");
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadVersions();
            Project.Versions.CollectionChanged += VersionsCollectionChanged;
        }

        private void VersionsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LoadVersions();
        }

        public void ViewAttached() { }

        public void ViewDetached() {
            Project.Versions.CollectionChanged -= VersionsCollectionChanged;
        }

        public string GetTabContents()
        {
            return "ManageVersions";
        }
    }
}
