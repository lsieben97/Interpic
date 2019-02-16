using HtmlAgilityPack;
using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Extensions;
using Interpic.Models;
using Interpic.Models.EventArgs;
using Interpic.Settings;
using Interpic.Web.Providers;
using Interpic.Web.Selenium.Tasks;
using Interpic.Web.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web
{
    public class WebProjectTypeProvider : IProjectTypeProvider
    {
        public IStudioEnvironment Studio { get; set; }

        public Settings.SettingsCollection GetDefaultControlSettings()
        {
            return null;
        }

        public Settings.SettingsCollection GetDefaultPageSettings()
        {
            SettingsCollection collection = new SettingsCollection();

            Setting<string> baseUrl = new Setting<string>();
            baseUrl.Key = "PageUrl";
            baseUrl.Name = "Page URL";
            baseUrl.Description = "URL of this page.\nThe base URL of the manual will be prepended to this value.";
            baseUrl.Value = "";

            collection.TextSettings.Add(baseUrl);
            return collection;
        }

        public Settings.SettingsCollection GetDefaultProjectSettings()
        {
            SettingsCollection collection = new SettingsCollection();

            Setting<string> baseUrl = new Setting<string>();
            baseUrl.Key = "BaseUrl";
            baseUrl.Name = "Base URL";
            baseUrl.Description = "Base url of the website for the manual.\nPage urls will be appended to this value.";
            baseUrl.Validator = new UrlValidator();
            baseUrl.Value = "";

            collection.TextSettings.Add(baseUrl);

            collection.Name = GetProjectTypeName();
            return collection;
        }

        public Settings.SettingsCollection GetDefaultSectionSettings()
        {
            return null;
        }

        public string GetProjectTypeDescription()
        {
            return "Create a manual for any web application. Suitable for most HTML web applications.";
        }

        public string GetProjectTypeId()
        {
            return "web";
        }

        public string GetProjectTypeName()
        {
            return "Web application";
        }

        public ISectionIdentifierSelector GetSectionSelector()
        {
            return new NodeSelector();
        }

        public ISourceProvider GetSourceProvider()
        {
            return new BasicSourceProvider();
        }

        public void TypeProviderConnected()
        {
            Studio.ProjectLoaded += StudioEnvironment_ProjectLoaded;
        }

        private void StudioEnvironment_ProjectLoaded(object sender, ProjectLoadedEventArgs e)
        {
            foreach (Page page in Studio.CurrentProject.Pages)
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(page.Source);
                page.Extensions = new WebPageExtensions(document);
            }
        }

        public IControlFinder GetControlFinder()
        {
            return new WebControlFinder();
        }

        public IControlIdentifierSelector GetControlSelector()
        {
            return new NodeSelector();
        }

        public (Control control, bool succes) RefreshControl(Control control, Section section, Page page, Project project)
        {
            List<AsyncTask> tasks = new List<AsyncTask>();
            StartSeleniumTask startTask = new StartSeleniumTask();
            startTask.PassThrough = true;
            startTask.PassThroughSource = "Selenium";
            startTask.PassThroughTarget = "Selenium";

            NavigateToPageTask navigateTask = new NavigateToPageTask(project.Settings.GetTextSetting("BaseUrl") + page.Settings.GetTextSetting("PageUrl"));
            navigateTask.PassThrough = true;
            navigateTask.PassThroughSource = "Selenium";
            navigateTask.PassThroughTarget = "Selenium";

            GetElementBoundsTask getElementBoundsTask = new GetElementBoundsTask(control.Identifier.Identifier);
            getElementBoundsTask.PassThrough = true;
            getElementBoundsTask.PassThroughSource = "Selenium";
            getElementBoundsTask.PassThroughTarget = "Selenium";

            CloseSeleniumTask closeSeleniumTask = new CloseSeleniumTask();

            tasks.Add(startTask);
            tasks.Add(navigateTask);
            tasks.Add(getElementBoundsTask);
            tasks.Add(closeSeleniumTask);

            ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks);
            try
            {
                dialog.ShowDialog();
                if (!dialog.AllTasksCanceled)
                {
                    control.ElementBounds = new ElementBounds(getElementBoundsTask.ElementBounds.point, getElementBoundsTask.ElementBounds.size);
                }
            }
            catch (Exception ex)
            {
                return (control, false);
            }
            return (control, true);
        }

        public (Section section, bool succes) RefreshSection(Section section, Page page, Project project)
        {
            List<AsyncTask> tasks = new List<AsyncTask>();
            StartSeleniumTask startTask = new StartSeleniumTask();
            startTask.PassThrough = true;
            startTask.PassThroughSource = "Selenium";
            startTask.PassThroughTarget = "Selenium";

            NavigateToPageTask navigateTask = new NavigateToPageTask(project.Settings.GetTextSetting("BaseUrl") + page.Settings.GetTextSetting("PageUrl"));
            navigateTask.PassThrough = true;
            navigateTask.PassThroughSource = "Selenium";
            navigateTask.PassThroughTarget = "Selenium";

            GetElementBoundsTask getElementBoundsTask = new GetElementBoundsTask(section.SectionIdentifier.Identifier);
            getElementBoundsTask.PassThrough = true;
            getElementBoundsTask.PassThroughSource = "Selenium";
            getElementBoundsTask.PassThroughTarget = "Selenium";

            CloseSeleniumTask closeSeleniumTask = new CloseSeleniumTask();

            tasks.Add(startTask);
            tasks.Add(navigateTask);
            tasks.Add(getElementBoundsTask);
            tasks.Add(closeSeleniumTask);

            ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks);
            try
            {
                dialog.ShowDialog();
                if (!dialog.AllTasksCanceled)
                {
                    section.ElementBounds = new ElementBounds(getElementBoundsTask.ElementBounds.point, getElementBoundsTask.ElementBounds.size);
                }
            }
            catch (Exception ex)
            {
                return (section, false);
            }
            return (section, true);
        }

        public (Page page, bool succes) RefreshPage(Page page, Project project)
        {
            List<AsyncTask> tasks = new List<AsyncTask>();
            StartSeleniumTask startTask = new StartSeleniumTask();
            startTask.PassThrough = true;
            startTask.PassThroughSource = "Selenium";
            startTask.PassThroughTarget = "Selenium";

            NavigateToPageTask navigateTask = new NavigateToPageTask(project.Settings.GetTextSetting("BaseUrl") + page.Settings.GetTextSetting("PageUrl"));
            navigateTask.PassThrough = true;
            navigateTask.PassThroughSource = "Selenium";
            navigateTask.PassThroughTarget = "Selenium";

            MakeScreenshotTask makeScreenshotTask = new MakeScreenshotTask();
            makeScreenshotTask.PassThrough = true;
            makeScreenshotTask.PassThroughSource = "Selenium";
            makeScreenshotTask.PassThroughTarget = "Selenium";

            CloseSeleniumTask closeSeleniumTask = new CloseSeleniumTask();

            tasks.Add(startTask);
            tasks.Add(navigateTask);
            tasks.Add(makeScreenshotTask);
            tasks.Add(closeSeleniumTask);

            ProcessTasksDialog dialog = new ProcessTasksDialog(ref tasks);
            try
            {
                dialog.ShowDialog();
                if (!dialog.AllTasksCanceled)
                {
                    page.Screenshot = makeScreenshotTask.Screenshot;
                }
            }
            catch (Exception ex)
            {
                return (page, false);
            }
            return (page, true);
        }
    }
}
