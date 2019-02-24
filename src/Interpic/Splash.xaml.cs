using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Builders.MarkdownMKDocs;
using Interpic.Models;
using Interpic.Studio.Functional;
using Interpic.Studio.Tasks;
using Interpic.Studio.Windows;
using Interpic.Web;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Interpic.Studio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();
            
            // register base project types
            Studio.AvailableProjectTypes.Add(new WebProjectTypeProvider());

            Studio.AvailableBuilders.Add(new MarkDownMKDocsBuilder());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRecentProjects();
            new ProcessTaskDialog(new LoadGlobalSettingsTask()).ShowDialog();
            CheckWorkspace();
        }

        private void CheckWorkspace()
        {
            
            if (App.GlobalSettings.GetPathSetting("workspaceDirectory") == string.Empty)
            {
                InfoAlert.Show("Welcome to Interpic studio!\nPlease select a folder to use as your workspace. The workspace will contain all Interpic projects you create.");
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    App.GlobalSettings.PathSettings.Find(setting => setting.Key == "workspaceDirectory").Value = dialog.SelectedPath;
                    App.SaveGlobalSettings();
                }
                else
                {
                    CheckWorkspace();
                }
            }
        }

        private void LoadRecentProjects()
        {
            List<RecentProject> recentProjects = RecentProjects.GetRecentProjects();
            if (recentProjects.Count > 0)
            {
                lbsRecentProjects.ItemsSource = recentProjects;
            }
            else
            {
                btnOpenRecentProject.IsEnabled = false;
                RecentProject empty = new RecentProject();
                empty.Name = "No recent projects found";
                empty.Path = string.Empty;
                lbsRecentProjects.Items.Add(empty);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnNewProject_Click(object sender, RoutedEventArgs e)
        {
            NewProject dialog = new NewProject();
            dialog.ShowDialog();
            if (dialog.Project != null)
            {
                LoadProject(dialog.Project.Path, true);
            }
        }

        private void btnOpenRecentProject_Click(object sender, RoutedEventArgs e)
        {
            if (lbsRecentProjects.SelectedItem != null)
            {
                RecentProject recentProject = (RecentProject)lbsRecentProjects.SelectedItem;
                if (File.Exists(recentProject.Path))
                {
                    LoadProject(recentProject.Path);
                }
                else
                {
                    List<RecentProject> projects = RecentProjects.GetRecentProjects();
                    projects.Remove(recentProject);
                    RecentProjects.WriteRecentProjects(projects);
                    ErrorAlert.Show("Project doesn't exist");
                    LoadRecentProjects();
                }
            }
        }

        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select a location";
            dialog.Filter = "Interpic Project files (.ipp)|*.ipp";

            bool? result = dialog.ShowDialog();

            if (result.Value == true)
            {
                LoadProject(dialog.FileName);
            }
        }

        private void LoadProject(string path, bool isNew = false)
        {
            List<AsyncTask> tasks = new List<AsyncTask>();
            tasks.Add(new LoadGlobalSettingsTask());
            LoadProjectTask task = new LoadProjectTask(path);
            task.PassThrough = true;
            task.PassThroughSource = "Project";
            task.PassThroughTarget = "Project";
            tasks.Add(task);

            StartStudioTask startStudioTask = new StartStudioTask();
            tasks.Add(startStudioTask);

            ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks, "Loading Interpic Studio...");
            dialog.ShowDialog();
            if (!dialog.AllTasksCanceled)
            {
                task.Project.IsNew = isNew;
                RecentProjects.AddToRecents(task.Project.Name, task.Project.Path);
                startStudioTask.Studio.Show();
                Close();
            }
        }
    }
}
