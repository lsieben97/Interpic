using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models
{
    /// <summary>
    /// Shows what actions require internet access. Used to by the studio to determine whether an action is allowed in offline mode. 
    /// </summary>
    public class InternetUsage
    {
        public InternetUsage()
        {
        }

        public InternetUsage(bool refreshingProject, bool refreshingVersion, bool refreshingPage, bool refreshingSection, bool refreshingControl)
        {
            RefreshingProject = refreshingProject;
            RefreshingVersion = refreshingVersion;
            RefreshingPage = refreshingPage;
            RefreshingSection = refreshingSection;
            RefreshingControl = refreshingControl;
        }

        public bool RefreshingProject { get; set; } = true;
        public bool RefreshingVersion { get; set; } = true;
        public bool RefreshingPage { get; set; } = true;
        public bool RefreshingSection { get; set; } = true;
        public bool RefreshingControl { get; set; } = true;
    }
}
