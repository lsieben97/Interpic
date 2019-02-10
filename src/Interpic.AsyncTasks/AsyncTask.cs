﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Interpic.AsyncTasks
{
    public abstract class AsyncTask
    {
        private string _cancelationConfirmationMessage = "Are you sure you want to cancel the task?";

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
        public string CancelationConfirmationMessage { get => _cancelationConfirmationMessage; set => _cancelationConfirmationMessage = value; }

        public virtual void BeforeExecution()
        {

        }

        public abstract Task Execute();

        public virtual void AfterExecution()
        {

        }
    }
}
