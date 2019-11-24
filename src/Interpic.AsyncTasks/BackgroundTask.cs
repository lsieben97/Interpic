namespace Interpic.AsyncTasks
{
    public abstract class BackgroundTask : AsyncTask
    {
        public bool Important { get; set; }
        public string ImportanceReason { get; set; }
    }
}
