﻿using Interpic.AsyncTasks;
using Interpic.Models;
using Interpic.Studio.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class LoadProjectTask : AsyncTask
    {
        public Project Project { get; set; }
        private string path;
        public LoadProjectTask(string path)
        {
            this.path = path;
            TaskName = "Loading project...";
            TaskDescription = path;
            ActionName = "Load project";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return new TaskFactory(scheduler).StartNew(() => { Run(); });
        }

        private void Run()
        {
            Project = Projects.LoadProject(path);
            if (Project == null)
            {
                Dialog.CancelAllTasks();
            }
        }
    }
}
