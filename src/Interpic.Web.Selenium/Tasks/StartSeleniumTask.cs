using Interpic.Alerts;
using Interpic.AsyncTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Selenium.Tasks
{
    public class StartSeleniumTask : AsyncTask
    {
        public SeleniumWrapper Selenium { get; set; }
        public StartSeleniumTask()
        {
            TaskName = "Starting Selenium...";
            TaskDescription = "Chrome Webdriver";
            ActionName = "Start Selenium";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => Run());
        }

        private void Run()
        {
            try
            {
                Selenium = new SeleniumWrapper();
                Selenium.Start();
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not start Selenium:\n" + ex.Message);
                Dialog.CancelAllTasks();
            }
        }

    }
}
