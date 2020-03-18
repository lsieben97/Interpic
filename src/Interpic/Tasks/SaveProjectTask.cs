using Interpic.Models;
using Interpic.Studio.Functional;
using Newtonsoft.Json;
using System;
using System.IO;
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

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };
                if (Project.BehaviourConfiguration == null)
                {
                    Project.BehaviourConfiguration = new Models.Behaviours.BehaviourConfiguration();
                }

                Project.BehaviourConfiguration.BehavioursJson = JsonConvert.SerializeObject(Project.BehaviourConfiguration.Behaviours, settings);
                
                File.WriteAllText(Project.ProjectFolder + "\\Behaviours.dat", JsonConvert.SerializeObject(Project.BehaviourConfiguration, settings));
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks("Could not save web behaviour configuration:\n" + ex.Message);
            }
        }
    }
}
