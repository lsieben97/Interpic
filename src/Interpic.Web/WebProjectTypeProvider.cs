using HtmlAgilityPack;
using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Models.Extensions;
using Interpic.Models;
using Interpic.Models.EventArgs;
using Interpic.Settings;
using Interpic.Web.Behaviours.Windows;
using Interpic.Web.Providers;
using Interpic.Web.Selenium.Tasks;
using Interpic.Web.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using static Interpic.Web.Selenium.SeleniumWrapper;
using Interpic.Web.Behaviours.Models;
using Interpic.Web.WebActions.BasicWebActions;
using Interpic.Web.Behaviours.Utils;
using Interpic.Web.Tasks;
using Interpic.Models.Behaviours;

namespace Interpic.Web
{
    public class WebProjectTypeProvider : IProjectTypeProvider
    {
        public IStudioEnvironment Studio { get; set; }
        public static Dictionary<BrowserType, Selenium.SeleniumWrapper> Selenium { get; set; } = new Dictionary<BrowserType, Selenium.SeleniumWrapper>();
        public InternetUsage InternetUsage { get; set; } = new InternetUsage();
        public ProjectCapabilities ProjectCapabilities { get; set; } = new ProjectCapabilities() { Behaviours = true };

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
            collection.Name = "Page settings";
            return collection;
        }

