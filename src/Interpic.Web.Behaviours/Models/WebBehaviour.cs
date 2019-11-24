using Interpic.AsyncTasks;
using Interpic.Web.Behaviours.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpic.Web.Behaviours.Models
{
    public class WebBehaviour
    {
        public static WebBehaviour None = new WebBehaviour
        {
            Name = "None",
            Description = "A web behaviour that does nothing.",
            Id = "None"
        };

        public static List<WebBehaviour> AvailableBehaviours { get; set; } = new List<WebBehaviour>();

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WebAction> Actions { get; set; } = new List<WebAction>();

        public virtual bool Execute(Selenium.SeleniumWrapper selenium)
        {
            List<AsyncTasks.AsyncTask> tasks = new List<AsyncTasks.AsyncTask>();

            foreach(WebAction action in Actions)
            {
                ExecuteWebActionTask task = new ExecuteWebActionTask(action, selenium);
                task.PassThrough = true;
                task.PassThroughSource = "Selenium";
                task.PassThroughTarget = "Selenium";
                tasks.Add(task);
            }

            tasks.Last().PassThrough = false;

            ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks, "Executing web actions...");
            dialog.ShowDialog();

            return dialog.AllTasksCanceled;
        }
    }
}
