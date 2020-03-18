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
