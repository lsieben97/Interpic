using HtmlAgilityPack;

namespace Interpic.Web
{
    public static class Utils
    {
        public static string GetFriendlyHtmlName(HtmlNode node)
        {
            string name = node.Name;
            if (string.IsNullOrWhiteSpace(node.Id) == false)
            {
                name += "#" + node.Id;
            }

            if (node.GetClasses() != null)
            {
                foreach(string className in node.GetClasses())
                {
                    name += " ." + className;
                }
            }
            return name;
        }
    }
}
