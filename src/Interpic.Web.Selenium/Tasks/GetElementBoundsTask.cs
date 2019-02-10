using Interpic.AsyncTasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
            return Task.Run(() => Run());
        }

        private void Run()
        {
            ElementBounds = Selenium.GetElementBounds(Xpath);
        }
    }
}
