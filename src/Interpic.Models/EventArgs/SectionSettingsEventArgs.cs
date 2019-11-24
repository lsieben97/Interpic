using Interpic.Settings;

namespace Interpic.Models.EventArgs
{
    public class SectionSettingsEventArgs : InterpicStudioEventArgs
    {
        /// <summary>
        /// The section.
        /// </summary>
        public Section Section { get; }

        /// <summary>
        /// The settings of the section.
        /// </summary>
        public SettingsCollection Settings { get; }

        public SettingsChanges Changes { get; }

        public SectionSettingsEventArgs(IStudioEnvironment environment, Section section, SettingsCollection settings, SettingsChanges changes) : base(environment)
        {
            Section = section;
            Settings = settings;
            Changes = changes;
        }
    }
}
