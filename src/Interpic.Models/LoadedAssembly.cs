using System.Reflection;

namespace Interpic.Models
{
    public class LoadedAssembly
    {
        public string Path { get; set; }
        public Extension RequestingExtension { get; set; }
        public Assembly Assembly { get; set; }
        public bool InPackage { get; set; }
        public string PackagePath { get; set; }
    }
}
