using Interpic.AsyncTasks;
using Interpic.Studio.InternalModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class LoadPackageCacheTask : AsyncTask
    {
        public PackageCache PackageCache { get; set; }

        public LoadPackageCacheTask()
        {
            TaskName = "Loading package cache...";
            TaskDescription = Path.Combine(App.EXECUTABLE_DIRECTORY, "packages", "package.cache");
            ActionName = "Load package cache";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            string path = Path.Combine(App.EXECUTABLE_DIRECTORY, "packages", "package.cache");
            if (File.Exists(path))
            {
                try
                {
                    PackageCache = JsonConvert.DeserializeObject<PackageCache>(File.ReadAllText(path));
                    App.PackageCache = PackageCache;
                }
                catch (Exception ex)
                {
                    Dialog.CancelAllTasks($"An error occured while tring to read the package cache file:\n{ex.Message}");
                }
            }
        }
    }
}
