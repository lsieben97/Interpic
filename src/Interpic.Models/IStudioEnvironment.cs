using Interpic.AsyncTasks;
using Interpic.Models;
using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        void ScheduleBackgroundTask(AsyncTask task);

        /// <summary>
        /// Cancel the scheduled background task with the given id.
        /// </summary>
        /// <param name="id">The id of the background task to cancel.</param>
        /// <returns>true when the task has been canceled. false when no task with the given id could be found or when the task is already executing.</returns>
        bool CancelScheduledBackgroundTask(string id);

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
        #endregion

    }
}
