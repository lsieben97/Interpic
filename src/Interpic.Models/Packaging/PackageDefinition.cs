using Newtonsoft.Json;

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
