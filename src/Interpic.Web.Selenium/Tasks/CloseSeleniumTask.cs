using Interpic.AsyncTasks;
using System.Threading.Tasks;

namespace Interpic.Web.Selenium.Tasks
{
    public class CloseSeleniumTask : AsyncTask
    {
        public SeleniumWrapper Selenium { get; set; }
        public CloseSeleniumTask()
        {
            TaskName = "Closing Selenium...";
            TaskDescription = "Chrome Webdriver";
            ActionName = "Close Selenium";
            IsIndeterminate = true;
        }

        public CloseSeleniumTask(SeleniumWrapper selenium)
        {
            Selenium = selenium;
        }

        public override Task Execute()
        {
            return Task.Run(() => Run());
        }

        private void Run()
        {
            Selenium.Close();
        }
    }
}
