using Interpic.Models;
using Interpic.Models.Behaviours;
using Interpic.Web.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web
{
    public class WebBehaviourExecutionContext : IBehaviourExecutionContext
    {
        private readonly SeleniumWrapper selenium;
        private readonly IStudioEnvironment studio;

        public WebBehaviourExecutionContext(SeleniumWrapper selenium, IStudioEnvironment studio)
        {
            this.selenium = selenium;
            this.studio = studio;
        }
        public ProjectState GetProjectState { get; set; }

        public T GetExecutionHelper<T>(string name) where T: class
        {
            if (name == "selenium")
            {
                return selenium as T;
            }
            else
            {
                return null;
            }
        }

        public IStudioEnvironment GetStudioEnvironment()
        {
            return studio;
        }

        public bool HasExecutionHelper(string name)
        {
            return name == "selenium";
        }
    }
}
