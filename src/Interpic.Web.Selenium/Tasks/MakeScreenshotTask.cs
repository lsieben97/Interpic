using Interpic.AsyncTasks;
using System.Threading.Tasks;

namespace Interpic.Web.Selenium.Tasks
{
    public class MakeScreenshotTask : AsyncTask
    {
        private string _url = "";

        public SeleniumWrapper Selenium { get; set; }
        public byte[] Screenshot { get; set; }
        public string Url { get => _url; set => _url = value; }
        public MakeScreenshotTask()
        {
            TaskName = "Making screenshot...";
            TaskDescription = Url;
            ActionName = "Make screenshot";
            IsIndeterminate = true;
        }

        public override Task Execute()
        {
            return Task.Run(() => Run());
        }

        private void Run()
        {
            Screenshot = Selenium.MakeScreenShot();
        }
    }
}
