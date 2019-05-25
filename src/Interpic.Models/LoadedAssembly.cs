using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models
{
    public class LoadedAssembly
    {
        public string Path { get; set; }
        public Extension RequestingExtension { get; set; }
        public Assembly Assembly { get; set; }
    }
}
