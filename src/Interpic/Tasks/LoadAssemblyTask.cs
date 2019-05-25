using Interpic.AsyncTasks;
using Interpic.Models;
using Interpic.Studio.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class LoadAssemblyTask : AsyncTask
    {
        public string Path { get; set; }
        public LoadedAssembly LoadedAssembly { get; set; }
        public Extension RequestingExtension { get; set; }
        public LoadAssemblyTask(string path, Extension requestingExtension)
        {
            Path = path;
            RequestingExtension = requestingExtension;
            TaskName = "Loading assembly...";
            TaskDescription = path + "\n\nRequested by " + requestingExtension.GetName();
            ActionName = "Load assembly";
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
                Assembly assembly = SafeExtensionManager.CreateAppDomainForAssembly(Path).Load(Path);
                LoadedAssembly = new LoadedAssembly();
                LoadedAssembly.Assembly = assembly;
                LoadedAssembly.Path = Path;
                LoadedAssembly.RequestingExtension = RequestingExtension;
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks("Could not load assembly:\n" + ex.Message);
            }
        }
    }
}
