using Interpic.AsyncTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    class LoadGlobalSettingsTask : AsyncTask
    {
        public LoadGlobalSettingsTask()
        {
            TaskName = "Loading global settings.";
            TaskDescription = App.EXECUTABLE_DIRECTORY + "/" + App.GLOBAL_SETTINGS_FILE;
            ActionName = "Load global settings";
            IsIndeterminate = true;
        }
        public override Task Execute()
        {
            return Task.Run(() => App.LoadGlobalSettings());
        }
    }
}
