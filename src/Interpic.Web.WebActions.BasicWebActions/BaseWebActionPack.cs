﻿using Interpic.Web.Behaviours.Models;
using System.Collections.Generic;

namespace Interpic.Web.WebActions.BasicWebActions
{
    public class BaseWebActionPack : WebActionPack
    {

        public new string Name { get; set; } = "Basic web actions pack";
        public new string Description { get; set; } = "This pack provides basic web actions for use on any website.";
        public override List<WebAction> GetActions()
        {
            return new List<WebAction>
            {
                new ElementExists(),
                new SetValue()
            };
        }
    }
}
