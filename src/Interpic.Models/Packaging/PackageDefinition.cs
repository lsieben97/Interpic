using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.Packaging
{
    public class PackageDefinition
    {
        public string Extension { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public string DisplayName => $"{Name} (.{Extension})";
    }
}
