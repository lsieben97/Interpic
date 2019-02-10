﻿using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Models;
using Interpic.Studio.Functional;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    class CreateProjectFilesTask : AsyncTask
    {
        public Project Project { get; set; }
        private string path;
        public CreateProjectFilesTask(Project project)
        {
            this.Project = project;
            TaskName = "Creating project files...";
            TaskDescription = "project folder, output folder and project definition.";
            ActionName = "Create project files";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => Run());
        }

        private void Run()
        {
            try
            {
                Directory.CreateDirectory(Project.ProjectFolder);
                Directory.CreateDirectory(Project.OutputFolder);
                if (!Projects.SaveProject(Project))
                {
                    // Throw exception to let the task fail.
                    throw new Exception();
                }
            }
            catch(Exception ex)
            {
                ErrorAlert dialog = ErrorAlert.Show("Could not create project files:\n" + ex.Message + "\n\nClick on 'OK' to retry");
               if (dialog.DialogResult.HasValue)
                {
                    if (dialog.DialogResult.Value)
                    {
                        Run();
                    }
                    else
                    {
                        IsCanceled = true;
                        Dialog.CancelAllTasks();
                    }
                }
            } 
        }
    }
}
