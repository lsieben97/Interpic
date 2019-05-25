using Interpic.Alerts;
using Interpic.Models.Extensions;
using Interpic.Models;
using Interpic.Settings;
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
    /// Interaction logic for Build.xaml
    /// </summary>
    public partial class Build : Window
    {
        List<Models.Version> selectedVersions = new List<Models.Version>();
        SettingsCollection buildSettings;
        Project project;
        IProjectBuilder builder;
        public Build(IProjectBuilder builder, Project project)
        {
            InitializeComponent();

            this.builder = builder;

            LoadVersions(project);

            buildSettings = builder.GetBuildSettings(project);
            btnBuildSettings.IsEnabled = buildSettings != null;

            this.project = project;
        }

        private void LoadVersions(Project project, Models.Version startVersion = null)
        {
            foreach (Models.Version version in project.Versions)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = version.Name;
                item.Tag = version;
                if (startVersion != null && startVersion == version)
                {
                    item.IsSelected = true;
                }
                lsbVersions.Items.Add(item);
            }
        }

        public Build(IProjectBuilder builder, Project project, Models.Version version)
        {
            InitializeComponent();

            this.builder = builder;

            LoadVersions(project, version);

            selectedVersions.Add(version);
            cbBuildSpecificPages.IsChecked = true;

            buildSettings = builder.GetBuildSettings(project);
            btnBuildSettings.IsEnabled = buildSettings != null;

            this.project = project;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnBuild_Click(object sender, RoutedEventArgs e)
        {
            BuildOptions options = new BuildOptions();
            options.CleanOutputDirectory = cbCleanOutputDirectory.IsChecked.Value;
            options.BuildSettings = buildSettings;
            options.Type = cbBuildEntireProject.IsChecked.Value ? BuildOptions.BuildType.EntireManualAllVersions : BuildOptions.BuildType.EntireManualSpecificVersions;
            options.VersionsToBuild = new List<Models.Version>();
            if (cbBuildSpecificPages.IsChecked.Value)
            {
                
                foreach (ListBoxItem item in lsbVersions.SelectedItems)
                {
                    options.VersionsToBuild.Add(item.Tag as Models.Version);
                }
            }
            else
            {
                options.VersionsToBuild.AddRange(project.Versions);
            }
            if (cbCleanOutputDirectory.IsChecked.Value)
            {
                if (!builder.CleanOutputDirectory(project))
                {
                    return;
                }
            }
            btnBuild.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnBuildSettings.IsEnabled = false;
            lsbVersions.IsEnabled = false;
            cbBuildEntireProject.IsEnabled = false;
            cbBuildSpecificPages.IsEnabled = false;
            bool succes = true;
           
            foreach (Models.Version version in options.VersionsToBuild)
            {
                if (! builder.Build(options, project, version))
                {
                    succes = false;
                }
            }
            
            if (succes)
            {
                Close();
                new BuildCompleted(project.OutputFolder).ShowDialog();
            }
            else
            {
                btnBuild.IsEnabled = true;
                btnCancel.IsEnabled = true;
                btnBuildSettings.IsEnabled = true;
                lsbVersions.IsEnabled = true;
                cbBuildEntireProject.IsEnabled = true;
                cbBuildSpecificPages.IsEnabled = true;
            }
        }

        private void BtnBuildSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsEditor editor = new SettingsEditor(buildSettings);
            editor.ShowDialog();
            buildSettings = editor.SettingsCollection;
        }
    }
}
