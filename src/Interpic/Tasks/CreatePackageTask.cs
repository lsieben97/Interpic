using Interpic.AsyncTasks;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class CreatePackageTask : AsyncTask
    {
        public CreatePackageTask(string targetDirectory, string targetFile)
        {
            TaskName = "Creating package...";
            ActionName = "Create package";
            TargetDirectory = targetDirectory;
            TargetFile = targetFile;
        }

        public List<string> PackageContents { get; set; }
        public string CopyTarget { get; set; }
        public string TargetDirectory { get; }
        public string TargetFile { get; }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            try
            {
                ZipFile.CreateFromDirectory(TargetDirectory, TargetFile, CompressionLevel.Optimal, false);
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks($"Could not create package:\n{ex.Message}");
            }
        }

    }
}
