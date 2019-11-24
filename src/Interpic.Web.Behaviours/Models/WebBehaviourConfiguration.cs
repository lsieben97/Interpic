using Newtonsoft.Json;
using System.Collections.Generic;

namespace Interpic.Web.Behaviours.Models
{
    public class WebBehaviourConfiguration
    {
        public List<string> WebActionpacks { get; set; } = new List<string>();

        [JsonIgnore]
        public List<WebActionPack> InternalWebActionPacks = new List<WebActionPack>();
        public List<WebBehaviour> Behaviours { get; set; } = new List<WebBehaviour>();
    }
}
