using Interpic.AsyncTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Selenium.Tasks
{
    public class NavigateToPageTask : AsyncTask
    {
        public SeleniumWrapper Selenium { get; set; }
        public string Url { get; set; }
        public NavigateToPageTask(string url)
        {
            Url = url;
            TaskName = "Loading Webpage...";
            TaskDescription = Url;
            ActionName = "Navigate to webpage";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => Run());
        }

        private void Run()
        {
            Selenium.Navigate(Url);
        }
    }
}
