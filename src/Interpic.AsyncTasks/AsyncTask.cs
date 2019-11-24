using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Interpic.AsyncTasks
{
    public abstract class AsyncTask
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        private bool canceledEventFired = false;
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string ActionName { get; set; }
        internal Image Icon { get; set; }
        public bool PassThrough { get; set; }
        public string PassThroughSource { get; set; }
        public string PassThroughTarget { get; set; }
        public CancellationTokenSource CancellationTokenSource {get; set;}
        public IProcessTaskDialog Dialog { get; set; }
        public bool IsIndeterminate { get; set; }
        public bool IsCancelable { get; set; }
        public bool IsCanceled { get; set; }
        public string CancelationConfirmationMessage { get; set; } = "Are you sure you want to cancel the task?";

        public virtual void BeforeExecution()
        {

        }

        public abstract Task Execute();

        public virtual void AfterExecution()
        {

        }

        public event OnTaskExecuted Executed;
        public event OnTaskCanceled Canceled;

        public void FireExecutedEvent(object source)
        {
            Executed?.Invoke(source, new EventArgs.AsyncTaskEventArgs(this));
        }

        public void FireCanceledEvent(object source)
        {
            if (!canceledEventFired)
            {
                Canceled?.Invoke(source, new EventArgs.AsyncTaskEventArgs(this));
                canceledEventFired = true;
            }
        }
    }
}
