using HtmlAgilityPack;
using Interpic.Models;

namespace Interpic.Web
{
    public class WebPageExtensions : ExtensionObject
    {
        public WebPageExtensions(HtmlDocument document)
        {
            Document = document;
            RaisePropertyChanged = false;
        }

        public HtmlDocument Document { get; set; }
    }
}
