using Interpic.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Interpic.AsyncTasks
{
    /// <summary>
    /// Interaction logic for ProcessTaskDialog.xaml
    /// </summary>
    public partial class ProcessTaskDialog : Window, IProcessTaskDialog
    {
        public AsyncTask TaskToExecute { get; set; }
        private IProgress<int> progress;
        private Task executingInnerTask;
        private bool complete;
        private bool canceling;

        public ProcessTaskDialog(AsyncTask task, string dialogTitle = "Processing...")
        {
            Title = dialogTitle;
            TaskToExecute = task;

            InitializeComponent();
            progress = new Progress<int>(percent => { if (percent > 0) { pbProgress.Value = percent; pbProgress.IsIndeterminate = false; } else { pbProgress.IsIndeterminate = true; } });
        }

        public void ReportProgress(int progress, bool indeterminate = false)
        {
            if (progress < 0)
            {
                progress = 0;
            }
            if (progress > 100)
            {
                progress = 100;
            }
            if (indeterminate)
            {
                this.progress.Report(-1);
            }
            else
            {
                this.progress.Report(progress);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTask();
        }

        private void LoadTask()
        {
            if (TaskToExecute != null)
            {
                lbTaskName.Text = TaskToExecute.TaskName;
                lbTaskDescription.Text = TaskToExecute.TaskDescription;
                ExecuteTask();

            }
            else
            {
                ErrorAlert.Show("No task to execute");
                Close();
            }
        }

        private async void ExecuteTask()
        {
            TaskToExecute.Dialog = this;
            TaskToExecute.BeforeExecution();
            pbProgress.IsIndeterminate = TaskToExecute.IsIndeterminate;

            if (TaskToExecute.IsCancelable)
            {
                TaskToExecute.CancellationTokenSource = new CancellationTokenSource();
                try
                {
                    executingInnerTask = TaskToExecute.Execute();
                    await executingInnerTask;
                }
                catch (OperationCanceledException)
                {
                    TaskToExecute.IsCanceled = true;
                    complete = true;
                    TaskToExecute.FireCanceledEvent(this);
                    Close();
                }
                if (!executingInnerTask.IsCanceled)
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
            else
            {
                executingInnerTask = TaskToExecute.Execute();
                await executingInnerTask;
                if (!executingInnerTask.IsCanceled)
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
            complete = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!complete)
            {
                if (!canceling)
                {
                    if (TaskToExecute.IsCancelable)
                    {
                        canceling = true;
                        lbTaskName.Text += " - Canceling...";
                        pbProgress.IsIndeterminate = true;
                        TaskToExecute.CancellationTokenSource.Cancel();
                    }
                    else
                    {
                        ErrorAlert.Show("The current task cannot be canceled.");
                    }
                    e.Cancel = true;
                }
            }

        }

        public void CancelAllTasks(string errorMessage = null)
        {
            TaskToExecute.IsCanceled = true;
            if (errorMessage != null)
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ErrorAlert.Show(errorMessage);
                    });
                }
                else
                {
                    ErrorAlert.Show(errorMessage);
                }
            }
            WarningAlert.Show("The task has been canceled.");
            Close();
        }
    }
}
