using Interpic.Settings;

namespace Interpic.Models.EventArgs
{
    public class GlobalSettingsEventArgs : InterpicStudioEventArgs
    {
        /// <summary>
        /// The settings of the project.
        /// </summary>
        public SettingsCollection Settings { get; }

        public SettingsChanges Changes { get; set; }

        public GlobalSettingsEventArgs(IStudioEnvironment environment, SettingsCollection settings, SettingsChanges changes) : base(environment)
        {
            Settings = settings;
            Changes = changes;
        }
    }
}
