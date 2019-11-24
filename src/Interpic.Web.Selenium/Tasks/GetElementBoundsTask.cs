using Interpic.Alerts;
using Interpic.AsyncTasks;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Interpic.Web.Selenium.Tasks
{
    public class GetElementBoundsTask : AsyncTask
    {
        public SeleniumWrapper Selenium { get; set; }
        public (Size size, Point point) ElementBounds { get; set; }
        public string Xpath { get; set; }
        public GetElementBoundsTask(string xpath)
        {
            Xpath = xpath;
            TaskName = "Getting element bounds...";
            TaskDescription = xpath;
            ActionName = "Get element bounds";
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
                ElementBounds = Selenium.GetElementBounds(Xpath);
            }
            catch(Exception ex)
            {
                ErrorAlert.Show("Could not get element bounds:\n" + ex.Message);
                Dialog.CancelAllTasks();
            }
        }
    }
}
