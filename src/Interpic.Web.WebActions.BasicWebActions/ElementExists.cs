using Interpic.Settings;
using Interpic.Web.Behaviours.Models;
using Interpic.Web.Behaviours.Utils;
using Interpic.Web.Selenium;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.WebActions.BasicWebActions
{
    public class ElementExists : CheckWebAction
    {
        public override string Id { get; } = "ElementExists";
        public override string Name { get; set; } = "Check if element exists";
        public override string Description { get; set; } = "Checks if the given element is present on the page.";
        public override void Execute()
        {
            ElementSelector selector = JsonConvert.DeserializeObject<ElementSelector>(Parameters.GetTextSetting(ActionParameters.ELEMENT_PARAMETER));
            IWebElement element = WebActionUtils.FindElement(Selenium, selector);
            CheckResult = element != null;
        }

        public override SettingsCollection GetDefaultParameters()
        {
            return new SettingsCollection
            {
                TextSettings = new List<Setting<string>>
                {
                    new Setting<string>
                    {
                        Name = "Element",
                        Description = "The element which existence needs to be checked.",
                        Key = ActionParameters.ELEMENT_PARAMETER,
                        Helper = new ElementSelectorSettingHelper()
                    }
                }
            };
        }

        public static class ActionParameters
        {
            public const string ELEMENT_PARAMETER = "element";
        }
    }
}
