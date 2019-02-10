namespace Interpic.AsyncTasks
{
    public interface IProcessTaskDialog
    {
        void ReportProgress(int progress, bool indeterminate = false);
        void CancelAllTasks();
    }
}