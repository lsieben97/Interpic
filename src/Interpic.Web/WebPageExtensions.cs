using HtmlAgilityPack;
using Interpic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
