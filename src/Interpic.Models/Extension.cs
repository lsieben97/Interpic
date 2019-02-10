using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models
{
    public class Extension
    {
        public string Name { get; set; }
        public ExtensionType Type { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            if (Name == "No active extensions")
            {
                return Name;
            }
            else
            {
                return Name + "(" + (Type == ExtensionType.Project ? "Global" : "Project") + ")";
            }
        }
    }
}
