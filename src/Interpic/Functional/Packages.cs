using Interpic.Models.Packaging;
using Interpic.Studio.InternalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;
using System.IO;
using Interpic.Alerts;
using Newtonsoft.Json;
using Interpic.AsyncTasks;
using Interpic.Studio.Tasks;

namespace Interpic.Studio.Functional
{
    public static class Packages
    {
        public static PackageCacheEntry Unpack(string path)
        {
            return Unpack(path, true);
        }

        public static PackageCacheEntry Unpack(string path, bool addToCache)
        {
            if (Studio.packageCache != null)
            {
                if (Studio.packageCache.Entries.Any((p) => { return p.PackageFile == path; }))
                {
                    return Studio.packageCache.Entries.Find((p) => { return p.PackageFile == path; });
                }
            }

            string targetDirectory = Path.Combine(App.EXECUTABLE_DIRECTORY, "packages", Guid.NewGuid().ToString());
            try
            {
                Directory.CreateDirectory(targetDirectory);
                ZipFile.ExtractToDirectory(path, targetDirectory);
                PackageCacheEntry entry = new PackageCacheEntry();
                entry.Folder = targetDirectory;
                entry.PackageFile = path;
                if (File.Exists(Path.Combine(targetDirectory, "manifest.json")) == false)
                {
                    throw new InvalidOperationException("No manifest.json file in package.");
                }
                entry.Manifest = JsonConvert.DeserializeObject<PackageManifest>(Path.Combine(targetDirectory, "manifest.json"));

                if (addToCache)
                {
                    App.PackageCache.CacheDate = DateTime.Now;
                    App.PackageCache.Entries.Add(entry);
                    Studio.Instance.ScheduleBackgroundTask(new SavePackageCacheTask(App.PackageCache));
                }

                return entry;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Could not unpack package:\n{ex.Message}");
            }
        }

        public static bool Pack(List<string> contents, string targetpath, PackageManifest manifest)
        {
            string tempDirectory = Path.Combine(Path.GetTempPath() + "interpic_temp_package_" + Guid.NewGuid().ToString());
            try
            {
                Directory.CreateDirectory(tempDirectory);
            }
            catch(Exception ex)
            {
                ErrorAlert.Show($"Could not create temporary directory '{tempDirectory}':\n{ex.Message}");
                return false;
            }

            List<AsyncTask> tasks = new List<AsyncTask>();
            tasks.Add(new CreatePackageManifestTask(Path.Combine(tempDirectory, "manifest.json"), manifest));
            tasks.Add(new PreparePackageContentsTask(contents, tempDirectory));
            tasks.Add(new CreatePackageTask(tempDirectory, targetpath));
            tasks.Add(new CleanUpAfterPackageCreationTask(tempDirectory));

            ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks, "Creating package...");
            dialog.ShowDialog();
            return !dialog.AllTasksCanceled;
        }

    }
}
