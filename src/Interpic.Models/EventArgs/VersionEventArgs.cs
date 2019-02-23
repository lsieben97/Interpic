using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.EventArgs
{
    public class VersionEventArgs : InterpicStudioEventArgs
    {
        public VersionEventArgs(IStudioEnvironment environment, Version version) : base(environment)
        {
            Version = version;
        }

        public Models.Version Version { get; set; }
    }
}
