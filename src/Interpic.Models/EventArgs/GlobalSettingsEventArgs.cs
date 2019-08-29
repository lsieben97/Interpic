using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
