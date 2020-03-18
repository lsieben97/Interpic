using Interpic.Models.Extensions;
using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Interpic.AsyncTasks;
using Interpic.Studio.Tasks;
using Interpic.Alerts;

namespace Interpic.Studio.Windows
{
    /// <summary>
    /// Interaction logic for NewProject.xaml
    /// </summary>
    public partial class NewProject : Window
    {
        private Project project;

        public Project Project { get => project; set => project = value; }

        private string projectPath = "";
        private string projectOutputFolder = "";
        public NewProject()
        {
            InitializeComponent();
            LoadProjectTypes();
            LoadBuilders();
        }

        private void LoadBuilders()
        {
            foreach(IProjectBuilder builder in Studio.AvailableBuilders.FindAll((build) => build.GetCompatibleProjectTypes() == null))
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = builder.GetBuilderName();
                item.Tag = builder;
                cbbOutputType.Items.Add(item);
            }
        }

        private void LoadProjectTypes()
        {
            foreach (IProjectTypeProvider provider in Studio.AvailableProjectTypes)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = provider.GetProjectTypeName();
                item.Tag = provider;
                cbbApplicationType.Items.Add(item);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            if (tbName.Text == string.Empty)
            {
                valid = false;
                lbNameError.Text = "Enter name";
            }
            else
            {
                lbNameError.Text = string.Empty;
            }

            if (cbbApplicationType.SelectedIndex == -1)
            {
                valid = false;
                lbApplicationTypeError.Text = "Enter application type";
            }
            else
            {
                lbApplicationTypeError.Text = string.Empty;
            }

            if (cbbOutputType.SelectedIndex == -1)
            {
                valid = false;
                lbOutputTypeError.Text = "Enter output type";
            }
            else
            {
                lbOutputTypeError.Text = string.Empty;
            }

            if (valid)
            {
                string directoryName = tbName.Text;
                foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                {
                    directoryName = directoryName.Replace(c.ToString(), "");
                }
                string fullDirectory = System.IO.Path.Combine(App.GlobalSettings.GetPathSetting("workspaceDirectory"), directoryName);
                if (System.IO.Directory.Exists(fullDirectory))
                {
                    WarningAlert.Show($"The folder {fullDirectory} already exists.\nPlease choose another project name.");
                    return;
                }
                project = new Project();
                project.Name = tbName.Text;
                IProjectTypeProvider provider = ((ComboBoxItem)cbbApplicationType.SelectedValue).Tag as IProjectTypeProvider;
                project.TypeProviderId = provider.GetProjectTypeId();
                IProjectBuilder builder = ((ComboBoxItem)cbbOutputType.SelectedValue).Tag as IProjectBuilder;
                project.OutputType = builder.GetBuilderId();
                project.Path = projectPath;
                
                project.LastSaved = DateTime.Now;
                project.IsNew = true;
                project.OutputFolder = System.IO.Path.Combine(App.GlobalSettings.GetPathSetting("workspaceDirectory"), directoryName, "\\Output\\");
                project.ProjectFolder = fullDirectory;
                project.Path = System.IO.Path.Combine(project.ProjectFolder, directoryName + ".ipp");
                NewVersion newVersionDialog = new NewVersion(provider.GetDefaultVersionSettings());
                newVersionDialog.ShowDialog();
                if (newVersionDialog.DialogResult.Value)
                {
                    project.Versions = new ObservableCollection<Models.Version>();
                    project.Versions.Add(newVersionDialog.Version);
                    List<AsyncTask> tasks = new List<AsyncTask>();
                    tasks.Add(new CreateProjectFilesTask(project));
                    tasks.Add(new SaveProjectTask(project));
                    ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks);
                    dialog.ShowDialog();
                    if (!dialog.AllTasksCanceled)
                    {
                        Close();
                    }
                }
            }
        }

        private void cbbApplicationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IProjectTypeProvider provider = ((ComboBoxItem)cbbApplicationType.SelectedValue).Tag as IProjectTypeProvider;
            tbProjectTypeDescription.Text = provider.GetProjectTypeDescription();
        }

        private void RefreshBuilders(List<string> compatibleBuilders)
        {
            if (compatibleBuilders == null)
            {
                compatibleBuilders = new List<string>();
            }
            cbbOutputType.Items.Clear();
            foreach (IProjectBuilder builder in Studio.AvailableBuilders.FindAll((build) => build.GetCompatibleProjectTypes() == null || compatibleBuilders.Contains(build.GetBuilderId())))
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = builder.GetBuilderName();
                item.Tag = builder;
                cbbOutputType.Items.Add(item);
            }
        }

        private void btnFromExtension_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Title = "Select an extension";
            //dialog.DefaultExt = ".dll";
            //dialog.Filter = "Interpic extension files (.dll)|*.dll";

            //bool? result = dialog.ShowDialog();

            //if (result.Value == true)
            //{
            //    List<Extensions.Extension> extensions = Functional.Extensions.GetExtensionsFromDll(dialog.FileName);
            //    foreach (Extensions.Extension extension in extensions)
            //    {
            //        if (extension.GetProjectTypeProvider() != null)
            //        {
            //            ComboBoxItem item = new ComboBoxItem();
            //            IProjectTypeProvider provider = extension.GetProjectTypeProvider();
            //            item.Content = provider.GetProjectTypeName();
            //            item.Tag = provider;
            //            cbbApplicationType.Items.Add(item);
            //        }
            //    }
            //}
        }

        private void CbbOutputType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IProjectBuilder builder = ((ComboBoxItem)cbbOutputType.SelectedValue).Tag as IProjectBuilder;
            tbOutputTypeDescription.Text = builder.GetBuilderDescription();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Project = null;
            Close();
        }
    }
}
