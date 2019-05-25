﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Behaviours.Models
{
    public abstract class CheckWebAction : WebAction
    {
        public CheckWebAction()
        {
            Type = WebActionType.Check;
        }

        public WebBehaviour BehaviourWhenTrue { get; set; }
        public WebBehaviour BehaviourWhenFalse { get; set; }

        public bool CheckResult { get; set; }
    }
}