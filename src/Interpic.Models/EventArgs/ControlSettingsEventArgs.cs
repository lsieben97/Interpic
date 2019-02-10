using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ControlSettingsEventArgs(IStudioEnvironment environment, Control control, SettingsCollection settings) : base(environment)
        {
            Control = control;
            Settings = settings;
        }
    }
}
