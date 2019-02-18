using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
