using Interpic.Alerts;
using Interpic.AsyncTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Interpic.Web.Selenium.SeleniumWrapper;

namespace Interpic.Web.Selenium.Tasks
{
    public class StartSeleniumTask : AsyncTask
    {
        public SeleniumWrapper Selenium { get; set; }
        public BrowserType Type { get; set; }
        public StartSeleniumTask(BrowserType type)
        {
            TaskName = "Starting Selenium (" + GetBrowserName(type) + ")...";
            TaskDescription = "Chrome Webdriver";
            ActionName = "Start Selenium";
            IsIndeterminate = true;
            Type = type;
        }

        public override Task Execute()
        {
            return Task.Run(() => { Run(); });
        }

        private void Run()
        {
            try
            {
                Selenium = new SeleniumWrapper(Type);
                Selenium.Start();
            }
            catch (Exception ex)
            {
                Dialog.CancelAllTasks("Could not start Selenium:\n" + ex.Message);
            }
        }

        private string GetBrowserName(BrowserType type)
        {
            switch (type)
            {
                case BrowserType.Chrome:
                    return "Chrome";
                case BrowserType.FireFox:
                    return "Firefox";
                default:
                    return "Chrome";
            }
        }

    }
}
