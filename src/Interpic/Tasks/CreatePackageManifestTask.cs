using Interpic.Models.Packaging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    class CreatePackageManifestTask : AsyncTasks.AsyncTask
    {
        public CreatePackageManifestTask(string targetFile, PackageManifest manifest)
        {
            TaskName = "Creating manifest file...";
            ActionName = "Create manifest file";
            TargetFile = targetFile;
            TargetFile = targetFile;
            Manifest = manifest;
        }

        public string TargetFile { get; }
        public PackageManifest Manifest { get; }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            try
            {
                File.WriteAllText(TargetFile, JsonConvert.SerializeObject(Manifest));
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks($"Could not create package manifest:\n{ex.Message}");
            }
        }
    }
}
