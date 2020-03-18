using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Models;
using Interpic.Models.Behaviours;
using Interpic.Studio.Functional;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class LoadBehavioursTask : AsyncTask
    {
        public Project Project { get; set; }
        private readonly Studio studio;

        public LoadBehavioursTask(Studio studio, Project project)
        {
            this.studio = studio;
            Project = project;
            TaskName = "Loading behaviours...";
            TaskDescription = studio.CurrentProject.ProjectFolder + "\\Behaviours.dat";
            ActionName = "Load behaviours";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return new TaskFactory(scheduler).StartNew(() => { Run(); });
        }

        private void Run()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            try
            {
                BehaviourConfiguration configuration = JsonConvert.DeserializeObject<BehaviourConfiguration>(File.ReadAllText(Project.ProjectFolder + "\\Behaviours.dat"), settings);
                Project.BehaviourConfiguration = configuration;
                List<ActionPack> buildInActionPacks = studio.ProjectTypeProvider.GetBuildInActionPacks();
                if (buildInActionPacks != null)
                {
                    Project.BehaviourConfiguration.InternalWebActionPacks.AddRange(buildInActionPacks);
                }
                
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks("Could not load web behaviour configuration:\n" + ex.Message);
            }

            List<string> assembliesToLoad = Project.BehaviourConfiguration.WebActionpacks.FindAll(pack => pack != "BasicWebActionsPack");
            if (assembliesToLoad.Count > 0)
            {

                List<LoadedAssembly> loadedAssemblies = studio.GetDLLManager().LoadAssemblies(assembliesToLoad, Studio.extensionRepresentingStudio);
                foreach (LoadedAssembly assembly in loadedAssemblies)
                {
                    try
                    {
                        Type packType = assembly.Assembly.GetExportedTypes().First(ass => ass.BaseType.Name == "WebActionPack");
                        ActionPack pack = Activator.CreateInstance(packType) as ActionPack;
                        Project.BehaviourConfiguration.InternalWebActionPacks.Add(pack);
                    }
                    catch (Exception ex)
                    {
                        Dialog.CancelAllTasks($"Could not load web action pack from assembly {assembly.Path}:\n\n{ex.Message}");
                    }
                }
            }
            Project.BehaviourConfiguration.Behaviours = JsonConvert.DeserializeObject<List<Behaviour>>(Project.BehaviourConfiguration.BehavioursJson, settings);
        }
    }
}
