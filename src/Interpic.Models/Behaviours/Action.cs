using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.Behaviours
{
    public abstract class Action
    {
        public abstract string Id { get; }
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
        public SettingsCollection Parameters { get; set; }
        public ActionType Type { get; set; } = ActionType.Action;


        public abstract void Execute(IBehaviourExecutionContext context);
        public abstract SettingsCollection GetDefaultParameters();

        

        public enum ActionType
        {
            Action,
            Check
        }
    }
}
