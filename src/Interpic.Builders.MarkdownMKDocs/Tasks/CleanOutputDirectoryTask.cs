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
    public class CleanOutputDirectoryTask : AsyncTasks.AsyncTask
    {
        public Project Project { get; set; }
        public CleanOutputDirectoryTask(Project project)
        {
            TaskName = "Cleaning output directory...";
            TaskDescription = project.OutputFolder;
            ActionName = "Clean output directory";
            IsIndeterminate = true;
            Project = project;
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
                DirectoryInfo directoryInfo = new DirectoryInfo(Project.OutputFolder);
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (Exception ex)
            {
                ErrorAlert error = new ErrorAlert("Could not clean directory.\n" + ex.Message + "\n\nPres OK to continue anyway.\nPress Cancel to cancel the build.", true);
                error.ShowDialog();
                if (error.DialogResult.Value == false)
                {
                    Dialog.CancelAllTasks();
                }
            }
            
            
        }
    }
}
