using Interpic.AsyncTasks;
using Interpic.Models.Extensions;
using Interpic.Models.Packaging;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Interpic.Models
{
    public interface IStudioEnvironment
    {
        /// <summary>
        /// Get the names of the extensions that are currently loaded.
        /// </summary>
        /// <returns>The names of the extensions that are currently loaded.</returns>
        List<string> GetLoadedExtensions();

        /// <summary>
        /// Get the path to the folder the given extension can store it's data in.
        /// if it doesn't exist calling this method will create the folder.
        /// </summary>
        /// <param name="extensionName">The name of the extension.</param>
        /// <returns>The path to the folder the given extension can store it's data in.</returns>
        string GetDataFolderForExtension(string extension);

        /// <summary>
        /// Get the path to the directory Interpic Studio is installed in.
        /// </summary>
        /// <returns>The path to the directory Interpic Studio is installed in.</returns>
        string GetStudioDirectory();

        /// <summary>
        /// Prompt the user to save the current project.
        /// </summary>
        /// <param name="extensionName">The name of the extension prompting the save.</param>
        /// <returns>Whether the user saved and if saving was sucessfull.</returns>
        SaveResult PromptProjectSave(string extensionName);

        /// <summary>
        /// Get the version of Interpic Studio.
        /// Format: x.y.z
        /// </summary>
        /// <returns>The version of Interpic Studio.</returns>
        string GetStudioVersion();

        /// <summary>
        /// The project that's currently loaded.
        /// </summary>
        Project CurrentProject { get; }

        /// <summary>
        /// Logger to write to the log for this session.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Schedule the given task to be executed in the background. Use the <see cref="AsyncTask.Executed"/> and <see cref="AsyncTask.Canceled"/> events to listen for when the task is executed.
        /// </summary>
        /// <param name="Task"></param>
        void ScheduleBackgroundTask(BackgroundTask task);

        /// <summary>
        /// Cancel the scheduled background task with the given id.
        /// </summary>
        /// <param name="id">The id of the background task to cancel.</param>
        /// <returns>true when the task has been canceled. false when no task with the given id could be found or when the task is already executing.</returns>
        bool CancelScheduledBackgroundTask(string id);

        /// <summary>
        /// Register the given menu item as an extra extension menu item and show it in the studio.
        /// </summary>
        /// <param name="menuItem">The menu item to register.</param>
        /// <returns>Whether registration was succesfull.</returns>
        bool RegisterExtensionMenuItem(MenuItem menuItem);

        IDLLManager GetDLLManager();

        void RemoveExtensionMenuItem(string menuItemId);

        bool OfflineMode { get; }

        void RegisterPackageDefinition(PackageDefinition definition);

        void CreateAndShowStudioTab<T>(string title, ImageSource icon, StudioView<T> view) where T : UserControl, IStudioViewHandler;

        bool TabWithContentExists(string contents);

        bool CloseTab(string contents, bool force);

        void ShowBuiltinStudioView(BuildInStudioViews type);
        void ShowBuiltinStudioView(Page page);
        void ShowBuiltinStudioView(Section section);
        void ShowBuiltinStudioView(Control control);

        bool ShowManualItemSettings(Project project);
        bool ShowManualItemSettings(Version version);
        bool ShowManualItemSettings(Page page);
        bool ShowManualItemSettings(Section section);
        bool ShowManualItemSettings(Control control);

        (Page page, bool succes) LoadManualItem(Page page);
        (Section page, bool succes) LoadManualItem(Section page);
        (Control control, bool succes) LoadManualItem(Control page);

        bool AddVersion(bool autoSwitch);
        bool AddPage(Version version, bool autoSwitch);
        bool AddSection(Page page, bool autoSwitch);
        bool AddControl(Section section, bool autoSwitch);

        bool RemoveManualItem(Version version, bool confirm);
        bool RemoveManualItem(Page page, bool confirm);
        bool RemoveManualItem(Section section, bool confirm);
        bool RemoveManualItem(Control control, bool confirm);

        #region Events
        /// <summary>
        /// Occurs when Interpic Studio has finished it's initalization but before a project is loaded.
        /// The <see cref="Logger">Logger</see> is now accessible.
        /// </summary>
        event OnStudioStartup StudioStartup;

        /// <summary>
        /// Occurs when Interpic Studio is about te be shutdown.
        /// After this event all extensions will be unloaded.
        /// The current project has allready been unloaded, thus <see cref="CurrentProject"/> = <code>null</code>.
        /// </summary>
        event OnStudioShutdown StudioShutdown;

        /// <summary>
        /// Occurs when a project has loaded.
        /// The <see cref="CurrentProject">current project</see> is now accessible.
        /// </summary>
        event OnProjectLoaded ProjectLoaded;

        /// <summary>
        /// Occurs when the project is about to be unloaded.
        /// This is the last time <see cref="CurrentProject"/> is available.
        /// </summary>
        event OnProjectUnloaded ProjectUnloaded;

        /// <summary>
        /// Occurs when a new project has been created.
        /// it's settings provided by the IProjectTypeProvider have allready been edited by the user.
        /// </summary>
        event OnProjectCreated ProjectCreated;

        /// <summary>
        /// Occurs before the project settings window is shown.
        /// Changes made to the settings will be visible in the window.
        /// </summary>
        event OnProjectSettingsOpening ProjectSettingsOpening;

        /// <summary>
        /// Occurs after the project settings window is shown.
        /// 
        /// This event should not be used to validate settings unless a reference to the manual tree is required.
        /// </summary>
        event OnProjectSettingsOpened ProjectSettingsOpened;

        /// <summary>
        /// Occurs before the version settings window is shown.
        /// Changes made to the settings will be visible in the window.
        /// This event occures before the <see cref="Version.SettingsOpening"/> event.
        /// </summary>
        event OnVersionSettingsOpening VersionSettingsOpening;

        /// <summary>
        /// Occurs after the version settings window is shown.
        /// 
        /// This event should not be used to validate settings unless a reference to the manual tree is required.
        /// This event occurs after the <see cref="Version.SettingsOpened"/> event.
        /// </summary>
        event OnVersionSettingsOpened VersionSettingsOpened;

        /// <summary>
        /// Occurs before the page settings window is shown.
        /// Changes made to the settings will be visible in the window.
        /// This event occures before the <see cref="Page.SettingsOpening"/> event.
        /// </summary>
        event OnPageSettingsOpening PageSettingsOpening;

        /// <summary>
        /// Occurs after the page settings window is shown.
        /// 
        /// This event should not be used to validate settings unless a reference to the manual tree is required.
        /// This event occurs after the <see cref="Page.SettingsOpened"/> event.
        /// </summary>
        event OnPageSettingsOpened PageSettingsOpened;

        /// <summary>
        /// Occurs before the section settings window is shown.
        /// Changes made to the settings will be visible in the window.
        /// This event occures before the <see cref="Section.SettingsOpening"/> event.
        /// </summary>
        event OnSectionSettingsOpening SectionSettingsOpening;

        /// <summary>
        /// Occurs after the section settings window is shown.
        /// 
        /// This event should not be used to validate settings unless a reference to the manual tree is required.
        /// This event occurs after the <see cref="Section.SettingsOpened"/> event.
        /// </summary>
        event OnSectionSettingsOpened SectionSettingsOpened;

        /// <summary>
        /// Occurs before the control settings window is shown.
        /// Changes made to the settings will be visible in the window.
        /// This event occures before the <see cref="Control.SettingsOpening"/> event.
        /// </summary>
        event OnControlSettingsOpening ControlSettingsOpening;

        /// <summary>
        /// Occurs after the control settings window is shown.
        /// 
        /// This event should not be used to validate settings unless a reference to the manual tree is required.
        /// This event occurs after the <see cref="Control.SettingsOpened"/> event.
        /// </summary>
        event OnControlSettingsOpened ControlSettingsOpened;

        /// <summary>
        /// Occurs after the global settings have been saved.
        /// </summary>
        event OnGlobalSettingsSaved GlobalSettingsSaved;



        /// <summary>
        /// Occurs when a version is about to be deleted. This is the last time the <see cref="Models.Version"/> object is available.
        /// </summary>
        event OnVersionRemoved VersionRemoved;

        /// <summary>
        /// Occurs when a page is about to be deleted. This is the last time the <see cref="Models.Page"/> object is available.
        /// This event occurs before the <see cref="Page.Removed"/> event.
        /// </summary>
        event OnPageRemoved PageRemoved;

        /// <summary>
        /// Occurs when a page is about to be deleted. This is the last time the <see cref="Models.Section"/> object is available.
        /// This event occurs before the <see cref="Section.Removed"/> event.
        /// </summary>
        event OnSectionRemoved SectionRemoved;

        /// <summary>
        /// Occurs when a page is about to be deleted. This is the last time the <see cref="Models.Control"/> object is available.
        /// This event occurs before the <see cref="Control.Removed"/> event.
        /// </summary>
        event OnControlRemoved ControlRemoved;

        /// <summary>
        /// Occurs after a new version has been added.
        /// </summary>
        event OnVersionAdded VersionAdded;

        /// <summary>
        /// Occurs after a new page has been added.
        /// </summary>
        event OnPageAdded PageAdded;

        /// <summary>
        /// Occurs after a new section has been added.
        /// </summary>
        event OnSectionAdded SectionAdded;

        /// <summary>
        /// Occurs after a new control has been added.
        /// </summary>
        event OnControlAdded ControlAdded;

        #endregion

    }
}
