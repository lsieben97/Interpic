using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.AsyncTasks
{
    public abstract class BackgroundTask : AsyncTask
    {
        public bool Important { get; set; }
        public string ImportanceReason { get; set; }
    }
}
