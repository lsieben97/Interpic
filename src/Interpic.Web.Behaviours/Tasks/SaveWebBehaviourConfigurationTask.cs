using Interpic.Models;
using Interpic.Web.Behaviours.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Interpic.Web.Behaviours.Tasks
{
    public class SaveWebBehaviourConfigurationTask : AsyncTasks.AsyncTask
    {
        public Project Project {get; set;}
        public WebBehaviourConfiguration WebBehaviourConfiguration { get; set; }
        public SaveWebBehaviourConfigurationTask(Project project, WebBehaviourConfiguration configuration)
        {
            Project = project;
            WebBehaviourConfiguration = configuration;
            TaskName = "Saving web behaviour configuration...";
            TaskDescription = Project.ProjectFolder + "\\Webbehaviours.dat";
            ActionName = "Save web behaviour config";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };

                File.WriteAllText(Project.ProjectFolder + "\\Webbehaviours.dat", JsonConvert.SerializeObject(WebBehaviourConfiguration, settings));
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks("Could not save web behaviour configuration:\n" + ex.Message);
            }
        }
    }
}
