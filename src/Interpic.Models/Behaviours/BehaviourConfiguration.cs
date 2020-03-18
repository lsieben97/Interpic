using Newtonsoft.Json;
using System.Collections.Generic;

namespace Interpic.Models.Behaviours
{
    public class BehaviourConfiguration
    {
        public List<string> WebActionpacks { get; set; } = new List<string>();

        [JsonIgnore]
        public List<ActionPack> InternalWebActionPacks = new List<ActionPack>();

        public string BehavioursJson { get; set; }

        [JsonIgnore]
        public List<Behaviour> Behaviours { get; set; } = new List<Behaviour>();
    }
}
