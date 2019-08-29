using Interpic.AsyncTasks;
using Interpic.Web.Behaviours.Models;
using Interpic.Web.Selenium;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Tasks
{
    public class ExecuteWebBehavioursTask : AsyncTask
    {
        public string BehavioursJson { get; }
        public SeleniumWrapper Selenium { get; set; }

        public ExecuteWebBehavioursTask(string behavioursJson, SeleniumWrapper selinium)
        {
            TaskName = "Executing web behaviours...";
            ActionName = "Execute web behaviours";
            BehavioursJson = behavioursJson;
            Selenium = selinium;
        }
        

        public override Task Execute()
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return new TaskFactory(scheduler).StartNew(() => { Run(); });
        }

        private void Run()
        {
            if (BehavioursJson != null)
            {
                List<WebBehaviour> behaviours = JsonConvert.DeserializeObject<List<WebBehaviour>>(BehavioursJson, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                List<AsyncTask> tasks = new List<AsyncTask>();
                foreach (WebBehaviour behaviour in behaviours)
                {
                    ExecuteWebBehaviourTask task = new ExecuteWebBehaviourTask(behaviour, Selenium);
                    tasks.Add(task);
                }
                tasks.Last().PassThrough = false;
                ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks, "Executing web behaviours...");
                dialog.ShowDialog();

                if (dialog.AllTasksCanceled == true)
                {
                    Dialog.CancelAllTasks();
                }
            }
        }
    }
}
