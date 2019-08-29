using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.Packaging
{
    public class PackageManifest
    {
        public string DllPath { get; set; }
        public string PackageName { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
    }
}
