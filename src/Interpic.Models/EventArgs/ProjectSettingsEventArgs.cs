using Interpic.Settings;

namespace Interpic.Models.EventArgs
{
    public class ProjectSettingsEventArgs : InterpicStudioEventArgs
    {
        /// <summary>
        /// The Project
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// The settings of the project.
        /// </summary>
        public SettingsCollection Settings { get; }

        public SettingsChanges Changes { get; }

        public ProjectSettingsEventArgs(IStudioEnvironment environment, Project project, SettingsCollection settings, SettingsChanges changes) : base(environment)
        {
            Project = project;
            Settings = settings;
            Changes = changes;
        }
    }
}
