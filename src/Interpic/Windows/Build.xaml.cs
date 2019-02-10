using Interpic.Alerts;
using Interpic.Extensions;
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
        List<Models.Page> selectedPages = new List<Models.Page>();
        SettingsCollection buildSettings;
        Project project;
        IProjectBuilder builder;
        public Build(IProjectBuilder builder, Project project)
        {
            InitializeComponent();

            this.builder = builder;

            LoadPages(project);

            buildSettings = builder.GetBuildSettings(project);
            btnBuildSettings.IsEnabled = buildSettings != null;

            this.project = project;
        }

        private void LoadPages(Project project, Models.Page startPage = null)
        {
            foreach (Models.Page page in project.Pages)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = page.Name;
                item.Tag = page;
                if (startPage != null && startPage == page)
                {
                    item.IsSelected = true;
                }
                lsbPages.Items.Add(item);
            }
        }

        public Build(IProjectBuilder builder, Project project, Models.Page page)
        {
            InitializeComponent();

            this.builder = builder;

            LoadPages(project, page);

            selectedPages.Add(page);
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
            options.Type = cbBuildEntireProject.IsChecked.Value ? BuildOptions.BuildType.EntireManual : BuildOptions.BuildType.SpecificPages;
            if (cbBuildSpecificPages.IsChecked.Value)
            {
                options.PagesToBuild = new List<Models.Page>();
                foreach (ListBoxItem item in lsbPages.SelectedItems)
                {
                    options.PagesToBuild.Add(item.Tag as Models.Page);
                }
            }
            btnBuild.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnBuildSettings.IsEnabled = false;
            lsbPages.IsEnabled = false;
            cbBuildEntireProject.IsEnabled = false;
            cbBuildSpecificPages.IsEnabled = false;
            if (builder.Build(options, project))
            {
                Close();
                new BuildCompleted(project.OutputFolder).ShowDialog();
            }
            else
            {
                btnBuild.IsEnabled = true;
                btnCancel.IsEnabled = true;
                btnBuildSettings.IsEnabled = true;
                lsbPages.IsEnabled = true;
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
