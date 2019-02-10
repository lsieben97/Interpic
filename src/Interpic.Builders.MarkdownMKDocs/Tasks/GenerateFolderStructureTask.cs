using Interpic.Alerts;
using Interpic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Builders.MarkdownMKDocs.Tasks
{
    public class GenerateFolderStructureTask : AsyncTasks.AsyncTask
    {
        public Project Project { get; set; }
        public BuildOptions Options { get; set; }
        public GenerateFolderStructureTask(BuildOptions options, Project project)
        {
            TaskName = "Generating directory structure...";
            TaskDescription = project.OutputFolder;
            ActionName = "Generate directory structure";
            IsIndeterminate = true;
            Project = project;
            Options = options;
        }
        public override Task Execute()
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return new TaskFactory(scheduler).StartNew(() => { Run(); });
        }

        private void Run()
        {
            try
            {
                Directory.CreateDirectory(Project.OutputFolder + Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.DOCS_DIRECTORY));
                Directory.CreateDirectory(Project.OutputFolder + Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.SITE_DIRECTORY));
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not create directory structure.\n" + ex.Message);
                Dialog.CancelAllTasks();
            }
        }
    }
}
