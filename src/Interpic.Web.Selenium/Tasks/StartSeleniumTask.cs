using Interpic.AsyncTasks;
using Interpic.Settings;
using System;
using System.Threading.Tasks;
using static Interpic.Web.Selenium.SeleniumWrapper;

namespace Interpic.Web.Selenium.Tasks
{
    public class StartSeleniumTask : BackgroundTask
    {
        public SeleniumWrapper Selenium { get; set; }
        public BrowserType Type { get; set; }
        public SettingsCollection Settings { get; }

        public StartSeleniumTask(BrowserType type, SettingsCollection settings)
        {
            TaskName = "Starting Selenium (" + GetBrowserName(type) + ")...";
            TaskDescription = "Chrome Webdriver";
            ActionName = "Start Selenium";
            IsIndeterminate = true;
            Type = type;
            Settings = settings;
            Important = true;
            ImportanceReason = "Processes needed by Interpic might not exit properly.";
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
                Selenium.Start(Settings);
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
