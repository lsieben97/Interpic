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
    internal class UnloadAssemblyTask : AsyncTask
    {
        public LoadedAssembly LoadedAssembly { get; set; }
        public UnloadAssemblyTask(LoadedAssembly loadedAssembly)
        {
            TaskName = "Unloading assembly...";
            TaskDescription = loadedAssembly.Path + "\n\nRequested by " + loadedAssembly.RequestingExtension.GetName();
            ActionName = "Unload assembly";
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
                if (! SafeExtensionManager.UnloadAppdomain(LoadedAssembly.Path))
                {
                    Dialog.CancelAllTasks("Could not unload assembly.");
                }
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks("Could not unload assembly:\n" + ex.Message);
            }
        }
    }
}
