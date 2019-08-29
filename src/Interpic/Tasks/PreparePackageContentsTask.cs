using Interpic.AsyncTasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class PreparePackageContentsTask : AsyncTask
    {
        public PreparePackageContentsTask(List<string> packageContents, string copyTarget)
        {
            TaskName = "Preparing package contents...";
            ActionName = "Prepare package contents";
            IsIndeterminate = false;
            PackageContents = packageContents;
            CopyTarget = copyTarget;
        }

        public List<string> PackageContents { get; set; }
        public string CopyTarget { get; set; }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            int counter = 1;
            foreach (string file in PackageContents)
            {
                try
                {
                    File.Copy(file, Path.Combine(CopyTarget, Path.GetFileName(file)));
                    Dialog.ReportProgress(100 / PackageContents.Count * counter);
                    counter++;
                }
                catch (Exception ex)
                {
                    Dialog.CancelAllTasks($"Could not copy file:\n{ex.Message}");
                    break;
                }
            }

        }
    }
}
