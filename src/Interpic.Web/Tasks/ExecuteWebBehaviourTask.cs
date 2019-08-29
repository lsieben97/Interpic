using Interpic.AsyncTasks;
using Interpic.Web.Behaviours.Models;
using Interpic.Web.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Tasks
{
    public class ExecuteWebBehaviourTask : AsyncTask
    {
        public WebBehaviour Behaviour { get; }
        public SeleniumWrapper Selinium { get; set; }

        public ExecuteWebBehaviourTask(WebBehaviour behaviour, SeleniumWrapper selinium)
        {
            TaskName = "Executing web behaviour...";
            ActionName = "Exectute web behaviour";
            TaskDescription = behaviour.Name;
            Behaviour = behaviour;
            Selinium = selinium;
        }
        

        public override Task Execute()
        {
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return new TaskFactory(scheduler).StartNew(() => { Run(); });
        }

        private void Run()
        {
            Behaviour.Execute(Selinium);
        }
    }
}
