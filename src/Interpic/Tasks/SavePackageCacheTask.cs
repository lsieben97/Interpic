using Interpic.AsyncTasks;
using Interpic.Studio.InternalModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class SavePackageCacheTask : BackgroundTask
    {
        public PackageCache PackageCache { get; set; }

        public SavePackageCacheTask(PackageCache packageCache)
        {
            TaskName = "Saving package cache...";
            TaskDescription = Path.Combine(App.EXECUTABLE_DIRECTORY, "packages", "package.cache");
            ActionName = "Save package cache";
            PackageCache = packageCache;
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            string path = Path.Combine(App.EXECUTABLE_DIRECTORY, "packages", "package.cache");

            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(PackageCache));
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks($"An error occured while tring to write the package cache file:\n{ex.Message}");
            }

        }
    }
}
