using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
