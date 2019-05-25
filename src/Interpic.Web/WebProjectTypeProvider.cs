using HtmlAgilityPack;
using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Models.Extensions;
using Interpic.Models;
using Interpic.Models.EventArgs;
using Interpic.Settings;
using Interpic.Web.Behaviours.Tasks;
using Interpic.Web.Behaviours.Windows;
using Interpic.Web.Providers;
using Interpic.Web.Selenium.Tasks;
using Interpic.Web.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static Interpic.Web.Selenium.SeleniumWrapper;
using Interpic.Web.Behaviours.Models;
using Interpic.Web.WebActions.BasicWebActions;

namespace Interpic.Web
{
    public class WebProjectTypeProvider : IProjectTypeProvider
    {
        public IStudioEnvironment Studio { get; set; }
        public static Dictionary<BrowserType, Selenium.SeleniumWrapper> Selenium { get; set; } = new Dictionary<BrowserType, Selenium.SeleniumWrapper>();
        public InternetUsage InternetUsage { get; set; } = new InternetUsage();
        public WebBehaviourConfiguration BehaviourConfiguration { get; set; }

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
            collection.Name = "page settings";
            return collection;
        }

        public Settings.SettingsCollection GetDefaultProjectSettings()
        {
            return null;
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
            Studio.ProjectUnloaded += StudioProjectUnloaded;
            Studio.VersionAdded += StudioVersionAdded;
        }

        private void StudioVersionAdded(object sender, VersionEventArgs e)
        {
            BrowserType type = GetBrowserType(e.Version.Settings.GetMultipleChoiceSetting("BrowserType"));
            if (! Selenium.ContainsKey(type))
            {
                Studio.ScheduleBackgroundTask(new StartSeleniumTask(type));
            }
        }

        private void StudioProjectUnloaded(object sender, InterpicStudioEventArgs e)
        {
            if (Selenium != null)
            {
                List<AsyncTask> tasks = new List<AsyncTask>();
                foreach(KeyValuePair<BrowserType, Selenium.SeleniumWrapper> browser in Selenium)
                {
                    tasks.Add(new CloseSeleniumTask(browser.Value));
                }
                new ProcessTasksDialog(ref tasks).ShowDialog();
            }
        }

        private void StudioEnvironment_ProjectLoaded(object sender, ProjectLoadedEventArgs e)
        {
            PrepareObjectModel();
            StartSelenium();
            RegisterMenuItems();
            LoadWebBehaviourConfiguration();
        }

        private void LoadWebBehaviourConfiguration()
        {
            if (File.Exists(Studio.CurrentProject.ProjectFolder + "\\Webbehaviours.dat"))
            {
                LoadWebBehaviourConfigurationTask loadWebBehaviourConfigurationTask = new LoadWebBehaviourConfigurationTask(Studio.CurrentProject);
                ProcessTaskDialog dialog = new ProcessTaskDialog(loadWebBehaviourConfigurationTask);
                dialog.ShowDialog();
                if (!dialog.TaskToExecute.IsCanceled)
                {
                    BehaviourConfiguration = loadWebBehaviourConfigurationTask.WebBehaviourConfiguration;
                    BehaviourConfiguration.InternalWebActionPacks.Add(new BaseWebActionPack());
                    List<string> assembliesToLoad = BehaviourConfiguration.WebActionpacks.FindAll(pack => pack != "BasicWebActionsPack");
                    if (assembliesToLoad.Count > 0)
                    {
                        
                        List<LoadedAssembly> loadedAssemblies = Studio.GetDLLManager().LoadAssemblies(assembliesToLoad, WebExtension.Instance);
                        foreach(LoadedAssembly assembly in loadedAssemblies)
                        {
                            try
                            {
                                Type packType = assembly.Assembly.GetExportedTypes().First(ass => ass.BaseType.Name == "WebActionPack");
                                WebActionPack pack = Activator.CreateInstance<WebActionPack>();
                                BehaviourConfiguration.InternalWebActionPacks.Add(pack);
                            }
                            catch(Exception ex)
                            {
                                ErrorAlert.Show($"Could not load web action pack from assembly {assembly.Path}:\n\n{ex.Message}");
                            }
                        }
                    }
                }
            }
            else
            {
                WebBehaviourConfiguration configuration = new WebBehaviourConfiguration();
                configuration.WebActionpacks.Add("BasicWebActionsPack");
                configuration.InternalWebActionPacks.Add(new BaseWebActionPack());
                BehaviourConfiguration = configuration;
                SaveWebBehaviourConfigurationTask saveWebBehaviourConfigurationTask = new SaveWebBehaviourConfigurationTask(Studio.CurrentProject, configuration);
                ProcessTaskDialog dialog = new ProcessTaskDialog(saveWebBehaviourConfigurationTask);
                dialog.ShowDialog();
            }
        }

