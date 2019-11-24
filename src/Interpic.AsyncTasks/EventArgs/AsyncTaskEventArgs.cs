namespace Interpic.AsyncTasks.EventArgs
{
    public class AsyncTaskEventArgs
    {
        public AsyncTaskEventArgs(AsyncTask task)
        {
            Task = task;
        }

        public AsyncTask Task { get; set; }
    }
}
