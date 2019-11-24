using Interpic.AsyncTasks;
using Interpic.Models;
using System.Threading.Tasks;

namespace Interpic.Studio.Tasks
{
    public class StartStudioTask : AsyncTask
    {
        public Studio Studio { get; set; }
        public Project Project { get; set; }
        public StartStudioTask()
        {
            TaskName = "Starting Studio;...";
            TaskDescription = "Interpic Studio v" + App.VERSION;
            ActionName = "Start Studio";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return new TaskFactory(scheduler).StartNew(() => { Run(); });
        }

        private void Run()
        {
            Studio = new Studio(Project);
        }
    }
}
