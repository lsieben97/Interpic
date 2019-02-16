using Interpic.Models;
using Interpic.Studio.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class SaveProjectTask : AsyncTasks.AsyncTask
    {
        public Project Project { get; set; }
        public SaveProjectTask(Project project)
        {
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
            bool result = Projects.SaveProject(Project);
            if (result == false)
            {
                Dialog.CancelAllTasks();
            }
        }
    }
}
