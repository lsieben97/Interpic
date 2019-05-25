using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Behaviours.Models
{
    public abstract class WebActionPack
    {
        public WebActionPack() { }
        public string Name { get; set; }
        public string Description { get; set; }
        public abstract List<WebAction> GetActions();
    }
}
