using Interpic.AsyncTasks;
using Interpic.Web.Behaviours.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Behaviours.Tasks
{
    public class ExecuteWebActionTask : AsyncTask
    {
        public WebAction WebAction { get; set; }
        public Selenium.SeleniumWrapper Selenium { get; set; }
        public ExecuteWebActionTask(WebAction webAction, Selenium.SeleniumWrapper selenium)
        {
            WebAction = webAction;
            Selenium = selenium;
            TaskName = "Executing web action...";
            TaskDescription = WebAction.Name;
            ActionName = WebAction.Name;
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            try
            {
                WebAction.Selenium = Selenium;
                WebAction.Execute();
                if (WebAction.Type == WebAction.WebActionType.Check)
                {
                    CheckWebAction checkWebAction = (CheckWebAction)WebAction;
                    WebBehaviour behaviourToExecute = checkWebAction.CheckResult ? checkWebAction.BehaviourWhenTrue : checkWebAction.BehaviourWhenFalse;
                    if (behaviourToExecute != null)
                    {
                        behaviourToExecute.Execute(Selenium);
                    }
                }
            }
            catch(Exception ex)
            {
                Dialog.CancelAllTasks($"Error while executing action: {ex.Message}");
            }
        }
    }
}
