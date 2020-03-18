using Interpic.Models.Behaviours;
using Interpic.Web.Behaviours.Models;
using System.Collections.Generic;

namespace Interpic.Web.WebActions.BasicWebActions
{
    public class BaseWebActionPack : ActionPack
    {

        public new string Name { get; set; } = "Basic web actions pack";
        public new string Description { get; set; } = "This pack provides basic web actions for use on any website.";
        public override List<Action> GetActions()
        {
            return new List<Action>
            {
                new ElementExists(),
                new SetValue()
            };
        }
    }
}
