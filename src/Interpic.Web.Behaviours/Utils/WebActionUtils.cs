using Interpic.Web.Behaviours.Models;
using Interpic.Web.Selenium;
using OpenQA.Selenium;
using System;

namespace Interpic.Web.Behaviours.Utils
{
    public static class WebActionUtils
    {
        public static IWebElement FindElement(SeleniumWrapper selenium, ElementSelector selector)
        {
            try
            {
                switch (selector.SelectorType)
                {
                    case ElementSelector.ElementSelectorType.Name:
                        return selenium.Driver.FindElementByName(selector.Selector);
                    case ElementSelector.ElementSelectorType.CssSelector:
                        return selenium.Driver.FindElementByCssSelector(selector.Selector);
                    case ElementSelector.ElementSelectorType.XPath:
                        return selenium.Driver.FindElementByXPath(selector.Selector);
                    case ElementSelector.ElementSelectorType.Id:
                        return selenium.Driver.FindElementById(selector.Selector);
                    case ElementSelector.ElementSelectorType.Class:
                        return selenium.Driver.FindElementByClassName(selector.Selector);
                    case ElementSelector.ElementSelectorType.TagName:
                        return selenium.Driver.FindElementByTagName(selector.Selector);
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
