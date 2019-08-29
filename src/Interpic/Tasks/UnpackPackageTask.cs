using Interpic.AsyncTasks;
using Interpic.Models;
using Interpic.Studio.Functional;
using Interpic.Studio.InternalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    internal class UnpackPackageTask : AsyncTask
    {
        public PackageCacheEntry UnpackedPackage { get; set; }
        public string Path { get; set; }
        public bool AddToCache { get; set; }

        public UnpackPackageTask(string path, bool addToCache )
        {
            TaskName = "Unpacking package...";
            TaskDescription = path;
            ActionName = "Unpack package";
            IsIndeterminate = true;
            Path = path;
            AddToCache = addToCache;
        }

        public override Task Execute()
        {
            return Task.Run(() => Run());
        }

        private void Run()
        {
            try
            {
                UnpackedPackage = Packages.Unpack(Path, AddToCache);
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks("Could not unpack package:\n" + ex.Message);
            }
        }
    }
}
