using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.EventArgs
{
    public class InterpicStudioEventArgs : System.EventArgs
    {
        /// <summary>
        /// The studio environment.
        /// </summary>
        public IStudioEnvironment StudioEnvironment { get; }

        public InterpicStudioEventArgs(IStudioEnvironment environment)
        {
            StudioEnvironment = environment;
        }
    }
}
