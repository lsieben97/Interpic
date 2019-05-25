using Interpic.Settings;
using Interpic.Web.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Behaviours.Models
{
    public abstract class WebAction
    {
        public abstract string Id { get; }
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
        public WebActionType Type { get; set; }
        public SeleniumWrapper Selenium { get; set; }
        public abstract SettingsCollection Parameters { get; set; }

        public abstract void Execute();

        public enum WebActionType
        {
            Action,
            Check
        }
    }
}
