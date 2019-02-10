using Interpic.Alerts;
using Interpic.Extensions;
using Interpic.Models;
using Interpic.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Interpic
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static readonly string GLOBAL_EXTENSION_FILE = "globalExtensions.json";
        internal static readonly string RECENT_PROJECTS_FILE = "recent.json";
        internal static readonly string GLOBAL_SETTINGS_FILE = "globalSettings.json";
        internal static readonly string EXECUTABLE_DIRECTORY = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
        internal static readonly string VERSION = "1.0.0";
        internal static readonly Logger ApplicationLogger = new Logger();

        internal static SettingsCollection GlobalSettings { get; set; }
        private static SettingsCollection DefaultGlobalSettings;

        internal static void LoadGlobalSettings()
        {
            LoadDefaulGlobalSettings();
            
            try
            {
                if (File.Exists(EXECUTABLE_DIRECTORY + "\\" + GLOBAL_SETTINGS_FILE))
                {
                    GlobalSettings = JsonConvert.DeserializeObject<SettingsCollection>(File.ReadAllText(EXECUTABLE_DIRECTORY + "\\" + GLOBAL_SETTINGS_FILE));
                }
                else
                {
                    GlobalSettings = DefaultGlobalSettings;
                    SaveGlobalSettings();
                }
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not load global settings file.\nLoading default settings.\nDetails:\n" + ex.Message);
            }
        }

        internal static void SaveGlobalSettings()
        {
            try
            {
                File.WriteAllText(EXECUTABLE_DIRECTORY + "\\" + GLOBAL_SETTINGS_FILE, JsonConvert.SerializeObject(GlobalSettings));
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not save global settings file.\nDefault settings are applied.\nDetails:\n" + ex.Message);
            }
        }

        private static void LoadDefaulGlobalSettings()
        {
            DefaultGlobalSettings = new SettingsCollection();
            DefaultGlobalSettings.Name = "global settings";

            Setting<int> autosaveWarning = new Setting<int>();
            autosaveWarning.Name = "Save warning";
            autosaveWarning.Key = "SaveWarningOffset";
            autosaveWarning.Description = "Amount of minutes after Interpic Studio colors the last saved text red when there are unsaved changes.";
            autosaveWarning.Value = 5;

            Setting<bool> showInfoOnAutomaticSettingsDialog = new Setting<bool>();
            showInfoOnAutomaticSettingsDialog.Name = "Show info message about settings.";
            showInfoOnAutomaticSettingsDialog.Description = "Show an info message when the settings dialog for a project, page, section or control is automatically opened.";
            showInfoOnAutomaticSettingsDialog.Value = true;
            showInfoOnAutomaticSettingsDialog.Key = "ShowInfoForSettings";

            Setting<bool> enableDeveloperMode = new Setting<bool>();
            enableDeveloperMode.Name = "Enable Developer mode.";
            enableDeveloperMode.Description = "Show the developer menu item in the menu bar.";
            enableDeveloperMode.Value = false;
            enableDeveloperMode.Key = "EnableDeveloperMode";

            PathSetting logDirectory = new PathSetting();
            logDirectory.Value = EXECUTABLE_DIRECTORY;
            logDirectory.DialogTitle = "Set log directory";
            logDirectory.Operation = PathSetting.PathOperation.Load;
            logDirectory.Type = PathSetting.PathType.Folder;
            logDirectory.Name = "Log directory";
            logDirectory.Description = "The directory where the log file is located.";
            logDirectory.Key = "logDirectory";

            DefaultGlobalSettings.NumeralSettings.Add(autosaveWarning);
            DefaultGlobalSettings.BooleanSettings.Add(showInfoOnAutomaticSettingsDialog);
            DefaultGlobalSettings.BooleanSettings.Add(enableDeveloperMode);
            DefaultGlobalSettings.PathSettings.Add(logDirectory);
        }

        internal static void InitializeLogger(string logfile, IStudioEnvironment environment)
        {
            ApplicationLogger.OpenLog(logfile, environment);
        }
    }
}
