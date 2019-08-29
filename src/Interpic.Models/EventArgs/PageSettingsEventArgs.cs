using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.EventArgs
{
    public class PageSettingsEventArgs : InterpicStudioEventArgs
    {
        /// <summary>
        /// The page.
        /// </summary>
        public Page Page { get; }

        /// <summary>
        /// The settings for the page.
        /// </summary>
        public SettingsCollection Settings { get; }

        public SettingsChanges Changes { get; set; }

        public PageSettingsEventArgs(IStudioEnvironment environment, Page page, SettingsCollection settings, SettingsChanges changes) : base(environment)
        {
            Page = page;
            Settings = settings;
            Changes = changes;
        }
    }
}
