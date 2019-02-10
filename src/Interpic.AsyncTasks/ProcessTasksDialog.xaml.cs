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
    /// Interaction logic for ProcessTasksDialog.xaml
    /// </summary>
    public partial class ProcessTasksDialog : Window, IProcessTaskDialog
    {
        public List<AsyncTask> TasksToExecute { get; set; }
        private AsyncTask TaskToExecute;
        private IProgress<int> progress;
        private Task executingInnerTask;
        private bool complete;
        private bool canceling;
        private int taskCounter = -1;

        public bool AllTasksCanceled { get { return TasksToExecute.All((task) => task.IsCanceled == true); }}

        public ProcessTasksDialog(List<AsyncTask> tasks, string dialogTitle = "Processing...")
        {
            Title = dialogTitle;
            TasksToExecute = tasks;

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
            LoadTasks();
        }

        private void LoadTasks()
        {
            if (TasksToExecute != null)
            {
                if (TasksToExecute.Count > 0)
                {
                    pbTotalProgress.Maximum = TasksToExecute.Count;
                    LoadTaskList();
                    ExecuteTasks();
                }
            }
            else
            {
                ErrorAlert.Show("No tasks to execute.");
                complete = true;
                Close();
            }
        }

        private void LoadTaskList()
        {
            foreach (AsyncTask task in TasksToExecute)
            {
                ListBoxItem item = new ListBoxItem();
                StackPanel stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.Margin = new Thickness(0, 0, 0, 0);
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Height = double.NaN;
                stackPanel.Width = double.NaN;

                Image image = new Image();
                image.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/WaitWhite.png", UriKind.RelativeOrAbsolute));
                image.Width = 24;
                image.Height = 24;
                image.ToolTip = task.TaskName + "\n\n" + task.TaskDescription;
                stackPanel.Children.Add(image);

                Label lbl = new Label();
                lbl.Content = task.ActionName;
                lbl.ToolTip = task.TaskName + "\n\n" + task.TaskDescription;
                lbl.FontFamily = new FontFamily("Verdana");
                lbl.FontSize = 14;
                lbl.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                stackPanel.Children.Add(lbl);
                item.Content = stackPanel;
                lsbTasks.Items.Add(item);

                task.Icon = image;
            }
        }

        private async void ExecuteTasks()
        {
            while (!complete)
            {
                taskCounter++;
                TaskToExecute = TasksToExecute[taskCounter];
                if (!TaskToExecute.IsCanceled)
                {
                    lbTaskName.Text = TaskToExecute.TaskName;
                    lbTaskDescription.Text = TaskToExecute.TaskDescription;

                    pbTotalProgress.Value = taskCounter;
                    TaskToExecute.Icon.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/ArrowRightBold.png", UriKind.RelativeOrAbsolute));
                    lbTitle.Text = String.Format("Processing task {0} of {1}", (taskCounter + 1).ToString(), TasksToExecute.Count.ToString());
                    Title = lbTitle.Text + ": " + lbTaskName.Text;
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
                            PassThrough();
                            TaskToExecute.Icon.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/CheckmarkGreen.png", UriKind.RelativeOrAbsolute));
                        }
                        catch (OperationCanceledException)
                        {
                            TaskToExecute.IsCanceled = true;
                            TaskToExecute.Icon.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/FailRed.png", UriKind.RelativeOrAbsolute));
                            if (taskCounter != TasksToExecute.Count - 1)
                            {
                                ConfirmAlert alert = new ConfirmAlert("Task canceled./nThe next task will now execute.");
                                if (alert.ShowDialog() == false)
                                {
                                    complete = true;
                                    Close();
                                }
                            }
                        }
                        if (!executingInnerTask.IsCanceled)
                        {
                            TaskToExecute.AfterExecution();
                            TaskToExecute.Icon.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/CheckmarkGreen.png", UriKind.RelativeOrAbsolute));
                        }
                    }
                    else
                    {
                        executingInnerTask = TaskToExecute.Execute();
                        await executingInnerTask;
                        PassThrough();
                        TaskToExecute.Icon.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/CheckmarkGreen.png", UriKind.RelativeOrAbsolute));
                    }

                    if (taskCounter == TasksToExecute.Count - 1)
                    {
                        if (TasksToExecute.Any(task => task.IsCanceled == true))
                        {
                            WarningAlert.Show("One or more tasks where canceled!");
                        }
                        complete = true;
                        Close();
                    }
                }
                else
                {
                    if (taskCounter == TasksToExecute.Count - 1)
                    {
                        if (TasksToExecute.Any(task => task.IsCanceled == true))
                        {
                            WarningAlert.Show("One or more tasks where canceled!");
                        }
                        complete = true;
                        Close();
                    }
                }
            }
        }

        private void PassThrough()
        {
            if (TaskToExecute.PassThrough)
            {
                if (TaskToExecute.GetType().GetProperties().Any((prop) => prop.Name == TaskToExecute.PassThroughSource))
                {
                    if (taskCounter != TasksToExecute.Count - 1)
                    {
                        AsyncTask nextTask = TasksToExecute[taskCounter + 1];
                        if (nextTask.GetType().GetProperties().Any((prop) => prop.Name == TaskToExecute.PassThroughTarget))
                        {
                            try
                            {
                                nextTask.GetType().GetProperty(TaskToExecute.PassThroughTarget).SetValue(nextTask, TaskToExecute.GetType().GetProperty(TaskToExecute.PassThroughSource).GetValue(TaskToExecute));
                            }
                            catch(Exception ex)
                            {
                                ErrorAlert.Show("Unable to pass property through:\n" + ex.Message);
                            }
                        }
                        else
                        {
                            ErrorAlert.Show("Unknown passthrough target '" + TaskToExecute.PassThroughTarget + "'.");
                        }
                    }
                    else
                    {
                        ErrorAlert.Show("Cannot passthrough property when there is no task to pass through to.");
                    }
                }
                else
                {
                    ErrorAlert.Show("Unknown passthrough source '" + TaskToExecute.PassThroughSource + "'.");
                }
            }
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

        public void CancelAllTasks()
        {
            foreach(AsyncTask task in TasksToExecute)
            {
                task.IsCanceled = true;
                task.Icon.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/FailRed.png", UriKind.RelativeOrAbsolute));
            }
        }
    }
}
