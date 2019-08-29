using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Models.Extensions;
using Interpic.Models;
using Interpic.Studio.Functional;
using Interpic.Studio.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Interpic.Models.Packaging;
using Interpic.Studio.InternalModels;

namespace Interpic.Studio
{
    public class DLLManager : IDLLManager
    {
        private IStudioEnvironment Studio;
        private List<LoadedAssembly> LoadedAsemblies = new List<LoadedAssembly>();

        internal static DLLManager Instance = new DLLManager();
        public List<LoadedAssembly> LoadAssemblies(List<string> paths, Extension requestingExtension)
        {
            List<LoadedAssembly> results = new List<LoadedAssembly>();
            List<AsyncTask> tasks = new List<AsyncTask>();
            foreach (string path in paths)
            {
                if (ValidatePath(path, requestingExtension))
                {
                    tasks.Add(new LoadAssemblyTask(path, requestingExtension));
                }
            }

            ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks);
            dialog.ShowDialog();
            foreach (LoadAssemblyTask task in tasks)
            {
                results.Add(task.LoadedAssembly);
            }
            return results;
        }

        public LoadedAssembly LoadAssembly(string path, Extension requestingExtension)
        {
            if (ValidatePath(path, requestingExtension))
            {
                LoadAssemblyTask loadAssemblyTask = new LoadAssemblyTask(path, requestingExtension);
                ProcessTaskDialog dialog = new ProcessTaskDialog(loadAssemblyTask);
                dialog.ShowDialog();
                if (dialog.TaskToExecute.IsCanceled)
                {
                    return null;
                }
                return loadAssemblyTask.LoadedAssembly;
            }
            else
            {
                return null;
            }
        }

        private bool ValidatePath(string path, Extension requestingExtension)
        {
            if (LoadedAsemblies.Any(assembly => assembly.Path == path))
            {
                return false;
            }

            if (!File.Exists(path))
            {
                return false;
            }

            if (!Studio.CurrentProject.TrustedAssemblies.Any(assembly => assembly.Path == path))
            {
                if (!AskUserPermission(path, requestingExtension))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidatePackagePath(string path, Extension requestingExtension)
        {
            if (LoadedAsemblies.Any(assembly => assembly.PackagePath == path))
            {
                return false;
            }

            if (!File.Exists(path))
            {
                return false;
            }

            if (!Studio.CurrentProject.TrustedAssemblies.Any(assembly => assembly.PackagePath == path))
            {
                if (!AskUserPermissionForPackage(path, requestingExtension))
                {
                    return false;
                }
            }

            return true;
        }

        public List<bool> UnloadAssemblies(List<LoadedAssembly> assemblies)
        {
            List<bool> results = new List<bool>();
            List<AsyncTask> tasks = new List<AsyncTask>();
            foreach(LoadedAssembly assembly in assemblies)
            {
                tasks.Add(new UnloadAssemblyTask(assembly));
            }

            ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks);
            dialog.ShowDialog();
            foreach (UnloadAssemblyTask task in tasks)
            {
                results.Add(task.IsCancelable);
            }
            return results;
        }

        public bool UnLoadAssembly(LoadedAssembly assembly)
        {
            UnloadAssemblyTask unloadAssemblyTask = new UnloadAssemblyTask(assembly);
            ProcessTaskDialog dialog = new ProcessTaskDialog(unloadAssemblyTask);
            dialog.ShowDialog();
            if (dialog.TaskToExecute.IsCanceled)
            {
                return false;
            }
            return true;
        }

        private bool AskUserPermission(string path, Extension requestingExtension)
        {
            return ConfirmAlert.Show("Extension " + requestingExtension.GetName() + " wants to load the following assembly:\n\n" + path + "\n\n" + "Press OK to load the assembly", true).Result;
        }

        private bool AskUserPermissionForPackage(string path, Extension requestingExtension)
        {
            return ConfirmAlert.Show("Extension " + requestingExtension.GetName() + " wants to load the following package:\n\n" + path + "\n\n" + "Press OK to load the assembly", true).Result;
        }

        public LoadedAssembly LoadAssembly(string path, Extension requestingExtension, PackageDefinition packagingDefinition)
        {
            if (ValidatePackagePath(path, requestingExtension))
            {
                UnpackPackageTask unpackPackageTask = new UnpackPackageTask(path, true);
                ProcessTaskDialog dialog = new ProcessTaskDialog(unpackPackageTask);
                dialog.ShowDialog();
                if (dialog.TaskToExecute.IsCanceled)
                {
                    return null;
                }
                PackageCacheEntry unpackedPackage = unpackPackageTask.UnpackedPackage;
                string assemblyPath = Path.Combine(unpackedPackage.Folder, unpackedPackage.Manifest.DllPath);
                LoadAssemblyTask loadAssemblyTask = new LoadAssemblyTask(assemblyPath, requestingExtension);
                dialog = new ProcessTaskDialog(loadAssemblyTask);
                dialog.ShowDialog();
                if (dialog.TaskToExecute.IsCanceled)
                {
                    return null;
                }
                loadAssemblyTask.LoadedAssembly.PackagePath = path;
                loadAssemblyTask.LoadedAssembly.InPackage = true;
                return loadAssemblyTask.LoadedAssembly;
            }
            else
            {
                return null;
            }
        }

        public List<LoadedAssembly> LoadAssemblies(List<string> paths, Extension requestingExtension, PackageDefinition packagingDefinition)
        {
            List<LoadedAssembly> results = new List<LoadedAssembly>();
            List<AsyncTask> tasks = new List<AsyncTask>();
            foreach (string path in paths)
            {
                if (ValidatePackagePath(path, requestingExtension))
                {
                    UnpackPackageTask task = new UnpackPackageTask(path, true);
                    ProcessTaskDialog processDialog = new ProcessTaskDialog(task);
                    if (!processDialog.TaskToExecute.IsCanceled)
                    {
                        string assemblyPath = Path.Combine(task.UnpackedPackage.Folder, task.UnpackedPackage.Manifest.DllPath);
                        tasks.Add(new LoadAssemblyTask(assemblyPath, requestingExtension));
                    }
                }
            }

            ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks);
            dialog.ShowDialog();
            foreach (LoadAssemblyTask task in tasks)
            {
                results.Add(task.LoadedAssembly);
            }
            return results;
        }

        public List<LoadedAssembly> GetLoadedAssemblies(Extension extension)
        {
            return LoadedAsemblies.FindAll((LoadedAssembly a) => { return a.RequestingExtension == extension; }).ToList();
        }

        public List<string> GetLoadedPackages()
        {
            return App.PackageCache.Entries.Select(entry => entry.PackageFile).ToList();
        }
    }
}
