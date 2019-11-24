using Interpic.Settings;

namespace Interpic.Models.EventArgs
{
    public class VersionSettingsEventArgs : InterpicStudioEventArgs
    {
        /// <summary>
        /// The Project
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// The settings of the project.
        /// </summary>
        public SettingsCollection Settings { get; }

        public SettingsChanges Changes { get; }

        public VersionSettingsEventArgs(IStudioEnvironment environment, Version version, SettingsCollection settings, SettingsChanges changes) : base(environment)
        {
            Version = version;
            Settings = settings;
            Changes = changes;
        }
    }
}
