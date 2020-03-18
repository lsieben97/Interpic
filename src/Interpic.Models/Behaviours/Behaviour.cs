using Interpic.AsyncTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.Behaviours
{
    public class Behaviour
    {
        public static Behaviour None = new Behaviour
        {
            Name = "None",
            Description = "A web behaviour that does nothing.",
            Id = "None"
        };

        public static List<Behaviour> AvailableBehaviours { get; set; } = new List<Behaviour>();

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Action> Actions { get; set; } = new List<Action>();
    }
}
