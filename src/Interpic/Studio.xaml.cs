using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Models.Extensions;
using Interpic.Models;
using Interpic.Models.EventArgs;
using Interpic.Settings;
using Interpic.Studio.Functional;
using Interpic.Studio.Tasks;
using Interpic.Studio.Windows;
using Interpic.Studio.Windows.Developer;
using Interpic.Studio.Windows.Selectors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ThomasJaworski.ComponentModel;
using Interpic.Models.Packaging;
using Interpic.Studio.InternalModels;
using Interpic.Utils;
using Interpic.Studio.StudioViews;
using Interpic.UI.Controls;

namespace Interpic.Studio
{
    /// <summary>
    /// Interaction logic for Studio.xaml
    /// </summary>
    public partial class Studio : Window, IStudioEnvironment, IProcessTaskDialog
    {
        internal static List<IProjectTypeProvider> AvailableProjectTypes = new List<IProjectTypeProvider>();
        internal static List<PackageDefinition> packageDefinitions = new List<PackageDefinition>();
        internal static List<IProjectBuilder> AvailableBuilders = new List<IProjectBuilder>();
        internal static PackageCache packageCache;
        internal static Studio Instance { get; set; }
        private DispatcherTimer checkTimer = new DispatcherTimer();
        private Models.Page currentPage;
        private Models.Section currentSection;
        private Models.Version currentVersion;
        private Models.Control currentControl;
        private bool openingNewProject;
        private bool offlineMode;
        private IProgress<int> progress;
        private Dictionary<string, System.Windows.Controls.MenuItem> ExtensionMenuItems = new Dictionary<string, System.Windows.Controls.MenuItem>();
        private Dictionary<StudioTabItem, IStudioViewHandler> tabs = new Dictionary<StudioTabItem, IStudioViewHandler>();

        private Stack<BackgroundTask> backgroundTasks = new Stack<BackgroundTask>();
        private BackgroundTask backgroundTask;
        private SilentTaskProcessor taskProcessor;

        #region Events
        public event OnStudioStartup StudioStartup;
        public event OnProjectLoaded ProjectLoaded;
        public event OnProjectSettingsOpening ProjectSettingsOpening;
        public event OnProjectSettingsOpened ProjectSettingsOpened;
        public event OnPageSettingsOpening PageSettingsOpening;
        public event OnPageSettingsOpened PageSettingsOpened;
        public event OnSectionSettingsOpening SectionSettingsOpening;
        public event OnSectionSettingsOpened SectionSettingsOpened;
        public event OnControlSettingsOpening ControlSettingsOpening;
        public event OnControlSettingsOpened ControlSettingsOpened;
        public event OnStudioShutdown StudioShutdown;
        public event OnProjectUnloaded ProjectUnloaded;
        public event OnProjectCreated ProjectCreated;
        public event OnGlobalSettingsSaved GlobalSettingsSaved;
        public event OnVersionAdded VersionAdded;
        public event OnVersionRemoved VersionRemoved;
        public event OnVersionSettingsOpening VersionSettingsOpening;
        public event OnVersionSettingsOpened VersionSettingsOpened;
        public event OnPageRemoved PageRemoved;
        public event OnSectionRemoved SectionRemoved;
        public event OnControlRemoved ControlRemoved;
        public event OnPageAdded PageAdded;
        public event OnSectionAdded SectionAdded;
        public event OnControlAdded ControlAdded;
        #endregion

        public Project CurrentProject { get; set; }
        internal IProjectTypeProvider ProjectTypeProvider { get; set; }

        internal IProjectBuilder ProjectBuilder { get; set; }

        private ChangeListener projectChangeListener;
        public ILogger Logger => App.ApplicationLogger;

        public bool OfflineMode => offlineMode;

        public Studio(Models.Project project)
        {
            Instance = this;
            openingNewProject = false;
            InitializeComponent();

            progress = new Progress<int>(percent => { if (percent > 0) { pbBackgroundTask.Value = percent; pbBackgroundTask.IsIndeterminate = false; } else { pbBackgroundTask.IsIndeterminate = true; } });

            if (AvailableProjectTypes == null)
            {
                AvailableProjectTypes = new List<IProjectTypeProvider>();
                AvailableProjectTypes.Add(new Web.WebProjectTypeProvider());
            }

            CurrentProject = project;

            InitializeProjectTypeProvider(project);
            InitializeProjectBuilder(project);

            App.InitializeLogger(App.GlobalSettings.GetPathSetting("logDirectory") + "\\last.log", this);
            Logger.LogInfo("Launching studio window for project '" + project.Name + "'.", "Studio");

            StudioStartup?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));

            InitializeObjectModel(project);

