using System.Collections.Generic;

namespace Interpic.Models.Behaviours
{
    public abstract class ActionPack
    {
        public ActionPack() { }
        public string Name { get; set; }
        public string Description { get; set; }
        public abstract List<Action> GetActions();
    }
}
