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
    }
}
