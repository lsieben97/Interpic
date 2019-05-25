using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web.Behaviours.Models
{
    public class ElementSelector
    {
        public string Selector { get; set; }
        public ElementSelectorType SelectorType {get; set;}

        public enum ElementSelectorType
        {
            Name,
            CssSelector,
            XPath,
            Id,
            Class,
            TagName
        }
    }
}
