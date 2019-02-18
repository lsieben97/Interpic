using Interpic.AsyncTasks.EventArgs;
using System;

namespace Interpic.AsyncTasks
{
    public delegate void OnTaskExecuted(object sender, AsyncTaskEventArgs eventArgs);
    public delegate void OnTaskCanceled(object sender, AsyncTaskEventArgs eventArgs);
}
