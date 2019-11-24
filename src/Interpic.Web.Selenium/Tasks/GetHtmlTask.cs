using Interpic.AsyncTasks;
using System.Threading.Tasks;

namespace Interpic.Web.Selenium.Tasks
{
    public class GetHtmlTask : AsyncTask
    {
        public SeleniumWrapper Selenium { get; set; }
        public string Html { get; set; }
        public GetHtmlTask(string url)
        {
            TaskName = "Retrieving HTML...";
            TaskDescription = url;
            ActionName = "Retrieve HTML";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => Run());
        }

        private void Run()
        {
            Html = Selenium.GetHtml();
        }
    }
}
