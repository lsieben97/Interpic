using Interpic.AsyncTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    class LoadGlobalExtensionsTask : AsyncTask
    {
        public LoadGlobalExtensionsTask()
        {
            TaskName = "Loading global extensions.";
            TaskDescription = App.EXECUTABLE_DIRECTORY + "/" + App.GLOBAL_EXTENSION_FILE;
            ActionName = "Load global extensions";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => { });
        }
    }
}
