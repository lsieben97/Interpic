using Interpic.Models;
using Interpic.Web.Behaviours.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Behaviours.Tasks
{
    public class LoadWebBehaviourConfigurationTask : AsyncTasks.AsyncTask
    {
        public Project Project {get; set;}
        public WebBehaviourConfiguration WebBehaviourConfiguration { get; set; }
        public LoadWebBehaviourConfigurationTask(Project project)
        {
            Project = project;
            TaskName = "Loading web behaviour configuration...";
            TaskDescription = Project.ProjectFolder + "\\Webbehaviours.dat";
            ActionName = "Load web behaviour config";
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
                WebBehaviourConfiguration = JsonConvert.DeserializeObject<WebBehaviourConfiguration>(File.ReadAllText(Project.ProjectFolder + "\\Webbehaviours.dat"), settings);
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks("Could not load web behaviour configuration:\n" + ex.Message);
            }
        }
    }
}
