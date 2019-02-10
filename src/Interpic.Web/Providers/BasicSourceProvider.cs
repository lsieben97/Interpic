using HtmlAgilityPack;
using Interpic.Alerts;
using Interpic.Extensions;
using Interpic.Models;
using Interpic.Web.Selenium.Tasks;
using Interpic.Web.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Interpic.Web.Providers
{
    public class BasicSourceProvider : ISourceProvider
    {
        public string GetSource(ref Project project, ref Page page)
        {
            string url = project.Settings.GetTextSetting("BaseUrl") + page.Settings.GetTextSetting("PageUrl");
            List<AsyncTasks.AsyncTask> tasks = new List<AsyncTasks.AsyncTask>();
            StartSeleniumTask startTask = new StartSeleniumTask();
            startTask.PassThrough = true;
            startTask.PassThroughSource = "Selenium";
            startTask.PassThroughTarget = "Selenium";

            NavigateToPageTask navigateTask = new NavigateToPageTask(url);
            navigateTask.PassThrough = true;
            navigateTask.PassThroughSource = "Selenium";
            navigateTask.PassThroughTarget = "Selenium";

            GetHtmlTask getHtmlTask = new GetHtmlTask(url);
            getHtmlTask.PassThrough = true;
            getHtmlTask.PassThroughSource = "Selenium";
            getHtmlTask.PassThroughTarget = "Selenium";

            CloseSeleniumTask closeSeleniumTask = new CloseSeleniumTask();

            tasks.Add(startTask);
            tasks.Add(navigateTask);
            tasks.Add(getHtmlTask);
            tasks.Add(closeSeleniumTask);

            new AsyncTasks.ProcessTasksDialog(tasks).ShowDialog();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(getHtmlTask.Html);
            page.Extensions = new WebPageExtensions(document);
            return getHtmlTask.Html;
        }
    }
}