        private void RegisterMenuItems()
        {
            MenuItem webToolsItem = new MenuItem("Web tools");
            MenuItem openInWebBrowserItem = new MenuItem("Open current page in webbrowser");
            openInWebBrowserItem.Description = "Open the current page in the default webbrowser.";
            openInWebBrowserItem.Icon = new BitmapImage(new Uri("/Interpic.Web.Icons;component/Icons/OpenExternal.png", UriKind.RelativeOrAbsolute));
            openInWebBrowserItem.Clicked += OpenInWebBrowserItemClicked;
            webToolsItem.SubItems.Add(openInWebBrowserItem);

            MenuItem behavioursMenuItem = new MenuItem("Web Behaviours");
            behavioursMenuItem.Icon = new BitmapImage(new Uri("/Interpic.Web.Icons;component/Icons/Behaviour.png", UriKind.RelativeOrAbsolute));

            MenuItem manageWebActionsMenuItem = new MenuItem("Manage Web Actions");
            manageWebActionsMenuItem.Icon = new BitmapImage(new Uri("/Interpic.Web.Icons;component/Icons/Behaviour.png", UriKind.RelativeOrAbsolute));
            manageWebActionsMenuItem.Clicked += ManageWebActionsMenuItemClicked;
            behavioursMenuItem.SubItems.Add(manageWebActionsMenuItem);

            MenuItem manageBehavioursMenuItem = new MenuItem("Manage Web Behaviours");
            manageBehavioursMenuItem.Icon = new BitmapImage(new Uri("/Interpic.Web.Icons;component/Icons/Behaviour.png", UriKind.RelativeOrAbsolute));
            manageBehavioursMenuItem.Clicked += ManageBehavioursMenuItemClicked;
            behavioursMenuItem.SubItems.Add(manageBehavioursMenuItem);

            webToolsItem.SubItems.Add(behavioursMenuItem);
            Studio.RegisterExtensionMenuItem(webToolsItem);
            
        }

        private void ManageBehavioursMenuItemClicked(object sender, ProjectStateEventArgs e)
        {
            ManageWebBehaviours manageWebBehaviours = new ManageWebBehaviours(BehaviourConfiguration, Studio.CurrentProject);
            manageWebBehaviours.ShowDialog();
            BehaviourConfiguration = manageWebBehaviours.Configuration;
            SaveWebBehaviourConfigurationTask saveWebBehaviourConfigurationTask = new SaveWebBehaviourConfigurationTask(Studio.CurrentProject, BehaviourConfiguration);
            ProcessTaskDialog dialog = new ProcessTaskDialog(saveWebBehaviourConfigurationTask);
            dialog.ShowDialog();
        }

        private void ManageWebActionsMenuItemClicked(object sender, ProjectStateEventArgs e)
        {
            ManageWebActions manageWebActions = new ManageWebActions(BehaviourConfiguration);
            manageWebActions.ShowDialog();
        }

        private void OpenInWebBrowserItemClicked(object sender, ProjectStateEventArgs e)
        {
            if(Studio.OfflineMode)
            {
                ErrorAlert.Show("Action not allowed.\n\nOffline mode is enabled.");
                return;
            }

            if (e.Page != null && e.Version != null)
            {
                string url = e.Version.Settings.GetTextSetting("BaseUrl") + e.Page.Settings.GetTextSetting("PageUrl");
                try
                {
                    Process.Start(url);
                }
                catch (Exception ex)
                {
                    ErrorAlert.Show("Could not open page in browser:\n" + ex.Message);
                }
                
            }
            else
            {
                ErrorAlert.Show("No page selected.");
            }
        }

        private void StartSelenium()
        {
            List<BrowserType> typesToStart = new List<BrowserType>();
            foreach (Models.Version version in Studio.CurrentProject.Versions)
            {
                if (!typesToStart.Contains(GetBrowserType(version.Settings.GetMultipleChoiceSetting("BrowserType"))))
                {
                    typesToStart.Add(GetBrowserType(version.Settings.GetMultipleChoiceSetting("BrowserType")));
                }
            }
            foreach (BrowserType type in typesToStart)
            {
                StartSeleniumTask task = new StartSeleniumTask(type);
                task.Executed += SeleniumStarted;
                Studio.ScheduleBackgroundTask(task);
            }
        }

