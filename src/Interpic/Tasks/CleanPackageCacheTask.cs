using Interpic.AsyncTasks;
using Interpic.Models;
using Interpic.Studio.Functional;
using Interpic.Studio.InternalModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class CleanPackageCacheTask : AsyncTask
    {
        public CleanPackageCacheTask()
        {
            TaskName = "Cleaning package cache...";
            TaskDescription = Path.Combine(App.EXECUTABLE_DIRECTORY, "packages", "package.cache");
            ActionName = "Clean package cache";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(App.EXECUTABLE_DIRECTORY, "packages"));
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
