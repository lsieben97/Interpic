using Interpic.Alerts;
using Interpic.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Interpic.Builders.MarkdownMKDocs.Tasks
{
    public class GenerateFolderStructureTask : AsyncTasks.AsyncTask
    {
        public Project Project { get; set; }
        public Interpic.Models.Version Version { get; set; }
        public BuildOptions Options { get; set; }
        public GenerateFolderStructureTask(BuildOptions options, Project project, Interpic.Models.Version version)
        {
            TaskName = "Generating directory structure...";
            TaskDescription = project.OutputFolder;
            ActionName = "Generate directory structure";
            IsIndeterminate = true;
            Project = project;
            Options = options;
            Version = version;
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
                Directory.CreateDirectory(Project.OutputFolder + Path.DirectorySeparatorChar + Version.Name + Path.DirectorySeparatorChar + Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.DOCS_DIRECTORY));
                Directory.CreateDirectory(Project.OutputFolder + Path.DirectorySeparatorChar + Version.Name + Path.DirectorySeparatorChar + Options.BuildSettings.GetSubSettings(Settings.CONFIGURATION_SETTINGS).GetTextSetting(Settings.ConfigurationSettings.SITE_DIRECTORY));
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not create directory structure.\n" + ex.Message);
                Dialog.CancelAllTasks();
            }
        }
    }
}