        private void PrepareObjectModel()
        {
            foreach (Models.Version version in Studio.CurrentProject.Versions)
            {
                foreach (Page page in version.Pages)
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(page.Source);
                    page.Extensions = new WebPageExtensions(document);
                }
            }
        }

        public static BrowserType GetBrowserType(string name)
        {
            switch(name)
            {
                case "chrome":
                    return BrowserType.Chrome;
                case "firefox":
                    return BrowserType.FireFox;
                default:
                    return BrowserType.Chrome;
            }
        }

        private void SeleniumStarted(object sender, AsyncTasks.EventArgs.AsyncTaskEventArgs eventArgs)
        {
            StartSeleniumTask task = eventArgs.Task as StartSeleniumTask;
            Selenium.Add(task.Type,task.Selenium);
        }

        public IControlFinder GetControlFinder()
        {
            return new WebControlFinder();
        }

        public IControlIdentifierSelector GetControlSelector()
        {
            return new NodeSelector();
        }

        public (Control control, bool succes) RefreshControl(Control control, Section section, Page page, Models.Version version, Project project)
        {
            BrowserType type = GetBrowserType(version.Settings.GetMultipleChoiceSetting("BrowserType"));
            if (CheckSelenium(type))
            {
                List<AsyncTask> tasks = new List<AsyncTask>();

                NavigateToPageTask navigateTask = new NavigateToPageTask(version.Settings.GetTextSetting("BaseUrl") + page.Settings.GetTextSetting("PageUrl"));
                navigateTask.Selenium = Selenium[type];
                navigateTask.PassThrough = true;
                navigateTask.PassThroughSource = "Selenium";
                navigateTask.PassThroughTarget = "Selenium";

                GetElementBoundsTask getElementBoundsTask = new GetElementBoundsTask(control.Identifier.Identifier);
                getElementBoundsTask.PassThrough = true;
                getElementBoundsTask.PassThroughSource = "Selenium";
                getElementBoundsTask.PassThroughTarget = "Selenium";

                tasks.Add(navigateTask);
                tasks.Add(getElementBoundsTask);

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
            else
            {
                return (control, false);
            }
        }

        public (Section section, bool succes) RefreshSection(Section section, Page page, Models.Version version, Project project)
        {
            BrowserType type = GetBrowserType(version.Settings.GetMultipleChoiceSetting("BrowserType"));
            if (CheckSelenium(type))
            {
                List<AsyncTask> tasks = new List<AsyncTask>();

                NavigateToPageTask navigateTask = new NavigateToPageTask(version.Settings.GetTextSetting("BaseUrl") + page.Settings.GetTextSetting("PageUrl"));
                navigateTask.Selenium = Selenium[type];
                navigateTask.PassThrough = true;
                navigateTask.PassThroughSource = "Selenium";
                navigateTask.PassThroughTarget = "Selenium";

                GetElementBoundsTask getElementBoundsTask = new GetElementBoundsTask(section.SectionIdentifier.Identifier);
                getElementBoundsTask.PassThrough = true;
                getElementBoundsTask.PassThroughSource = "Selenium";
                getElementBoundsTask.PassThroughTarget = "Selenium";

                tasks.Add(navigateTask);
                tasks.Add(getElementBoundsTask);

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
            else
            {
                return (section, false);
            }
        }

        public (Page page, bool succes) RefreshPage(Page page, Models.Version version, Project project)
        {
            List<AsyncTask> tasks = new List<AsyncTask>();
            BrowserType type = GetBrowserType(version.Settings.GetMultipleChoiceSetting("BrowserType"));
            if (CheckSelenium(type))
            { 
                NavigateToPageTask navigateTask = new NavigateToPageTask(version.Settings.GetTextSetting("BaseUrl") + page.Settings.GetTextSetting("PageUrl"));
                navigateTask.Selenium = Selenium[type];
                navigateTask.PassThrough = true;
                navigateTask.PassThroughSource = "Selenium";
                navigateTask.PassThroughTarget = "Selenium";

                MakeScreenshotTask makeScreenshotTask = new MakeScreenshotTask();
                makeScreenshotTask.PassThrough = true;
                makeScreenshotTask.PassThroughSource = "Selenium";
                makeScreenshotTask.PassThroughTarget = "Selenium";

                tasks.Add(navigateTask);
                tasks.Add(makeScreenshotTask);

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
            else
            {
                return (page, false);
            }
        }

        public SettingsCollection GetDefaultVersionSettings()
        {
            SettingsCollection collection = new SettingsCollection();

            Setting<string> baseUrl = new Setting<string>();
            baseUrl.Key = "BaseUrl";
            baseUrl.Name = "Base URL";
            baseUrl.Description = "Base url of the website for the manual.\nPage urls will be appended to this value.";
            baseUrl.Validator = new UrlValidator();
            baseUrl.Value = "";

            collection.TextSettings.Add(baseUrl);

            MultipleChoiceSetting browserSetting = new MultipleChoiceSetting();
            browserSetting.Key = "BrowserType";
            browserSetting.Name = "Browser";
            browserSetting.Description = "The browser to use for this version.\n\nFor all browsers:\nThe browser MUST be installed localy on the system.";
            browserSetting.Choices = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Chrome", "chrome"),
                new KeyValuePair<string, string>("Firefox", "firefox"),
            };
            browserSetting.Value = "chrome";
            collection.Name = GetProjectTypeName();
            collection.MultipleChoiceSettings.Add(browserSetting);
            return collection;
        }

        public static bool CheckSelenium(BrowserType type)
        {
            if (Selenium.ContainsKey(type))
            {
                return true;
            }
            else
            {
                ErrorAlert.Show("Please wait until selenium has started.");
                return false;
            }
        }
    }
}
