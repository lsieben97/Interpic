using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models
{
    public class ProjectState
    {
        public Version Version { get; }
        public Page Page { get; }
        public Section Section { get; }
        public Control Control { get; }
        public bool HasVersion { get => Version != null; }
        public bool HasPage { get => Page != null; }
        public bool HasSection { get => Section != null; }
        public bool HasControl { get => Control != null; }
    }
}
