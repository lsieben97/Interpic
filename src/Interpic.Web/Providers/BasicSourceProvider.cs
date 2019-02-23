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
using static Interpic.Web.Selenium.SeleniumWrapper;

namespace Interpic.Web.Providers
{
    public class BasicSourceProvider : ISourceProvider
    {
        public string GetSource(ref Project project, ref Models.Version version, ref Page page)
        {
            BrowserType type = WebProjectTypeProvider.GetBrowserType(version.Settings.GetMultipleChoiceSetting("BrowserType"));
            if (WebProjectTypeProvider.CheckSelenium(type))
            {
                string url = version.Settings.GetTextSetting("BaseUrl") + page.Settings.GetTextSetting("PageUrl");
                List<AsyncTasks.AsyncTask> tasks = new List<AsyncTasks.AsyncTask>();


                NavigateToPageTask navigateTask = new NavigateToPageTask(url);
                navigateTask.Selenium = WebProjectTypeProvider.Selenium[type];
                navigateTask.PassThrough = true;
                navigateTask.PassThroughSource = "Selenium";
                navigateTask.PassThroughTarget = "Selenium";

                GetHtmlTask getHtmlTask = new GetHtmlTask(url);
                getHtmlTask.PassThrough = true;
                getHtmlTask.PassThroughSource = "Selenium";
                getHtmlTask.PassThroughTarget = "Selenium";

                tasks.Add(navigateTask);
                tasks.Add(getHtmlTask);

                AsyncTasks.ProcessTasksDialog dialog = new AsyncTasks.ProcessTasksDialog(ref tasks);
                try
                {
                    dialog.ShowDialog();
                    if (!dialog.AllTasksCanceled)
                    {
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(getHtmlTask.Html);
                        page.Extensions = new WebPageExtensions(document);
                        return getHtmlTask.Html;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
        }
    }
}
