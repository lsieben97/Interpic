using Interpic.AsyncTasks;
using Interpic.Models;
using Interpic.Studio.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    class LoadProjectTask : AsyncTask
    {
        public Project Project { get; set; }
        private string path;
        public LoadProjectTask(string path)
        {
            this.path = path;
            TaskName = "Loading project.";
            TaskDescription = path;
            ActionName = "Load project";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => Run());
        }

        private void Run()
        {
            Project = Projects.LoadProject(path);
        }
    }
}