            CheckDeveloperMode();
            BindEvents();
        }

        private void InitializeProjectBuilder(Project project)
        {
            if (AvailableBuilders.Any((type) => type.GetBuilderId() == CurrentProject.OutputType))
            {
                ProjectBuilder = AvailableBuilders.Single((type) => type.GetBuilderId() == CurrentProject.OutputType);
            }
            else
            {
                ErrorAlert.Show("No suitable builder found for this project.\nClosing project.");
                Close();
                new Splash().Show();
            }
        }

        private void BindEvents()
        {
            GlobalSettingsSaved += (object sender, GlobalSettingsEventArgs e) => CheckDeveloperMode();
        }

        private void CheckDeveloperMode()
        {
            if (App.GlobalSettings.GetBooleanSetting("EnableDeveloperMode") == true)
            {
                miDeveloper.Visibility = Visibility.Visible;
            }
            else
            {
                miDeveloper.Visibility = Visibility.Collapsed;
            }
        }

        private void InitializeNewProject()
        {
            currentVersion = CurrentProject.Versions[0];
            CurrentProject.LastViewedVersionId = CurrentProject.Versions[0].Id;
            lbCurrentVersion.Text = "Current version: " + currentVersion.Name;
            if (CurrentProject.HasSettingsAvailable)
            {
                if (App.GlobalSettings.GetBooleanSetting("ShowInfoForSettings"))
                {
                    InfoAlert.Show("The manual settings dialog will now be shown to further configure the project.");
                }
                SettingsCollection oldSettings = CurrentProject.Settings.Copy();
                ProjectSettingsEventArgs eventArgs = new ProjectSettingsEventArgs(this, CurrentProject, CurrentProject.Settings, null);
                ProjectSettingsOpening?.Invoke(this, eventArgs);
                ShowProjectSettings(eventArgs);
                ProjectSettingsOpened?.Invoke(this, new ProjectSettingsEventArgs(this, CurrentProject, CurrentProject.Settings, SettingsCollection.GetChanges(oldSettings, CurrentProject.Settings)));
                ProjectCreated?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
            }
        }

        private void ShowProjectSettings(ProjectSettingsEventArgs eventArgs)
        {

            SettingsEditor editor = new SettingsEditor(eventArgs.Settings);
            editor.ShowDialog();
            if (editor.DialogResult.HasValue)
            {
                if (editor.DialogResult.Value == true)
                {
                    if (editor.SettingsCollection.Validate())
                    {
                        CurrentProject.Settings = editor.SettingsCollection;
                    }
                    else
                    {
                        ErrorAlert.Show("Invalid settings entered.");
                        ShowProjectSettings(eventArgs);
                    }
                }
                else
                {
                    ErrorAlert.Show("Invalid settings entered.");
                    ShowProjectSettings(eventArgs);
                }
            }
            else
            {
                ErrorAlert.Show("Invalid settings entered.");
                ShowProjectSettings(eventArgs);
            }
        }

        private void InitializeUI()
        {
            SetStatusBar("Project loaded.");
            lbLastSaved.Text = "Last saved: " + CurrentProject.LastSaved.ToShortDateString() + " " + CurrentProject.LastSaved.ToLongTimeString();
            Title = CurrentProject.Name + " - " + currentVersion.Name + " - Interpic Studio";
            checkTimer.Interval = new TimeSpan(0, 0, 1);
            checkTimer.Tick += CheckTimer_Tick;
            checkTimer.Start();
        }

        private void InitializeProjectTypeProvider(Project project)
        {
            LoadTypeProviderForCurrentProject();
            ProjectTypeProvider.Studio = this;
            if (ProjectTypeProvider.GetDefaultProjectSettings() != null && project.Settings == null)
            {
                project.Settings = ProjectTypeProvider.GetDefaultProjectSettings();
            }
            ProjectTypeProvider.TypeProviderConnected();
        }

        private void InitializeObjectModel(Project project)
        {
            foreach (Models.Version version in CurrentProject.Versions)
            {
                version.Parent = CurrentProject;
                foreach (Models.Page page in version.Pages)
                {
                    page.Parent = version;
                    foreach (Models.Section section in page.Sections)
                    {
                        section.Parent = page;
                        foreach (Models.Control control in section.Controls)
                        {
                            control.Parent = section;
                        }
                    }
                }
            }

            projectChangeListener = ChangeListener.Create(CurrentProject);
            projectChangeListener.PropertyChanged += ProjectChangeListener_PropertyChanged;
            projectChangeListener.CollectionChanged += ProjectChangeListener_CollectionChanged;
            ProjectLoaded?.Invoke(this as IStudioEnvironment, new ProjectLoadedEventArgs(this, CurrentProject));
        }

        private void ProjectChangeListener_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InternalProjectChanged(sender.ToString());
        }

        private void ProjectChangeListener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InternalProjectChanged(e.PropertyName);
        }

        private void InternalProjectChanged(string property)
        {
            CurrentProject.Changed = true;
            imgChangeIndicator.Visibility = Visibility.Visible;
            RedrawTreeView();
        }

        private void LoadTypeProviderForCurrentProject()
        {
            if (AvailableProjectTypes.Any((type) => type.GetProjectTypeId() == CurrentProject.TypeProviderId))
            {
                ProjectTypeProvider = AvailableProjectTypes.Single((type) => type.GetProjectTypeId() == CurrentProject.TypeProviderId);
            }
            else
            {
                ErrorAlert.Show("No suitable type provider found for this project.\nClosing project.");
                Close();
                new Splash().Show();
            }
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (CurrentProject.Changed == true && CurrentProject.LastSaved.AddMinutes(App.GlobalSettings.GetNumeralSetting("SaveWarningOffset")) < DateTime.Now)
            {
                lbLastSaved.Foreground = new SolidColorBrush(Color.FromRgb(176, 0, 32));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentProject.IsNew)
            {
                InitializeNewProject();
            }
            currentVersion = CurrentProject.Versions.Single(version => version.Id == CurrentProject.LastViewedVersionId);
            currentVersion.IsCurrent = true;
            lbCurrentVersion.Text = "Current version: " + currentVersion.Name;
            lsbVersions.ItemsSource = CurrentProject.Versions;
            lsbVersions.SelectedValue = currentVersion.Id;
            lsbVersions.SelectionChanged += LsbVersions_SelectionChanged;
            RedrawTreeView();
            (tvManualTree.Items[0] as TreeViewItem).IsSelected = true;
            InitializeUI();
            if (App.GlobalSettings.GetBooleanSetting("showHomeOnProjectLoad"))
            {
                CreateAndShowStudioTab("Home", ImageUtils.ImageFromString("HomeWhite.png"), new HomeStudioView());
            }

        }

        private void StudioTabClosed(object sender, RoutedEventArgs e)
        {
            StudioTabItem tabItem = sender as StudioTabItem;
            IStudioViewHandler handler = tabs[tabItem];
            if (tabItem.DoesContainChanges && tabItem.ForceClose == false)
            {
                if (WarningAlert.Show($"Tab {tabItem.Title} has unsaved changes.\nThe tab will now be closed.", true).DialogResult.Value)
                {
                    handler.ViewDetached();
                    tcTabs.Items.Remove(sender);
                    tabs.Remove(tabItem);
                }
            }
            else
            {
                handler.ViewDetached();
                tcTabs.Items.Remove(sender);
                tabs.Remove(tabItem);
            }

        }

        public void RedrawTreeView()
        {
            string previousPath = tvManualTree.SelectedValuePath;
            if (tvManualTree.Items.Count > 0)
            {
                tvManualTree.Items.RemoveAt(0);
            }
            tvManualTree.Items.Add(Projects.GetTreeViewForProject(CurrentProject, currentVersion));
            foreach (object item in tvManualTree.Items)
            {
                TreeViewItem treeItem = tvManualTree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                    ExpandAll(treeItem, true);
                treeItem.IsExpanded = true;
            }
            tvManualTree.SelectedValuePath = previousPath;

        }

        internal void ReselectPage(Models.Page page)
        {
            page.TreeViewItem.IsSelected = false;
            page.TreeViewItem.IsSelected = true;
        }

        private void miExtensions_Click(object sender, RoutedEventArgs e)
        {
            Project project = CurrentProject;
            new ExtensionManager(ref project).ShowDialog();
            CurrentProject = project;
        }

        private void AddPage()
        {
            //if (offlineMode && ProjectTypeProvider.InternetUsage.RefreshingPage)
            //{
            //    ShowOfflineError();
            //    return;
            //}
            //NewPage dialog = new NewPage();
            //dialog.ShowDialog();
            //if (dialog.Page != null)
            //{
            //    if (ProjectTypeProvider.GetDefaultPageSettings() != null)
            //    {
            //        dialog.Page.Settings = ProjectTypeProvider.GetDefaultPageSettings();
            //        if (App.GlobalSettings.GetBooleanSetting("ShowInfoForSettings"))
            //        {
            //            InfoAlert.Show("The page settings dialog will now be shown to further configure the page.");
            //        }
            //        SettingsCollection oldSettings = dialog.Page.Settings.Copy();
            //        PageSettingsEventArgs eventArgs = new PageSettingsEventArgs(this, dialog.Page, dialog.Page.Settings, null);
            //        PageSettingsOpening?.Invoke(this, eventArgs);
            //        ShowPageSettings(dialog, eventArgs);
            //        PageSettingsOpened?.Invoke(this, new PageSettingsEventArgs(this, dialog.Page, dialog.Page.Settings, SettingsCollection.GetChanges(oldSettings, dialog.Page.Settings)));
            //    }
            //    Project currentProject = CurrentProject;
            //    Models.Page currentPage = dialog.Page;
            //    Models.Version currentVersion = this.currentVersion;
            //    string document = ProjectTypeProvider.GetSourceProvider().GetSource(ref currentProject, ref currentVersion, ref currentPage);
            //    CurrentProject = currentProject;
            //    this.currentVersion = currentVersion;
            //    dialog.Page = currentPage;
            //    if (document != null)
            //    {
            //        dialog.Page.Source = document;
            //        dialog.Page.Parent = currentVersion;

            //        (Models.Page page, bool succes) result = ProjectTypeProvider.RefreshPage(dialog.Page, currentVersion, CurrentProject);
            //        if (result.succes)
            //        {
            //            if (result.page.Screenshot != null)
            //            {
            //                currentVersion.Pages.Add(dialog.Page);
            //                RedrawTreeView();
            //                dialog.Page.TreeViewItem.IsSelected = true;
            //            }
            //            else
            //            {
            //                SetStatusBar("page not added because refreshing failed.");
            //            }
            //        }
            //    }

            //}
        }

        private void ShowPageSettings(NewPage dialog, PageSettingsEventArgs eventArgs)
        {

            SettingsEditor editor = new SettingsEditor(eventArgs.Settings);
            editor.ShowDialog();
            if (editor.DialogResult.HasValue)
            {
                if (editor.DialogResult.Value == true)
                {
                    if (editor.SettingsCollection.Validate())
                    {
                        dialog.Page.Settings = editor.SettingsCollection;
                    }
                    else
                    {
                        ErrorAlert.Show("Invalid settings entered.");
                        ShowPageSettings(dialog, eventArgs);
                    }
                }
                else
                {
                    ErrorAlert.Show("Invalid settings entered.");
                    ShowPageSettings(dialog, eventArgs);
                }
            }
            else
            {
                ErrorAlert.Show("Invalid settings entered.");
                ShowPageSettings(dialog, eventArgs);
            }
        }

        private void ShowOfflineError()
        {
            ErrorAlert.Show("Action not allowed.\n\nOffline mode is enabled.");
        }

        private void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null)
                    item.IsExpanded = true;
            }
        }

        private void miSave_Click(object sender, RoutedEventArgs e)
        {
            SaveProjectTask task = new SaveProjectTask(CurrentProject);
            ProcessTaskDialog dialog = new ProcessTaskDialog(task, "Saving...");
            dialog.ShowDialog();
            if (!dialog.TaskToExecute.IsCanceled)
            {
                SetStatusBar("Project saved.");
                lbLastSaved.Text = "Last saved: " + CurrentProject.LastSaved.ToShortDateString() + " " + CurrentProject.LastSaved.ToLongTimeString();
                lbLastSaved.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                imgChangeIndicator.Visibility = Visibility.Hidden;
            }

        }

        public void SetStatusBar(string message)
        {
            lbStatusBar.Text = message;
        }

        private void miGlobalSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsCollection oldSettings = App.GlobalSettings.Copy();
            SettingsEditor editor = new SettingsEditor(App.GlobalSettings);
            editor.ShowDialog();
            if (editor.DialogResult.Value == true)
            {
                App.GlobalSettings = editor.SettingsCollection;
                App.SaveGlobalSettings();
                SetStatusBar("Global settings saved.");
                GlobalSettingsSaved?.Invoke(this, new GlobalSettingsEventArgs(this, App.GlobalSettings, SettingsCollection.GetChanges(oldSettings, App.GlobalSettings)));
            }
            else
            {
                SetStatusBar("Changes in global settings canceled.");
            }

        }

        public List<string> GetLoadedExtensions()
        {
            List<string> extensionNames = new List<string>();
            //foreach (Extensions.Extension extension in Functional.Extensions.LoadedExtensions)
            //{
            //    extensionNames.Add(extension.GetName());
            //}
            return extensionNames;
        }

        public string GetDataFolderForExtension(string extension)
        {
            string directory = App.EXECUTABLE_DIRECTORY + "\\" + "Extensions\\" + extension;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        public string GetStudioDirectory()
        {
            return App.EXECUTABLE_DIRECTORY;
        }

        public SaveResult PromptProjectSave(string extensionName)
        {
            SaveResult result = new SaveResult();
            ConfirmAlert alert = new ConfirmAlert(extensionName + " Wants you to save your project.");
            alert.ShowDialog();
            if (alert.Result)
            {
                result.PromptAccepted = true;
                result.Saved = Projects.SaveProject(CurrentProject);
            }
            return result;
        }

        public string GetStudioVersion()
        {
            return App.VERSION;
        }

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentProject.Changed)
            {
                if (ConfirmProjectClose())
                {
                    LoadNewProject();
                }
            }
            else
            {
                LoadNewProject();
            }
        }

        private void LoadNewProject()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Select a location";
            dialog.Filter = "Interpic Project files (.ipp)|*.ipp";

            bool? result = dialog.ShowDialog();

            if (result.Value == true)
            {

                LoadProjectTask loadTask = new LoadProjectTask(dialog.FileName);
                ProcessTaskDialog taskDialog = new ProcessTaskDialog(loadTask, "Loading...");
                taskDialog.ShowDialog();
                if (!taskDialog.TaskToExecute.IsCanceled)
                {
                    ProjectUnloaded?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                    UnloadCurrentProject();
                    openingNewProject = true;
                    checkTimer.Stop();
                    new Studio(loadTask.Project).Show();
                    Close();
                }
            }
        }

        private void miProjectSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsCollection oldSettings = CurrentProject.Settings.Copy();
            ProjectSettingsEventArgs eventArgs = new ProjectSettingsEventArgs(this, CurrentProject, CurrentProject.Settings, null);
            ProjectSettingsOpening?.Invoke(this, eventArgs);
            SettingsEditor editor = new SettingsEditor(CurrentProject.Settings);
            editor.ShowDialog();
            if (editor.DialogResult.Value == true)
            {
                if (editor.SettingsCollection.Validate())
                {
                    CurrentProject.Settings = editor.SettingsCollection;
                    SetStatusBar("Project settings saved.");
                    ProjectSettingsOpened?.Invoke(this, new ProjectSettingsEventArgs(this, CurrentProject, CurrentProject.Settings, SettingsCollection.GetChanges(oldSettings, CurrentProject.Settings)));
                }
                else
                {
                    ErrorAlert.Show("Invalid settings entered.");
                }
            }
            else
            {
                SetStatusBar("Changes in project settings canceled.");
            }

        }

        private void DisplayControlHint(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (((FrameworkElement)sender).Tag != null)
            {
                if (((FrameworkElement)sender).Tag is string)
                {
                    SetStatusBar(((FrameworkElement)sender).Tag.ToString());
                }
                else
                {
                    Models.MenuItem item = ((FrameworkElement)sender).Tag as Models.MenuItem;
                    SetStatusBar(item.Description);
                }
            }
        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        private void miNewProject_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentProject.Changed)
            {
                if (ConfirmProjectClose())
                {
                    NewProject dialog = new NewProject();
                    dialog.ShowDialog();
                    if (dialog.Project != null)
                    {
                        ProjectUnloaded?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                        UnloadCurrentProject();
                        openingNewProject = true;
                        checkTimer.Stop();
                        new Studio(dialog.Project).Show();
                        Close();

                    }
                }
            }
            else
            {
                NewProject dialog = new NewProject();
                dialog.ShowDialog();
                if (dialog.Project != null)
                {
                    ProjectUnloaded?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                    UnloadCurrentProject();
                    openingNewProject = true;
                    checkTimer.Stop();
                    new Studio(dialog.Project).Show();
                    Close();
                }
            }
        }

        private void UnloadCurrentProject()
        {
            ProjectUnloaded?.Invoke(this, new InterpicStudioEventArgs(this));
            CurrentProject = null;
            currentPage = null;
            currentSection = null;
            packageDefinitions.Clear();
            LoadStudioPackageDefinitions();
            if (backgroundTask != null)
            {
                if (backgroundTask.Important)
                {
                    if (WarningAlert.Show($"An important background task is currenly running:\n{backgroundTask.ActionName}\n{backgroundTask.ImportanceReason}\n\nThe background task will now be canceled.", true).Result)
                    {
                        backgroundTask.CancellationTokenSource.Cancel();
                        backgroundTask.FireCanceledEvent(this);
                        backgroundTasks.Clear();
                    }
                }
            }
        }

        private void LoadStudioPackageDefinitions()
        {

        }

        private bool ConfirmProjectClose()
        {
            WarningAlert alert = new WarningAlert(CurrentProject.Name + " has unsaved changes.\nThe project will now be closed.");
            alert.ShowDialog();
            return alert.Result;
        }

        private void miSaveAs_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select a new project folder.";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveAsNewProjectTask task = new SaveAsNewProjectTask(CurrentProject, dialog.SelectedPath);
                ProcessTaskDialog saveDialog = new ProcessTaskDialog(task, "Saving...");
                saveDialog.ShowDialog();
                if (!saveDialog.TaskToExecute.IsCanceled)
                {
                    SetStatusBar("Project saved.");
                }
            }
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentProject.Changed)
            {
                if (ConfirmProjectClose())
                {
                    ProjectUnloaded?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                    UnloadCurrentProject();
                    StudioShutdown?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                    Close();
                }
            }

        }

        private void miBuildProject_Click(object sender, RoutedEventArgs e)
        {
            new Build(ProjectBuilder, CurrentProject).ShowDialog();
        }

        private void ShowSectionSettings(ref NewSection dialog, SectionSettingsEventArgs eventArgs)
        {
            SettingsEditor editor = new SettingsEditor(eventArgs.Settings);
            editor.ShowDialog();
            if (editor.DialogResult.HasValue)
            {
                if (editor.DialogResult.Value == true)
                {
                    if (editor.SettingsCollection.Validate())
                    {
                        dialog.Section.Settings = editor.SettingsCollection;
                    }
                    else
                    {
                        ErrorAlert.Show("Invalid settings entered.");
                        ShowSectionSettings(ref dialog, eventArgs);
                    }
                }
                else
                {
                    ErrorAlert.Show("Invalid settings entered.");
                    ShowSectionSettings(ref dialog, eventArgs);
                }
            }
            else
            {
                ErrorAlert.Show("Invalid settings entered.");
                ShowSectionSettings(ref dialog, eventArgs);
            }
        }

        private void ShowControlSettings(ref AddControl dialog, ControlSettingsEventArgs eventArgs)
        {
            SettingsEditor editor = new SettingsEditor(eventArgs.Settings);
            editor.ShowDialog();
            if (editor.DialogResult.HasValue)
            {
                if (editor.DialogResult.Value == true)
                {
                    if (editor.SettingsCollection.Validate())
                    {
                        dialog.Control.Settings = editor.SettingsCollection;
                    }
                    else
                    {
                        ErrorAlert.Show("Invalid settings entered.");
                        ShowControlSettings(ref dialog, eventArgs);
                    }
                }
                else
                {
                    ErrorAlert.Show("Invalid settings entered.");
                    ShowControlSettings(ref dialog, eventArgs);
                }
            }
            else
            {
                ErrorAlert.Show("Invalid settings entered.");
                ShowControlSettings(ref dialog, eventArgs);
            }
        }

        private void MiOpenObjectModelViewer_Click(object sender, RoutedEventArgs e)
        {
            new ObjectModelViewer(CurrentProject).ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!openingNewProject)
            {
                UnloadCurrentProject();
                StudioShutdown?.Invoke(this, new InterpicStudioEventArgs(this));
                Application.Current.Shutdown();
            }
        }

        private void MiExportJson_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "Select a location";
            dialog.Filter = "JSON files (.json)|*.json";

            bool? result = dialog.ShowDialog();

            if (result.Value == true)
            {
                SaveProjectAsJsonTask task = new SaveProjectAsJsonTask(CurrentProject, dialog.FileName);
                ProcessTaskDialog taskDialog = new ProcessTaskDialog(task, "Exporting...");
                if (!taskDialog.TaskToExecute.IsCanceled)
                {
                    SetStatusBar("Manual exported.");
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            InfoAlert.Show("No user manual available now.");
        }

        private void SwitchVersion(string id)
        {
            currentVersion = CurrentProject.Versions.Single(version => version.Id == id);
            SetStatusBar("Switched to version '" + currentVersion.Name + "'.");
            CurrentProject.LastViewedVersionId = currentVersion.Id;
            RedrawTreeView();
            CurrentProject.TreeViewItem.IsSelected = true;
            Title = CurrentProject.Name + " - " + currentVersion.Name + " - Interpic Studio";
            lbCurrentVersion.Text = "Current version: " + currentVersion.Name;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (CurrentProject.Changed)
            {
                if (!ConfirmProjectClose())
                {
                    e.Cancel = true;
                }
            }
        }

        private void MiShowLog_Click(object sender, RoutedEventArgs e)
        {
            new Log(Logger as Logger).Show();
        }

        public void ScheduleBackgroundTask(BackgroundTask task)
        {
            task.IsCancelable = true;
            if (backgroundTask != null)
            {
                backgroundTasks.Push(task);
            }
            else
            {
                backgroundTask = task;
                ExecuteBackgroundTask();
            }
        }

        private void ExecuteBackgroundTask()
        {
            backgroundTask.BeforeExecution();
            taskProcessor = new SilentTaskProcessor(backgroundTask, this);
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(() =>
                {
                    pbBackgroundTask.IsIndeterminate = true;
                    if (backgroundTask.IsIndeterminate)
                    {
                        imgBackgroundTaskLoading.Visibility = Visibility.Visible;
                        pbBackgroundTask.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        imgBackgroundTaskLoading.Visibility = Visibility.Collapsed;
                        pbBackgroundTask.Visibility = Visibility.Visible;
                    }
                    lbCurrentBackgroundTask.Text = backgroundTask.TaskName;
                });
            }
            else
            {
                pbBackgroundTask.IsIndeterminate = true;
                if (backgroundTask.IsIndeterminate)
                {
                    imgBackgroundTaskLoading.Visibility = Visibility.Visible;
                    pbBackgroundTask.Visibility = Visibility.Collapsed;
                }
                else
                {
                    imgBackgroundTaskLoading.Visibility = Visibility.Collapsed;
                    pbBackgroundTask.Visibility = Visibility.Visible;
                }
                lbCurrentBackgroundTask.Text = backgroundTask.TaskName;
            }

            backgroundTask.Executed += CheckForNewTasks;
            backgroundTask.Canceled += CheckForNewTasks;
            taskProcessor.ProcessTask();
        }

        private void CheckForNewTasks(object sender, AsyncTasks.EventArgs.AsyncTaskEventArgs eventArgs)
        {
            backgroundTask = null;
            if (backgroundTasks.Count > 0)
            {
                backgroundTask = backgroundTasks.Pop();
                ExecuteBackgroundTask();
            }
            else
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        lbCurrentBackgroundTask.Text = string.Empty;
                        pbBackgroundTask.Value = 0;
                        pbBackgroundTask.Visibility = Visibility.Collapsed;
                        imgBackgroundTaskLoading.Visibility = Visibility.Collapsed;
                    });
                }
                else
                {
                    lbCurrentBackgroundTask.Text = string.Empty;
                    pbBackgroundTask.Value = 0;
                    pbBackgroundTask.Visibility = Visibility.Collapsed;
                    imgBackgroundTaskLoading.Visibility = Visibility.Collapsed;
                }

            }
        }

        public bool CancelScheduledBackgroundTask(string id)
        {
            if (backgroundTasks.Any(task => task.Id == id))
            {
                List<BackgroundTask> list = backgroundTasks.ToList();
                BackgroundTask task = list.Find(t => t.Id == id);
                if (backgroundTask.Dialog != null)
                {
                    backgroundTask.CancellationTokenSource.Cancel();
                    backgroundTask.FireCanceledEvent(this);
                }
                task.FireCanceledEvent(this);
                list.Remove(task);
                backgroundTasks = new Stack<BackgroundTask>(list);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ReportProgress(int progress, bool indeterminate = false)
        {
            if (progress < 0)
            {
                progress = 0;
            }
            if (progress > 100)
            {
                progress = 100;
            }
            if (indeterminate)
            {
                this.progress.Report(-1);
            }
            else
            {
                this.progress.Report(progress);
            }
        }

        public void CancelAllTasks(string errorMessage = null)
        {
            backgroundTask.IsCanceled = true;
            backgroundTask.FireCanceledEvent(this);
            if (errorMessage != null)
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ErrorAlert.Show(errorMessage);
                    });
                }
                else
                {
                    ErrorAlert.Show(errorMessage);
                }
            }
        }

        public bool RegisterExtensionMenuItem(Models.MenuItem menuItem)
        {
            try
            {
                System.Windows.Controls.MenuItem menuItemControl = CreateMenuItem(menuItem);
                menuItemControl.HorizontalAlignment = HorizontalAlignment.Right;
                ExtensionMenuItems.Add(menuItem.Id, menuItemControl);
                mStudio.Items.Add(menuItemControl);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error while registering extension menu items: {ex.Message}", "Studio");
                return false;
            }
        }

        private System.Windows.Controls.MenuItem CreateMenuItem(Models.MenuItem menuItem)
        {
            System.Windows.Controls.MenuItem item = new System.Windows.Controls.MenuItem();
            item.Header = menuItem.Name;
            if (menuItem.Icon != null)
            {
                Image image = new Image();
                image.Source = menuItem.Icon;
                item.Icon = image;
            }
            item.Click += ExtensionMenuItemClick;
            item.MouseEnter += DisplayControlHint;
            item.Tag = menuItem;
            foreach (Models.MenuItem subItem in menuItem.SubItems)
            {
                item.Items.Add(CreateMenuItem(subItem));
            }
            return item;
        }

        private void ExtensionMenuItemClick(object sender, RoutedEventArgs e)
        {
            Models.MenuItem item = ((System.Windows.Controls.MenuItem)e.Source).Tag as Models.MenuItem;
            item.FireClickedEvent(this, new ProjectStateEventArgs(this, CurrentProject, currentVersion, currentPage, currentSection, currentControl));
            e.Handled = true;
        }

        public void RemoveExtensionMenuItem(string menuItemId)
        {
            if (ExtensionMenuItems.ContainsKey(menuItemId))
            {
                mStudio.Items.Remove(ExtensionMenuItems[menuItemId]);
                ExtensionMenuItems.Remove(menuItemId);
            }
        }

        private void ImOfflineModeIndicator_MouseUp(object sender, MouseButtonEventArgs e)
        {
            offlineMode = false;
            imOfflineModeIndicator.Visibility = Visibility.Collapsed;
            InfoAlert.Show("Offline mode disabled.");
        }

        private void MiOfflineMode_Click(object sender, RoutedEventArgs e)
        {
            if (offlineMode)
            {
                offlineMode = false;
                imOfflineModeIndicator.Visibility = Visibility.Collapsed;
                InfoAlert.Show("Offline mode disabled.");
            }
            else
            {
                offlineMode = true;
                imOfflineModeIndicator.Visibility = Visibility.Visible;
                WarningAlert.Show("Offline mode is now on. Actions that require internet access are not available.\n\nClick this button again to disable offline mode.");
            }
        }

        public IDLLManager GetDLLManager()
        {
            return DLLManager.Instance;
        }

        public void RegisterPackageDefinition(PackageDefinition definition)
        {
            if (packageDefinitions.Contains(definition))
            {
                Logger.LogError("Tried to register same defintion twice, ignoring.", "Studio");
                return;
            }

            packageDefinitions.Add(definition);
        }

        private void MiCreatePackage_Click(object sender, RoutedEventArgs e)
        {
            new CreatePackage(packageDefinitions).ShowDialog();
        }

        private void LsbVersions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lsbVersions.SelectedValue != null)
            {
                SwitchVersion(lsbVersions.SelectedValue.ToString());
            }
        }

        public void CreateAndShowStudioTab<T>(string title, ImageSource icon, StudioView<T> view) where T : UserControl, IStudioViewHandler
        {
            StudioTabItem tabItem = new StudioTabItem();
            tabItem.Icon = icon;
            tabItem.Title = title;
            tcTabs.Items.Add(tabItem);
            view.Title = title;
            tabItem.SetStudioView(view.Title, view.Icon, view.View);
            tabItem.CloseTab += StudioTabClosed;
            tabItem.IsSelected = true;
            view.View.Studio = this;
            view.View.StudioTab = tabItem;
            view.View.ViewAttached();
            tabs.Add(tabItem, view.View);
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tvManualTree.SelectedItem != null)
            {
                object selection = (tvManualTree.SelectedItem as TreeViewItem).Tag;

                if (selection.GetType() == typeof(Models.Page))
                {
                    Models.Page page = selection as Models.Page;
                    ShowBuiltinStudioView(page);
                }
                else if (selection.GetType() == typeof(Section))
                {
                    Section section = selection as Models.Section;
                    ShowBuiltinStudioView(section);
                }
                else if (selection.GetType() == typeof(Models.Control))
                {
                    Models.Control control = selection as Models.Control;
                    ShowBuiltinStudioView(control);
                }
                else if (selection.GetType() == typeof(Project))
                {
                    ShowBuiltinStudioView(BuildInStudioViews.Version);
                }
            }
        }

        public bool TabWithContentExists(string contents)
        {
            return tabs.Any(t => t.Value.GetTabContents() == contents);
        }

        public void ShowBuiltinStudioView(BuildInStudioViews type)
        {
            switch (type)
            {
                case BuildInStudioViews.Home:
                    if (TabWithContentExists("Home"))
                    {
                        ShowExistingTab("Home");
                    }
                    else
                    {
                        CreateAndShowStudioTab("Home", ImageUtils.ImageFromString("HomeWhite.png"), new HomeStudioView());
                    }
                    break;
                case BuildInStudioViews.ManageVersions:
                    if (TabWithContentExists("ManageVersions"))
                    {
                        ShowExistingTab("ManageVersions");
                    }
                    else
                    {
                        CreateAndShowStudioTab("Manage Versions", ImageUtils.ImageFromString("VersionWhite.png"), new ManageVersionsStudioView(CurrentProject, ProjectTypeProvider, this));
                    }
                    break;
                case BuildInStudioViews.Version:
                    if (TabWithContentExists(currentVersion.Id))
                    {
                        ShowExistingTab(currentVersion.Id);
                    }
                    else
                    {
                        ProjectStudioView studioView = new ProjectStudioView(currentVersion);
                        CreateAndShowStudioTab(CurrentProject.Name, ImageUtils.ImageFromString("ProjectWhite.png"), studioView);
                    }
                    break;
            }
        }

        public bool ShowManualItemSettings(Project project)
        {
            if (project.HasSettingsAvailable)
            {
                ProjectSettingsEventArgs eventArgs = new ProjectSettingsEventArgs(this, project, project.Settings, null);
                ProjectSettingsOpening?.Invoke(this, eventArgs);
                SettingsEditor editor = new SettingsEditor(eventArgs.Settings);
                editor.ShowDialog();
                eventArgs = new ProjectSettingsEventArgs(this, project, editor.SettingsCollection, SettingsCollection.GetChanges(project.Settings, editor.SettingsCollection));
                project.Settings = editor.SettingsCollection;
                ProjectSettingsOpened?.Invoke(this, eventArgs);
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool ShowManualItemSettings(Models.Version version)
        {
            if (version.HasSettingsAvailable)
            {
                VersionSettingsEventArgs eventArgs = new VersionSettingsEventArgs(this, version, version.Settings, null);
                VersionSettingsOpening?.Invoke(this, eventArgs);
                version.FireSettingsOpeningEvent(this, eventArgs);
                SettingsEditor editor = new SettingsEditor(eventArgs.Settings);
                editor.ShowDialog();
                eventArgs = new VersionSettingsEventArgs(this, version, editor.SettingsCollection, SettingsCollection.GetChanges(version.Settings, editor.SettingsCollection));
                version.Settings = editor.SettingsCollection;
                VersionSettingsOpened?.Invoke(this, eventArgs);
                version.FireSettingsOpenedEvent(this, eventArgs);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddPage(Models.Version version, bool autoShow)
        {
            if (offlineMode && ProjectTypeProvider.InternetUsage.RefreshingPage)
            {
                ShowOfflineError();
                return false;
            }

            NewPage dialog = new NewPage();
            dialog.ShowDialog();
            if (dialog.Page != null)
            {
                if (dialog.Page.Type == Models.Page.PAGE_TYPE_REFERENCE)
                {
                    if (ProjectTypeProvider.GetDefaultPageSettings() != null)
                    {
                        dialog.Page.Settings = ProjectTypeProvider.GetDefaultPageSettings();
                        if (App.GlobalSettings.GetBooleanSetting("ShowInfoForSettings"))
                        {
                            InfoAlert.Show("The page settings dialog will now be shown to further configure the page.");
                        }
                        SettingsCollection oldSettings = dialog.Page.Settings.Copy();
                        PageSettingsEventArgs eventArgs = new PageSettingsEventArgs(this, dialog.Page, dialog.Page.Settings, null);
                        PageSettingsOpening?.Invoke(this, eventArgs);
                        dialog.Page.FireSettingsOpeningEvent(this, eventArgs);
                        ShowPageSettings(dialog, eventArgs);
                        PageSettingsOpened?.Invoke(this, new PageSettingsEventArgs(this, dialog.Page, dialog.Page.Settings, SettingsCollection.GetChanges(oldSettings, dialog.Page.Settings)));
                        dialog.Page.FireSettingsOpenedEvent(this, new PageSettingsEventArgs(this, dialog.Page, dialog.Page.Settings, SettingsCollection.GetChanges(oldSettings, dialog.Page.Settings)));

                    }
                }
                else
                {
                    if (ProjectTypeProvider.GetDefaultTextPageSettings() != null)
                    {
                        dialog.Page.Settings = ProjectTypeProvider.GetDefaultTextPageSettings();
                        if (App.GlobalSettings.GetBooleanSetting("ShowInfoForSettings"))
                        {
                            InfoAlert.Show("The page settings dialog will now be shown to further configure the page.");
                        }
                        SettingsCollection oldSettings = dialog.Page.Settings.Copy();
                        PageSettingsEventArgs eventArgs = new PageSettingsEventArgs(this, dialog.Page, dialog.Page.Settings, null);
                        PageSettingsOpening?.Invoke(this, eventArgs);
                        dialog.Page.FireSettingsOpeningEvent(this, eventArgs);
                        ShowPageSettings(dialog, eventArgs);
                        PageSettingsOpened?.Invoke(this, new PageSettingsEventArgs(this, dialog.Page, dialog.Page.Settings, SettingsCollection.GetChanges(oldSettings, dialog.Page.Settings)));
                        dialog.Page.FireSettingsOpenedEvent(this, new PageSettingsEventArgs(this, dialog.Page, dialog.Page.Settings, SettingsCollection.GetChanges(oldSettings, dialog.Page.Settings)));

                    }
                }
                dialog.Page.Parent = version;
                version.Pages.Add(dialog.Page);
                RedrawTreeView();
                PageAdded?.Invoke(this, new PageEventArgs(this, dialog.Page));
                if (autoShow)
                {
                    dialog.Page.TreeViewItem.IsSelected = true;
                    dialog.Page.TreeViewItem.BringIntoView();
                    ShowBuiltinStudioView(dialog.Page);
                }
                return true;
            }

            return false;
        }

        private void ShowExistingTab(string contents)
        {
            if (tabs.Any((tab) => tab.Value.GetTabContents() == contents))
            {
                IEnumerable<KeyValuePair<StudioTabItem, IStudioViewHandler>> tabs = this.tabs.Where((tab) => tab.Value.GetTabContents() == contents);
                if (tabs.Any())
                {
                    (tabs.First().Key as IStudioTab).Focus();
                }
            }
        }

        public bool CloseTab(string contents, bool force)
        {
            if (tabs.Any((tab) => tab.Value.GetTabContents() == contents))
            {
                IEnumerable<KeyValuePair<StudioTabItem, IStudioViewHandler>> tabs = this.tabs.Where((tab) => tab.Value.GetTabContents() == contents);
                if (tabs.Any())
                {
                    tabs.First().Value.StudioTab.ForceClose = force;
                    tabs.First().Value.StudioTab.CloseTab();
                    return force ? true : !tabs.First().Value.StudioTab.DoesContainChanges;
                }
            }
            return false;
        }

        public void ShowBuiltinStudioView(Models.Page page)
        {
            if (TabWithContentExists(page.Id))
            {
                ShowExistingTab(page.Id);
            }
            else
            {
                PageStudioView studioView = new PageStudioView(page);
                CreateAndShowStudioTab(page.Name, ImageUtils.ImageFromString("PageWhite.png"), studioView);
            }
        }

        public void ShowBuiltinStudioView(Section section)
        {
            if (TabWithContentExists(section.Id))
            {
                ShowExistingTab(section.Id);
            }
            else
            {
                SectionStudioView studioView = new SectionStudioView(section);
                CreateAndShowStudioTab(section.Name, ImageUtils.ImageFromString("SectionWhite.png"), studioView);
            }
        }

        public void ShowBuiltinStudioView(Models.Control control)
        {
            if (TabWithContentExists(control.Id))
            {
                ShowExistingTab(control.Id);
            }
            else
            {
                ControlStudioView studioView = new ControlStudioView(control);
                CreateAndShowStudioTab(control.Name, ImageUtils.ImageFromString("ControlWhite.png"), studioView);
            }
        }

        public bool ShowManualItemSettings(Models.Page page)
        {
            if (page.HasSettingsAvailable)
            {
                PageSettingsEventArgs eventArgs = new PageSettingsEventArgs(this, page, page.Settings, null);
                PageSettingsOpening?.Invoke(this, eventArgs);
                page.FireSettingsOpeningEvent(this, eventArgs);
                SettingsEditor editor = new SettingsEditor(eventArgs.Settings);
                editor.ShowDialog();
                eventArgs = new PageSettingsEventArgs(this, page, editor.SettingsCollection, SettingsCollection.GetChanges(page.Settings, editor.SettingsCollection));
                page.Settings = editor.SettingsCollection;
                PageSettingsOpened?.Invoke(this, eventArgs);
                page.FireSettingsOpenedEvent(this, eventArgs);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ShowManualItemSettings(Section section)
        {
            if (section.HasSettingsAvailable)
            {
                SectionSettingsEventArgs eventArgs = new SectionSettingsEventArgs(this, section, section.Settings, null);
                SectionSettingsOpening?.Invoke(this, eventArgs);
                section.FireSettingsOpeningEvent(this, eventArgs);
                SettingsEditor editor = new SettingsEditor(eventArgs.Settings);
                editor.ShowDialog();
                eventArgs = new SectionSettingsEventArgs(this, section, editor.SettingsCollection, SettingsCollection.GetChanges(section.Settings, editor.SettingsCollection));
                section.Settings = editor.SettingsCollection;
                SectionSettingsOpened?.Invoke(this, eventArgs);
                section.FireSettingsOpenedEvent(this, eventArgs);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ShowManualItemSettings(Models.Control control)
        {
            if (control.HasSettingsAvailable)
            {
                ControlSettingsEventArgs eventArgs = new ControlSettingsEventArgs(this, control, control.Settings, null);
                ControlSettingsOpening?.Invoke(this, eventArgs);
                control.FireSettingsOpeningEvent(this, eventArgs);
                SettingsEditor editor = new SettingsEditor(eventArgs.Settings);
                editor.ShowDialog();
                eventArgs = new ControlSettingsEventArgs(this, control, editor.SettingsCollection, SettingsCollection.GetChanges(control.Settings, editor.SettingsCollection));
                control.Settings = editor.SettingsCollection;
                ControlSettingsOpened?.Invoke(this, eventArgs);
                control.FireSettingsOpenedEvent(this, eventArgs);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddVersion(bool autoSwitch)
        {
            NewVersion dialog = new NewVersion(ProjectTypeProvider.GetDefaultVersionSettings());
            if (dialog.ShowDialog().Value)
            {
                dialog.Version.Parent = CurrentProject;
                CurrentProject.Versions.Add(dialog.Version);
                VersionAdded?.Invoke(this, new VersionEventArgs(this, dialog.Version));
                if (autoSwitch)
                {
                    SwitchVersion(dialog.Version.Id);
                }
                return true;
            }
            return false;
        }

        public bool AddSection(Models.Page page, bool autoShow)
        {
            if (page.Type == Models.Page.PAGE_TYPE_TEXT)
            {
                WarningAlert.Show("the parent page is a text page and cannot contain any sections");
                return false;
            }
            if (page.IsLoaded == false)
            {
                WarningAlert.Show("The page must be loaded before any sections can be added.");
                return false;
            }
            NewSection dialog = new NewSection(page);
            dialog.sectionIdentifierSelector = ProjectTypeProvider.GetSectionSelector();
            dialog.ShowDialog();
            if (dialog.Section != null)
            {
                dialog.Section.Parent = page;
                dialog.Section.Settings = ProjectTypeProvider.GetDefaultSectionSettings();
                if (dialog.Section.HasSettingsAvailable)
                {
                    SettingsCollection oldSettings = dialog.Section.Settings.Copy();
                    SectionSettingsEventArgs eventArgs = new SectionSettingsEventArgs(this, dialog.Section, dialog.Section.Settings, null);
                    SectionSettingsOpening?.Invoke(this, eventArgs);
                    ShowSectionSettings(ref dialog, eventArgs);
                    SectionSettingsOpened?.Invoke(this, new SectionSettingsEventArgs(this, dialog.Section, dialog.Section.Settings, SettingsCollection.GetChanges(oldSettings, dialog.Section.Settings)));
                }

                if (ProjectTypeProvider.GetControlFinder() != null)
                {
                    ObservableCollection<DiscoveredControl> foundControls = ProjectTypeProvider.GetControlFinder().FindControls(dialog.Section);
                    if (foundControls == null)
                    {
                        foundControls = new ObservableCollection<DiscoveredControl>();
                    }

                    dialog.Section.DiscoveredControls = foundControls;
                    SetStatusBar("Found " + foundControls.Count.ToString() + " controls.");
                }
                else
                {
                    SetStatusBar("No control finder found for this project type, skipping control discovery.");
                }

                dialog.Section.Parent = page;
                page.Sections.Add(dialog.Section);
                RedrawTreeView();
                SectionAdded?.Invoke(this, new SectionEventArgs(this, dialog.Section));
                if (autoShow)
                {
                    dialog.Section.TreeViewItem.IsSelected = true;
                    dialog.Section.TreeViewItem.BringIntoView();
                    ShowBuiltinStudioView(dialog.Section);
                }
                return true;
            }
            return false;
        }

        public bool AddControl(Section section, bool autoShow)
        {
            if (section.IsLoaded == false)
            {
                WarningAlert.Show("The section must be loaded before any controls can be added.");
                return false;
            }

            if (offlineMode && ProjectTypeProvider.InternetUsage.RefreshingControl)
            {
                ShowOfflineError();
                return false;
            }
            IControlIdentifierSelector selector = ProjectTypeProvider.GetControlSelector();
            selector.Section = section;
            AddControl dialog = new AddControl(section.DiscoveredControls, selector);
            dialog.ShowDialog();
            if (dialog.Control != null)
            {
                dialog.Control.Parent = section;
                dialog.Control.Settings = ProjectTypeProvider.GetDefaultSectionSettings();
                if (dialog.Control.HasSettingsAvailable)
                {
                    SettingsCollection oldSettings = dialog.Control.Settings.Copy();
                    ControlSettingsEventArgs eventArgs = new ControlSettingsEventArgs(this, dialog.Control, dialog.Control.Settings, null);
                    ControlSettingsOpening?.Invoke(this, eventArgs);
                    ShowControlSettings(ref dialog, eventArgs);
                    ControlSettingsOpened?.Invoke(this, new ControlSettingsEventArgs(this, dialog.Control, dialog.Control.Settings, SettingsCollection.GetChanges(oldSettings, dialog.Control.Settings)));
                }
                dialog.Control.Parent = section;
                section.Controls.Add(dialog.Control);
                RedrawTreeView();
                ControlAdded?.Invoke(this, new ControlEventArgs(this, dialog.Control));
                if (autoShow)
                {
                    dialog.Control.TreeViewItem.IsSelected = true;
                    dialog.Control.TreeViewItem.BringIntoView();
                    ShowBuiltinStudioView(dialog.Control);
                }
                return true;
            }
            return false;
        }

        (Models.Page page, bool succes) IStudioEnvironment.LoadManualItem(Models.Page page)
        {
            if (page.IsLoaded)
            {
                return (page, false);
            }
            Models.Page sourceResult = ProjectTypeProvider.GetSourceProvider().GetSource(page);
            if (sourceResult != null)
            {
                page = sourceResult;
            }
            else
            {
                return (page, false);
            }

            (Models.Page page, bool succes) result = ProjectTypeProvider.RefreshPage(page, page.Parent, CurrentProject);
            if (result.succes)
            {
                result.page.IsLoaded = true;
            }
            return result;
        }

        (Section page, bool succes) IStudioEnvironment.LoadManualItem(Section section)
        {
            if (section.IsLoaded)
            {
                return (section, false);
            }

            (Models.Section section, bool succes) result = ProjectTypeProvider.RefreshSection(section, section.Parent, section.Parent.Parent, CurrentProject);
            if (result.succes)
            {
                result.section.IsLoaded = true;
            }
            return result;
        }

        (Models.Control control, bool succes) IStudioEnvironment.LoadManualItem(Models.Control control)
        {
            if (control.IsLoaded)
            {
                return (control, false);
            }

            (Models.Control control, bool succes) result = ProjectTypeProvider.RefreshControl(control, control.Parent, control.Parent.Parent, control.Parent.Parent.Parent, CurrentProject);
            if (result.succes)
            {
                result.control.IsLoaded = true;
            }
            return result;
        }

        public bool RemoveManualItem(Models.Version version, bool confirm)
        {
            if (confirm)
            {
                if (ConfirmAlert.Show($"The version '{version.Name}' and all it's pages will be removed.").Result == false)
                {
                    return false;
                }
            }

            List<string> ids = ObjectModelUtils.GetAllIds(version);
            foreach (string id in ids)
            {
                if (TabWithContentExists(id))
                {
                    CloseTab(id, true);
                }
            }

            VersionRemoved?.Invoke(this, new VersionEventArgs(this, version));
            version.FireRemovedEvent(this, new VersionEventArgs(this, version));
            version.Parent.Versions.Remove(version);
            RedrawTreeView();
            return true;
        }

        public bool RemoveManualItem(Models.Page page, bool confirm)
        {
            if (confirm)
            {
                if (ConfirmAlert.Show($"The page '{page.Name}' and all it's sections will be removed.").Result == false)
                {
                    return false;
                }
            }

            List<string> ids = ObjectModelUtils.GetAllIds(page);
            foreach (string id in ids)
            {
                if (TabWithContentExists(id))
                {
                    CloseTab(id, true);
                }
            }

            PageRemoved?.Invoke(this, new PageEventArgs(this, page));
            page.FireRemovedEvent(this, new PageEventArgs(this, page));
            page.Parent.Pages.Remove(page);
            RedrawTreeView();
            return true;
        }

        public bool RemoveManualItem(Section section, bool confirm)
        {
            if (confirm)
            {
                if (ConfirmAlert.Show($"The section '{section.Name}' and all it's controls will be removed.").Result == false)
                {
                    return false;
                }
            }

            List<string> ids = ObjectModelUtils.GetAllIds(section);
            foreach (string id in ids)
            {
                if (TabWithContentExists(id))
                {
                    CloseTab(id, true);
                }
            }

            SectionRemoved?.Invoke(this, new SectionEventArgs(this, section));
            section.FireRemovedEvent(this, new SectionEventArgs(this, section));
            section.Parent.Sections.Remove(section);
            RedrawTreeView();
            return true;
        }

        public bool RemoveManualItem(Models.Control control, bool confirm)
        {
            if (confirm)
            {
                if (ConfirmAlert.Show($"The control '{control.Name}' and all it's controls will be removed.").Result == false)
                {
                    return false;
                }
            }

            if (TabWithContentExists(control.Id))
            {
                CloseTab(control.Id, true);
            }


            ControlRemoved?.Invoke(this, new ControlEventArgs(this, control));
            control.FireRemovedEvent(this, new ControlEventArgs(this, control));
            control.Parent.Controls.Remove(control);
            RedrawTreeView();
            return true;
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowBuiltinStudioView(BuildInStudioViews.ManageVersions);
        }
    }
}
