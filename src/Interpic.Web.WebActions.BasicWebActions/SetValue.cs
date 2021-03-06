﻿using Interpic.Models.Behaviours;
using Interpic.Settings;
using Interpic.Web.Behaviours.Models;
using Interpic.Web.Behaviours.Utils;
using Interpic.Web.Selenium;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace Interpic.Web.WebActions.BasicWebActions
{
    public class SetValue : Action
    {
        public override string Id { get; } = "SetValue";

        public override string Name { get; set; } = "Set Value";
        public override string Description { get; set; } = "Sets the 'value' attribute of the selected element to the value specified";

        public override void Execute(IBehaviourExecutionContext context)
        {
            Selenium.SeleniumWrapper selenium = context.GetExecutionHelper<Selenium.SeleniumWrapper>("selenium");
            ElementSelector selector = JsonConvert.DeserializeObject<ElementSelector>(Parameters.GetTextSetting(ActionParameters.ELEMENT_PARAMETER));
            IWebElement element = WebActionUtils.FindElement(selenium, selector);
            SetAttribute(selenium, element, "value", Parameters.GetTextSetting(ActionParameters.VALUE_PARAMETER));

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
                        Description = "The element which needs to be changed.",
                        Key = ActionParameters.ELEMENT_PARAMETER,
                        Helper = new ElementSelectorSettingHelper()
                    },
                    new Setting<string>
                    {
                        Name = "Value",
                        Description = "The value that the 'value' attribute will have.",
                        Key = ActionParameters.VALUE_PARAMETER
                    }
                }
            };
        }

        private void SetAttribute(SeleniumWrapper selenium, IWebElement element, string attName, string attValue)
        {
            selenium.Driver.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", element, attName, attValue);
        }

        public static class ActionParameters
        {
            public const string ELEMENT_PARAMETER = "element";
            public const string VALUE_PARAMETER = "value";
        }
    }
}
