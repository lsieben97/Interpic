using Interpic.Models;
using Interpic.Studio.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class SaveAsNewProjectTask : AsyncTasks.AsyncTask
    {
        public Project Project { get; set; }
        private string path;
        public SaveAsNewProjectTask(Project project, string path)
        {
            this.path = path;
            Project = project;
            TaskName = "Saving project...";
            TaskDescription = project.Path;
            ActionName = "Save project";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return new TaskFactory(scheduler).StartNew(() => { Run(); });
        }

        private void Run()
        {
            bool result = Projects.SaveAsNewProject(Project, path);
            if (result == false)
            {
                Dialog.CancelAllTasks();
            }
        }
    }
}