        public Settings.SettingsCollection GetDefaultProjectSettings()
        {
            return new SettingsCollection
            {
                Name = "Web Project Settings",
                SubSettings = new List<Setting<SettingsCollection>>
                {
                    new Setting<SettingsCollection> {
                        Name = "Debug Settings",
                        Description = "Settings for debugging the web project.",
                        Key = "debugSettings",
                        Value = new SettingsCollection
                        {
                            Name = "Debug Settings",
                            SubSettings = new List<Setting<SettingsCollection>>
                            {
                                new Setting<SettingsCollection>
                                {
                                    Key = "chromeSettings",
                                    Name = "Chrome Settings",
                                    Description = "Debug settings for Chrome.",
                                    Value = new SettingsCollection
                                    {
                                        Name = "Debug settings for Chrome",
                                        BooleanSettings = new List<Setting<bool>>
                                        {
                                            new Setting<bool>
                                            {
                                                Name = "Debug mode enabled",
                                                Key = "debugEnabled",
                                                Description = "Whether debug settings are enabled."
                                            },
                                            new Setting<bool>
                                            {
                                                Name = "Browser visible",
                                                Key = "browserVisible",
                                                Description = "Whether the browser window is visible."
                                            }
                                        }
                                    }
                                },
                                new Setting<SettingsCollection>
                                {
                                    Key = "firefoxSettings",
                                    Name = "Firefox Settings",
                                    Description = "Debug settings for Firefox.",
                                    Value = new SettingsCollection
                                    {
                                        Name = "Debug settings for Firefox",
                                        BooleanSettings = new List<Setting<bool>>
                                        {
                                            new Setting<bool>
                                            {
                                                Name = "Debug mode enabled",
                                                Key = "debugEnabled",
                                                Description = "Whether debug settings are enabled."
                                            },
                                            new Setting<bool>
                                            {
                                                Name = "Browser visible",
                                                Key = "browserVisible",
                                                Description = "Whether the browser window is visible."
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
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
            Studio.ProjectSettingsOpened += StudioProjectSettingsOpened;
        }

        private void StudioProjectSettingsOpened(object sender, ProjectSettingsEventArgs e)
        {
            if (e.Changes.SubSettingsChanges.ContainsKey("debugSettings"))
            {
                if (e.Changes.SubSettingsChanges["debugSettings"].SubSettingsChanges.ContainsKey("chromeSettings"))
                {
                    RestartSelenium(e.Settings, BrowserType.Chrome);
                }

                if (e.Changes.SubSettingsChanges["debugSettings"].SubSettingsChanges.ContainsKey("firefoxSettings"))
                {
                    RestartSelenium(e.Settings, BrowserType.FireFox);
                }
            }
        }

        private void RestartSelenium(SettingsCollection projectSettings, BrowserType browserType)
        {
            if (VersionForBrowserExists(browserType))
            {
                (bool debugModeOn, SettingsCollection browserSettings) settings = GetDebugSettings(projectSettings.GetSubSettings("debugSettings"), browserType);
                if (settings.debugModeOn)
                {
                    if (!CheckSelenium(browserType))
                    {
                        Studio.CancelScheduledBackgroundTask("start" + GetBrowserTypeString(browserType));
                        Studio.ScheduleBackgroundTask(new StartSeleniumTask(browserType, settings.browserSettings));
                    }
                }
            }
        }

        private bool VersionForBrowserExists(BrowserType type)
        {
            return Studio.CurrentProject.Versions.ToList().Find(version => version.Settings.GetMultipleChoiceSetting("BrowserType") == GetBrowserTypeString(type)) != null;
        }

        private (bool debugModeOn, SettingsCollection browserSettings) GetDebugSettings(SettingsCollection settings, BrowserType browserType)
        {
            string browserKey = "";
            if (browserType == BrowserType.Chrome)
            {
                browserKey = "chromeSettings";
            }
            else if (browserType == BrowserType.FireFox)
            {
                browserKey = "FirefoxSettings";
            }
            return (settings.GetSubSettings(browserKey).GetBooleanSetting("debugEnabled"), settings.GetSubSettings(browserKey));
        }

        private void StudioVersionAdded(object sender, VersionEventArgs e)
        {
            BrowserType type = GetBrowserType(e.Version.Settings.GetMultipleChoiceSetting("BrowserType"));
            if (!Selenium.ContainsKey(type))
            {
                (bool debugModeOn, SettingsCollection browserSettings) settings = GetDebugSettings(Studio.CurrentProject.Settings.GetSubSettings("debugSettings"), type);
                Studio.ScheduleBackgroundTask(new StartSeleniumTask(type, settings.browserSettings));
            }
        }

        private void StudioProjectUnloaded(object sender, InterpicStudioEventArgs e)
        {
            if (Selenium != null)
            {
                List<AsyncTask> tasks = new List<AsyncTask>();
                foreach (KeyValuePair<BrowserType, Selenium.SeleniumWrapper> browser in Selenium)
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
            Studio.RegisterPackageDefinition(new Models.Packaging.PackageDefinition() { Name = "Web action package", Extension = "iwp" });
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
                (bool debugModeOn, SettingsCollection browserSettings) settings = GetDebugSettings(Studio.CurrentProject.Settings.GetSubSettings("debugSettings"), type);
                StartSeleniumTask task = new StartSeleniumTask(type, settings.browserSettings);
                task.Id = "start" + GetBrowserTypeString(type);
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
                    if (page.IsLoaded)
                    {
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(page.Source);
                        page.Extensions = new WebPageExtensions(document);
                    }
                }
            }
        }

        public static BrowserType GetBrowserType(string name)
        {
            switch (name)
            {
                case "chrome":
                    return BrowserType.Chrome;
                case "firefox":
                    return BrowserType.FireFox;
                default:
                    return BrowserType.Chrome;
            }
        }

        public static string GetBrowserTypeString(BrowserType browserType)
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    return "chrome";
                case BrowserType.FireFox:
                    return "firefox";
                default:
                    return "";
            }
        }

        private void SeleniumStarted(object sender, AsyncTasks.EventArgs.AsyncTaskEventArgs eventArgs)
        {
            StartSeleniumTask task = eventArgs.Task as StartSeleniumTask;
            Selenium.Add(task.Type, task.Selenium);
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
                tasks.Add(navigateTask);

                GetElementBoundsTask getElementBoundsTask = new GetElementBoundsTask(control.Identifier.Identifier);
                getElementBoundsTask.PassThrough = true;
                getElementBoundsTask.PassThroughSource = "Selenium";
                getElementBoundsTask.PassThroughTarget = "Selenium";
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
                tasks.Add(navigateTask);

                GetElementBoundsTask getElementBoundsTask = new GetElementBoundsTask(section.SectionIdentifier.Identifier);
                getElementBoundsTask.PassThrough = true;
                getElementBoundsTask.PassThroughSource = "Selenium";
                getElementBoundsTask.PassThroughTarget = "Selenium";
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
                tasks.Add(navigateTask);

                MakeScreenshotTask makeScreenshotTask = new MakeScreenshotTask();
                makeScreenshotTask.PassThrough = true;
                makeScreenshotTask.PassThroughSource = "Selenium";
                makeScreenshotTask.PassThroughTarget = "Selenium";
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

        public SettingsCollection GetDefaultTextPageSettings()
        {
            return null;
        }

        public IBehaviourExecutionContext GetBehaviourExecutionContext()
        {
            BrowserType type = GetBrowserType(Studio.CurrentVersion.Settings.GetMultipleChoiceSetting("BrowserType"));
            if (CheckSelenium(type))
            {
                return new WebBehaviourExecutionContext(Selenium[type], Studio);
            }
            else
            {
                return null;
            }
        }

        public List<ActionPack> GetBuildInActionPacks()
        {
            return new List<ActionPack>()
            {
                new BaseWebActionPack()
            };
        }
    }
}
