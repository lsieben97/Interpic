using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SectionSettingsEventArgs(IStudioEnvironment environment, Section section, SettingsCollection settings) : base(environment)
        {
            Section = section;
            Settings = settings;
        }
    }
}
