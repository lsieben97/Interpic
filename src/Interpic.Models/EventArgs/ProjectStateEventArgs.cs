using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.EventArgs
{
    /// <summary>
    /// Contains the entire project state. objects that are unavailable (for example no section is selected) will be <code>null</code>.
    /// </summary>
    public class ProjectStateEventArgs : InterpicStudioEventArgs
    {
        public ProjectStateEventArgs(IStudioEnvironment environment, Project project, Version version, Page page, Section section, Control control) : base(environment)
        {
            Project = project;
            Version = version;
            Page = page;
            Section = section;
            Control = control;
        }

        public Project Project { get; set; }
        public Version Version { get; set; }
        public Page Page { get; set; }
        public Section Section { get; set; }
        public Control Control { get; set; }
    }
}
