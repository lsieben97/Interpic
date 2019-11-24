using System.Threading.Tasks;

namespace Interpic.AsyncTasks
{
    /// <summary>
    /// Silent task processor executes the given task without showing a dialog while reporting progress back to the given <see cref="IProcessTaskDialog"/>.
    /// </summary>
    public class SilentTaskProcessor
    {
        public SilentTaskProcessor(AsyncTask taskToExecute, IProcessTaskDialog reportBack)
        {
            TaskToExecute = taskToExecute;
            ReportBack = reportBack;
        }

        public AsyncTask TaskToExecute { get; set; }
        public IProcessTaskDialog ReportBack { get; set; }

        private Task executingInnerTask;

        public void CancelAllTasks()
        {
            TaskToExecute.IsCanceled = true;
            ReportBack.CancelAllTasks();
        }

        public void ReportProgress(int progress, bool indeterminate = false)
        {
            if (ReportBack != null)
            {
                ReportBack.ReportProgress(progress, indeterminate);
            }
        }

        public async void ProcessTask()
        {
            TaskToExecute.Dialog = ReportBack;
            TaskToExecute.BeforeExecution();

            executingInnerTask = TaskToExecute.Execute();
            await executingInnerTask;

            if (!TaskToExecute.IsCanceled)
            {
                TaskToExecute.AfterExecution();
                TaskToExecute.FireExecutedEvent(this);
            }
            else
            {
                TaskToExecute.AfterExecution();
                TaskToExecute.FireCanceledEvent(this);
            }
        }
    }
}
