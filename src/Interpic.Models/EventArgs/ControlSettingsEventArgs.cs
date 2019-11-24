using Interpic.Settings;

namespace Interpic.Models.EventArgs
{
    public class ControlSettingsEventArgs : InterpicStudioEventArgs
    {
        /// <summary>
        /// The control.
        /// </summary>
        public Control Control { get; }

        /// <summary>
        /// The settings for the control.
        /// </summary>
        public SettingsCollection Settings { get; }

        public SettingsChanges Changes { get; }

        public ControlSettingsEventArgs(IStudioEnvironment environment, Control control, SettingsCollection settings, SettingsChanges changes) : base(environment)
        {
            Control = control;
            Settings = settings;
            Changes = changes;
        }
    }
}
