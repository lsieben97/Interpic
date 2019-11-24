using HtmlAgilityPack;
using Interpic.Models.Extensions;
using Interpic.Models;
using Interpic.Web.Selenium.Tasks;
using System;
using System.Collections.Generic;
using static Interpic.Web.Selenium.SeleniumWrapper;

namespace Interpic.Web.Providers
{
    public class BasicSourceProvider : ISourceProvider
    {
        public Page GetSource(Page page)
        {
            BrowserType type = WebProjectTypeProvider.GetBrowserType(page.Parent.Settings.GetMultipleChoiceSetting("BrowserType"));
            if (WebProjectTypeProvider.CheckSelenium(type))
            {
                string url = page.Parent.Settings.GetTextSetting("BaseUrl") + page.Settings.GetTextSetting("PageUrl");
                List<AsyncTasks.AsyncTask> tasks = new List<AsyncTasks.AsyncTask>();


                NavigateToPageTask navigateTask = new NavigateToPageTask(url);
                navigateTask.Selenium = WebProjectTypeProvider.Selenium[type];
                navigateTask.PassThrough = true;
                navigateTask.PassThroughSource = "Selenium";
                navigateTask.PassThroughTarget = "Selenium";

                GetHtmlTask getHtmlTask = new GetHtmlTask(url);

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
                        page.Source = getHtmlTask.Html;
                        return page;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
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
