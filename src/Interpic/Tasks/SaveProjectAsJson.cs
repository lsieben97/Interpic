using Interpic.Models;
using Interpic.Studio.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class SaveProjectAsJsonTask : AsyncTasks.AsyncTask
    {
        public Project Project { get; set; }
        private string path;
        public SaveProjectAsJsonTask(Project project, string path)
        {
            this.path = path;
            Project = project;
            TaskName = "Exporting project to JSON...";
            TaskDescription = project.Path;
            ActionName = "Export project to JSON";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return new TaskFactory(scheduler).StartNew(() => { Run(); });
        }

        private void Run()
        {
            bool result = Projects.SaveAsJson(Project, path);
            if (result == false)
            {
                Dialog.CancelAllTasks();
            }
            Dialog.CancelAllTasks();
        }
    }
}
