using Interpic.AsyncTasks;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class CleanUpAfterPackageCreationTask : AsyncTask
    {
        public CleanUpAfterPackageCreationTask(string targetDirectory)
        {
            TaskName = "Cleaning up...";
            ActionName = "Clean up";
            TargetDirectory = targetDirectory;
        }

        public string TargetDirectory { get; }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            try
            {
                Directory.Delete(TargetDirectory, true);
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks($"Could not create package:\n{ex.Message}");
            }
        }

    }
}
