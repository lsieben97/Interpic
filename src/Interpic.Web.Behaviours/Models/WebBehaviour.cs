using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Behaviours.Models
{
    public class WebBehaviour
    {
        public static WebBehaviour None = new WebBehaviour
        {
            Name = "None",
            Description = "A web behaviour that does nothing.",
            Id = "None"
        };

        public static List<WebBehaviour> AvailableBehaviours { get; set; } = new List<WebBehaviour>();

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WebAction> Actions { get; set; } = new List<WebAction>();
    }
}
